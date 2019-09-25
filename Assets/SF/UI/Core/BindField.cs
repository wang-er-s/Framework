using SF.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.SF.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.SF.UI.Core
{
    public class BindField<TComponent, TData> where  TComponent : Component
    {
        private TComponent component;
        private Action<TData> ValueChangeEvent;
        private Func<TData, TData> wrapFunc;
        private BindableProperty<TData> field;
        private IBindData<TData> bindData;

        public BindField(TComponent _component, BindableProperty<TData> _field)
        {
            component = _component;
            field = _field;
        }

        public BindField<TComponent, TData> For(Action<TData> _dataChanged)
        {
            ValueChangeEvent = _dataChanged;
            return this;
        }

        public BindField<TComponent, TData> Wrap(Func<TData,TData> _wrapFunc)
        {
            wrapFunc = _wrapFunc;
            return this;
        }

        public BindField<TComponent, TData> TwoWayBind()
        {
            return this;
        }

        public void Init()
        {
            bindData = WrapTool.GetBindData<TData>(component);
            if (ValueChangeEvent == null)
                ValueChangeEvent = bindData.GetBindFieldFunc();
            if (wrapFunc != null)
            {
                field?.AddChangeEvent((value) => ValueChangeEvent(wrapFunc(value)));
            }
            else
            {
                field?.AddChangeEvent((value) => ValueChangeEvent(value));
            }
            field?.ValueChanged(field.Value);
        }

    }

    public class BindField<TComponent, TData1, TData2, TResult> where  TComponent : Component
    {
        private TComponent component;
        private Action<TResult> ValueChangeEvent;
        private BindableProperty<TData1> field1;
        private BindableProperty<TData2> field2;
        private IBindData<TResult> bindData;
        private Func<TData1, TData2, TResult> wrapFunc;
        public BindField(TComponent _component, BindableProperty<TData1> _field1, BindableProperty<TData2> _field2)
        {
            component = _component;
            field1 = _field1;
            field2 = _field2;
        }

        public BindField<TComponent, TData1, TData2, TResult> For(Action<TResult> _dataChanged)
        {
            ValueChangeEvent = _dataChanged;
            return this;
        }

        public BindField<TComponent, TData1, TData2, TResult> Wrap(Func<TData1, TData2, TResult> _wrapFunc)
        {
            wrapFunc = _wrapFunc;
            return this;
        }

        public void Init()
        {
            bindData = WrapTool.GetBindData<TResult>(component);
            if (ValueChangeEvent == null)
                ValueChangeEvent = bindData.GetBindFieldFunc();
            field1.AddChangeEvent((data1) => ValueChangeEvent?.Invoke(wrapFunc(data1, field2.Value)));
            field2.AddChangeEvent((data2) => ValueChangeEvent?.Invoke(wrapFunc(field1.Value, data2)));
            field1?.ValueChanged(field1.Value);
            field2?.ValueChanged(field2.Value);
        }

    }


}
