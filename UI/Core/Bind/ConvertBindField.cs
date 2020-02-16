using System;
using Framework.UI.Wrap;
using UnityEngine.Events;

namespace Framework.UI.Core
{
    public class ConvertBindField<TComponent,TData,TResult> where  TComponent : class
    {
        private TComponent component;
        private Action<TResult> fieldChangeCb;
        private UnityEvent<TResult> componentEvent;
        private Func<TData, TResult> field2CpntConvert;
        private Func<TResult, TData> cpnt2FieldConvert;
        private IBindableProperty<TData> property;
        private object defaultBind;

        public ConvertBindField(TComponent _component, IBindableProperty<TData> _property,
            Action<TResult> _fieldChangeCb,
            Func<TData, TResult> _field2CpntConvert,
            Func<TResult, TData> _cpnt2FieldConvert,
            UnityEvent<TResult> _componentEvent)
        {
            SetValue(_component,_property,_fieldChangeCb,_field2CpntConvert,_cpnt2FieldConvert,_componentEvent);
            InitEvent();
            InitCpntValue();
        }

        public void UpdateValue(TComponent _component, IBindableProperty<TData> _property,
            Action<TResult> _fieldChangeCb,
            Func<TData, TResult> _field2CpntConvert,
            Func<TResult, TData> _cpnt2FieldConvert,
            UnityEvent<TResult> _componentEvent)
        {
            SetValue(_component, _property, _fieldChangeCb, _field2CpntConvert, _cpnt2FieldConvert, _componentEvent);
            InitCpntValue();
        }
        
        private void SetValue(TComponent _component, IBindableProperty<TData> _property,
            Action<TResult> _fieldChangeCb,
            Func<TData, TResult> _field2CpntConvert,
            Func<TResult, TData> _cpnt2FieldConvert,
            UnityEvent<TResult> _componentEvent)
        {
            component = _component;
            fieldChangeCb = _fieldChangeCb;
            componentEvent = _componentEvent;
            property = _property;
            field2CpntConvert = _field2CpntConvert;
            cpnt2FieldConvert = _cpnt2FieldConvert;
        }
        
        private void InitCpntValue()
        {
            if (field2CpntConvert != null)
            {
                fieldChangeCb(field2CpntConvert(property.Value));
            }
        }

        private void InitEvent()
        {
            defaultBind = BindTool.GetDefaultBind(component);
            componentEvent = componentEvent ?? (defaultBind as IComponentEvent<TResult>)?.GetComponentEvent();
            fieldChangeCb = fieldChangeCb ?? (defaultBind as IFieldChangeCb<TResult>)?.GetFieldChangeCb();
            
            Debugger.Assert(field2CpntConvert != null || cpnt2FieldConvert != null);
            if (field2CpntConvert != null)
            {
                property.AddListener((value) => fieldChangeCb(field2CpntConvert(value)));
                
            }
            if (cpnt2FieldConvert != null)
                componentEvent?.AddListener((val) => property.Value = cpnt2FieldConvert(val));
        }
    }
}