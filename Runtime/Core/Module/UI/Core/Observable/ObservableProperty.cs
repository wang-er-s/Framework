using System;

namespace Framework
{
    public class ObservableProperty<T> :  IObservable, IReference , IResetBind
    {
        private ObservableProperty()
        {
        }
        
        public static ObservableProperty<T> Create(T value = default)
        {
            var res = ReferencePool.Allocate<ObservableProperty<T>>();
            res._value = value;
            return res;
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

        public UnRegister AddListener(Action<T> changeAction)
        {
            changeAction(_value);
            OnValueChanged += changeAction;
            UnRegister result = new UnRegister(() => RemoveListener(changeAction));
            return result;
        }
        
        public UnRegister AddListenerWithoutCall(Action<T> changeAction)
        {
            OnValueChanged += changeAction;
            UnRegister result = new UnRegister(() => RemoveListener(changeAction));
            return result;
        }

        public void RemoveListener(Action<T> changeAction)
        {
            OnValueChanged -= changeAction;
        }
        
        public override string ToString()
        {
            return _value != null ? _value.ToString() : "null";
        }

        public void Reset()
        {
            Clear();
        }

        void IObservable.AddRawListener(Action<object> listener)
        {
            OnValueChanged += obj => listener(obj);
        }
        
        object IObservable.RawValue => _value;
        Type IObservable.RawType => typeof(T);

        void IObservable.InitRawValueWithoutCb(object val)
        {
            _value = (T)val;
        }

        void IObservable.ForceTrigger()
        {
            OnValueChanged?.Invoke(_value);
        }

        public static implicit operator T(ObservableProperty<T> property)
        {
            return property._value;
        }
        
        public void Clear()
        {
            OnValueChanged = null;
            _value = default;
        }
    }
}