using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NPinyin;
using UnityEditor;
using UnityEngine;

public class CommonAssetProcessor : AssetPostprocessor
{

    #region 命名规则

    public static bool IsUI(string path)
    {
        return path.Contains("/UI/");
    }

    public static bool HasScale(string path)
    {
        return Path.GetFileNameWithoutExtension(path).Contains("_Scale");
    }

    public static bool ReadWrite(string path)
    {
        return Path.GetFileNameWithoutExtension(path).Contains("_ReadWrite");
    }

    public static bool HasExtraUv(string path)
    {
        return Path.GetFileNameWithoutExtension(path).Contains("_UV2");
    }

    public static bool HasMipMap(string path)
    {
        return Path.GetFileNameWithoutExtension(path).Contains("_MipMap");
    }

    public static bool FirstImport(AssetImporter importer)
    {
        // 如果是忽略的文件夹，则跳过
        if (Ignore(importer.assetPath)) return false;
        return importer.importSettingsMissing;
    }

    public static bool Ignore(string path)
    {
        if (path.Contains("Plugins")) return true;
        if (path.Contains("Packages")) return true;
        return false;
    }
    #endregion
    
    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        void renameAssets(string path, string newPath)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Move(path, newPath);
                    var oldMeta = path + ".meta";
                    var newMeta = newPath + ".meta";
                    if (File.Exists(oldMeta))
                        File.Move(oldMeta, newMeta);
                }
                catch (Exception)
                {
                }
            }
        }

        List<string> chineseFilePath = new List<string>();
        // 删除名字中的空格和中文
        foreach (var importedAsset in importedAssets)
        {
            var sourceFilePath = importedAsset;
            var fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            var extension = Path.GetExtension(sourceFilePath);
            var dir = Path.GetDirectoryName(sourceFilePath);
            // 如果是文件夹，自行修改
            if (Directory.Exists(importedAsset))
            {
                if (Regex.IsMatch(fileName, @"[\u4e00-\u9fbb]"))
                {
                    EditorUtility.DisplayDialog("注意", $"{importedAsset} 的名字含有空格或中文，请修改", "好的");
                    continue;
                }
            }
            var trimFileName = fileName.Trim();
            if (trimFileName != fileName)
            {
                fileName = trimFileName;
                var newFile = Path.Combine(dir, $"{fileName}{extension}");
                renameAssets(sourceFilePath, newFile);
                sourceFilePath = newFile;
            }
            if (Regex.IsMatch(fileName, @"[\u4e00-\u9fbb]"))
            {
                chineseFilePath.Add(sourceFilePath);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < fileName.Length; i++)
                {
                    var c = fileName[i].ToString();
                    if (Regex.IsMatch(c, @"[\u4e00-\u9fbb]"))
                    {
                        sb.Append(Pinyin.GetPinyin(c).UppercaseFirst());
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                var newFile = Path.Combine(dir, $"{sb}{extension}");
                renameAssets(sourceFilePath, newFile);
            }
        }

        if (chineseFilePath.Count > 0)
        {
            StringBuilder tips = new StringBuilder();
            tips.AppendLine($"有{chineseFilePath.Count}个文件的名字含有中文，是否自动修改");
            int count = 0;
            foreach (var path in chineseFilePath)
            {
                count++;
                tips.AppendLine(path);
                if (count > 5)
                {
                    tips.AppendLine("....");
                    break;
                }
            }
            if (EditorUtility.DisplayDialog("注意", tips.ToString(), "好", "不用，我自己改"))
            {
                foreach (var sourceFilePath in chineseFilePath)
                {
                    var fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
                    var extension = Path.GetExtension(sourceFilePath);
                    var dir = Path.GetDirectoryName(sourceFilePath);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < fileName.Length; i++)
                    {
                        var c = fileName[i].ToString();
                        if (Regex.IsMatch(c, @"[\u4e00-\u9fbb]"))
                        {
                            sb.Append(Pinyin.GetPinyin(c).UppercaseFirst());
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    var newFile = Path.Combine(dir, $"{sb}{extension}");
                    renameAssets(sourceFilePath, newFile);
                }
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/格式化资源", false)]
    private static void FormatModel()
    {
        List<ModelImporter> modelImporters = new List<ModelImporter>();
        var objs = Selection.objects;
        foreach (var o in objs)
        {
            string path = AssetDatabase.GetAssetPath(o);
            modelImporters.AddRange(GetImporterByPath<ModelImporter>(path));
        }
        List<TextureImporter> textureImporters = new List<TextureImporter>();
        foreach (var o in objs)
        {
            string path = AssetDatabase.GetAssetPath(o);
            textureImporters.AddRange(GetImporterByPath<TextureImporter>(path));
        }
        int totalCount = modelImporters.Count + textureImporters.Count;
        float index = 0;
        foreach (var modelImporter in modelImporters)
        {
            index++;
            EditorUtility.DisplayProgressBar("正在格式化资源", modelImporter.assetPath, index / totalCount);
            ModelProcessor.FormatModel(modelImporter);
        }
        
        foreach (var importer in textureImporters)
        {
            index++;
            EditorUtility.DisplayProgressBar("正在格式化资源", importer.assetPath, index / totalCount);
            TextureProcessor.FormatTexture(importer);
        }
        
        EditorUtility.ClearProgressBar();
    }

    private static List<T> GetImporterByPath<T>(string path) where T : AssetImporter
    {
        List<T> result = new List<T>();
        // 如果是单个文件的话
        if (File.Exists(path))
        {
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer is T res)
            {
                result.Add(res);
                return result;
            }
        }

        // 如果是文件夹的话
        if (Directory.Exists(path))
        {
            var filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                if (filePath.EndsWith(".meta")) continue;
                AssetImporter importer = AssetImporter.GetAtPath(filePath);
                if (importer is T res)
                {
                    result.Add(res);
                }
            }
            return result;
        }
        return result;
    }
}