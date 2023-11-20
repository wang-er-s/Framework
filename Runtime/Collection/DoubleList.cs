using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class DoubleList<K, V> : IReference, IEnumerable<KeyValuePair<K, V>>
    {
        private readonly List<K> keyList;
        private readonly List<V> valueList;

        public int Count => keyList.Count;

        public DoubleList(int capacity)
        {
            keyList = new List<K>(capacity);
            valueList = new List<V>(capacity);
        }

        public DoubleList()
        {
            keyList = new List<K>();
            valueList = new List<V>();
        }

        public void Add(K key, V value)
        {
            keyList.Add(key);
            valueList.Add(value);
        }

        public bool Remove(K key)
        {
            int index = keyList.IndexOf(key);
            if (index < 0)
            {
                return false;
            }

            keyList.RemoveAt(index);
            valueList.RemoveAt(index);
            return true;
        }

        public bool Contains(K key)
        {
            return keyList.Contains(key);
        }

        public bool TryGetValue(K key, out V value)
        {
            value = default;
            int index = keyList.IndexOf(key);
            if (index < 0)
            {
                return false;
            }

            value = valueList[index];
            return true;
        }

        public KeyValuePair<K, V> GetKVByIndex(int index)
        {
            if (index < 0 || index >= keyList.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return new KeyValuePair<K, V>(keyList[index], valueList[index]);
        }

        public K GetKeyByIndex(int index)
        {
            if (index < 0 || index >= keyList.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return keyList[index];
        }

        public V GetValueByIndex(int index)
        {
            if (index < 0 || index >= keyList.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return valueList[index];
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            for (int i = 0; i < keyList.Count; i++)
            {
                var key = keyList[i];
                var value = valueList[i];
                yield return new KeyValuePair<K, V>(key, value);
            }
        }

        public void Clear()
        {
            keyList.Clear();
            valueList.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}