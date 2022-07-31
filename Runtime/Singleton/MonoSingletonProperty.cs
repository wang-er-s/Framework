namespace Framework
{
    using UnityEngine;

    public static class MonoSingletonProperty<T> where T : MonoBehaviour
    {
        private static T instance = null;

        public static T Ins
        {
            get
            {
                if (null == instance)
                {
                    instance = MonoSingletonCreator.CreateMonoSingleton<T>();
                }

                return instance;
            }
        }

        public static void Dispose()
        {
            Object.Destroy(instance.gameObject);
            instance = null;
        }
    }
}