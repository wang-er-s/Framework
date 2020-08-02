using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Framework.UI.Core.Bind
{
    public class BindableList<T> : IList<T>, IClearListener
    {
        private Dictionary<object, List<object>> caller2Action =
            new Dictionary<object, List<object>>();
        private event Action<NotifyCollectionChangedAction, T, int> collectionChanged;
        
        private IList<T> items;
        private object locker = new object();
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
            foreach (var item in list) items.Add(item);
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
            get => items[index];
            set
            {
                if (IsReadOnly)
                    throw new NotSupportedException("ReadOnlyCollection");
                Insert(index, value);
            }
        }

        private void AddItem(T item)
        {
            lock (locker)
            {
                items.Add(item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item, Count - 1);
            }
        }

        private void RemoveItem(int index)
        {
            lock (locker)
            {
                var item = items[index];
                items.RemoveAt(index);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
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

        private void InsertItem(int index, T item)
        {
            lock (locker)
            {
                items.Insert(index, item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            }
        }

        private void ClearItems()
        {
            lock (locker)
            {
                items.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset, default(T), -1);
            }
        }

        protected void SetItem(int index, T item)
        {
            lock (locker)
            {
                items[index] = item;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, item, index);
            }
        }

        public void AddListener(Action<NotifyCollectionChangedAction, T, int> listener, object caller = null)
        {
            collectionChanged += listener;
            AddListener(caller, listener);
        }

        public void RemoveListener(Action<NotifyCollectionChangedAction, T, int> listener)
        {
            if (collectionChanged != null) collectionChanged -= listener;
        }

        public void AddListener(Action<BindableList<T>> listener, object caller = null)
        {
            listUpdateChanged += listener;
            AddListener(caller, listener);
        }

        private void AddListener(object caller, object action)
        {
            if (caller == null) return;
            if (!caller2Action.TryGetValue(caller, out var actions))
            {
                actions = new List<object>();
                caller2Action.Add(caller, actions);
            }
            actions.Add(action);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction type, T item, int index)
        {
            collectionChanged?.Invoke(type, item, index);
            listUpdateChanged?.Invoke(this);
        }

        public void ClearListener(object caller)
        {
            if (caller == null)
            {
                listUpdateChanged = null;
                collectionChanged = null;
                return;
            }
            if(!caller2Action.TryGetValue(caller, out var actions)) return;
            foreach (var action in actions)
            {
                switch (action)
                {
                    case Action<BindableList<T>> ac:
                        listUpdateChanged -= ac;
                        continue;
                    case Action<NotifyCollectionChangedAction, T, int> ac2:
                        collectionChanged -= ac2;
                        break;
                }
            }
            
        }
    }
}