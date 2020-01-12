using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace AD.UI.Core
{
    public class BindableList<T> : IList<T> , IClearListener
    {
        private event Action<NotifyCollectionChangedAction, T, T, int> collectionChanged;
        private IList<T> items;
        private static object locker = new object();
        private event Action<BindableList<T>> listUpdateChanged;
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
            RemoveItem(IndexOf(item));
            return true;
        }

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            InsertItem(index, item);
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            RemoveItem(index);
        }

        public T this[int index]
        {
            get { return items[index]; }
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
                    default(T), item, Count - 1);
            }
        }

        protected void RemoveItem(int index)
        {
            lock (locker)
            {
                T item = items[index];
                items.RemoveAt(index);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, default(T), index);
            }
        }

        private void RemoveItem(T item)
        {
            lock (locker)
            {
                var index = items.IndexOf(item);
                RemoveItem(index);
            }
        }

        private void InsertItem( int index ,T item)
        {
            lock (locker)
            {
                items.Insert(index, item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, default(T), item, index);
            }
        }

        protected void ClearItems()
        {
            lock (locker)
            {
                items.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset,
                    default(T), default(T), -1);
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

        public void AddListUpdateListener(Action<BindableList<T>> listener)
        {
            listUpdateChanged += listener;
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction type, T originItem, T item, int index)
        {
            collectionChanged?.Invoke(type, originItem, item, index);
            listUpdateChanged?.Invoke(this);
        }

        public void ClearListener()
        {
            listUpdateChanged = null;
            collectionChanged = null;
        }
    }
}