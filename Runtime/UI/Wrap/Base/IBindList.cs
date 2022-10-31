using System;
using System.Collections.Specialized;

namespace Framework
{
    public interface IBindList<T>
    {
        Action<NotifyCollectionChangedAction, T, int> GetBindListFunc();
    }
}