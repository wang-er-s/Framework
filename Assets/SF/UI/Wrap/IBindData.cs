using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SF.UI.Wrap
{
    public interface IBindData<T>
    {
        Action<T> GetDefaultBindFunc();
    }
}
