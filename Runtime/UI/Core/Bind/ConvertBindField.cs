using System;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    public class ConvertBindField<TComponent, TData, TResult> : BaseBind where TComponent : class
    {
        private TComponent _component;
        private Action<TResult> _propChangeCb;
        private UnityEvent<TResult> _componentEvent;
        private Func<TData, TResult> _prop2CpntWrap;
        private Func<TResult, TData> _cpnt2PropWrap;
        private ObservableProperty<TData> _property;
        private object _defaultWrapper;


        public void Reset(TComponent component, ObservableProperty<TData> property,
            Action<TResult> propChangeCb,
            Func<TData, TResult> prop2CpntWrap,
            Func<TResult, TData> cpnt2PropWrap,
            UnityEvent<TResult> componentEvent)
        {
            SetValue(component, property, propChangeCb, prop2CpntWrap, cpnt2PropWrap, componentEvent);
            InitEvent();
        }

        private void SetValue(TComponent component, ObservableProperty<TData> property,
            Action<TResult> propChangeCb,
            Func<TData, TResult> prop2CpntWrap,
            Func<TResult, TData> cpnt2PropWrap,
            UnityEvent<TResult> componentEvent)
        {
            this._component = component;
            this._propChangeCb = propChangeCb;
            this._componentEvent = componentEvent;
            this._property = property;
            this._prop2CpntWrap = prop2CpntWrap;
            this._cpnt2PropWrap = cpnt2PropWrap;
        }

        private void InitEvent()
        {
            _defaultWrapper = BindTool.GetDefaultWrapper(Container, _component);
            _componentEvent = _componentEvent ?? (_component as IComponentEvent<TResult>)?.GetComponentEvent() ?? (_defaultWrapper as IComponentEvent<TResult>)?.GetComponentEvent();
            _propChangeCb = _propChangeCb ?? (_component as IFieldChangeCb<TResult>)?.GetFieldChangeCb() ?? (_defaultWrapper as IFieldChangeCb<TResult>)?.GetFieldChangeCb();
            Debug.Assert(_prop2CpntWrap != null || _cpnt2PropWrap != null, "至少有一个wrapper");
            if (_prop2CpntWrap != null)
            {
                Debug.Assert(_propChangeCb != null, "_propChangeCb 不能为空");
                _property.AddListener(PropertyListener);
            }
            if (_cpnt2PropWrap != null)
            {
                Debug.Assert(_componentEvent != null, "_componentEvent 不能为空");
                _componentEvent.AddListener(ComponentListener);
            }
        }

        private void ComponentListener(TResult val)
        {
            _property.Value = _cpnt2PropWrap(val);
        }

        private void PropertyListener(TData data)
        {
            _propChangeCb(_prop2CpntWrap(data));
        }

        protected override void OnReset()
        {
            if (_prop2CpntWrap != null)
            {
                _property.RemoveListener(PropertyListener);
            }
            
            if (_cpnt2PropWrap != null)
            {
                _componentEvent.RemoveListener(ComponentListener);
            }
        }

        protected override void OnClear()
        {
            _component = default;
            _propChangeCb = default;
            _componentEvent = default;
            _prop2CpntWrap = default;
            _cpnt2PropWrap = default;
            _property = default;
            _defaultWrapper = default;
        }
    }
}