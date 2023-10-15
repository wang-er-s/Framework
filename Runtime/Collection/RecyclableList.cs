using System;
using System.Collections.Generic;

namespace Framework
{
    public class RecyclableList<T> : List<T>, IDisposable , IReference
    {
        private RecyclableList()
        {
        }

        public static RecyclableList<T> Create()
        {
            var result = ReferencePool.Allocate<RecyclableList<T>>();
            return result;
        }

        public static RecyclableList<T> Create(IEnumerable<T> collection)
        {
            var result = Create();
            result.AddRange(collection);
            return result;
        }
        
        public override string ToString()
        {
            return $"[ {string.Join(" | ", this)} ]";
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