using System;
using Framework.UI.Core;
using Framework.UI.Wrap.Base;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ButtonWrapper : BaseWrapper<Button>, IComponentEvent , IFieldChangeCb<string>
    {
        private Text text;
        private TextMeshProUGUI tmpText;
        UnityEvent IComponentEvent.GetComponentEvent()
        {
            return Component.onClick;
        }

        public ButtonWrapper(Button component, View view) : base(component, view)
        {
            text = component.GetComponentInChildren<Text>();
            tmpText = component.GetComponentInChildren<TextMeshProUGUI>();
        }

        public Action<string> GetFieldChangeCb()
        {
            return val =>
            {
                if (text)
                    text.text = val;
                if (tmpText)
                    tmpText.text = val;
            };
        }
    }
}