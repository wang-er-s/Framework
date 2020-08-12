using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Framework.UI.Core.Bind
{
    public class ObservableProperty<T> : IClearListener
    {
        private readonly Dictionary<object, List<Action<T>>> _caller2Action = new Dictionary<object, List<Action<T>>>();
        
        public ObservableProperty(T value)
        {
            this.value = value;
        }

        public ObservableProperty()
        {
        }

        private event Action<T> OnValueChanged;

        private T value;
        public T Value
        {
            get => value;
            set
            {
                if (Equals(this.value, value)) return;
                this.value = value;
                ValueChanged(this.value);
            }
        }

        private void ValueChanged(T newValue)
        {
            OnValueChanged?.Invoke(newValue);
        }

        public void AddListener(Action<T> changeAction, object caller = null)
        {
            OnValueChanged += changeAction;
            if (caller == null) return;
            if (!_caller2Action.TryGetValue(caller, out var actions))
            {
                actions = new List<Action<T>>();
                _caller2Action.Add(caller, actions);
            }
            actions.Add(changeAction);
        }

        public void RemoveListener(Action<T> changeAction)
        {
            if (OnValueChanged == null) return;
            OnValueChanged -= changeAction;
        }

        public void ClearListener(object caller)
        {
            if (caller == null)
            {
                OnValueChanged = null;
                return;
            }
            if(!_caller2Action.TryGetValue(caller, out var actions)) return;
            foreach (var action in actions)
            {
                OnValueChanged -= action;
            }
        }

        public override string ToString()
        {
            return value != null ? value.ToString() : "null";
        }

        public static implicit operator T(ObservableProperty<T> property)
        {
            return property.value;
        }
    }

    public interface IClearListener
    {
        /// <summary>
        /// 更换vm时清空原vm绑定的view
        /// 加call预防多个view绑定同一vm的情况
        /// </summary>
        /// <param name="caller"></param>
        void ClearListener(object caller);
    }
}