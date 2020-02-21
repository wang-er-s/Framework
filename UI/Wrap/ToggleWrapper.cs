using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ToggleWrapper : BaseWrapper<Toggle>, IFieldChangeCb<bool>, IComponentEvent<bool>
    {

        public ToggleWrapper(Toggle toggle) : base(toggle)
        {
            _view = toggle;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            return (value) => _view.isOn = value;
        }

        UnityEvent<bool> IComponentEvent<bool>.GetComponentEvent()
        {
            return _view.onValueChanged;
        }
    }
}
