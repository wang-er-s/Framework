using System;
using AD.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;

namespace AD.UI.Core
{
    public class BindFieldWrap<TComponent,TData,TResult> : IInitBind where  TComponent : Component
    {
        
        private TComponent component;
        private Action<TResult> valueChangeEvent;
        private UnityEvent<TResult> componentChangEvent;
        private Func<TData, TResult> wrapFunc;
        private Func<TResult, TData> revertWrapFunc;
        private BindableProperty<TData> field;
        private IBindData<TResult> bindData;
        private IBindCommand<TResult> bindCommand;
        private BaseWrapper<TComponent> baseWrapper;

        public BindFieldWrap(TComponent component, BindableProperty<TData> field, Action<TResult> valueChangeEvent,
            Func<TData, TResult> wrapFunc = null, Func<TResult, TData> revertWrapFunc = null)
        {
            this.component = component;
            this.valueChangeEvent = valueChangeEvent;
            this.field = field;
            this.wrapFunc = wrapFunc;
            this.revertWrapFunc = revertWrapFunc;
        }

        public BindFieldWrap<TComponent, TData, TResult> Wrap(Func<TData, TResult> wrapFunc)
        {
            this.wrapFunc = wrapFunc;
            return this;
        }
        
        public BindFieldWrap<TComponent, TData, TResult> For(Action<TResult> _valueChangeEvent)
        {
            valueChangeEvent = _valueChangeEvent;
            return this;
        }
        
        public BindFieldWrap<TComponent, TData, TResult> For(UnityEvent<TResult> _dataChanged)
        {
            componentChangEvent = _dataChanged;
            return this;
        }

        private void Init()
        {
            if (bindData != null) return;
            baseWrapper = WrapTool.GetWrapper(component);
            bindData = baseWrapper as IBindData<TResult>;
            bindCommand = baseWrapper as IBindCommand<TResult>;
            if (valueChangeEvent == null)
                valueChangeEvent = bindData?.GetBindFieldFunc();
            if (componentChangEvent == null)
                componentChangEvent = bindCommand?.GetBindCommandFunc();
        }

        public void InitBind()
        {
            Init();
            if (wrapFunc == null && revertWrapFunc == null)
                Debug.LogError(
                    $"{component.name} --> BindableProperty type is not match valueChangeEvent , must have a wrap func.");
            if (wrapFunc != null)
                field?.AddChangeEvent((value) => valueChangeEvent(wrapFunc(value)));
            if (revertWrapFunc != null)
                componentChangEvent?.AddListener((val) => field.Value = revertWrapFunc(val));
        }
    }
}