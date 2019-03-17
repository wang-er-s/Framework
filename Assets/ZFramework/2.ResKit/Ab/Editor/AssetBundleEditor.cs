using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZFramework
{
    public struct MarkAsset : IEquatable<MarkAsset>
    {
        public MarkAsset(string assetName, string assetbundleName, string sceneName)
        {
            AssetName = assetName;
            AssetbundleName = assetbundleName;
            SceneName = sceneName;
        }

        public string AssetName { get; private set; }
        public string AssetbundleName { get; private set; }
        public string SceneName { get; private set; }

        #region 重写方法

        public bool Equals(MarkAsset other)
        {
            return string.Equals(AssetName, other.AssetName) && string.Equals(AssetbundleName, other.AssetbundleName) && string.Equals(SceneName, other.SceneName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is MarkAsset && Equals((MarkAsset)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (AssetName != null ? AssetName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AssetbundleName != null ? AssetbundleName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SceneName != null ? SceneName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{AssetName}-->{AssetbundleName}-->{SceneName}";
        }

        #endregion

    }

    public class AssetBundleEditor
    {

        #region 自动标记

        /*
         * 思路
         * 1.找到资源保存的文件夹
         * 2.遍历里面的场景文件夹
         * 3.遍历每个场景文件夹里面的所有文件系统
         * 4.如果访问的是问价家，继续访问里面的所有文件系统，直到找到文件
         * 5.找到文件修改assetbundle lablels
         * 6.用AssetImporter类修改包名和后缀
         * 7.保存对应的 文件名 和 对应的ab包名
         */

        private static List<MarkAsset> markAssets;

        [MenuItem("AssetBundle/Mark Res")]
        private static void SetAssetBundleLables()
        {
            //移除所有没有使用的标记
            AssetDatabase.RemoveUnusedAssetBundleNames();
            markAssets = new List<MarkAsset>();
            string assetDir = Application.dataPath + "/Res";
            DirectoryInfo directoryInfo = new DirectoryInfo(assetDir);
            DirectoryInfo[] sceneDirectorInfos = directoryInfo.GetDirectories();
            foreach (DirectoryInfo tempDirectorInfo in sceneDirectorInfos)
            {
                Dictionary<string, List<string>> namePathDic = new Dictionary<string, List<string>>();
                int index =
                    tempDirectorInfo.FullName.LastIndexOf("\\", StringComparison.Ordinal);
                if(index == -1)
                    index = tempDirectorInfo.FullName.LastIndexOf("/", StringComparison.Ordinal);
                string sceneName = tempDirectorInfo.FullName.Substring(index + 1);
                onSceneFileSystemInfo(tempDirectorInfo, sceneName, namePathDic);
                onWriteConfig(sceneName, namePathDic);
            }
            //CreateMarkAssetsCs();
            AssetDatabase.Refresh();
            Debug.Log("资源标记成功！");
        }

        /// <summary>
        /// 记录配置文件
        /// </summary>
        private static void onWriteConfig(string sceneName, Dictionary<string, List<string>> namePahtDict)
        {
            string path = PathUtil.GetAssetBundleOutPath() + "/" + sceneName + "Record.txt";
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(namePahtDict.Count);
                    foreach (KeyValuePair<string, List<string>> kv in namePahtDict)
                    {
                        foreach (string item in kv.Value)
                        {
                            sw.WriteLine(kv.Key + "--" + item);
                        }
                    }
                }
            }
        }

        private static void onSceneFileSystemInfo(FileSystemInfo fileSystemInfo, string sceneName,
                                            Dictionary<string, List<string>> namePathDic)
        {
            if (!fileSystemInfo.Exists)
            {
                Debug.LogError(fileSystemInfo.FullName + "不存在");
                return;
            }

            DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
            foreach (FileSystemInfo tempFileSystemInfo in fileSystemInfos)
            {
                FileInfo fileInfo = tempFileSystemInfo as FileInfo;
                if (fileInfo == null)
                {
                    //强转失败就不是文件 是文件夹
                    onSceneFileSystemInfo(tempFileSystemInfo, sceneName, namePathDic);
                }
                else
                {
                    //是文件，做标记
                    setLabels(fileInfo, sceneName, namePathDic);
                }
            }
        }

        private static void setLabels(FileInfo fileInfo, string sceneName, Dictionary<string, List<string>> namePathDic)
        {
            if (fileInfo.Extension.Equals(".meta")) return;
            string bundleName = getBundleName(fileInfo, sceneName);
            int index = fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal);
            string assetPath = fileInfo.FullName.Substring(index);
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);  
            assetImporter.assetBundleName = bundleName;
            string extension = "";
            extension = fileInfo.Extension.Equals(".unity") ? "u3d" : "assetbundle";
            assetImporter.assetBundleVariant = extension;
            if (!namePathDic.ContainsKey(sceneName))
                namePathDic.Add(sceneName, new List<string>());
            string temp = (bundleName.Contains("/") ? bundleName.Split('/')[1] : bundleName) + "." + extension;
            if (!namePathDic[sceneName].Contains(temp))
            {
                namePathDic[sceneName].Add(temp);
            }
            markAssets.Add(new MarkAsset(fileInfo.Name.Split('.')[0], temp.Split('.')[0], sceneName));
        }

        private static string getBundleName(FileInfo fileInfo, string sceneName)
        {
            string computerPath = fileInfo.FullName;
            //Mac Path : /Users/tcl/Desktop/New Unity Project/Assets/Res/Scene1/Material/RedMat.mat
            int sceneIndex = computerPath.IndexOf(sceneName, StringComparison.Ordinal) + sceneName.Length;
            string bundlePath = computerPath.Substring(sceneIndex + 1);
            string bundleName = "";
            char compareStr = 'a';
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                compareStr = '\\';
            }else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                compareStr = '/';
            }
            if (bundlePath.Contains(compareStr.ToString()))
            {
                Debug.Log(bundlePath.Split(compareStr)[0]);
                bundleName = sceneName + "/" + bundlePath.Split(compareStr)[0];
            }
            else
            {
                bundleName = sceneName;
            }

            return bundleName.ToLower();
        }

        private static void CreateMarkAssetsCs()
        {
            string path = "Assets/Scripts/Base/AssetsItem.cs";
            if (!Directory.Exists("Assets/Scripts/Base"))
                Directory.CreateDirectory("Assets/Scripts/Base");
            if (File.Exists(path)) File.Delete(path);
            var sw = new StreamWriter(path, false, new UTF8Encoding(false));
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine("public class AssetsItem");
            strBuilder.AppendLine("{");
            foreach (MarkAsset markAsset in markAssets)
            {
                strBuilder.AppendLine(
                    $"\tpublic const string {markAsset.SceneName.UppercaseFirst()}{markAsset.AssetbundleName.UppercaseFirst()}{markAsset.AssetName.UppercaseFirst()} = "
                    + $"\"{markAsset.SceneName}-{markAsset.AssetbundleName}-{markAsset.AssetName}\";");
            }
            strBuilder.AppendLine("}");
            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }

        #endregion

        #region 打包

        [MenuItem("AssetBundle/CreateAB")]
        private static void PackAssetBundle()
        {
            string path = PathUtil.GetAssetBundleOutPath();
            Debug.Log(path);
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.Android);
        }

        #endregion

        #region 删除

        [MenuItem("AssetBundle/Delete All")]
        private static void DeleteAB()
        {
            string path = PathUtil.GetAssetBundleOutPath();
            Directory.Delete(path, true);
            File.Delete(path + ".meta");
            AssetDatabase.Refresh();
        }

        #endregion

    }
}