using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.UI.Wrap
{
    public class InputFieldWrapper : BaseWrapper<InputField>, IBindData<string>, IBindCommand<string>
    {

        public InputFieldWrapper(InputField _inputField) : base(_inputField)
        {
            component = _inputField;
        }

        Action<string> IBindData<string>.GetBindFieldFunc()
        {
            return (value) => component.text = value;
        }

        UnityEvent<string> IBindCommand<string>.GetBindCommandFunc()
        {
            return component.onEndEdit;
        }
    }
}
