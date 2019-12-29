namespace AD
{
    using UnityEngine;

    public static class MonoSingletonProperty<T> where T : MonoBehaviour
    {
        private static T mInstance = null;

        public static T Ins
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
                }

                return mInstance;
            }
        }

        public static void Dispose()
        {
            Object.Destroy(mInstance.gameObject);
            mInstance = null;
        }
    }
}