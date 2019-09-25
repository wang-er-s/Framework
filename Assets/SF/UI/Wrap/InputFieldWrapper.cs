using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.SF.UI.Wrap
{
    public class InputFieldWrapper : IBindData<string>, IBindCommand<string>
    {
        private readonly InputField inputField;

        public InputFieldWrapper(InputField _inputField)
        {
            inputField = _inputField;
        }

        public Action<string> GetBindFieldFunc()
        {
            return (value) => inputField.text = value;
        }

        public UnityEvent<string> GetBindCommandFunc()
        {
            return inputField.onValueChanged;
        }
    }
}
