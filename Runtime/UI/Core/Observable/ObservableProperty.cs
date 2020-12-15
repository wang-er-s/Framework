using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Framework.UI.Core.Bind
{
    public class ObservableProperty<T>
    {
        public ObservableProperty(T value)
        {
            this._value = value;
        }

        public ObservableProperty()
        {
            _value = default;
        }

        private event Action<T> OnValueChanged;

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (Equals(this._value, value)) return;
                this._value = value;
                ValueChanged(this._value);
            }
        }

        private void ValueChanged(T newValue)
        {
            OnValueChanged?.Invoke(newValue);
        }

        public void AddListener(Action<T> changeAction)
        {
            OnValueChanged += changeAction;
        }

        public void RemoveListener(Action<T> changeAction)
        {
            OnValueChanged -= changeAction;
        }

        public void ClearListener()
        {
            OnValueChanged = null;
        }

        public override string ToString()
        {
            return _value != null ? _value.ToString() : "null";
        }

        public static implicit operator T(ObservableProperty<T> property)
        {
            return property._value;
        }
    }
}