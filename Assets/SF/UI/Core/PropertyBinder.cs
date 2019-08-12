using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace SF.UI.Core
{
    public class PropertyBinder<T> where T : ViewModelBase
    {
        private delegate void BindHandler(T viewmodel);

        private delegate void UnBindHandler(T viewmodel);

        private readonly List<BindHandler> _binders = new List<BindHandler>();
        private readonly List<UnBindHandler> _unbinders = new List<UnBindHandler>();

        public void Add<TProperty>(UnityEngine.Object component, BindableProperty<TProperty> property,
            BindableProperty<TProperty>.ValueChangedHandler valueChangedHandler)
        {
            if (property.OnValueChanged == null)
                property.OnValueChanged = valueChangedHandler;
            else
                property.OnValueChanged += valueChangedHandler;
        }

        private void BindComponent(UnityEngine.Object component)
        {

        }


        public void Bind(T viewmodel)
        {
            if (viewmodel != null)
            {
                for (int i = 0; i < _binders.Count; i++)
                {
                    _binders[i](viewmodel);
                }
            }
        }

        public void Unbind(T viewmodel)
        {
            if (viewmodel != null)
            {
                for (int i = 0; i < _unbinders.Count; i++)
                {
                    _unbinders[i](viewmodel);
                }
            }
        }
    }
}