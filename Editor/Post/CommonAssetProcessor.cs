using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

public class CommonAssetProcessor : AssetPostprocessor
{

    #region 命名规则

    public static bool IsUI(string path)
    {
        return path.ToLower().Contains("/ui/");
    }

    public static bool AnimationHasScale(string path)
    {
        return Path.GetFileNameWithoutExtension(path).ToLower().Contains("_scale");
    }

    public static bool ReadWrite(string path)
    {
        return Path.GetFileNameWithoutExtension(path).ToLower().Contains("_rw");
    }

    public static bool HasExtraUv(string path)
    {
        return Path.GetFileNameWithoutExtension(path).ToLower().Contains("_uv1");
    }

    public static bool HasVertexColor(string path)
    {
        return Path.GetFileNameWithoutExtension(path).ToLower().Contains("_vc");
    }

    public static bool FirstImport(AssetImporter importer)
    {
        if (Ignore(importer.assetPath)) return false;
        return importer.importSettingsMissing;
    }

    private static bool Ignore(string path)
    { 
        if (path.Contains("Assets/Res/"))
        {
            return false;
        }
        if (path.Contains("Assets/Art/"))
        {
            return false;
        }
        return true;
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
        StringBuilder chineseTips = new StringBuilder();
        List<string> chineseDirPath = new List<string>();
        chineseTips.AppendLine("有多个文件夹含有中文");
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
                if (Regex.IsMatch(dir, @"[\u4e00-\u9fbb]"))
                {
                    if (chineseDirPath.Count > 5) continue;
                    if (!chineseDirPath.Contains(dir))
                    {
                        chineseTips.AppendLine(dir);
                        chineseDirPath.Add(dir);
                    }
                    // EditorUtility.DisplayDialog("注意", $"{importedAsset} 的名字含有空格或中文，请修改", "好的");
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
            }
        }

        if (chineseDirPath.Count > 0)
        {
            chineseTips.AppendLine("....");
            EditorUtility.DisplayDialog("注意", chineseTips.ToString(), "好");
        }
        
        if (chineseFilePath.Count > 0)
        {
            StringBuilder tips = new StringBuilder();
            tips.AppendLine($"有{chineseFilePath.Count}个文件的名字含有中文");
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
            EditorUtility.DisplayDialog("注意", tips.ToString(), "好");
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
        List<AudioImporter> audioImporters = new List<AudioImporter>();
        foreach (var o in objs)
        {
            string path = AssetDatabase.GetAssetPath(o);
            audioImporters.AddRange(GetImporterByPath<AudioImporter>(path));
        }
        int totalCount = modelImporters.Count + textureImporters.Count + audioImporters.Count;
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
        
        foreach (var importer in audioImporters)
        {
            index++;
            EditorUtility.DisplayProgressBar("正在格式化资源", importer.assetPath, index / totalCount);
            AudioProcessor.FormatAudio(importer);
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