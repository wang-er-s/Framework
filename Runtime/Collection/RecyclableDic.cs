using System;
using System.Collections.Generic;

namespace Framework
{

    public class RecyclableDic<TKey,TValue> : Dictionary<TKey, TValue> , IDisposable , IReference
    {
        private RecyclableDic()
        {
        }

        public static RecyclableDic<TKey,TValue> Create()
        {
            var result = ReferencePool.Allocate<RecyclableDic<TKey,TValue>>();
            return result;
        }

        void IReference.Clear()
        {
            this.Clear();
        }

        public void Dispose()
        {
            ReferencePool.Free(this);
        }
    }
}