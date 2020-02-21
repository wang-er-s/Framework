using System;
using Framework.UI.Core;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class SliderWrapper: BaseWrapper<Slider>, IFieldChangeCb<float>, IComponentEvent<float>
    {
        public SliderWrapper(Slider slider) : base(slider)
        {
            _view = slider;
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return (value) => _view.value = value;
        }

        UnityEvent<float> IComponentEvent<float>.GetComponentEvent()
        {
            return _view.onValueChanged;
        }

        public void TwoWayBind(IBindableProperty<float> property)
        {
            _view.onValueChanged.AddListener((value) => property.Value = value);
        }

    }
}
