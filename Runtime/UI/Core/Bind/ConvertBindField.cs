using System;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
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

        public ConvertBindField(object container) : base(container)
        {
        }

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
            Log.Assert(_prop2CpntWrap != null || _cpnt2PropWrap != null);
            if (_prop2CpntWrap != null)
                _property.AddListener((value) => _propChangeCb(_prop2CpntWrap(value)));
            if (_cpnt2PropWrap != null)
                _componentEvent.AddListener((val) => _property.Value = _cpnt2PropWrap(val));
        }

        public override void Clear()
        {
            _componentEvent?.RemoveAllListeners();
            _property.Clear();
        }
    }
}