using SF.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.SF.UI.Core
{
    public class BindField<TComponent, TData>
    {
        private TComponent component;
        private Action<TData> ValueChangeEvent;
        private BindableProperty<TData> field;

        public BindField(TComponent component, BindableProperty<TData> field)
        {
            this.component = component;
            this.field = field;
        }

        public BindField<TComponent, TData> For(Action<TData> dataChanged)
        {
            ValueChangeEvent = dataChanged;
            return this;
        }

        public void Init()
        {
            field?.AddChangeEvent((value) => ValueChangeEvent(value));
        }

    }

    public class BindField<TComponent, TData1, TData2, TResult>
    {
        private TComponent component;
        private Action<TResult> ValueChangeEvent;
        private BindableProperty<TData1> field1;
        private BindableProperty<TData2> field2;
        private Func<TData1, TData2, TResult> wrapFunc;
        public BindField(TComponent component, BindableProperty<TData1> field1, BindableProperty<TData2> field2)
        {
            this.component = component;
            this.field1 = field1;
            this.field2 = field2;
        }

        public BindField<TComponent, TData1, TData2, TResult> For(Action<TResult> dataChanged)
        {
            ValueChangeEvent = dataChanged;
            return this;
        }

        public BindField<TComponent, TData1, TData2, TResult> Wrap(Func<TData1, TData2, TResult> wrapFunc)
        {
            this.wrapFunc = wrapFunc;
            return this;
        }

        public void Init()
        {
            field1.AddChangeEvent((data1) => ValueChangeEvent?.Invoke(wrapFunc(data1, field2.Value)));
            field2.AddChangeEvent((data2) => ValueChangeEvent?.Invoke(wrapFunc(field1.Value, data2)));
        }

    }


}
