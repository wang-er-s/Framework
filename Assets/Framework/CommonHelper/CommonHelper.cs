using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.CommonHelper
{
    public static class CommonHelper
    {
        /// <summary>
        /// 资源清理和垃圾回收
        /// </summary>
        public static void ClearMemory()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        #region Editor

#if UNITY_EDITOR

        public static GameObject[] GetCurSceneRootObjs()
        {
            Scene curScene = SceneManager.GetActiveScene();
            return curScene.GetRootGameObjects();
        }

        public static T LoadScriptableAsset<T>(string assetPath, bool createDefault = true) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath + ".asset");
            if (asset == null && createDefault)
            {
                asset = CreateScriptableAsset<T>(assetPath);
            }

            return asset;
        }

        public static T CreateScriptableAsset<T>(string assetPath, T obj = null) where T : ScriptableObject
        {
            if (obj == null) obj = ScriptableObject.CreateInstance<T>();
            int pIndex = assetPath.LastIndexOfAny(new[] {'\\', '/'});
            if (pIndex > 0)
            {
                string parentFolderPath = assetPath.Substring(0, pIndex);
                if (!Directory.Exists(parentFolderPath))
                {
                    Directory.CreateDirectory(parentFolderPath);
                }
            }

            AssetDatabase.CreateAsset(obj, assetPath + ".asset");
            return obj;
        }

        public static void ReimportAsset<T>(bool includePackages = true) where T : UnityEngine.Object
        {
            string filter = string.Format("t:{0}", typeof(T)).Replace("UnityEngine.", "");
            string progressTitle = "reimport assets:" + filter;
            string[] searchFolders = includePackages ? null : new[] {"Assets"};
            string[] guids = AssetDatabase.FindAssets(filter, searchFolders);
            EditorUtility.DisplayProgressBar(progressTitle, "", 0);
            Debug.Log($"waiting for {guids.Length} objs to import");
            for (int i = 0; i < guids.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                EditorUtility.DisplayProgressBar(progressTitle, assetPath, (float) i / guids.Length);
                AssetDatabase.ImportAsset(assetPath);
            }

            EditorUtility.ClearProgressBar();
            Debug.Log("reimport done");
        }
#endif

        #endregion
    }
}