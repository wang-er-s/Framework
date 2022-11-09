using System;
using System.Collections.Generic;

namespace Framework
{
    public class RecyclableDic<TKey,TValue> : Dictionary<TKey, TValue> , IDisposable
    {
        private RecyclableDic()
        {
        }
        private static Queue<RecyclableDic<TKey, TValue>> cache = new();

        public static RecyclableDic<TKey,TValue> Create()
        {
            RecyclableDic<TKey,TValue> result = null;
            if (cache.Count > 0)
            {
                result = cache.Dequeue();
            }
            if (result == null)
                result = new RecyclableDic<TKey, TValue>(); 
            return result;
        }

        public void Dispose()
        {
            this.Clear();
            cache.Enqueue(this);
        }
    }
}