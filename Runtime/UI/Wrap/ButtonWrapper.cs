using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework
{
    public class ButtonWrapper : BaseWrapper<Button>, IComponentEvent , IFieldChangeCb<string> , IFieldChangeCb<int> , IFieldChangeCb<float> , IFieldChangeCb<long> , IFieldChangeCb<double>
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

        Action<int> IFieldChangeCb<int>.GetFieldChangeCb()
        {
            return val =>
            {
                if (text)
                    text.text = val.ToString();
                if (tmpText)
                    tmpText.text = val.ToString();
            };
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return val =>
            {
                if (text)
                    text.text = val.ToString();
                if (tmpText)
                    tmpText.text = val.ToString();
            };
        }

        Action<long> IFieldChangeCb<long>.GetFieldChangeCb()
        {
            return val =>
            {
                if (text)
                    text.text = val.ToString();
                if (tmpText)
                    tmpText.text = val.ToString();
            };
        }

        Action<double> IFieldChangeCb<double>.GetFieldChangeCb()
        {
            return val =>
            {
                if (text)
                    text.text = val.ToString();
                if (tmpText)
                    tmpText.text = val.ToString();
            };
        }
    }
}