using System;
using System.Globalization;
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

        public override void Init(object component, object container)
        {
            base.Init(component, container);
            text = Component.GetComponentInChildren<Text>();
            tmpText = Component.GetComponentInChildren<TextMeshProUGUI>();
        }

        public override void Clear()
        {
            base.Clear();
            text = null;
            tmpText = null;
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
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
                    text.text = val.ToString(CultureInfo.InvariantCulture);
                if (tmpText)
                    tmpText.text = val.ToString(CultureInfo.InvariantCulture);
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
                    text.text = val.ToString(CultureInfo.InvariantCulture);
                if (tmpText)
                    tmpText.text = val.ToString(CultureInfo.InvariantCulture);
            };
        }
    }
}