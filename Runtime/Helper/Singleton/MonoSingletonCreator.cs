using UnityEngine;

namespace Framework
{
    public class MonoSingletonCreator
    {
        public static T CreateMonoSingleton<T>() where T : MonoBehaviour
        {
            T instance = null;

            instance = Object.FindObjectOfType<T>();

            if (instance != null)
            {
                return instance;
            }

            if (instance == null)
            {
                var obj = new GameObject(typeof(T).Name);
                if (Application.isPlaying)
                    Object.DontDestroyOnLoad(obj);
                instance = obj.AddComponent<T>();
            }
            
            return instance;
        }
    }
}