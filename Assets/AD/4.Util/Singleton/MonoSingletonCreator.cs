using System.Reflection;
using UnityEngine;

namespace AD
{
    public class MonoSingletonCreator
    {
        public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
        {
            T instance = null;

            instance = Object.FindObjectOfType<T>();

            if (instance != null)
            {
                instance.OnSingletonInit();
                return instance;
            }

            if (instance == null)
            {
                var obj = new GameObject(typeof(T).Name);
                Object.DontDestroyOnLoad(obj);
                instance = obj.AddComponent<T>();
            }

            instance.OnSingletonInit();
            return instance;
        }
    }
}