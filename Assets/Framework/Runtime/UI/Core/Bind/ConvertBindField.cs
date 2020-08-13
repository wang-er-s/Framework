using System;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
{
    public class ConvertBindField<TComponent, TData, TResult> where TComponent : class
    {
        private TComponent _component;
        private Action<TResult> _fieldChangeCb;
        private UnityEvent<TResult> _componentEvent;
        private Func<TData, TResult> _field2CpntConvert;
        private Func<TResult, TData> _cpnt2FieldConvert;
        private ObservableProperty<TData> _property;
        private object _defaultWrapper;

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
            this._component = component;
            this._fieldChangeCb = fieldChangeCb;
            this._componentEvent = componentEvent;
            this._property = property;
            this._field2CpntConvert = field2CpntConvert;
            this._cpnt2FieldConvert = cpnt2FieldConvert;
        }

        private void InitCpntValue()
        {
            if (_field2CpntConvert != null) _fieldChangeCb(_field2CpntConvert(_property.Value));
        }

        private void InitEvent()
        {
            _defaultWrapper = BindTool.GetDefaultWrapper(_component);
            if (_componentEvent == null)
                _componentEvent = (_defaultWrapper as IComponentEvent<TResult>)?.GetComponentEvent();
            if (_fieldChangeCb == null)
                _fieldChangeCb = (_defaultWrapper as IFieldChangeCb<TResult>)?.GetFieldChangeCb();
            Log.Assert(_field2CpntConvert != null || _cpnt2FieldConvert != null);
            if (_field2CpntConvert != null)
                _property.AddListener((value) => _fieldChangeCb(_field2CpntConvert(value)));
            if (_cpnt2FieldConvert != null)
                _componentEvent?.AddListener((val) => _property.Value = _cpnt2FieldConvert(val));
        }
    }
}