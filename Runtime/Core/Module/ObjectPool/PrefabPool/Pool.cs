using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Pool<T>
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
            var obj = CacheStack.Count == 0 ? Factory() : CacheStack.Pop();
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
}
