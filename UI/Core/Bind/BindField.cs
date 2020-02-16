using System;
using Framework.UI.Wrap;
using UnityEngine.Events;

namespace Framework.UI.Core
{

    public class BindField<TComponent,TData> where  TComponent : class
    {

        private TComponent component;
        private Action<TData> propChangeCb;
        private UnityEvent<TData> componentEvent;
        private Func<TData, TData> prop2CpntWrap;
        private Func<TData, TData> cpnt2PropWrap;
        private IBindableProperty<TData> _property;
        private object defaultBind;
        private BindType bindType;

        public BindField(TComponent _component, IBindableProperty<TData> property, Action<TData> _fieldChangeCb,
            UnityEvent<TData> _componentEvent, BindType _bindType,
            Func<TData, TData> _property2CpntWrap, Func<TData, TData> _cpnt2PropWrap)
        {
            SetValue(_component, property, _fieldChangeCb, _componentEvent, _bindType, _property2CpntWrap,
                _cpnt2PropWrap);
            InitEvent();
            InitCpntValue();
        }
        
        public void UpdateValue(TComponent _component, IBindableProperty<TData> property, Action<TData> _fieldChangeCb,
            UnityEvent<TData> _componentEvent, BindType _bindType,
            Func<TData, TData> _property2CpntWrap, Func<TData, TData> _cpnt2PropWrap)
        {
            SetValue(_component, property, _fieldChangeCb, _componentEvent, _bindType, _property2CpntWrap,
                _cpnt2PropWrap);
            InitCpntValue();
        }

        private void SetValue(TComponent _component, IBindableProperty<TData> property, Action<TData> _fieldChangeCb,
            UnityEvent<TData> _componentEvent, BindType _bindType,
            Func<TData, TData> _property2CpntWrap, Func<TData, TData> _cpnt2PropWrap)
        {
            component = _component;
            _property = property;
            bindType = _bindType;
            prop2CpntWrap = _property2CpntWrap;
            cpnt2PropWrap = _cpnt2PropWrap;
            propChangeCb = _fieldChangeCb;
            componentEvent = _componentEvent;
        }

        /// <summary>
        /// 将field的值初始化给component显示
        /// </summary>
        private void InitCpntValue()
        {
            switch (bindType)
            {
                case BindType.OnWay:
                    propChangeCb(prop2CpntWrap == null ? _property.Value : prop2CpntWrap(_property.Value));
                    break;
                case BindType.Revert:
                    propChangeCb?.Invoke(prop2CpntWrap == null ? _property.Value : prop2CpntWrap(_property.Value));
                    break;
            }
        }

        private void InitEvent()
        {
            defaultBind = BindTool.GetDefaultBind(component);
            propChangeCb = propChangeCb ?? (defaultBind as IFieldChangeCb<TData>)?.GetFieldChangeCb();
            componentEvent =
                componentEvent ?? (defaultBind as IComponentEvent<TData>)?.GetComponentEvent();
            switch (bindType)
            {
                case BindType.OnWay:
                    _property.AddListener((value) =>
                        propChangeCb(prop2CpntWrap == null ? value : prop2CpntWrap(value)));
                    break;
                case BindType.Revert:
                    componentEvent.AddListener((data) =>
                        _property.Value = cpnt2PropWrap == null ? data : cpnt2PropWrap(data));
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
