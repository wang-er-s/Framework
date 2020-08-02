using System;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ToggleWrapper : BaseWrapper<Toggle>, IFieldChangeCb<bool>, IComponentEvent<bool>
    {
        public ToggleWrapper(Toggle toggle) : base(toggle)
        {
            view = toggle;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            return (value) => view.isOn = value;
        }

        UnityEvent<bool> IComponentEvent<bool>.GetComponentEvent()
        {
            return view.onValueChanged;
        }
    }
}