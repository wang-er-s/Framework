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
            View = toggle;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            return (value) => View.isOn = value;
        }

        UnityEvent<bool> IComponentEvent<bool>.GetComponentEvent()
        {
            return View.onValueChanged;
        }
    }
}