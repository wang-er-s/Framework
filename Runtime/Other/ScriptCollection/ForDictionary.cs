using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    public class ForDictionary<TKey, TValue> :
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IEnumerable,
        IDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        IReadOnlyDictionary<TKey, TValue>,
        ICollection,
        IDictionary
    {

        private Dictionary<TKey, TValue> dic;
        private List<TKey> keyList;

        public ForDictionary()
        {
            dic = new Dictionary<TKey, TValue>();
            keyList = new List<TKey>();
        }

        public ForDictionary(ForDictionary<TKey, TValue> dictionary)
        {
            dic = new Dictionary<TKey, TValue>(dictionary);
            keyList = new List<TKey>(dictionary.keyList);
        }

        public ForDictionary(int capacity)
        {
            dic = new Dictionary<TKey, TValue>(capacity);
            keyList = new List<TKey>(capacity);
        }

        public bool Contains(object key)
        {
            return dic.ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new Exception("使用For来代替");
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        bool IDictionary.IsFixedSize => false;
        bool IDictionary.IsReadOnly => false;
        object IDictionary.this[object key]
        {
            get => dic[(TKey)key];
            set => Add((TKey)key, (TValue)value);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            throw new Exception("使用For来代替");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new Exception("使用For来代替");
        }

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            keyList.Add(item.Key);
            ((ICollection<KeyValuePair<TKey, TValue>>)dic).Add(item);
        }

        public void Clear()
        {
            dic.Clear();
            keyList.Clear();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            dic.Clear();
            keyList.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dic).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dic).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            keyList.Remove(item.Key);
            return ((ICollection<KeyValuePair<TKey, TValue>>)dic).Remove(item);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)dic).CopyTo(array, index);
        }

        int ICollection.Count => dic.Count;
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => false;
        int ICollection<KeyValuePair<TKey, TValue>>.Count => dic.Count;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public void Foreach(Action<TKey, TValue> action)
        {
            for (int i = 0; i < Count; i++)
            {
                var key = keyList[i];
                action(key, dic[key]);
            }
        }
        
        /// <summary>
        /// 返回值为true的时候会跳出循环
        /// </summary>
        /// <param name="action"></param>
        public void Foreach(Func<TKey, TValue, bool> action)
        {
            for (int i = 0; i < Count; i++)
            {
                var key = keyList[i];
                if(action(key, dic[key])) break;
            }
        }

        /// <summary>
        /// 返回值为true的时候会跳出循环
        /// </summary>
        /// <param name="action"></param>
        public void ForeachKey(Func<TKey,bool> action)
        {
            for (int i = 0; i < Count; i++)
            {
                if(action(keyList[i])) break;
            }
        }
        
        public void ForeachKey(Action<TKey> action)
        {
            for (int i = 0; i < Count; i++)
            {
                action(keyList[i]);
            }
        }

        /// <summary>
        /// 返回值为true的时候会跳出循环
        /// </summary>
        /// <param name="action"></param>
        public void ForeachValue(Func<TValue,bool> action)
        {
            for (int i = 0; i < Count; i++)
            {
                if(action(dic[keyList[i]])) break;
            }
        }
        
        public void ForeachValue(Action<TValue> action)
        {
            for (int i = 0; i < Count; i++)
            {
                action(dic[keyList[i]]);
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (!dic.ContainsKey(key))
            {
                keyList.Add(key);
            }
            dic[key] = value;
        }

        public TValue GetByIndex(int index)
        {
            return dic[keyList[index]];
        }

        public bool ContainsKey(TKey key)
        {
            return dic.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dic.TryGetValue(key, out value);
        }

        public bool Remove(TKey key)
        {
            if (!dic.ContainsKey(key))
            {
                throw new KeyNotFoundException($"not exist key :{key}");
            }
            keyList.Remove(key);
            return dic.Remove(key);
        }

        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return dic.ContainsKey(key);
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return dic.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => dic[key];
            set => Add(key, value);
        }
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dic.Keys;
        ICollection IDictionary.Values => dic.Values;
        ICollection IDictionary.Keys => dic.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dic.Values;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => dic.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => dic.Values;
        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => dic.Count;
        public int Count => dic.Count;
    }
}