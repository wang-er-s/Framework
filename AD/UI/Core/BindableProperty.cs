using System;

namespace AD.UI.Core
{
    public class BindableProperty<T> : INotifyWhenChanged<T>
    {

        public BindableProperty(T value)
        {
            _value = value;
        }
        
        public BindableProperty()
        {
        }
        
        private event Action<T> OnValueChanged;

        private T _value;
        public T Value
        {
            get { return _value; }
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
            changeAction(_value);
            OnValueChanged += changeAction;
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
