using System;
using AD.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;

namespace AD.UI.Core
{
    public class BindFieldWrap<TComponent,TData,TResult> where  TComponent : Component
    {
        
        private TComponent component;
        private Action<TResult> valueChangeEvent;
        private UnityEvent<TData> componentChangEvent;
        private Func<TData, TResult> wrapFunc;
        private BindableProperty<TData> field;
        private IBindData<TResult> bindData;
        private BaseWrapper<TComponent> baseWrapper;

        public BindFieldWrap(TComponent component, BindableProperty<TData> field, Action<TResult> valueChangeEvent,
            Func<TData, TResult> wrapFunc)
        {
            this.component = component;
            this.valueChangeEvent = valueChangeEvent;
            this.field = field;
            this.wrapFunc = wrapFunc;
        }

        public BindFieldWrap<TComponent, TData, TResult> Wrap(Func<TData, TResult> wrapFunc)
        {
            this.wrapFunc = wrapFunc;
            return this;
        }

        public void OneWay()
        {
            Init();
            if (wrapFunc == null)
                Log.Error(
                    $"{component.name} --> BindableProperty type is not match valueChangeEvent , must have a wrap func.");
            field?.AddChangeEvent((value) => valueChangeEvent(wrapFunc(value)));
        }

        private void Init()
        {
            if (bindData != null) return;
            baseWrapper = WrapTool.GetWrapper(component);
            bindData = baseWrapper as IBindData<TResult>;
            if (valueChangeEvent == null)
                valueChangeEvent = bindData?.GetBindFieldFunc();
        }
        
    }
}