using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static AD.ResourceModule;

namespace AD
{
    public static class ResPath
    {
        static ResPath()
        {
            InitResourcePath();
        }

        public static string BuildPlatformName
        {
            get { return GetBuildPlatformName(); }
        } // ex: IOS, Android, AndroidLD

        public static string BundlesDirName
        {
            get { return Configs.BundlesDirName; }
        }

        public static string FileProtocol
        {
            get { return GetFileProtocol(); }
        } // for WWW...with file:///xxx

        /// <summary>
        /// Unity Editor load AssetBundle directly from the Asset Bundle Path,
        /// whth file:// protocol
        /// </summary>
        public static string EditorAssetBundleFullPath
        {
            get
            {
                return Configs.EditorAssetBundlePath;
            }
        }

        /// <summary>
        /// Product Folder's Relative Path   -  Default: ../Product,   which means Assets/../Product
        /// </summary>
        public static string ProductRelPath
        {
            //get { return AD.AppEngine.GetConfig(ADDefaultConfigs.ProductRelPath); }
            get { return "Resources"; }
        }

        /// <summary>
        /// Product Folder Full Path , Default: C:\xxxxx\xxxx\../Product
        /// </summary>
        public static string EditorProductFullPath
        {
            get { return Path.GetFullPath(ProductRelPath); }
        }

        /// <summary>
        /// StreamingAssetsPath/Bundles/Android/ etc.
        /// WWW的读取，是需要Protocol前缀的
        /// </summary>
        public static string ProductPathWithProtocol { get; private set; }

        public static string ProductPathWithoutFileProtocol { get; private set; }

        /// <summary>
        /// Bundles/Android/ etc... no prefix for streamingAssets
        /// </summary>
        public static string BundlesPathRelative { get; private set; }

        public static string ApplicationPath { get; private set; }

        public static string DocumentResourcesPathWithoutFileProtocol
        {
            get
            {
                return Application.persistentDataPath; ; // 各平台通用
            }
        }

        public static string DocumentResourcesPath;

        private static string _unityEditorEditorUserBuildSettingsActiveBuildTarget;

        /// <summary>
        /// UnityEditor.EditorUserBuildSettings.activeBuildTarget, Can Run in any platform~
        /// </summary>
        public static string UnityEditor_EditorUserBuildSettings_activeBuildTarget
        {
            get
            {
                if (Application.isPlaying && !string.IsNullOrEmpty(_unityEditorEditorUserBuildSettingsActiveBuildTarget))
                {
                    return _unityEditorEditorUserBuildSettingsActiveBuildTarget;
                }
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var a in assemblies)
                {
                    if (a.GetName().Name == "UnityEditor")
                    {
                        Type lockType = a.GetType("UnityEditor.EditorUserBuildSettings");
                        var p = lockType.GetProperty("activeBuildTarget");

                        var em = p.GetGetMethod().Invoke(null, new object[] { }).ToString();
                        _unityEditorEditorUserBuildSettingsActiveBuildTarget = em;
                        return em;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Different platform's assetBundles is incompatible.
        /// CosmosEngine put different platform's assetBundles in different folder.
        /// Here, get Platform name that represent the AssetBundles Folder.
        /// </summary>
        /// <returns>Platform folder Name</returns>
        public static string GetBuildPlatformName()
        {
            string buildPlatformName = "Windows"; // default

            if (Application.isEditor)
            {
                var buildTarget = UnityEditor_EditorUserBuildSettings_activeBuildTarget;
                //UnityEditor.EditorUserBuildSettings.activeBuildTarget;
                switch (buildTarget)
                {
                    case "StandaloneOSXIntel":
                    case "StandaloneOSXIntel64":
                    case "StandaloneOSXUniversal":
                        buildPlatformName = "MacOS";
                        break;
                    case "StandaloneWindows": // UnityEditor.BuildTarget.StandaloneWindows:
                    case "StandaloneWindows64": // UnityEditor.BuildTarget.StandaloneWindows64:
                        buildPlatformName = "Windows";
                        break;
                    case "Android": // UnityEditor.BuildTarget.Android:
                        buildPlatformName = "Android";
                        break;
                    case "iPhone": // UnityEditor.BuildTarget.iPhone:
                    case "iOS":
                        buildPlatformName = "iOS";
                        break;
                    default:
                        Debugger.Assert(false);
                        break;
                }
            }
            else
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.OSXPlayer:
                        buildPlatformName = "MacOS";
                        break;
                    case RuntimePlatform.Android:
                        buildPlatformName = "Android";
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        buildPlatformName = "iOS";
                        break;
                    case RuntimePlatform.WindowsPlayer:
#if !UNITY_5_4_OR_NEWER
                    case RuntimePlatform.WindowsWebPlayer:
#endif
                        buildPlatformName = "Windows";
                        break;
                    default:
                        Debugger.Assert(false);
                        break;
                }
            }

            return buildPlatformName;
        }

        /// <summary>
        /// On Windows, file protocol has a strange rule that has one more slash
        /// </summary>
        /// <returns>string, file protocol string</returns>
        public static string GetFileProtocol()
        {
            string fileProtocol = "file://";
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer)
                fileProtocol = "file:///";

            return fileProtocol;
        }

        /// <summary>
        /// 统一在字符串后加上.box, 取决于配置的AssetBundle后缀
        /// </summary>
        /// <param name="path"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        public static string GetAssetBundlePath(string path, params object[] formats)
        {
            return string.Format(path + Configs.AssetBundleExt, formats);
        }


        /// <summary>
        /// 完整路径，www加载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="inAppPathType"></param>
        /// <param name="withFileProtocol">是否带有file://前缀</param>
        /// <param name="isLog"></param>
        /// <returns></returns>
        public static string GetResourceFullPath(string url, bool withFileProtocol = true, bool isLog = true)
        {
            string fullPath;
            if (GetResourceFullPath(url, withFileProtocol, out fullPath, isLog) != ResourceModule.GetResourceFullPathType.Invalid)
                return fullPath;

            return null;
        }

        /// <summary>
        /// 根据相对路径，获取到StreamingAssets完整路径，或Resources中的路径
        /// </summary>
        public static GetResourceFullPathType GetResourceFullPath(string url, bool withFileProtocol, out string fullPath,
            bool isLog = true)
        {
            if (string.IsNullOrEmpty(url))
                Log.Error("尝试获取一个空的资源路径！");

            string docUrl;
            bool hasDocUrl = TryGetDocumentResourceUrl(url, withFileProtocol, out docUrl);

            string inAppUrl;
            bool hasInAppUrl = TryGetInAppStreamingUrl(url, withFileProtocol, out inAppUrl);

            if (Configs.ResourcePathPriorityType == KResourcePathPriorityType.PersistentDataPathPriority) // 優先下載資源模式
            {
                if (hasDocUrl)
                {
                    if (Application.isEditor)
                        Log.Warning("[Use PersistentDataPath] {0}", docUrl);
                    fullPath = docUrl;
                    return ResourceModule.GetResourceFullPathType.InDocument;
                }
                // 優先下載資源，但又沒有下載資源文件！使用本地資源目錄 
            }

            if (!hasInAppUrl) // 连本地资源都没有，直接失败吧 ？？ 沒有本地資源但又遠程資源？竟然！!?
            {
                if (isLog)
                    Log.Error("[Not Found] StreamingAssetsPath Url Resource: {0}", url);
                fullPath = null;
                return ResourceModule.GetResourceFullPathType.Invalid;
            }

            fullPath = inAppUrl; // 直接使用本地資源！

            return ResourceModule.GetResourceFullPathType.InApp;
        }

        // 检查资源是否存在
        public static bool ContainsResourceUrl(string resourceUrl)
        {
            string fullPath;
            return GetResourceFullPath(resourceUrl, false, out fullPath, false) != ResourceModule.GetResourceFullPathType.Invalid;
        }

        /// <summary>
        /// 可被WWW读取的Resource路径
        /// </summary>
        /// <param name="url"></param>
        /// <param name="withFileProtocol">是否带有file://前缀</param>
        /// <param name="newUrl"></param>
        /// <returns></returns>
        public static bool TryGetDocumentResourceUrl(string url, bool withFileProtocol, out string newUrl)
        {
            if (withFileProtocol)
                newUrl = DocumentResourcesPath + url;
            else
                newUrl = DocumentResourcesPathWithoutFileProtocol + url;

            if (File.Exists(DocumentResourcesPathWithoutFileProtocol + url))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// (not android ) only! Android资源不在目录！
        /// Editor返回文件系统目录，运行时返回StreamingAssets目录
        /// </summary>
        /// <param name="url"></param>
        /// <param name="withFileProtocol">是否带有file://前缀</param>
        /// <param name="newUrl"></param>
        /// <returns></returns>
        public static bool TryGetInAppStreamingUrl(string url, bool withFileProtocol, out string newUrl)
        {
            if (withFileProtocol)
                newUrl = ProductPathWithProtocol + url;
            else
                newUrl = ProductPathWithoutFileProtocol + url;

            // 注意，StreamingAssetsPath在Android平台時，壓縮在apk里面，不要做文件檢查了
            if (!Application.isEditor && Application.platform == RuntimePlatform.Android)
            {
                if (!ADAndroidPlugin.IsAssetExists(url))
                    return false;
            }
            else
            {
                // Editor, 非android运行，直接进行文件检查
                if (!File.Exists(ProductPathWithoutFileProtocol + url))
                {
                    return false;
                }
            }

            // Windows/Edtiro平台下，进行大小敏感判断
            if (Application.isEditor)
            {
                var result = FileExistsWithDifferentCase(ProductPathWithoutFileProtocol + url);
                if (!result)
                {
                    Log.Error("[大小写敏感]发现一个资源 {0}，大小写出现问题，在Windows可以读取，手机不行，请改表修改！", url);
                }
            }
            return true;
        }

        /// <summary>
        /// 大小写敏感地进行文件判断, Windows Only
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool FileExistsWithDifferentCase(string filePath)
        {
            if (File.Exists(filePath))
            {
                string directory = Path.GetDirectoryName(filePath);
                string fileTitle = Path.GetFileName(filePath);
                string[] files = Directory.GetFiles(directory, fileTitle);
                var realFilePath = files[0].Replace("\\", "/");
                filePath = filePath.Replace("\\", "/");

                return String.CompareOrdinal(realFilePath, filePath) == 0;
            }
            return false;
        }

        /// <summary>
        /// Initialize the path of AssetBundles store place ( Maybe in PersitentDataPath or StreamingAssetsPath )
        /// </summary>
        /// <returns></returns>
        static void InitResourcePath()
        {

            string editorProductPath = EditorProductFullPath;

            BundlesPathRelative = string.Format("{0}/{1}/", BundlesDirName, GetBuildPlatformName());
            DocumentResourcesPath = FileProtocol + DocumentResourcesPathWithoutFileProtocol;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    {
                        ApplicationPath = string.Format("{0}{1}", GetFileProtocol(), editorProductPath);
                        ProductPathWithProtocol = GetFileProtocol() + EditorProductFullPath + "/";
                        ProductPathWithoutFileProtocol = EditorProductFullPath + "/";
                        // Resources folder
                    }
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                    {
                        string path = Application.streamingAssetsPath.Replace('\\', '/');//Application.dataPath.Replace('\\', '/');
                        //                        path = path.Substring(0, path.LastIndexOf('/') + 1);
                        ApplicationPath = string.Format("{0}{1}", GetFileProtocol(), Application.dataPath);
                        ProductPathWithProtocol = string.Format("{0}{1}/", GetFileProtocol(), path);
                        ProductPathWithoutFileProtocol = string.Format("{0}/", path);
                        // Resources folder
                    }
                    break;
                case RuntimePlatform.Android:
                    {
                        ApplicationPath = string.Concat("jar:", GetFileProtocol(), Application.dataPath, "!/assets");
                        ProductPathWithProtocol = string.Concat(ApplicationPath, "/");
                        ProductPathWithoutFileProtocol = string.Concat(Application.dataPath,
                            "!/assets/");
                        // 注意，StramingAsset在Android平台中，是在壓縮的apk里，不做文件檢查
                        // Resources folder
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    {
                        ApplicationPath =
                            System.Uri.EscapeUriString(GetFileProtocol() + Application.streamingAssetsPath); // MacOSX下，带空格的文件夹，空格字符需要转义成%20

                        ProductPathWithProtocol = string.Format("{0}/", ApplicationPath);
                        // only iPhone need to Escape the fucking Url!!! other platform works without it!!! Keng Die!
                        ProductPathWithoutFileProtocol = Application.streamingAssetsPath + "/";
                        // Resources folder
                    }
                    break;
                default:
                    {
                        Debugger.Assert(false);
                    }
                    break;
            }
        }
    }
}