using System;
using System.Globalization;
using Framework.UI.Wrap.Base;
using TMPro;

namespace Framework.UIComponent
{
    public class CustomTextMeshPro : TextMeshProUGUI, IFieldChangeCb<bool>, IFieldChangeCb<string>, IFieldChangeCb<int>, IFieldChangeCb<float>, IFieldChangeCb<long>, IFieldChangeCb<double>
    {
        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString();
            };
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString();
            };
        }

        Action<int> IFieldChangeCb<int>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString();
            };
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString(CultureInfo.InvariantCulture);
            };
        }

        Action<long> IFieldChangeCb<long>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString();
            };
        }

        Action<double> IFieldChangeCb<double>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString(CultureInfo.InvariantCulture);
            };
        }
    }
}