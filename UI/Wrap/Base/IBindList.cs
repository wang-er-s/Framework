using System;
using System.Collections.Specialized;
using Framework.UI.Core;

namespace Framework.UI.Wrap
{
    public interface IBindList<T> where T : ViewModel
    {
        Action<NotifyCollectionChangedAction, T, int> GetBindListFunc();
    }
}
