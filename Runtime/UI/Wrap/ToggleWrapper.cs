using System;
using Framework.UI.Core;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
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

        public ToggleWrapper(Toggle component, View view) : base(component, view)
        {
        }
    }
}