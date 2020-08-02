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
            view = slider;
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return (value) => view.value = value;
        }

        UnityEvent<float> IComponentEvent<float>.GetComponentEvent()
        {
            return view.onValueChanged;
        }
    }
}