using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.UI.Wrap
{
    public class ToggleWrapper : BaseWrapper<Toggle>, IFieldChangeCb<bool>, IComponentEvent<bool>
    {
        private readonly Toggle toggle;

        public ToggleWrapper(Toggle _toggle) : base(_toggle)
        {
            toggle = _toggle;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            return (value) => toggle.isOn = value;
        }

        UnityEvent<bool> IComponentEvent<bool>.GetComponentEvent()
        {
            return toggle.onValueChanged;
        }
    }
}
