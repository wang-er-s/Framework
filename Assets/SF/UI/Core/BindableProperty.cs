using System;

namespace SF.UI.Core
{
    public class BindableProperty<T>
    {
        private event Action<T> OnValueChanged;

        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!Equals(_value, value))
                {
                    T old = _value;
                    _value = value;
                    ValueChanged( _value);
                }
            }
        }

        private void ValueChanged(T newValue)
        {
            OnValueChanged?.Invoke(newValue);
        }

        public void AddChangeEvent(Action<T> changeAction)
        {
            if (OnValueChanged == null)
                OnValueChanged = changeAction;
            else
                OnValueChanged += changeAction;
        }

        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }
    }
}
