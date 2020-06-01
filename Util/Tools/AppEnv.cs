using System.Collections.Generic;
using System.IO;
using Framework.BaseUtil;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    public static class AppEnv
    {
        private static EnvSetting setting;
        private static string resPath;
        private static string runPath;
        private static string luaRootPath;
        private static List<string> buildScenes;

        private static string EditorResPath => null == setting ? null : setting.editorResPath;
        public static bool UseBundleInEditor => null != setting && setting.useBundleInEditor;
        private static bool UseStreamingAssetsInEditor => null != setting && setting.useStreamingAssetsInEditor;
        public static bool UseOriginalData => null != setting && setting.useOriginalData;

        public static bool IsDev => !Application.isMobilePlatform && null != setting && setting.isDev;

        public static string LuaRootPath => luaRootPath;

        public static bool ResVerbose => null != setting && setting.resVerbose;

#if UNITY_EDITOR
        public static bool UseTrdBundlePath => EditorPrefs.GetBool("UseTrdBundlePath", false);
#else
        public static bool UseTrdBundlePath => !Application.isMobilePlatform && setting != null && setting.useTrdBundlePath;
#endif

        public static void Init(EnvSetting _setting)
        {
            setting = _setting;
            Setup();
            FetchBuildScenes();
        }

        public static void Release()
        {
            setting = null;
            buildScenes = null;
        }

        public static bool IsSceneInBuild(string levelName)
        {
            return null != buildScenes && buildScenes.Contains(levelName);
        }

        private static string NormalizePath(string path, string defaultPath = null, string relativeBase = null)
        {
            relativeBase = Application.dataPath ?? relativeBase;
            if (string.IsNullOrEmpty(path))
                return defaultPath;

            if (path.StartsWith("./") || path.StartsWith("../"))
            {
                return Path.GetFullPath(Path.Combine(relativeBase, path));
            }
            return path;
        }
        
        private static void Setup()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            //respath
            resPath = UseStreamingAssetsInEditor
                ? Application.streamingAssetsPath
                : NormalizePath(EditorResPath, Application.streamingAssetsPath);
            //run path
            runPath = NormalizePath("../_run");
            if (!Directory.Exists(runPath))
                Directory.CreateDirectory(runPath);
            //trd bundle path
            string trdBundlePath = null;
            if (UseTrdBundlePath)
            {
                trdBundlePath = null == setting ? null : setting.trdBundlePath;
                trdBundlePath = NormalizePath(trdBundlePath);
            }
            BundleConfig.TrdPath = trdBundlePath;
#else
            resPath = Application.streamingAssetsPath;
            runPath = Application.persistentDataPath;
#endif
            Log.Msg($"use resPath = {resPath},runPath = {runPath}");
            FileUtility.Init(resPath,runPath,null,"archeage");
        }

        private static void FetchBuildScenes()
        {
            if(null == buildScenes)
                buildScenes = new List<string>();
            else
                buildScenes.Clear();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                //int lastSlash = scenePath.LastIndexOf('/');
                //string name = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf('.') - lastSlash - 1);
                buildScenes.Add(scenePath);
            }
        }
    }
}