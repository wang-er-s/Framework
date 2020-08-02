using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Framework.UI.Core.Bind
{
    public class BindableProperty<T> : IClearListener
    {
        private Dictionary<object, List<Action<T>>> caller2Action = new Dictionary<object, List<Action<T>>>();
        
        public BindableProperty(T value)
        {
            this.value = value;
        }

        public BindableProperty()
        {
        }

        private event Action<T> onValueChanged;

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
            onValueChanged?.Invoke(newValue);
        }

        public void AddListener(Action<T> changeAction, object caller = null)
        {
            onValueChanged += changeAction;
            if (caller == null) return;
            if (!caller2Action.TryGetValue(caller, out var actions))
            {
                actions = new List<Action<T>>();
                caller2Action.Add(caller, actions);
            }
            actions.Add(changeAction);
        }

        public void RemoveListener(Action<T> changeAction)
        {
            if (onValueChanged == null) return;
            onValueChanged -= changeAction;
        }

        public void ClearListener(object caller)
        {
            if (caller == null)
            {
                onValueChanged = null;
                return;
            }
            if(!caller2Action.TryGetValue(caller, out var actions)) return;
            foreach (var action in actions)
            {
                onValueChanged -= action;
            }
        }

        public override string ToString()
        {
            return value != null ? value.ToString() : "null";
        }

        public static implicit operator T(BindableProperty<T> property)
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