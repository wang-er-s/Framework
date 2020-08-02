using System;
using System.Collections.Specialized;

namespace Framework.UI.Wrap.Base
{
    public interface IBindList<T>
    {
        Action<NotifyCollectionChangedAction, T, int> GetBindListFunc();
    }
}