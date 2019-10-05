using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.SF.UI.Wrap
{
    public class ToggleWrapper : BaseWrapper<Toggle>, IBindData<bool>,IBindCommand<bool>
    {
        private readonly Toggle toggle;

        public ToggleWrapper(Toggle _toggle) : base(_toggle)
        {
            toggle = _toggle;
        }

        public Action<bool> GetBindFieldFunc()
        {
            return (value) => toggle.isOn = value;
        }

        public UnityEvent<bool> GetBindCommandFunc()
        {
            return toggle.onValueChanged;
        }
    }
}
