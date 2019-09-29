using SF.UI.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.SF.UI.Wrap;
using UnityEngine.Events;
using Component = UnityEngine.Component;

namespace Assets.SF.UI.Core
{

    public class BindField<TComponent,TData> where  TComponent : Component
    {
        private TComponent component;
        private Action<TData> valueChangeEvent;
        private UnityEvent<TData> componentChangEvent;
        private Func<TData, TData> wrapFunc;
        private BindingAbleProperty<TData> field;
        private IBindData<TData> bindData;
        private IBindCommand<TData> bindCommand;

        public BindField(TComponent _component, BindingAbleProperty<TData> _field)
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
            if (wrapFunc != null)
            {
                field?.AddChangeEvent((value) => valueChangeEvent(wrapFunc(value)));
            }
            else
            {
                field?.AddChangeEvent((value) => valueChangeEvent(value));
            }
        }

        public void TwoWay()
        {
            OneWay();
            Revert();
        }

        public void Revert()
        {
            Init();
            if (wrapFunc != null)
            {
                componentChangEvent?.AddListener((data) => field.Value = wrapFunc(data));
            }
            else
            {
                componentChangEvent?.AddListener((data) => field.Value = data);
            }
        }

        private void Init()
        {
            if (bindData != null) return;
            bindData = WrapTool.GetBindData<TData>(component);
            bindCommand = WrapTool.GetBindCommand<TData>(component);
            if (valueChangeEvent == null)
                valueChangeEvent = bindData.GetBindFieldFunc();
            if (componentChangEvent == null)
                componentChangEvent = bindCommand?.GetBindCommandFunc();
            
        }

    }

    public class BindField<TComponent, TData1, TData2, TResult> where  TComponent : Component
    {
        private TComponent component;
        private Action<TResult> ValueChangeEvent;
        private BindingAbleProperty<TData1> field1;
        private BindingAbleProperty<TData2> field2;
        private IBindData<TResult> bindData;
        private Func<TData1, TData2, TResult> wrapFunc;
        public BindField(TComponent _component, BindingAbleProperty<TData1> _field1, BindingAbleProperty<TData2> _field2, Func<TData1, TData2, TResult> _wrapFunc)
        {
            component = _component;
            field1 = _field1;
            field2 = _field2;
            wrapFunc = _wrapFunc;
        }

        public BindField<TComponent, TData1, TData2, TResult> For(Action<TResult> _dataChanged)
        {
            ValueChangeEvent = _dataChanged;
            return this;
        }

        public void Init()
        {
            bindData = WrapTool.GetBindData<TResult>(component);
            if (ValueChangeEvent == null)
                ValueChangeEvent = bindData.GetBindFieldFunc();
            field1.AddChangeEvent((data1) => ValueChangeEvent?.Invoke(wrapFunc(data1, field2.Value)));
            field2.AddChangeEvent((data2) => ValueChangeEvent?.Invoke(wrapFunc(field1.Value, data2)));
        }

    }


}
