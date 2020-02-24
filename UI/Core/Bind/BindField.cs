using System;
using Framework.UI.Wrap;
using UnityEngine.Events;

namespace Framework.UI.Core
{

    public class BindField<TComponent,TData> where  TComponent : class
    {

        private TComponent _component;
        private Action<TData> _propChangeCb;
        private UnityEvent<TData> _componentEvent;
        private Func<TData, TData> _prop2CpntWrap;
        private Func<TData, TData> _cpnt2PropWrap;
        private IBindableProperty<TData> _property;
        private object _defaultBind;
        private BindType _bindType;

        public BindField(TComponent component, IBindableProperty<TData> property, Action<TData> fieldChangeCb,
            UnityEvent<TData> componentEvent, BindType bindType,
            Func<TData, TData> property2CpntWrap, Func<TData, TData> cpnt2PropWrap)
        {
            SetValue(component, property, fieldChangeCb, componentEvent, bindType, property2CpntWrap,
                cpnt2PropWrap);
            InitEvent();
            InitCpntValue();
        }
        
        public void UpdateValue(TComponent component, IBindableProperty<TData> property, Action<TData> fieldChangeCb,
            UnityEvent<TData> componentEvent, BindType bindType,
            Func<TData, TData> property2CpntWrap, Func<TData, TData> cpnt2PropWrap)
        {
            SetValue(component, property, fieldChangeCb, componentEvent, bindType, property2CpntWrap,
                cpnt2PropWrap);
            InitCpntValue();
        }

        private void SetValue(TComponent component, IBindableProperty<TData> property, Action<TData> fieldChangeCb,
            UnityEvent<TData> componentEvent, BindType bindType,
            Func<TData, TData> property2CpntWrap, Func<TData, TData> cpnt2PropWrap)
        {
            _component = component;
            _property = property;
            _bindType = bindType;
            _prop2CpntWrap = property2CpntWrap;
            _cpnt2PropWrap = cpnt2PropWrap;
            _propChangeCb = fieldChangeCb;
            _componentEvent = componentEvent;
        }

        /// <summary>
        /// 将field的值初始化给component显示
        /// </summary>
        private void InitCpntValue()
        {
            switch (_bindType)
            {
                case BindType.OnWay:
                    _propChangeCb(_prop2CpntWrap == null ? _property.Value : _prop2CpntWrap(_property.Value));
                    break;
                case BindType.Revert:
                    _propChangeCb?.Invoke(_prop2CpntWrap == null ? _property.Value : _prop2CpntWrap(_property.Value));
                    break;
            }
        }

        private void InitEvent()
        {
            _defaultBind = BindTool.GetDefaultBind(_component);
            if (_propChangeCb == null)
                _propChangeCb = (_defaultBind as IFieldChangeCb<TData>)?.GetFieldChangeCb();
            if (_componentEvent == null)
                _componentEvent = (_defaultBind as IComponentEvent<TData>)?.GetComponentEvent();
            switch (_bindType)
            {
                case BindType.OnWay:
                    Debugger.Assert(_propChangeCb != null);
                    _property.AddListener((value) =>
                        _propChangeCb(_prop2CpntWrap == null ? value : _prop2CpntWrap(value)));
                    break;
                case BindType.Revert:
                    Debugger.Assert(_componentEvent != null);
                    _componentEvent.AddListener((data) =>
                        _property.Value = _cpnt2PropWrap == null ? data : _cpnt2PropWrap(data));
                    break;
            }
        }
    }

    public class BindField<TComponent, TData1, TData2, TResult> where TComponent : class
    {
        private TComponent component;
        private Action<TResult> filedChangeCb;
        private IBindableProperty<TData1> property1;
        private IBindableProperty<TData2> property2;
        private Func<TData1, TData2, TResult> wrapFunc;
        private object defaultBind;

        public BindField(TComponent _component, IBindableProperty<TData1> _property1, IBindableProperty<TData2> _property2,
            Func<TData1, TData2, TResult> _wrapFunc, Action<TResult> _filedChangeCb = null)
        {
            SetValue(_component,_property1,_property2,_wrapFunc,_filedChangeCb);
            InitEvent();
            InitCpntValue();
        }

        public void UpdateValue(TComponent _component, IBindableProperty<TData1> _property1, IBindableProperty<TData2> _property2,
            Func<TData1, TData2, TResult> _wrapFunc, Action<TResult> _filedChangeCb)
        {
            SetValue(_component,_property1,_property2,_wrapFunc,_filedChangeCb);
            InitCpntValue();
        }
        
        private void SetValue(TComponent _component, IBindableProperty<TData1> _property1, IBindableProperty<TData2> _property2,
            Func<TData1, TData2, TResult> _wrapFunc, Action<TResult> _filedChangeCb)
        {
            component = _component;
            property1 = _property1;
            property2 = _property2;
            wrapFunc = _wrapFunc;
            filedChangeCb = _filedChangeCb;
        }
        
        private void InitCpntValue()
        {
            filedChangeCb(wrapFunc(property1.Value, property2.Value));
        }

        private void InitEvent()
        {
            defaultBind = BindTool.GetDefaultBind(component);
            filedChangeCb = filedChangeCb ?? (defaultBind as IFieldChangeCb<TResult>).GetFieldChangeCb();
            property1.AddListener((data1) => filedChangeCb(wrapFunc(data1, property2.Value)));
            property2.AddListener((data2) => filedChangeCb(wrapFunc(property1.Value, data2)));
            filedChangeCb(wrapFunc(property1.Value, property2.Value));
        }
    }


}
