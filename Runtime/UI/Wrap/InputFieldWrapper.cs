using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework
{
    public class InputFieldWrapper : BaseWrapper<InputField>, IFieldChangeCb<string>, IComponentEvent<string>
    {
        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return value => Component.text = value;
        }

        UnityEvent<string> IComponentEvent<string>.GetComponentEvent()
        {
            return Component.onEndEdit;
        }
    }
    
    public class TMPInputFieldWrapper : BaseWrapper<TMP_InputField>, IFieldChangeCb<string>, IComponentEvent<string>
    {

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return (value) => Component.text = value;
        }

        UnityEvent<string> IComponentEvent<string>.GetComponentEvent()
        {
            return Component.onEndEdit;
        }
    }
}