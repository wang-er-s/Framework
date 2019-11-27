using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.AD.UI.Wrap
{
    public interface IBindData<T>
    {
        Action<T> GetBindFieldFunc();
    }
}
