using System;
using System.Collections.Generic;

namespace Framework
{
    public class MultiMap<K, V> : SortedDictionary<K, List<V>>
    {
        private readonly List<V> Empty = new List<V>();

        public void Add(K t, V k)
        {
            List<V> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                list = new List<V>();
                this.Add(t, list);
            }

            list.Add(k);
        }

        public bool Remove(K t, V k)
        {
            List<V> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }

            if (!list.Remove(k))
            {
                return false;
            }

            if (list.Count == 0)
            {
                this.Remove(t);
            }

            return true;
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public V[] GetAll(K t)
        {
            List<V> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return Array.Empty<V>();
            }

            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public new List<V> this[K t]
        {
            get
            {
                this.TryGetValue(t, out List<V> list);
                return list ?? Empty;
            }
        }


        public V GetOne(K t)
        {
            List<V> list;
            this.TryGetValue(t, out list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }

            return default;
        }

        public bool Contains(K t, V k)
        {
            List<V> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }

            return list.Contains(k);
        }
    }
}