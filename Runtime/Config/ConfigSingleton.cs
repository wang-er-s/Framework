using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace Framework
{

    public abstract class ConfigSingleton<T> : ISingleton , ISupportInitialize where T : ConfigSingleton<T>, new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    var configPath = (typeof(T).GetCustomAttribute(typeof(ConfigAttribute)) as ConfigAttribute)
                        .Path;
                    var bytes = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(configPath).text;
                    var method = typeof(T).GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic);
                    instance = new T();
                    method.Invoke(instance, new[] { bytes });
                }
                else
#endif
                {
                    instance = ConfigComponent.Instance.LoadOneConfig(typeof(T)) as T; 
                }

                return instance;
            }
        }

        void ISingleton.Register()
        {
            if (instance != null)
            {
                throw new Exception($"singleton register twice! {typeof(T).Name}");
            }

            instance = (T)this;
        }

        void ISingleton.Destroy()
        {
            T t = instance;
            instance = null;
            t.Dispose();
        }

        bool ISingleton.IsDisposed()
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
        }

        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }
    }
}