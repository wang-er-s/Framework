using System;
using System.Globalization;
using TMPro;
using UnityEngine.UI;

namespace Framework
{
    public class TmpWrapper : BaseWrapper<TextMeshProUGUI>, IFieldChangeCb<string>, IFieldChangeCb<int>, IFieldChangeCb<float>,
        IFieldChangeCb<double>, IFieldChangeCb<long>
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

        Action<long> IFieldChangeCb<long>.GetFieldChangeCb()
        {
            return value => Component.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}