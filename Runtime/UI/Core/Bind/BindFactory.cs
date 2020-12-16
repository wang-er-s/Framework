using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
{
    public class BindFactory
    {
        protected List<BaseBind> Binds = new List<BaseBind>();

        //单向绑定
        public void Bind<TComponent, TData>
        (TComponent component, ObservableProperty<TData> property, Action<TData> propChangeCb = null,
            Func<TData, TData> prop2CpntWrap = null) where TComponent : class
        {
            var bind = new BindField<TComponent, TData>(component, property, propChangeCb, null, BindType.OnWay,
                prop2CpntWrap, null);
            Binds.Add(bind);
        }

        //反向绑定
        public void RevertBind<TComponent, TData>
        (TComponent component, ObservableProperty<TData> property,
            UnityEvent<TData> componentEvent = null,
            Func<TData, TData> cpnt2PropWrap = null) where TComponent : class
        {
            var bind = new BindField<TComponent, TData>(component, property, null, componentEvent, BindType.Revert,
                null, cpnt2PropWrap);
            Binds.Add(bind);
        }

        //同类型双向绑定
        public void TwoWayBind<TComponent, TData>
        (TComponent component, ObservableProperty<TData> property,
            UnityEvent<TData> componentEvent = null,
            Action<TData> propChangeCb = null,
            Func<TData, TData> cpnt2PropWrap = null,
            Func<TData, TData> prop2CpntWrap = null) where TComponent : class
        {
            Bind(component, property, propChangeCb, prop2CpntWrap);
            RevertBind(component, property, componentEvent, cpnt2PropWrap);
        }

        //wrap不同类型单向绑定
        public void Bind<TComponent, TData, TResult>(TComponent component,
            ObservableProperty<TData> property, Func<TData, TResult> prop2CpntWrap,
            Action<TResult> propChangeCb = null) where TComponent : class
        {
            var bind = new ConvertBindField<TComponent, TData, TResult>(component, property, propChangeCb,
                prop2CpntWrap, null, null);
            Binds.Add(bind);
        }

        //wrap不同类型反向绑定
        public void RevertBind<TComponent, TData, TResult>(TComponent component,
            ObservableProperty<TData> property,
            Func<TResult, TData> cpnt2PropWrap,
            UnityEvent<TResult> componentEvent = null) where TComponent : class
        {
            var bind = new ConvertBindField<TComponent, TData, TResult>(component, property, null, null,
                cpnt2PropWrap, componentEvent);
            Binds.Add(bind);
        }

        //不同类型双向绑定
        public void TwoWayBind<TComponent, TData, TViewEvent>
        (TComponent component, ObservableProperty<TData> property,
            Func<TViewEvent, TData> cpnt2PropWrap, Func<TData, TViewEvent> prop2CpntWrap,
            UnityEvent<TViewEvent> componentEvent = null, Action<TViewEvent> propChangeCb = null)
            where TComponent : class
        {
            Bind(component, property, prop2CpntWrap, propChangeCb);
            RevertBind(component, property, cpnt2PropWrap, componentEvent);
        }

        //绑定两个field
        public void Bind<TComponent, TData1, TData2, TResult>
        (TComponent component, ObservableProperty<TData1> property1, ObservableProperty<TData2> property2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> filedChangeCb = null)
            where TComponent : class
        {
            var bind = new BindField<TComponent, TData1, TData2, TResult>(component, property1, property2,
                wrapFunc, filedChangeCb);
            Binds.Add(bind);
        }

        public void BindData<TData>(ObservableProperty<TData> property, Action<TData> cb)
        {
            cb?.Invoke(property);
            property.AddListener(cb);
        }

        //绑定command
        public void BindCommand<TComponent>
        (TComponent component, Action command, UnityEvent componentEvent = null,
            Func<Action, Action> wrapFunc = null) where TComponent : class
        {
            var bind = new BindCommand<TComponent>(component, command, componentEvent, wrapFunc);
            Binds.Add(bind);
        }

        //绑定带参数的command
        public void BindCommand<TComponent, TData>
        (TComponent component, Action<TData> command, UnityEvent<TData> componentEvent = null,
            Func<Action<TData>, Action<TData>> wrapFunc = null) where TComponent : class
        {
            var bind = new BindCommandWithPara<TComponent, TData>(component, command, componentEvent, wrapFunc);
            Binds.Add(bind);
        }

        public void BindList<TComponent, TData>(TComponent component, ObservableList<TData> property)
        {
            var bind = new BindList<TComponent, TData>(component, property);
            Binds.Add(bind);
        }

        public void Reset()
        {
            foreach (var bind in Binds)
            {
                bind.ClearBind();
            }
            Binds.Clear();
        }
    }

    public enum BindType
    {
        OnWay,
        Revert
    }
}