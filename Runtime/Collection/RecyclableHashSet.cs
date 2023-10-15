using System;
using System.Collections.Generic;

namespace Framework
{
    public class RecyclableHashSet<T> : HashSet<T>, IDisposable ,IReference
    {
        private RecyclableHashSet(){}
        public static RecyclableHashSet<T> Create()
        {
            var result = ReferencePool.Allocate<RecyclableHashSet<T>>();
            return result;
        }

        void IReference.Clear()
        {
            Clear();
        }

        public void Dispose()
        {
            ReferencePool.Free(this);
        }
    }
}