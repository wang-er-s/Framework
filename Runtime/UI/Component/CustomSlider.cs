using System;
using UnityEngine.UI;

namespace Framework
{
    public class CustomSlider : Slider , IFieldChangeCb<float>, IFieldChangeCb<int>
    {
        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return f => value = f;
        }

        Action<int> IFieldChangeCb<int>.GetFieldChangeCb()
        {
            return f => value = f;
        }
    }
}