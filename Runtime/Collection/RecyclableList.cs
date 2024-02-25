using System;
using System.Collections.Generic;

namespace Framework
{
    public class RecyclableList<T> : List<T>, IDisposable , IReference
    {
        public static RecyclableList<T> Create()
        {
            var result = ReferencePool.Allocate<RecyclableList<T>>();
            return result;
        }
        
        public static RecyclableList<T> Create(T value)
        {
            var result = Create();
            result.Add(value);
            return result;
        } 
        
        public static RecyclableList<T> Create(T value1, T value2)
        {
            var result = Create();
            result.Add(value1);
            result.Add(value2);
            return result;
        } 
        
        public static RecyclableList<T> Create(T value1, T value2, T value3)
        {
            var result = Create();
            result.Add(value1);
            result.Add(value2);
            result.Add(value3);
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