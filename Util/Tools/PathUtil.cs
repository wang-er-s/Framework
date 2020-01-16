/*
* Create by Soso
* Time : 2019-01-01-02 下午
*/
using UnityEngine;
using System;
using System.IO;

namespace AD
{
	public static class PathUtil  
	{
        public static string PersistentDataPath
        {
            get
            {
                return Application.persistentDataPath + "/";
            }
        }

        public static string StreamingAssetsPath
        {
            get
            {
                return Application.streamingAssetsPath + "/";
            }
        }

        public static string GetAssetBundleOutPath()
        {
            string outPath = getPlatformPath() + "/" + GetPlatformName();
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
            return outPath;
        }

        /// <summary>
        /// 获取WWW协议的路径
        /// </summary>
        public static string GetWWWPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "jar:file://" + GetAssetBundleOutPath();
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "file:///" + GetAssetBundleOutPath();
                default:
                    return null;
            }
        }


        /// <summary>
        /// 获取对应平台的路径
        /// </summary>
        private static string getPlatformPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return Application.persistentDataPath;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsPlayer:
                    return Application.streamingAssetsPath;
                default:
                    return null;
            }
        }

        public static string GetPlatformName()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXEditor:
                    return "Mac";
                default:
                    return null;
            }
        }
    }
}
