using System;
using System.Collections;
using System.Collections.Generic;

namespace AD
{
    public class Registers<T> :IList<Action<T>>, IRegisters
    {
        private List<Action<T>> events { get; }

        public Registers()
        {
            events = new List<Action<T>>();
        }

        public IEnumerator<Action<T>> GetEnumerator()
        {
            return events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Action<T> item)
        {
            events.Add(item);
        }

        public void Clear()
        {
            events.Clear();
        }

        public bool Contains(Action<T> item)
        {
            return events.Contains(item);
        }

        public void CopyTo(Action<T>[] array, int arrayIndex)
        {
            events.CopyTo(array,arrayIndex);
        }

        public bool Remove(Action<T> item)
        {
            return events.Remove(item);
        }

        public int Count => events.Count;
        public bool IsReadOnly => false;

        public int IndexOf(Action<T> item)
        {
            return events.IndexOf(item);
        }

        public void Insert(int index, Action<T> item)
        {
            events.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            events.RemoveAt(index);
        }

        public Action<T> this[int index]
        {
            get { return events[index]; }
            set { events[index] = value; }
        }
    }
    
    public interface IRegisters
    {
    }
}