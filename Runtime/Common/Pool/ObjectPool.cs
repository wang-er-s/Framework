using System;
using Framework.Pool.Factory;

namespace Framework.Pool
{
    public class ObjectPool<T> : Pool<T>
    {
        public ObjectPool(IFactory<T> factory, Action<T> onAlloc, Action<T> onFree, Action<T> onRelease) : base(factory, onAlloc, onFree, onRelease)
        {
        }
    }

    public class AsyncObjectPool<T> : AsyncPool<T>
    {
        public AsyncObjectPool(IAsyncFactory<T> factory, Action<T> onAlloc, Action<T> onFree, Action<T> onRelease) : base(factory, onAlloc, onFree, onRelease)
        {
        }
    }
}
