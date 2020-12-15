using System;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class SliderWrapper : BaseWrapper<Slider>, IFieldChangeCb<float>, IComponentEvent<float>
    {
        public SliderWrapper(Slider slider) : base(slider)
        {
            View = slider;
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return (value) => View.value = value;
        }

        UnityEvent<float> IComponentEvent<float>.GetComponentEvent()
        {
            return View.onValueChanged;
        }
    }
}