using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework
{
    public class ToggleWrapper : BaseWrapper<Toggle>, IFieldChangeCb<bool>, IComponentEvent<bool>
    {
        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            return (value) => Component.isOn = value;
        }

        UnityEvent<bool> IComponentEvent<bool>.GetComponentEvent()
        {
            return Component.onValueChanged;
        }
    }
}