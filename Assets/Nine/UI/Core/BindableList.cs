using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Nine.UI.Core
{
    public class BindableList<T> : IList<T>
    {
        private event Action<NotifyCollectionChangedAction, T, T, int> collectionChanged;
        private IList<T> items;
        private object locker = new object();

        public int Count => items.Count;
        public bool IsReadOnly => items.IsReadOnly;

        public BindableList()
        {
            items = new List<T>();
        }

        public BindableList(int capacity)
        {
            items = new List<T>(capacity);
        }

        public BindableList(IList<T> list)
        {
            if (list == null)
                throw new ArgumentException();
            items = new List<T>(list.Count);
            foreach (T item in list)
            {
                items.Add(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            AddItem(item);
        }

        public void Clear()
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            ClearItems();
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            if (Count <= 0) return false;
            int index = IndexOf(item);
            if (index < 0) return false;
            return items.Remove(item);
        }

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            RemoveItem(index);
        }

        public T this[int index]
        {
            get => items[index];
            set
            {
                if (IsReadOnly)
                    throw new NotSupportedException("ReadOnlyCollection");
                Insert(index, value);
            }
        }

        protected void AddItem(T item)
        {
            lock (locker)
            {
                items.Add(item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add,
                    default, item, Count - 1);
            }
        }

        protected void RemoveItem(int index)
        {
            lock (locker)
            {
                T item = items[index];
                items.RemoveAt(index);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, default, index);
            }
        }

        protected void ClearItems()
        {
            lock (locker)
            {
                items.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset,
                    default, default, -1);
            }
        }

        protected void SetItem(int index, T item)
        {
            lock (locker)
            {
                T originItem = this[index];
                items[index] = item;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace,
                    originItem, item, index);
            }
        }

        public void AddListener(Action<NotifyCollectionChangedAction, T, T, int> listener)
        {
            collectionChanged += listener;
        }

        public void RemoveListener(Action<NotifyCollectionChangedAction, T, T, int> listener)
        {
            if (collectionChanged != null) collectionChanged -= listener;
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction type, T originItem, T item, int index)
        {
            if(collectionChanged == null) return;
            lock (collectionChanged)
            {
                collectionChanged(type, originItem, item, index);
            }
        }
    }
}