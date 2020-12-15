using System;
using System.Globalization;
using Framework.UI.Wrap.Base;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class TextWrapper : BaseWrapper<Text>, IFieldChangeCb<string>, IFieldChangeCb<int>, IFieldChangeCb<float>,
        IFieldChangeCb<double>
    {
        public TextWrapper(Text text) : base(text)
        {
            View = text;
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return (value) => View.text = value;
        }

        public Action<int> GetFieldChangeCb()
        {
            return value => View.text = value.ToString();
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return value => View.text = value.ToString(CultureInfo.InvariantCulture);
        }

        Action<double> IFieldChangeCb<double>.GetFieldChangeCb()
        {
            return value => View.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}