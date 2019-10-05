using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.SF.UI.Wrap
{
    public class InputFieldWrapper : BaseWrapper<InputField>, IBindData<string>, IBindCommand<string>
    {

        public InputFieldWrapper(InputField _inputField) : base(_inputField)
        {
            component = _inputField;
        }

        public Action<string> GetBindFieldFunc()
        {
            return (value) => component.text = value;
        }

        public UnityEvent<string> GetBindCommandFunc()
        {
            return component.onEndEdit;
        }
    }
}
