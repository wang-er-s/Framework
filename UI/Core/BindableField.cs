using System;

namespace AD.UI.Core
{
    public class BindableField<T> : IBindableField<T>
    {

        public BindableField(T value)
        {
            _value = value;
        }
        
        public BindableField()
        {
        }
        
        private event Action<T> OnValueChanged;

        private T _value;
        T IBindableField<T>.Value
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
        
        public static implicit operator T(BindableField<T> field)
        {
            return field._value;
        }
    }

    public interface IBindableField<T>
    {
        T Value { get; set; }
        void AddListener(Action<T> changedAction);
        void RemoveListener(Action<T> changedAction);
        void ClearListener();
    }
}
