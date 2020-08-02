namespace Framework
{
    using UnityEngine;

    public static class MonoSingletonProperty<T> where T : MonoBehaviour
    {
        private static T _instance = null;

        public static T Ins
        {
            get
            {
                if (null == _instance)
                {
                    _instance = MonoSingletonCreator.CreateMonoSingleton<T>();
                }

                return _instance;
            }
        }

        public static void Dispose()
        {
            Object.Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}