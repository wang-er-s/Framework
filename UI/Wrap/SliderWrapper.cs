using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.UI.Core;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class SliderWrapper: BaseWrapper<Slider>, IFieldChangeCb<float>, IComponentEvent<float>
    {
        private readonly Slider slider;

        public SliderWrapper(Slider _slider) : base(_slider)
        {
            slider = _slider;
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return (value) => slider.value = value;
        }

        UnityEvent<float> IComponentEvent<float>.GetComponentEvent()
        {
            return slider.onValueChanged;
        }

        public void TwoWayBind(IBindableProperty<float> property)
        {
            slider.onValueChanged.AddListener((value) => property.Value = value);
        }

    }
}
