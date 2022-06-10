using System;
using System.Collections.Generic;
using Framework.Asynchronous;
using UnityEngine;

namespace Framework.Pool
{
    public class Pool<T> : IPool<T>
    {
        protected Func<T> Factory;
        protected readonly Stack<T> CacheStack = new Stack<T>();
        protected Action<T> OnAlloc;
        protected Action<T> OnFree;
        protected Action<T> OnDispose;
        protected int InitCount;

        public Pool(Func<T> factory, int initCount = 1, Action<T> onAlloc = null, Action<T> onFree = null,
            Action<T> onDispose = null)
        {
            Factory = factory;
            OnAlloc = onAlloc;
            OnFree = onFree;
            OnDispose = onDispose;
            InitCount = initCount;
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < InitCount; i++)
            {
                Free(Factory());
            }
        }

        public virtual void Free(T obj)
        {
            OnFree?.Invoke(obj);
            CacheStack.Push(obj);
        }

        public virtual T Allocate()
        {
            var obj = CacheStack.Count == 0 ?
                Factory() : CacheStack.Pop();
            OnAlloc?.Invoke(obj);
            return obj;
        }

        public virtual void Dispose()
        {
            while (CacheStack.Count > 0)
            {
                T item = CacheStack.Pop();
                OnDispose?.Invoke(item);
            }
            CacheStack.Clear();
        }
    }
    
    public class PoolWithKey<TKey,T> : IPoolWithKey<TKey,T>
    {
        protected Func<TKey,T> Factory;

        protected readonly Dictionary<TKey, Stack<T>> CacheStack = new Dictionary<TKey, Stack<T>>();
        protected Action<T> OnAlloc;
        protected Action<T> OnFree;
        protected Action<T> OnDispose;

        protected PoolWithKey(Func<TKey, T> factory, Action<T> onAlloc = null, Action<T> onFree = null,
            Action<T> onDispose = null)
        {
            Factory = factory;
            OnAlloc = onAlloc;
            OnFree = onFree;
            OnDispose = onDispose;
        }

        public void Dispose()
        {
            foreach (var stack in CacheStack.Values)
            {
                while (stack.Count > 0)
                {
                    var item = stack.Pop();
                    OnDispose?.Invoke(item);
                }
            }
            CacheStack.Clear();
        }

        public virtual T Allocate(TKey key)
        {
            T result;
            if (!CacheStack.ContainsKey(key))
            {
                CacheStack[key] = new Stack<T>();
                result = Factory(key);
            }
            else
            {
                if (CacheStack[key].Count > 0)
                    result = CacheStack[key].Pop();
                else
                    result = Factory(key);
            }

            OnAlloc?.Invoke(result);
            return result;
        }

        public virtual void Free(TKey key, T obj)
        {
            OnFree?.Invoke(obj);
            CacheStack[key].Push(obj);
        }
    }
    
    public class AsyncPool<T> : IAsyncPool<T>
    {
        protected Func<AsyncResult<T>> Factory;
        protected readonly Stack<T> CacheStack = new Stack<T>();
        protected Action<T> OnAlloc;
        protected Action<T> OnFree;
        protected Action<T> OnDispose;

        public virtual void Free(T obj)
        {
            OnFree?.Invoke(obj);
            CacheStack.Push(obj);
        }

        protected AsyncPool(Func<AsyncResult<T>> factory, Action<T> onAlloc = null, Action<T> onFree = null,
            Action<T> onDispose = null)
        {
            Factory = factory;
            OnAlloc = onAlloc;
            OnFree = onFree;
            OnDispose = onDispose;
        }

        public virtual AsyncResult<T> Allocate()
        {
            var result = CacheStack.Count == 0 ? Factory() : new AsyncResult<T>();
            result.Callbackable().OnCallback(asyncResult => OnAlloc?.Invoke(asyncResult.Result));
            result.SetResult(CacheStack.Pop());
            return result;
        }

        public void Dispose()
        {
            while (CacheStack.Count > 0)
            {
                T item = CacheStack.Pop();
                OnDispose?.Invoke(item);
            }
            CacheStack.Clear();
        }
    }
}
