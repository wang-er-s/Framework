using System;
using UnityEngine.Experimental.UIElements;

namespace SF.UI.Core
{
    public class BindingAbleProperty<T> : INotifyWhenChanged<T>
    {
        private event Action<T> OnValueChanged;

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;
                T old = _value;
                _value = value;
                ValueChanged(_value);
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
            ValueChanged(_value);
        }

        public void RemoveChangeEvent(Action<T> changeAction)
        {
            if(OnValueChanged == null) return;
            OnValueChanged -= changeAction;
        }

        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }
    }

    public interface INotifyWhenChanged<out T>
    {
        void AddChangeEvent(Action<T> changedAction);
    }
}
