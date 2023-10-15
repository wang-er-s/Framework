#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;

namespace Framework
{
    public static class ScriptableAssetHelper
    {
#if UNITY_EDITOR

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
#endif
    }
}