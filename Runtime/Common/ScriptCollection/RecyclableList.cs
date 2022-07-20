using System;
using System.Collections.Generic;
using Framework.Pool;

namespace Framework
{
    public class RecyclableList<T> : List<T>, IDisposable
    {
        private static Pool<RecyclableList<T>> pool;

        public static RecyclableList<T> Create()
        {
            if (pool == null)
            {
                pool = new Pool<RecyclableList<T>>(() => new RecyclableList<T>());
            }
            return pool.Allocate();
        }
        
        public static RecyclableList<T> Create(IEnumerable<T> collection)
        {
            var result = Create();
            result.AddRange(collection);
            return result;
        }

        private RecyclableList() : base()
        {
        }

        private RecyclableList(IEnumerable<T> collection) : base(collection)
        {
        }

        public void Dispose()
        {
            Clear();
            pool.Free(this);
        }
    }
}