using System;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
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

        public BindField(object container, TComponent component, ObservableProperty<TData> property, Action<TData> propChangeCb,
            UnityEvent<TData> componentEvent, BindType bindType,
            Func<TData, TData> property2CpntWrap, Func<TData, TData> cpnt2PropWrap) : base(container)
        {
            SetValue(component, property, propChangeCb, componentEvent, bindType, property2CpntWrap,
                cpnt2PropWrap);
            InitEvent();
            InitCpntValue();
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

        /// <summary>
        /// 将field的值初始化给component显示
        /// </summary>
        private void InitCpntValue()
        {
            if (_bindType != BindType.OnWay) return;
            _propChangeCb(_prop2CpntWrap == null ? _property.Value : _prop2CpntWrap(_property.Value));
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
                    Log.Assert(_propChangeCb != null,
                        $"_propChangeCb != null , can not found wrapper , check if the folder(Runtime/UI/Wrap) has {typeof(TComponent).Name} wrapper or {typeof(TComponent).Name} implements IFieldChangeCb<{typeof(TData).Name}> interface");
                    _property.AddListener((value) =>
                        _propChangeCb(_prop2CpntWrap == null ? value : _prop2CpntWrap(value)));
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
                    Log.Assert(_componentEvent != null,
                        $" can not found wrapper , check if the folder(Runtime/UI/Wrap) has {typeof(TComponent).Name} wrapper or {typeof(TComponent).Name} implements IComponentEvent<{typeof(TData).Name}> interface");
                    _componentEvent.AddListener((data) =>
                        _property.Value = _cpnt2PropWrap == null ? data : _cpnt2PropWrap(data));
                    break;
            }
        }

        public override void ClearBind()
        {
            _property.ClearListener();
            _componentEvent?.RemoveAllListeners();
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

        public BindField(object container, TComponent component, ObservableProperty<TData1> property1, ObservableProperty<TData2> property2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> filedChangeCb = null)  : base(container)
        {
            SetValue(component, property1, property2, wrapFunc, filedChangeCb);
            InitEvent();
            InitCpntValue();
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

        private void InitCpntValue()
        {
            _propertyChangeCb(_wrapFunc(_property1.Value, _property2.Value));
        }

        private void InitEvent()
        {
            if (_propertyChangeCb == null)
            {
                IFieldChangeCb<TResult> changeCb = _defaultWrapper as IFieldChangeCb<TResult>;
                if (_component is IFieldChangeCb<TResult> cb)
                {
                    changeCb = cb;
                }
                _propertyChangeCb = changeCb?.GetFieldChangeCb();
            }
            _defaultWrapper = BindTool.GetDefaultWrapper(Container, _component);
            Log.Assert(_propertyChangeCb != null,
                $" can not found wrapper , check if the folder(Runtime/UI/Wrap) has {typeof(TComponent).Name} wrapper or {typeof(TComponent).Name} implements IFieldChangeCb<{typeof(TResult).Name}> interface");
            _property1.AddListener((data1) => _propertyChangeCb(_wrapFunc(data1, _property2.Value)));
            _property2.AddListener((data2) => _propertyChangeCb(_wrapFunc(_property1.Value, data2)));
            _propertyChangeCb(_wrapFunc(_property1.Value, _property2.Value));
        }

        public override void ClearBind()
        {
            _property1.ClearListener();
            _property2.ClearListener();
        }
    }
}