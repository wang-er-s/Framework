using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ButtonWrapper : BaseWrapper<Button>, IComponentEvent
    {
        private readonly Button button;

        public ButtonWrapper(Button _button) : base(_button)
        {
            button = _button;
        }

        UnityEvent IComponentEvent.GetComponentEvent()
        {
            return button.onClick;
        }
    }
}
