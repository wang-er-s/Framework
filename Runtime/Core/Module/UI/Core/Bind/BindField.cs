using System;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    public class BindField<TComponent, TData> : BaseBind
    {
        private TComponent _component;
        private Action<TData> _propChangeCb;
        private UnityEvent<TData> _componentEvent;
        private Func<TData, TData> _prop2CpntWrap;
        private Func<TData, TData> _cpnt2PropWrap;
        private ObservableProperty<TData> _property;
        private object _defaultWrapper;
        private BindType _bindType;

        private BindField()
        {
        }

        public void Reset(TComponent component, ObservableProperty<TData> property,
            Action<TData> propChangeCb,
            UnityEvent<TData> componentEvent, BindType bindType,
            Func<TData, TData> property2CpntWrap, Func<TData, TData> cpnt2PropWrap)
        {
            SetValue(component, property, propChangeCb, componentEvent, bindType, property2CpntWrap,
                cpnt2PropWrap);
            InitEvent();
        }

        private void SetValue(TComponent component, ObservableProperty<TData> property, Action<TData> propChangeCb,
            UnityEvent<TData> componentEvent, BindType bindType,
            Func<TData, TData> property2CpntWrap, Func<TData, TData> cpnt2PropWrap)
        {
            this._component = component;
            this._property = property;
            this._bindType = bindType;
            _prop2CpntWrap = property2CpntWrap;
            this._cpnt2PropWrap = cpnt2PropWrap;
            _propChangeCb = propChangeCb;
            this._componentEvent = componentEvent;
        }

        private void InitEvent()
        {
            if (_propChangeCb == null || _componentEvent == null)
            {
                _defaultWrapper = BindTool.GetDefaultWrapper(Container, _component);
            }

            switch (_bindType)
            {
                case BindType.OnWay:
                    if (_propChangeCb == null)
                    {
                        IFieldChangeCb<TData> changeCb = _defaultWrapper as IFieldChangeCb<TData>;
                        if (_component is IFieldChangeCb<TData> cb)
                        {
                            changeCb = cb;
                        }

                        _propChangeCb = changeCb?.GetFieldChangeCb();
                    }

                    Debug.Assert(_propChangeCb != null,
                        $"_propChangeCb != null , can not found wrapper , check if the folder(Runtime/UI/Wrap) has {typeof(TComponent).Name} wrapper or {typeof(TComponent).Name} implements IFieldChangeCb<{typeof(TData).Name}> interface");
                    _property.AddListener(PropertyListener);
                    break;
                case BindType.Revert:

                    if (_componentEvent == null)
                    {
                        IComponentEvent<TData> changeCb = _defaultWrapper as IComponentEvent<TData>;
                        if (_component is IComponentEvent<TData> cb)
                        {
                            changeCb = cb;
                        }

                        _componentEvent = changeCb?.GetComponentEvent();
                    }

                    Debug.Assert(_componentEvent != null,
                        $" can not found wrapper , check if the folder(Runtime/UI/Wrap) has {typeof(TComponent).Name} wrapper or {typeof(TComponent).Name} implements IComponentEvent<{typeof(TData).Name}> interface");
                    _componentEvent.AddListener(ComponentListener);
                    break;
            }
        }

        private void PropertyListener(TData data)
        {
            _propChangeCb(_prop2CpntWrap == null ? data : _prop2CpntWrap(data));
        }

        private void ComponentListener(TData data)
        {
            _property.Value = _cpnt2PropWrap == null ? data : _cpnt2PropWrap(data);
        }

        protected override void OnReset()
        {
            _componentEvent?.RemoveListener(ComponentListener);
            _property?.RemoveListener(PropertyListener);
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
            _bindType = default;
        }
    }

    public class BindField<TComponent, TData1, TData2, TResult> : BaseBind where TComponent : class
    {
        private TComponent _component;
        private Action<TResult> _propertyChangeCb;
        private ObservableProperty<TData1> _property1;
        private ObservableProperty<TData2> _property2;
        private Func<TData1, TData2, TResult> _wrapFunc;
        private object _defaultWrapper;

        private BindField()
        {
        }

        public void Reset(TComponent component, ObservableProperty<TData1> property1,
            ObservableProperty<TData2> property2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> filedChangeCb)
        {
            SetValue(component, property1, property2, wrapFunc, filedChangeCb);
            InitEvent();
        }

        private void SetValue(TComponent component, ObservableProperty<TData1> property1,
            ObservableProperty<TData2> property2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> propertyChangeCb)
        {
            this._component = component;
            this._property1 = property1;
            this._property2 = property2;
            this._wrapFunc = wrapFunc;
            this._propertyChangeCb = propertyChangeCb;
        }

        private void InitEvent()
        {
            _defaultWrapper = BindTool.GetDefaultWrapper(Container, _component);
            if (_propertyChangeCb == null)
            {
                IFieldChangeCb<TResult> changeCb = _defaultWrapper as IFieldChangeCb<TResult>;
                if (_component is IFieldChangeCb<TResult> cb)
                {
                    changeCb = cb;
                }

                _propertyChangeCb = changeCb?.GetFieldChangeCb();
            }

            Debug.Assert(_propertyChangeCb != null,
                $" can not found wrapper , check if the folder(Runtime/UI/Wrap) has {typeof(TComponent).Name} wrapper or {typeof(TComponent).Name} implements IFieldChangeCb<{typeof(TResult).Name}> interface");
            _property1.AddListener(Property1Listener);
            _property2.AddListener(Property2Listener);
            _propertyChangeCb(_wrapFunc(_property1.Value, _property2.Value));
        }


        private void Property1Listener(TData1 data1)
        {
            _propertyChangeCb(_wrapFunc(data1, _property2.Value));
        }

        private void Property2Listener(TData2 data2)
        {
            _propertyChangeCb(_wrapFunc(_property1.Value, data2));
        }

        protected override void OnReset()
        {
            _property1.RemoveListener(Property1Listener);
            _property2.RemoveListener(Property2Listener);
        }

        protected override void OnClear()
        {
            _component = default;
            _propertyChangeCb = default;
            _property1 = default;
            _property2 = default;
            _wrapFunc = default;
            _defaultWrapper = default;
        }
    }
}