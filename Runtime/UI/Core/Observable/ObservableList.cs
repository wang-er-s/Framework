using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Framework.UI.Core.Bind
{
    public class ObservableList<T> : IList<T> , IObservable
    {
        private event Action<NotifyCollectionChangedAction, T, int> CollectionChanged;
        
        private List<T> _items;
        private readonly object _locker = new object();
        private event Action<List<T>> ListUpdateChanged;
        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public ObservableList()
        {
            _items = new List<T>();
        }

        public ObservableList(IEnumerable<T> collection)
        {
            _items = new List<T>(collection);
        }
        
        public ObservableList(int capacity)
        {
            _items = new List<T>(capacity);
        }

        public ObservableList(IList<T> list)
        {
            if (list == null)
                throw new ArgumentException();
            _items = new List<T>(list.Count);
            foreach (var item in list) _items.Add(item);
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
            get => _items[index];
            set
            {
                if (IsReadOnly)
                    throw new NotSupportedException("ReadOnlyCollection");
                SetItem(index, value);
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
                var item = _items[index];
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                _items.RemoveAt(index);
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

        private void InsertItem(int index, T item)
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
                var count = Count;
                _items.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset, default, count);
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
            CollectionChanged += listener;
        }

        public void RemoveListener(Action<NotifyCollectionChangedAction, T, int> listener)
        {
            if (CollectionChanged != null) CollectionChanged -= listener;
        }
        
        public void RemoveListener(Action<List<T>> listener)
        {
            ListUpdateChanged -= listener;
        }

        public void AddListener(Action<List<T>> listener)
        {
            ListUpdateChanged += listener;
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction type, T item, int index)
        {
            CollectionChanged?.Invoke(type, item, index);
            ListUpdateChanged?.Invoke(this);
        }

        public void ClearListener()
        {
            ListUpdateChanged = null;
            CollectionChanged = null;
        }
        
        public static implicit operator List<T>(ObservableList<T> self)
        {
            return self._items;
        }

        void IObservable.AddListener(Action<object> listener)
        {
            ListUpdateChanged += listener;
        }
        
        object IObservable.RawValue => _items;
        Type IObservable.RawType => _items.GetType();
        void IObservable.InitValueWithoutCb(object val)
        {
            _items = (List<T>)val;
        }
    }
}