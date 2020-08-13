using Framework.UI.Wrap.Base;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ButtonWrapper : BaseWrapper<Button>, IComponentEvent
    {
        public ButtonWrapper(Button button) : base(button)
        {
            View = button;
        }

        UnityEvent IComponentEvent.GetComponentEvent()
        {
            return View.onClick;
        }
    }
}