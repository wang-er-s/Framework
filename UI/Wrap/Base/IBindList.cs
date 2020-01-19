using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.UI.Core;

namespace Framework.UI.Wrap
{
    public interface IBindList<T> where T : ViewModel
    {
        Action<NotifyCollectionChangedAction, T, T, int> GetBindListFunc();
    }
}
