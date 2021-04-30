using System;
using System.Collections.Generic;
using Framework.Asynchronous;
using Framework.Pool.Factory;

namespace Framework.Pool
{
    public abstract class Pool<T> : IPool<T>
    {
        public int CurCount => CacheStack.Count;

        protected IFactory<T> Factory;

        protected readonly Stack<T> CacheStack = new Stack<T>();
        protected Action<T> OnAlloc;
        protected Action<T> OnFree;
        
        protected Pool(IFactory<T> factory, Action<T> onAlloc, Action<T> onFree)
        {
            Factory = factory;
            OnAlloc = onAlloc;
            OnFree = onFree;
        }

        public virtual void Free(T obj)
        {
            OnFree?.Invoke(obj);
            CacheStack.Push(obj);
        }

        public virtual T Allocate()
        {
            var obj = CacheStack.Count == 0 ?
                Factory.Create() : CacheStack.Pop();
            OnAlloc?.Invoke(obj);
            return obj;
        }
    }
    
    public abstract class AsyncPool<T> : IAsyncPool<T>
    {
        public int CurCount => CacheStack.Count;

        protected IAsyncFactory<T> Factory;

        protected readonly Stack<T> CacheStack = new Stack<T>();

        protected Action<T> OnAlloc;
        protected Action<T> OnFree;

        public virtual void Free(T obj)
        {
            OnFree?.Invoke(obj);
            CacheStack.Push(obj);
        }

        protected AsyncPool(IAsyncFactory<T> factory, Action<T> onAlloc, Action<T> onFree)
        {
            Factory = factory;
            OnAlloc = onAlloc;
            OnFree = onFree;
        }
        
        public virtual AsyncResult<T> Allocate()
        {
            var result = CacheStack.Count == 0 ? Factory.Create() : new AsyncResult<T>();
            result.Callbackable().OnCallback(asyncResult => OnAlloc?.Invoke(asyncResult.Result));
            result.SetResult(CacheStack.Pop());
            return result;
        }
    }
}
