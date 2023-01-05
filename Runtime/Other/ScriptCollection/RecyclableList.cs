using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class RecyclableList<T> : List<T>, IDisposable
    {
        private bool disposed = false;

        private RecyclableList(IEnumerable<T> collection) : base(collection)
        {
        }

        private RecyclableList(int count) : base(count)
        {
        }

        private static List<RecyclableList<T>> cache = new();

        public static RecyclableList<T> Create(int count = -1)
        {
            RecyclableList<T> result = null;
            if (cache.Count > 0)
            {
                if (count != -1)
                {
                    for (int i = 0; i < cache.Count; i++)
                    {
                        if (cache[i].Capacity == count)
                        {
                            result = cache[i];
                            cache.RemoveAt(i);
                            break;
                        }
                    }
                }

                if (result == null)
                {
                    result = cache.RemoveLast();
                }
            }

            if (result == null)
                result = new RecyclableList<T>(count == -1 ? 6 : count);

            result.disposed = false;
            return result;
        }

        public static RecyclableList<T> Create(IEnumerable<T> collection, int count = -1)
        {
            var result = Create(count);
            if (collection != null)
                result.AddRange(collection);
            return result;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            this.Clear();
            if (cache.Count > 0)
            {
                var first = cache.First();
                cache[0] = this;
                cache.Add(first);
            }
            else
            {
                cache.Add(this);
            }
        }

        public override string ToString()
        {
            return $"[ {string.Join(" | ", this)} ]";
        }
    }
}