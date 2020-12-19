using System.IO;
using UnityEngine;
using UnityIO;

namespace Framework
{
    
    public class ConfigBase : ScriptableObject
    {
        public static ConfigBase Load<T>() where T : ConfigBase
        {
            IO.Root.CreateDirectory("Resources/Config");
            var path = $"Config/{typeof(T).Name}";
            T config = Resources.Load(path,typeof(T)) as T;
            if (config == null)
            {
                config = CreateInstance(typeof(T)) as T;
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.CreateAsset(config, path + ".asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
#endif
            }

            return config;
        }
    }
}