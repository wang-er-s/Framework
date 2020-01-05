using AD.UI.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;
using Component = UnityEngine.Component;

namespace AD.UI.Core
{

    public class BindField<TComponent,TData> where  TComponent : Component
    {

        private TComponent component;
        private Action<TData> fieldValueChangeEvent;
        private UnityEvent<TData> componentValueChangeEvent;
        private Func<TData, TData> file2ComponentWrapFunc;
        private Func<TData, TData> component2FieldWrapFunc;
        private IBindableField<TData> field;
        private BaseWrapper<TComponent> wrapper;
        private BindType bindType;

        private BindField(TComponent _component, IBindableField<TData> _field, Action<TData> _valueChangeEvent,
            UnityEvent<TData> _componentValueChangeEvent, BindType _bindType,
            Func<TData, TData> _file2ComponentWrapFunc, Func<TData, TData> _component2FieldWrapFunc)
        {
            component = _component;
            field = _field;
            bindType = _bindType;
            file2ComponentWrapFunc = _file2ComponentWrapFunc;
            component2FieldWrapFunc = _component2FieldWrapFunc;
            fieldValueChangeEvent = _valueChangeEvent;
            componentValueChangeEvent = _componentValueChangeEvent;
            InitEvent();
        }

        public BindField(TComponent _component, IBindableField<TData> _field, Action<TData> _valueChangeEvent,
            Func<TData, TData> _wrapFunc = null, BindType _bindType = BindType.OnWay) : this(_component, _field,
            _valueChangeEvent, null, _bindType, _wrapFunc, null)
        {
        }

        public BindField(TComponent _component, IBindableField<TData> _field,
            UnityEvent<TData> _componentValueChangeEvent = null,
            Func<TData, TData> _component2FieldWrapFunc = null,
            BindType _bindType = BindType.Revert) :
            this(_component, _field, null, _componentValueChangeEvent, _bindType,
                null, _component2FieldWrapFunc)
        {
        }

        private void InitEvent()
        {
            wrapper = WrapTool.GetWrapper(component);
            fieldValueChangeEvent = fieldValueChangeEvent ?? (wrapper as IBindData<TData>)?.GetBindFieldFunc();
            componentValueChangeEvent = componentValueChangeEvent ?? (wrapper as IBindCommand<TData>)?.GetBindCommandFunc();
            switch (bindType)
            {
                case BindType.OnWay:
                    field?.AddListener((value) => fieldValueChangeEvent(file2ComponentWrapFunc == null ? value : file2ComponentWrapFunc(value)));
                    break;
                case BindType.Revert:
                    fieldValueChangeEvent?.Invoke(field.Value);
                    componentValueChangeEvent?.AddListener((data) => field.Value = component2FieldWrapFunc == null ? data : component2FieldWrapFunc(data));
                    break;
            }
        }
    }

    public class BindField<TComponent, TData1, TData2, TResult> where TComponent : Component
    {
        private TComponent component;
        private Action<TResult> filedValueChangeEvent;
        private IBindableField<TData1> field1;
        private IBindableField<TData2> field2;
        private Func<TData1, TData2, TResult> wrapFunc;
        private BaseWrapper<TComponent> wrapper;

        public BindField(TComponent _component, IBindableField<TData1> _field1, IBindableField<TData2> _field2,
            Func<TData1, TData2, TResult> _wrapFunc, Action<TResult> _filedValueChangeEvent = null)
        {
            component = _component;
            field1 = _field1;
            field2 = _field2;
            wrapFunc = _wrapFunc;
            filedValueChangeEvent = _filedValueChangeEvent;
            InitEvent();
        }

        private void InitEvent()
        {
            wrapper = WrapTool.GetWrapper(component);
            filedValueChangeEvent = filedValueChangeEvent ?? (wrapper as IBindData<TResult>)?.GetBindFieldFunc();
            field1.AddListener((data1) => filedValueChangeEvent?.Invoke(wrapFunc(data1, field2.Value)));
            field2.AddListener((data2) => filedValueChangeEvent?.Invoke(wrapFunc(field1.Value, data2)));
        }
    }


}
