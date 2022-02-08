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
        
        public void Dispose()
        {
            Clear();
            pool.Free(this);
        }
    }
}