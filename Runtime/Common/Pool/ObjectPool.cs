using System;
using Framework.Pool.Factory;

namespace Framework.Pool
{
    public class ObjectPool<T> : Pool<T>
    {
        public ObjectPool(IFactory<T> factory, Action<T> onAlloc, Action<T> onFree) : base(factory, onAlloc, onFree)
        {
        }
    }

    public class AsyncObjectPool<T> : AsyncPool<T>
    {
        public override void Free(T obj)
        {
            
        }

        public AsyncObjectPool(IAsyncFactory<T> factory, Action<T> onAlloc, Action<T> onFree) : base(factory, onAlloc, onFree)
        {
        }
    }
}
