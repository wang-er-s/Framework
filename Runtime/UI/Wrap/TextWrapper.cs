using System;
using System.Globalization;
using Framework.UI.Core;
using Framework.UI.Wrap.Base;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class TextWrapper : BaseWrapper<Text>, IFieldChangeCb<string>, IFieldChangeCb<int>, IFieldChangeCb<float>,
        IFieldChangeCb<double>
    {

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return (value) => Component.text = value;
        }

        public Action<int> GetFieldChangeCb()
        {
            return value => Component.text = value.ToString();
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return value => Component.text = value.ToString(CultureInfo.InvariantCulture);
        }

        Action<double> IFieldChangeCb<double>.GetFieldChangeCb()
        {
            return value => Component.text = value.ToString(CultureInfo.InvariantCulture);
        }

        public TextWrapper(Text component, View view) : base(component, view)
        {
        }
    }
}