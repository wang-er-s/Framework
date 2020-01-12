using System;

namespace AD.UI.Core
{
    public class BindableProperty<T> : IBindableProperty<T>
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
        T IBindableProperty<T>.Value
        {
            get { return _value; }
            set
            {
                if (Equals(_value, value)) return;
                _value = value;
                ValueChanged(_value);
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
            if(OnValueChanged == null) return;
            OnValueChanged -= changeAction;
        }

        public void ClearListener()
        {
            OnValueChanged = null;
        }

        public override string ToString()
        {
            return (_value != null ? _value.ToString() : "null");
        }
        
        public static implicit operator T(BindableProperty<T> property)
        {
            return property._value;
        }
    }

    public interface IBindableProperty<T> : IClearListener
    {
        T Value { get; set; }
        void AddListener(Action<T> changedAction);
        void RemoveListener(Action<T> changedAction);
    }

    public interface IClearListener
    {
        void ClearListener();
    }
    
}
