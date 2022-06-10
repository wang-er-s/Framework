using System;
using System.Collections.Generic;
using Framework.Pool;

namespace Framework
{
    public class RecyclableList<T> : List<T>, IDisposable
    {
        private static Pool<RecyclableList<T>> pool;

        public static RecyclableList<T> Create(int count = 4)
        {
            if (pool == null)
            {
                pool = new Pool<RecyclableList<T>>(() => new RecyclableList<T>(count));
            }
            return pool.Allocate();
        }

        private RecyclableList(int count) : base(count)
        {
        }

        public void Dispose()
        {
            Clear();
            pool.Free(this);
        }
    }
}