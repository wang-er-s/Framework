using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using CatJson;

namespace Framework
{
    public class ObservableList<T> : IList<T> , IObservable, IList, IReference
    {
        private event Action<NotifyCollectionChangedAction, T, int> CollectionChanged;
        
        private List<T> _items;
        private readonly object _locker = new object();
        private event Action<List<T>> ListUpdateChanged;
        public void CopyTo(Array array, int index)
        {
            ((IList)_items).CopyTo(array, index);
        }

        public int Count => _items.Count;
        public bool IsSynchronized => ((IList)_items).IsSynchronized;
        public object SyncRoot => ((IList)_items).SyncRoot;
        public bool IsReadOnly => false;

        object IList.this[int index]
        {
            get => _items[index];
            set => _items[index] = (T)value;
        }

        static ObservableList()
        {
            JsonParser.AddCustomJsonFormatter(typeof(ObservableList<>), new ListFormatter());
        }

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

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public int Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }

        public void Clear()
        {
            if (IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            ClearItems();
        }

        void IReference.Clear()
        {
            _items.Clear();
            ClearListener();
        }

        public bool Contains(object value)
        {
            return Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public void Remove(object value)
        {
            Remove((T)value);
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

        public bool IsFixedSize => ((IList)_items).IsFixedSize;

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
                OnCollectionChanged(NotifyCollectionChangedAction.Reset, default, count);
                _items.Clear();
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
        
        public void Sort()
        {
            Sort(0, Count, null);
        }
        
        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            _items.Sort(index, count, comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            _items.Sort(comparison);
        }

        public UnRegister AddListener(Action<NotifyCollectionChangedAction, T, int> listener)
        {
            CollectionChanged += listener;
            return new UnRegister(() => CollectionChanged -= listener);
        }

        public void RemoveListener(Action<NotifyCollectionChangedAction, T, int> listener)
        {
            if (CollectionChanged != null) CollectionChanged -= listener;
        }
        
        public void RemoveListener(Action<List<T>> listener)
        {
            ListUpdateChanged -= listener;
        }

        public UnRegister AddListener(Action<List<T>> listener)
        {
            listener(_items);
            ListUpdateChanged += listener;
            return new UnRegister(() => ListUpdateChanged -= listener);
        }
        
        public UnRegister AddListenerWithoutCall(Action<List<T>> listener)
        {
            ListUpdateChanged += listener;
            return new UnRegister(() => ListUpdateChanged -= listener);
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

        void IObservable.AddRawListener(Action<object> listener)
        {
            ListUpdateChanged += (l)=> listener(l);
        }
        
        object IObservable.RawValue => _items;
        Type IObservable.RawType => _items.GetType();
        void IObservable.InitRawValueWithoutCb(object val)
        {
            _items = (List<T>)val;
        }
        void IObservable.ForceTrigger()
        {
            throw new NotSupportedException("List not support force trigger");
        }
    }
}