using System;

namespace Framework
{
    public interface ISingleton : IDisposable
    {
        void Register();
        void Destroy();
        bool IsDisposed();
    }

    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private bool isDisposed;
        private static T instance;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!UnityEngine.Application.isPlaying)
                {
                    instance ??= Activator.CreateInstance<T>();
                }
#endif
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
            if (this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;

            instance.Dispose();
            instance = null;
        }

        bool ISingleton.IsDisposed()
        {
            return this.isDisposed;
        }

        public virtual void Dispose()
        {
        }
    }
}