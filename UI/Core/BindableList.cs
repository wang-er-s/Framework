using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Framework.UI.Core
{
    public class BindableList<T> : IList<T> , IClearListener
    {
        private event Action<NotifyCollectionChangedAction, T, int> _collectionChanged;
        private IList<T> _items;
        private static object _locker = new object();
        private event Action<BindableList<T>> _listUpdateChanged;
        public int Count => _items.Count;
        public bool IsReadOnly => _items.IsReadOnly;

        public BindableList()
        {
            _items = new List<T>();
        }

        public BindableList(int capacity)
        {
            _items = new List<T>(capacity);
        }

        public BindableList(IList<T> list)
        {
            if (list == null)
                throw new ArgumentException();
            _items = new List<T>(list.Count);
            foreach (T item in list)
            {
                _items.Add(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
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
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            _items.CopyTo(array, arrayIndex);
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
            return _items.IndexOf(item);
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
            get { return _items[index]; }
            set
            {
                if (IsReadOnly)
                    throw new NotSupportedException("ReadOnlyCollection");
                Insert(index, value);
            }
        }

        private void AddItem(T item)
        {
            lock (_locker)
            {
                _items.Add(item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item, Count - 1);
            }
        }

        private void RemoveItem(int index)
        {
            lock (_locker)
            {
                T item = _items[index];
                _items.RemoveAt(index);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
            }
        }

        private void RemoveItem(T item)
        {
            lock (_locker)
            {
                var index = _items.IndexOf(item);
                RemoveItem(index);
            }
        }

        private void InsertItem( int index ,T item)
        {
            lock (_locker)
            {
                _items.Insert(index, item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            }
        }

        private void ClearItems()
        {
            lock (_locker)
            {
                _items.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset, default(T), -1);
            }
        }

        protected void SetItem(int index, T item)
        {
            lock (_locker)
            {
                _items[index] = item;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, item, index);
            }
        }

        public void AddListener(Action<NotifyCollectionChangedAction, T, int> listener)
        {
            _collectionChanged += listener;
        }

        public void RemoveListener(Action<NotifyCollectionChangedAction, T, int> listener)
        {
            if (_collectionChanged != null) _collectionChanged -= listener;
        }

        public void AddListUpdateListener(Action<BindableList<T>> listener)
        {
            _listUpdateChanged += listener;
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction type, T item, int index)
        {
            _collectionChanged?.Invoke(type, item, index);
            _listUpdateChanged?.Invoke(this);
        }

        public void ClearListener()
        {
            _listUpdateChanged = null;
            _collectionChanged = null;
        }
    }
}