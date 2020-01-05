using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AD
{
    public enum KResourceQuality
    {
        Sd = 2,
        Hd = 1,
        Ld = 4,
    }

    /// <summary>
    /// 资源路径优先级，优先使用
    /// </summary>
    public enum KResourcePathPriorityType
    {
        Invalid,

        /// <summary>
        /// 忽略PersitentDataPath, 优先寻找Resources或StreamingAssets路径 (取决于ResourcePathType)
        /// </summary>
        InAppPathPriority,

        /// <summary>
        /// 尝试在Persistent目錄尋找，找不到再去StreamingAssets,
        /// 这一般用于进行热更新版本号判断后，设置成该属性
        /// </summary>
        PersistentDataPathPriority,
    }

    public class ResourceModule : MonoSingleton<ResourceModule>
    {
        public delegate void AsyncLoadABAssetDelegate(Object asset, object[] args);

        public enum LoadingLogLevel
        {
            None,
            ShowTime,
            ShowDetail,
        }

        public static KResourceQuality Quality = KResourceQuality.Sd;

        public static float TextureScale
        {
            get { return 1f / (float) Quality; }
        }

        public static bool LoadByQueue = false;
        public static int LogLevel = (int) LoadingLogLevel.None;
        public static System.Func<string, string> CustomGetResourcesPath; // 自定义资源路径。。。


        /// <summary>
        /// 用于GetResourceFullPath函数，返回的类型判断
        /// </summary>
        public enum GetResourceFullPathType
        {
            Invalid,
            InApp,
            InDocument,
        }

        private void Update()
        {
            AbstractResourceLoader.CheckGcCollect();
        }

        /// <summary>
        /// Load Async Asset Bundle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback">cps style async</param>
        /// <returns></returns>
        public static AbstractResourceLoader LoadBundleAsync(string path,
            AssetFileLoader.AssetFileBridgeDelegate callback = null)
        {
            var request = AssetFileLoader.Load(path, callback);
            return request;
        }

        /// <summary>
        /// load asset bundle immediatly
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static AbstractResourceLoader LoadBundle(string path,
            AssetFileLoader.AssetFileBridgeDelegate callback = null)
        {
            var request = AssetFileLoader.Load(path, callback, LoaderMode.Sync);
            return request;
        }

        /// <summary>
        /// check file exists of streamingAssets. On Android will use plugin to do that.
        /// </summary>
        /// <param name="path">relative path,  when file is "file:///android_asset/test.txt", the pat is "test.txt"</param>
        /// <returns></returns>
        public static bool IsStreamingAssetsExists(string path)
        {
            if (Application.platform == RuntimePlatform.Android)
                return ADAndroidPlugin.IsAssetExists(path);

            var fullPath = Path.Combine(Application.streamingAssetsPath, path);
            return File.Exists(fullPath);
        }

        /// <summary>
        /// Load file from streamingAssets. On Android will use plugin to do that.
        /// </summary>
        /// <param name="path">relative path,  when file is "file:///android_asset/test.txt", the pat is "test.txt"</param>
        /// <returns></returns>
        public static byte[] LoadSyncFromStreamingAssets(string path)
        {
            if (!IsStreamingAssetsExists(path))
                throw new Exception("Not exist StreamingAssets path: " + path);

            if (Application.platform == RuntimePlatform.Android)
                return ADAndroidPlugin.GetAssetBytes(path);

            var fullPath = Path.Combine(Application.streamingAssetsPath, path);
            return ReadAllBytes(fullPath);
        }

        public static bool IsPersistentDataExist(string path)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, path);
            return File.Exists(fullPath);
        }

        public static byte[] LoadSyncFromPersistentDataPath(string path)
        {
            if (!IsPersistentDataExist(path))
                throw new Exception("Not exist PersistentData path: " + path);

            var fullPath = Path.Combine(Application.persistentDataPath, path);
            return ReadAllBytes(fullPath);
        }

        /// <summary>
        /// 无视锁文件，直接读bytes
        /// </summary>
        /// <param name="resPath"></param>
        public static byte[] ReadAllBytes(string resPath)
        {
            byte[] bytes;
            using (FileStream fs = File.Open(resPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int) fs.Length);
            }
            return bytes;
        }

        public static void LogRequest(string resType, string resPath)
        {
            if (LogLevel < (int) LoadingLogLevel.ShowDetail)
                return;

            Debugger.Log($"[Request] {resType}, {resPath}");
        }

        public static void LogLoadTime(string resType, string resPath, System.DateTime begin)
        {
            if (LogLevel < (int) LoadingLogLevel.ShowTime)
                return;

            Debugger.Log($"[Load] {resType}, {resPath}, {(System.DateTime.Now - begin).TotalSeconds}s");
        }

        /// <summary>
        /// Collect all AD's resource unused loaders
        /// </summary>
        public static void Collect()
        {
            while (AbstractResourceLoader.UnUsesLoaders.Count > 0)
                AbstractResourceLoader.DoGarbageCollect();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

        }
    }
}
