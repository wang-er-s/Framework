using Framework.UI.Core;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ButtonWrapper : BaseWrapper<Button>, IComponentEvent
    {
        UnityEvent IComponentEvent.GetComponentEvent()
        {
            return Component.onClick;
        }

        public ButtonWrapper(Button component, View view) : base(component, view)
        {
        }
    }
}