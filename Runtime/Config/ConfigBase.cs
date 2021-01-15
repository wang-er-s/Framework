using System.IO;
using UnityEngine;
using UnityIO;

namespace Framework
{
    public class ConfigBase : ScriptableObject
    {
        public static T Load<T>() where T : ScriptableObject
        {
            IO.Root.CreateDirectory("Resources/Config");
            var path = $"Config/{typeof(T).Name}";
            T config = Resources.Load(path,typeof(T)) as T;
            if (config == null)
            {
                Debug.Log("创建了新的");
                config = CreateInstance(typeof(T)) as T;
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.CreateAsset(config, "Assets/Resources/" + path + ".asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
#endif
            }

            return config;
        }
    }
}