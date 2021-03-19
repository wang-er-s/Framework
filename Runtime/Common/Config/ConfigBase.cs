using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public class ConfigBase : ScriptableObject
    {
        private static List<ScriptableObject> _configs = new List<ScriptableObject>();
        
        public static T Load<T>() where T : ScriptableObject
        {
            foreach (var conf in _configs)
            {
                if (conf is T result)
                    return result;
            }
            var path = $"Config/{typeof(T).Name}";
            T config = Resources.Load(path,typeof(T)) as T;
#if UNITY_EDITOR
            if (config == null)
            {
                Directory.CreateDirectory($"{FApplication.AssetsRoot}/Resources/Config");
                config = CreateInstance(typeof(T)) as T;
                UnityEditor.AssetDatabase.CreateAsset(config, "Assets/Resources/" + path + ".asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
#endif
            _configs.Add(config);
            return config;
        }
    }
}