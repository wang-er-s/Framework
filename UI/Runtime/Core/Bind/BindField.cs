using System;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
{
    public class BindField<TComponent, TData>
    {
        private TComponent component;
        private Action<TData> propChangeCb;
        private UnityEvent<TData> componentEvent;
        private Func<TData, TData> prop2CpntWrap;
        private Func<TData, TData> cpnt2PropWrap;
        private BindableProperty<TData> property;
        private object defaultWrapper;
        private BindType bindType;

        public BindField(TComponent component, BindableProperty<TData> property, Action<TData> fieldChangeCb,
            UnityEvent<TData> componentEvent, BindType bindType,
            Func<TData, TData> property2CpntWrap, Func<TData, TData> cpnt2PropWrap)
        {
            SetValue(component, property, fieldChangeCb, componentEvent, bindType, property2CpntWrap,
                cpnt2PropWrap);
            InitEvent();
            InitCpntValue();
        }

        public void UpdateValue(TComponent component, BindableProperty<TData> property, Action<TData> fieldChangeCb,
            UnityEvent<TData> componentEvent, BindType bindType,
            Func<TData, TData> property2CpntWrap, Func<TData, TData> cpnt2PropWrap)
        {
            SetValue(component, property, fieldChangeCb, componentEvent, bindType, property2CpntWrap,
                cpnt2PropWrap);
            InitCpntValue();
        }

        private void SetValue(TComponent component, BindableProperty<TData> property, Action<TData> fieldChangeCb,
            UnityEvent<TData> componentEvent, BindType bindType,
            Func<TData, TData> property2CpntWrap, Func<TData, TData> cpnt2PropWrap)
        {
            this.component = component;
            this.property = property;
            this.bindType = bindType;
            prop2CpntWrap = property2CpntWrap;
            this.cpnt2PropWrap = cpnt2PropWrap;
            propChangeCb = fieldChangeCb;
            this.componentEvent = componentEvent;
        }

        /// <summary>
        /// 将field的值初始化给component显示
        /// </summary>
        private void InitCpntValue()
        {
            if (bindType != BindType.OnWay) return;
            propChangeCb(prop2CpntWrap == null ? property.Value : prop2CpntWrap(property.Value));
        }

        private void InitEvent()
        {
            defaultWrapper = BindTool.GetDefaultWrapper(component);
            if (propChangeCb == null)
                propChangeCb = (defaultWrapper as IFieldChangeCb<TData>)?.GetFieldChangeCb();
            if (componentEvent == null)
                componentEvent = (defaultWrapper as IComponentEvent<TData>)?.GetComponentEvent();
            switch (bindType)
            {
                case BindType.OnWay:
                    Log.Assert(propChangeCb != null);
                    if(propChangeCb == null) return;
                    property.AddListener((value) =>
                        propChangeCb(prop2CpntWrap == null ? value : prop2CpntWrap(value)));
                    break;
                case BindType.Revert:
                    Log.Assert(componentEvent != null);
                    if(componentEvent == null) return;
                    componentEvent.AddListener((data) =>
                        property.Value = cpnt2PropWrap == null ? data : cpnt2PropWrap(data));
                    break;
            }
        }
    }

    public class BindField<TComponent, TData1, TData2, TResult> where TComponent : class
    {
        private TComponent component;
        private Action<TResult> filedChangeCb;
        private BindableProperty<TData1> property1;
        private BindableProperty<TData2> property2;
        private Func<TData1, TData2, TResult> wrapFunc;
        private object defaultWrapper;

        public BindField(TComponent component, BindableProperty<TData1> property1, BindableProperty<TData2> property2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> filedChangeCb = null)
        {
            SetValue(component, property1, property2, wrapFunc, filedChangeCb);
            InitEvent();
            InitCpntValue();
        }

        public void UpdateValue(TComponent component, BindableProperty<TData1> property1,
            BindableProperty<TData2> property2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> filedChangeCb)
        {
            SetValue(component, property1, property2, wrapFunc, filedChangeCb);
            InitCpntValue();
        }

        private void SetValue(TComponent component, BindableProperty<TData1> property1,
            BindableProperty<TData2> property2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> filedChangeCb)
        {
            this.component = component;
            this.property1 = property1;
            this.property2 = property2;
            this.wrapFunc = wrapFunc;
            this.filedChangeCb = filedChangeCb;
        }

        private void InitCpntValue()
        {
            filedChangeCb(wrapFunc(property1.Value, property2.Value));
        }

        private void InitEvent()
        {
            defaultWrapper = BindTool.GetDefaultWrapper(component);
            filedChangeCb = filedChangeCb ?? (defaultWrapper as IFieldChangeCb<TResult>).GetFieldChangeCb();
            property1.AddListener((data1) => filedChangeCb(wrapFunc(data1, property2.Value)));
            property2.AddListener((data2) => filedChangeCb(wrapFunc(property1.Value, data2)));
            filedChangeCb(wrapFunc(property1.Value, property2.Value));
        }
    }
}