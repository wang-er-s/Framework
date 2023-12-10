using System;
using System.Collections.Generic;

namespace Framework
{
    public class RecyclableHashSet<T> : HashSet<T>, IDisposable ,IReference
    {
        public static RecyclableHashSet<T> Create()
        {
            var result = ReferencePool.Allocate<RecyclableHashSet<T>>();
            return result;
        }
        public static RecyclableHashSet<T> Create(T value)
        {
            var result = Create();
            result.Add(value);
            return result;
        } 
        
        public static RecyclableHashSet<T> Create(T value1, T value2)
        {
            var result = Create();
            result.Add(value1);
            result.Add(value2);
            return result;
        } 
        
        public static RecyclableHashSet<T> Create(T value1, T value2, T value3)
        {
            var result = Create();
            result.Add(value1);
            result.Add(value2);
            result.Add(value3);
            return result;
        } 

        public static RecyclableHashSet<T> Create(IEnumerable<T> collection)
        {
            var result = Create();
            foreach (T val in collection)
            {
                result.Add(val);
            }
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