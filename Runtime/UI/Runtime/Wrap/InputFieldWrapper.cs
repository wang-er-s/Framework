using System;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class InputFieldWrapper : BaseWrapper<InputField>, IFieldChangeCb<string>, IComponentEvent<string>
    {
        public InputFieldWrapper(InputField inputField) : base(inputField)
        {
            view = inputField;
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return (value) => view.text = value;
        }

        UnityEvent<string> IComponentEvent<string>.GetComponentEvent()
        {
            return view.onEndEdit;
        }
    }
}