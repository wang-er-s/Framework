using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.UI.Wrap
{
    public class InputFieldWrapper : BaseWrapper<InputField>, IFieldChangeCb<string>, IComponentEvent<string>
    {

        public InputFieldWrapper(InputField _inputField) : base(_inputField)
        {
            component = _inputField;
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return (value) => component.text = value;
        }

        UnityEvent<string> IComponentEvent<string>.GetComponentEvent()
        {
            return component.onEndEdit;
        }
    }
}
