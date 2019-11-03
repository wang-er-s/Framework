using Nine.UI.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Nine.UI.Wrap;
using UnityEngine.Events;
using Component = UnityEngine.Component;

namespace Assets.Nine.UI.Core
{

    public class BindField<TComponent,TData> where  TComponent : Component
    {

        public static BindField<TComponent, TData> Empty = new BindField<TComponent, TData> (null, null);
        
        private TComponent component;
        private Action<TData> valueChangeEvent;
        private UnityEvent<TData> componentChangEvent;
        private Func<TData, TData> wrapFunc;
        private BindableProperty<TData> field;
        private IBindData<TData> bindData;
        private IBindCommand<TData> bindCommand;
        private BaseWrapper<TComponent> baseWrapper;

        public BindField(TComponent _component, BindableProperty<TData> _field)
        {
            component = _component;
            field = _field;
        }

        public BindField<TComponent, TData> For(Action<TData> _dataChanged)
        {
            valueChangeEvent = _dataChanged;
            return this;
        }

        public BindField<TComponent, TData> For(UnityEvent<TData> _componentChanged)
        {
            componentChangEvent = _componentChanged;
            return this;
        }

        public BindField<TComponent, TData> Wrap(Func<TData,TData> _wrapFunc)
        {
            wrapFunc = _wrapFunc;
            return this;
        }

        public void OneWay()
        {
            Init();
            field?.AddChangeEvent((value) => valueChangeEvent(wrapFunc == null ? value : wrapFunc(value)));
        }

        public void TwoWay()
        {
            OneWay();
            Revert();
        }

        public void Revert()
        {
            Init();
            componentChangEvent?.AddListener((data) => field.Value = wrapFunc == null ? data : wrapFunc(data));
        }

        private void Init()
        {
            if (bindData != null) return;
            baseWrapper = WrapTool.GetWrapper(component);
            bindData = baseWrapper as IBindData<TData>;
            bindCommand = baseWrapper as IBindCommand<TData>;
            if (valueChangeEvent == null)
                valueChangeEvent = bindData?.GetBindFieldFunc();
            if (componentChangEvent == null)
                componentChangEvent = bindCommand?.GetBindCommandFunc();
            
        }

    }

    public class BindField<TComponent, TData1, TData2, TResult> where  TComponent : Component
    {
        private TComponent component;
        private Action<TResult> valueChangeEvent;
        private BindableProperty<TData1> field1;
        private BindableProperty<TData2> field2;
        private IBindData<TResult> bindData;
        private Func<TData1, TData2, TResult> wrapFunc;
        private UnityEvent<TResult> componentChangEvent;
        private BaseWrapper<TComponent> baseWrapper;
        private IBindCommand<TResult> bindCommand;

        public BindField(TComponent _component, BindableProperty<TData1> _field1, BindableProperty<TData2> _field2, Func<TData1, TData2, TResult> _wrapFunc)
        {
            component = _component;
            field1 = _field1;
            field2 = _field2;
            wrapFunc = _wrapFunc;
        }

        public BindField<TComponent, TData1, TData2, TResult> For(Action<TResult> _dataChanged)
        {
            valueChangeEvent = _dataChanged;
            return this;
        }

        public void OneWay()
        {
            Init();
            field1.AddChangeEvent((data1) => valueChangeEvent?.Invoke(wrapFunc(data1, field2.Value)));
            field2.AddChangeEvent((data2) => valueChangeEvent?.Invoke(wrapFunc(field1.Value, data2)));
        }

        private void Init()
        {
            baseWrapper = WrapTool.GetWrapper(component);
            bindData = baseWrapper as IBindData<TResult>;
            bindCommand = baseWrapper as IBindCommand<TResult>;
            if (valueChangeEvent == null)
                valueChangeEvent = bindData?.GetBindFieldFunc();
            if (componentChangEvent == null)
                componentChangEvent = bindCommand?.GetBindCommandFunc();
            field1.AddChangeEvent((data1) => valueChangeEvent?.Invoke(wrapFunc(data1, field2.Value)));
            field2.AddChangeEvent((data2) => valueChangeEvent?.Invoke(wrapFunc(field1.Value, data2)));
        }

    }


}
