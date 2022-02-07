using System;
using Framework.UI.Wrap.Base;
using UnityEngine.UI;

namespace Framework.UIComponent
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