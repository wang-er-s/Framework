using System;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
{
    public class ConvertBindField<TComponent, TData, TResult> where TComponent : class
    {
        private TComponent component;
        private Action<TResult> fieldChangeCb;
        private UnityEvent<TResult> componentEvent;
        private Func<TData, TResult> field2CpntConvert;
        private Func<TResult, TData> cpnt2FieldConvert;
        private ObservableProperty<TData> property;
        private object defaultWrapper;

        public ConvertBindField(TComponent component, ObservableProperty<TData> property,
            Action<TResult> fieldChangeCb,
            Func<TData, TResult> field2CpntConvert,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent)
        {
            SetValue(component, property, fieldChangeCb, field2CpntConvert, cpnt2FieldConvert, componentEvent);
            InitEvent();
            InitCpntValue();
        }

        public void UpdateValue(TComponent component, ObservableProperty<TData> property,
            Action<TResult> fieldChangeCb,
            Func<TData, TResult> field2CpntConvert,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent)
        {
            SetValue(component, property, fieldChangeCb, field2CpntConvert, cpnt2FieldConvert, componentEvent);
            InitCpntValue();
        }

        private void SetValue(TComponent component, ObservableProperty<TData> property,
            Action<TResult> fieldChangeCb,
            Func<TData, TResult> field2CpntConvert,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent)
        {
            this.component = component;
            this.fieldChangeCb = fieldChangeCb;
            this.componentEvent = componentEvent;
            this.property = property;
            this.field2CpntConvert = field2CpntConvert;
            this.cpnt2FieldConvert = cpnt2FieldConvert;
        }

        private void InitCpntValue()
        {
            if (field2CpntConvert != null) fieldChangeCb(field2CpntConvert(property.Value));
        }

        private void InitEvent()
        {
            defaultWrapper = BindTool.GetDefaultWrapper(component);
            if (componentEvent == null)
                componentEvent = (defaultWrapper as IComponentEvent<TResult>)?.GetComponentEvent();
            if (fieldChangeCb == null)
                fieldChangeCb = (defaultWrapper as IFieldChangeCb<TResult>)?.GetFieldChangeCb();
            Log.Assert(field2CpntConvert != null || cpnt2FieldConvert != null);
            if (field2CpntConvert != null)
                property.AddListener((value) => fieldChangeCb(field2CpntConvert(value)));
            if (cpnt2FieldConvert != null)
                componentEvent?.AddListener((val) => property.Value = cpnt2FieldConvert(val));
        }
    }
}