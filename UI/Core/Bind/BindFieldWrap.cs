using System;
using AD.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;

namespace AD.UI.Core
{
    public class BindFieldWrap<TComponent,TData,TResult> where  TComponent : Component
    {
        private TComponent component;
        private Action<TResult> fieldValueChangeEvent;
        private UnityEvent<TResult> componentValueChangeEvent;
        private Func<TData, TResult> fieldWrapFunc;
        private Func<TResult, TData> componentWrapFunc;
        private IBindableField<TData> field;
        private BaseWrapper<TComponent> wrapper;

        private BindFieldWrap(TComponent _component, IBindableField<TData> _field,
            Action<TResult> _fieldValueChangeEvent,
            Func<TData, TResult> _fieldWrapFunc,
            Func<TResult, TData> _componentWrapFunc,
            UnityEvent<TResult> _componentValueChangeEvent)
        {
            component = _component;
            fieldValueChangeEvent = _fieldValueChangeEvent;
            componentValueChangeEvent = _componentValueChangeEvent;
            field = _field;
            fieldWrapFunc = _fieldWrapFunc;
            componentWrapFunc = _componentWrapFunc;
            InitEvent();
        }

        public BindFieldWrap(TComponent _component, IBindableField<TData> _field,
            Func<TData, TResult> _fieldWrapFunc,
            Action<TResult> _fieldValueChangeEvent = null) : this(
            _component, _field, _fieldValueChangeEvent, _fieldWrapFunc, null, null)
        {
        }

        public BindFieldWrap(TComponent _component, IBindableField<TData> _field,
            Func<TResult, TData> _componentWrapFunc,
            UnityEvent<TResult> _componentValueChangeEvent = null) : this(
            _component, _field, null, null, _componentWrapFunc, _componentValueChangeEvent)
        {
        }

        private void InitEvent()
        {
            wrapper = WrapTool.GetWrapper(component);
            componentValueChangeEvent = componentValueChangeEvent ?? (wrapper as IBindCommand<TResult>)?.GetBindCommandFunc();
            fieldValueChangeEvent = fieldValueChangeEvent ?? (wrapper as IBindData<TResult>)?.GetBindFieldFunc();
            if (fieldWrapFunc != null)
            {
                field.AddListener((value) => fieldValueChangeEvent(fieldWrapFunc(value)));
                fieldValueChangeEvent(fieldWrapFunc(field.Value));
            }
            if (componentWrapFunc != null)
                componentValueChangeEvent?.AddListener((val) => field.Value = componentWrapFunc(val));
        }
    }
}