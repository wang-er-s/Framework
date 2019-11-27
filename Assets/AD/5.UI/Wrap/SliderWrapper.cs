using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD.UI.Core;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.AD.UI.Wrap
{
    public class SliderWrapper: BaseWrapper<Slider>, IBindData<float>, IBindCommand<float>
    {
        private readonly Slider slider;

        public SliderWrapper(Slider _slider) : base(_slider)
        {
            slider = _slider;
        }

        Action<float> IBindData<float>.GetBindFieldFunc()
        {
            return (value) => slider.value = value;
        }

        UnityEvent<float> IBindCommand<float>.GetBindCommandFunc()
        {
            return slider.onValueChanged;
        }

        public void TwoWayBind(BindableProperty<float> property)
        {
            slider.onValueChanged.AddListener((value) => property.Value = value);
        }

    }
}
