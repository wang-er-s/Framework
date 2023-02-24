using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CatJson;
using UnityEngine;

namespace Framework
{
    public class ObservableDictionary<TKey,TValue>  : IDictionary<TKey, TValue>, IDictionary , IObservable , IReference
    {
        
        private event Action<NotifyCollectionChangedAction, KeyValuePair<TKey, TValue>, KeyValuePair<TKey,TValue>> CollectionChanged;
        private event Action<Dictionary<TKey, TValue>> dicChanged;

        private Dictionary<TKey, TValue> dictionary;

        static ObservableDictionary()
        {
            JsonParser.AddCustomJsonFormatter(typeof(ObservableDictionary<,>), new DictionaryFormatter());
        }

        public ObservableDictionary()
        {
            this.dictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);
        }
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(comparer);
        }
        public ObservableDictionary(int capacity)
        {
            this.dictionary = new Dictionary<TKey, TValue>(capacity);
        }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!dictionary.ContainsKey(key))
                    return default;
                return dictionary[key];
            }
            set => Insert(key, value, false);
        }

        public ICollection<TKey> Keys => dictionary.Keys;

        public ICollection<TValue> Values => dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            TValue value;
            dictionary.TryGetValue(key, out value);
            var removed = dictionary.Remove(key);
            if (removed)
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, default,
                    new KeyValuePair<TKey, TValue>(key, value));
            }

            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public bool TryRemove(TKey key)
        {
            if (ContainsKey(key))
            {
                Remove(key);
                return true;
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, true);
        }

        public void AddListener(
            Action<NotifyCollectionChangedAction, KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> changeCb)
        {
            CollectionChanged += changeCb;
        }

        public UnRegister AddListener(Action<Dictionary<TKey, TValue>> changeCb)
        {
            changeCb(dictionary);
            dicChanged += changeCb;
            return new UnRegister(() => dicChanged -= changeCb);
        }
        
        public UnRegister AddListenerWithoutCall(Action<Dictionary<TKey, TValue>> changeCb)
        {
            dicChanged += changeCb;
            return new UnRegister(() => dicChanged -= changeCb);
        }

        public void RemoveListener(
            Action<NotifyCollectionChangedAction, KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> changeCb)
        {
            CollectionChanged -= changeCb;
        }

        public void RemoveListener(Action<Dictionary<TKey, TValue>> changeCb)
        {
            dicChanged -= changeCb;
        }

        public void ClearListener()
        {
            CollectionChanged = null;
            dicChanged = null;
        }
        
        public void Clear()
        {
            if (dictionary.Count > 0)
            {
                dictionary.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset, default, default);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary)this.dictionary).CopyTo(array, arrayIndex);
        }

        public int Count => dictionary.Count;

        public bool IsReadOnly => ((IDictionary)this.dictionary).IsReadOnly;

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)dictionary).GetEnumerator();
        }

        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (items.Count > 0)
            {
                if (this.dictionary.Count > 0)
                {
                    if (items.Keys.Any((k) => this.dictionary.ContainsKey(k)))
                        throw new ArgumentException("An item with the same key has already been added.");
                    else
                    {
                        foreach (var item in items)
                            ((IDictionary<TKey, TValue>)this.dictionary).Add(item);
                    }
                }
                else
                {
                    this.dictionary = new Dictionary<TKey, TValue>(items);
                }

                foreach (var value in items)
                {
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, value, default);
                }
            }
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (dictionary.TryGetValue(key, out var item))
            {
                if (add)
                    throw new ArgumentException("An item with the same key has already been added.");

                if (Equals(item, value))
                    return;

                dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), default);
            }
        }
        

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            CollectionChanged?.Invoke(action, newItem, oldItem);
            dicChanged?.Invoke(dictionary);
        }
        
        void IReference.Clear()
        {
            dictionary.Clear();
            ClearListener();
        } 
        

        object IDictionary.this[object key]
        {
            get => ((IDictionary)this.dictionary)[key];
            set => Insert((TKey)key, (TValue)value, false);
        }

        ICollection IDictionary.Keys => ((IDictionary)this.dictionary).Keys;

        ICollection IDictionary.Values => ((IDictionary)this.dictionary).Values;

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)this.dictionary).Contains(key);
        }

        void IDictionary.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)this.dictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            this.Remove((TKey)key);
        }

        bool IDictionary.IsFixedSize => ((IDictionary)this.dictionary).IsFixedSize;

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)dictionary).CopyTo(array, index);
        }

        object ICollection.SyncRoot => ((IDictionary)this.dictionary).SyncRoot;

        bool ICollection.IsSynchronized => ((IDictionary)this.dictionary).IsSynchronized;
        
        public static implicit operator Dictionary<TKey, TValue>(ObservableDictionary<TKey, TValue> self)
        {
            return self.dictionary;
        }

        void IObservable.AddRawListener(Action<object> listener)
        {
            dicChanged += dic => listener(dic);
        }

        object IObservable.RawValue => dictionary;
        
        Type IObservable.RawType => dictionary.GetType();
        void IObservable.InitRawValueWithoutCb(object val)
        {
            dictionary = (Dictionary<TKey,TValue>)val;
        }
        void IObservable.ForceTrigger()
        {
            throw new NotSupportedException("Dic not support force trigger");
        }
    }
}