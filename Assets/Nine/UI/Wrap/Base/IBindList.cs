using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Nine.UI.Wrap
{
    public interface IBindList<T>
    {
        Action<NotifyCollectionChangedAction, T, T, int> GetBindListFunc();
    }
}
