using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Framework.Execution;
using UnityEngine;

namespace Framework.UI.Core.Bind
{
    public class ObservableDictionary<TKey,TValue>  : IDictionary<TKey, TValue>, IDictionary , IObservable
    {
        
        private event Action<NotifyCollectionChangedAction, KeyValuePair<TKey, TValue>, KeyValuePair<TKey,TValue>> CollectionChanged;
        private event Action<Dictionary<TKey, TValue>> dicChanged; 

        protected Dictionary<TKey, TValue> Dictionary;

        public ObservableDictionary()
        {
            this.Dictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.Dictionary = new Dictionary<TKey, TValue>(dictionary);
        }
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            this.Dictionary = new Dictionary<TKey, TValue>(comparer);
        }
        public ObservableDictionary(int capacity)
        {
            this.Dictionary = new Dictionary<TKey, TValue>(capacity);
        }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!Dictionary.ContainsKey(key))
                    return default;
                return Dictionary[key];
            }
            set => Insert(key, value, false);
        }

        public ICollection<TKey> Keys => Dictionary.Keys;

        public ICollection<TValue> Values => Dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            TValue value;
            Dictionary.TryGetValue(key, out value);
            var removed = Dictionary.Remove(key);
            if (removed)
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, default,
                    new KeyValuePair<TKey, TValue>(key, value));
            }

            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
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

        public void AddListener(Action<Dictionary<TKey, TValue>> changeCb)
        {
            //dicChanged += changeCb;
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
            if (Dictionary.Count > 0)
            {
                Dictionary.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset, default, default);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary)this.Dictionary).CopyTo(array, arrayIndex);
        }

        public int Count => Dictionary.Count;

        public bool IsReadOnly => ((IDictionary)this.Dictionary).IsReadOnly;

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Dictionary).GetEnumerator();
        }

        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (items.Count > 0)
            {
                if (this.Dictionary.Count > 0)
                {
                    if (items.Keys.Any((k) => this.Dictionary.ContainsKey(k)))
                        throw new ArgumentException("An item with the same key has already been added.");
                    else
                    {
                        foreach (var item in items)
                            ((IDictionary<TKey, TValue>)this.Dictionary).Add(item);
                    }
                }
                else
                {
                    this.Dictionary = new Dictionary<TKey, TValue>(items);
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

            if (Dictionary.TryGetValue(key, out var item))
            {
                if (add)
                    throw new ArgumentException("An item with the same key has already been added.");

                if (Equals(item, value))
                    return;

                Dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                Dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), default);
            }
        }
        

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            CollectionChanged?.Invoke(action, newItem, oldItem);
            dicChanged?.Invoke(Dictionary);
        }
        

        object IDictionary.this[object key]
        {
            get => ((IDictionary)this.Dictionary)[key];
            set => Insert((TKey)key, (TValue)value, false);
        }

        ICollection IDictionary.Keys => ((IDictionary)this.Dictionary).Keys;

        ICollection IDictionary.Values => ((IDictionary)this.Dictionary).Values;

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)this.Dictionary).Contains(key);
        }

        void IDictionary.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)this.Dictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            this.Remove((TKey)key);
        }

        bool IDictionary.IsFixedSize => ((IDictionary)this.Dictionary).IsFixedSize;

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)Dictionary).CopyTo(array, index);
        }

        object ICollection.SyncRoot => ((IDictionary)this.Dictionary).SyncRoot;

        bool ICollection.IsSynchronized => ((IDictionary)this.Dictionary).IsSynchronized;
        
        public static implicit operator Dictionary<TKey, TValue>(ObservableDictionary<TKey, TValue> self)
        {
            return self.Dictionary;
        }

        void IObservable.AddListener(Action<object> listener)
        {
            dicChanged += listener;
        }

        object IObservable.RawValue => Dictionary;
        
        Type IObservable.RawType => Dictionary.GetType();
        void IObservable.InitValueWithoutCb(object val)
        {
            Dictionary = (Dictionary<TKey,TValue>)val;
        }
        void IObservable.ForceTrigger()
        {
            throw new NotSupportedException("Dic not support force trigger");
        }
    }
}