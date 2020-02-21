using System;
using Framework.UI.Wrap;
using UnityEngine.Events;

namespace Framework.UI.Core
{
    public class ConvertBindField<TComponent,TData,TResult> where  TComponent : class
    {
        private TComponent _component;
        private Action<TResult> _fieldChangeCb;
        private UnityEvent<TResult> _componentEvent;
        private Func<TData, TResult> _field2CpntConvert;
        private Func<TResult, TData> _cpnt2FieldConvert;
        private IBindableProperty<TData> _property;
        private object _defaultBind;

        public ConvertBindField(TComponent component, IBindableProperty<TData> property,
            Action<TResult> fieldChangeCb,
            Func<TData, TResult> field2CpntConvert,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent)
        {
            SetValue(component,property,fieldChangeCb,field2CpntConvert,cpnt2FieldConvert,componentEvent);
            InitEvent();
            InitCpntValue();
        }

        public void UpdateValue(TComponent component, IBindableProperty<TData> property,
            Action<TResult> fieldChangeCb,
            Func<TData, TResult> field2CpntConvert,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent)
        {
            SetValue(component, property, fieldChangeCb, field2CpntConvert, cpnt2FieldConvert, componentEvent);
            InitCpntValue();
        }
        
        private void SetValue(TComponent component, IBindableProperty<TData> property,
            Action<TResult> fieldChangeCb,
            Func<TData, TResult> field2CpntConvert,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent)
        {
            _component = component;
            _fieldChangeCb = fieldChangeCb;
            _componentEvent = componentEvent;
            _property = property;
            _field2CpntConvert = field2CpntConvert;
            _cpnt2FieldConvert = cpnt2FieldConvert;
        }
        
        private void InitCpntValue()
        {
            if (_field2CpntConvert != null)
            {
                _fieldChangeCb(_field2CpntConvert(_property.Value));
            }
        }

        private void InitEvent()
        {
            _defaultBind = BindTool.GetDefaultBind(_component);
            _componentEvent ??= (_defaultBind as IComponentEvent<TResult>)?.GetComponentEvent();
            _fieldChangeCb ??= (_defaultBind as IFieldChangeCb<TResult>)?.GetFieldChangeCb();
            Debugger.Assert(_field2CpntConvert != null || _cpnt2FieldConvert != null);
            if (_field2CpntConvert != null)
                _property.AddListener((value) => _fieldChangeCb(_field2CpntConvert(value)));
            if (_cpnt2FieldConvert != null)
                _componentEvent?.AddListener((val) => _property.Value = _cpnt2FieldConvert(val));
        }
    }
}