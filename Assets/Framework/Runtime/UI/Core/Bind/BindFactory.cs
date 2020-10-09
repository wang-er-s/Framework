using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
{
    public class BindFactory
    {
        protected List<BaseBind> Binds;

        //单向绑定
        public void Bind<TComponent, TData>
        (TComponent component, ObservableProperty<TData> property, Action<TData> fileChangeCb = null,
            Func<TData, TData> prop2CpntWrap = null) where TComponent : class
        {
            var bind = new BindField<TComponent, TData>(component, property, fileChangeCb, null, BindType.OnWay,
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
            Action<TData> fileChangeCb = null,
            Func<TData, TData> cpnt2PropWrap = null,
            Func<TData, TData> prop2CpntWrap = null) where TComponent : class
        {
            Bind(component, property, fileChangeCb, prop2CpntWrap);
            RevertBind(component, property, componentEvent, cpnt2PropWrap);
        }

        //wrap不同类型单向绑定
        public void Bind<TComponent, TData, TResult>(TComponent component,
            ObservableProperty<TData> property, Func<TData, TResult> field2CpntConvert,
            Action<TResult> fieldChangeCb = null) where TComponent : class
        {
            var bind = new ConvertBindField<TComponent, TData, TResult>(component, property, fieldChangeCb,
                field2CpntConvert, null, null);
            Binds.Add(bind);
        }

        //wrap不同类型反向绑定
        public void RevertBind<TComponent, TData, TResult>(TComponent component,
            ObservableProperty<TData> property,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent = null) where TComponent : class
        {
            var bind = new ConvertBindField<TComponent, TData, TResult>(component, property, null, null,
                cpnt2FieldConvert, componentEvent);
            Binds.Add(bind);
        }

        //不同类型双向绑定
        public void TwoWayBind<TComponent, TData, TViewEvent>
        (TComponent component, ObservableProperty<TData> property,
            Func<TViewEvent, TData> cpnt2FieldConvert, Func<TData, TViewEvent> field2CpntConvert,
            UnityEvent<TViewEvent> componentEvent = null, Action<TViewEvent> fileChangeCb = null)
            where TComponent : class
        {
            Bind(component, property, field2CpntConvert, fileChangeCb);
            RevertBind(component, property, cpnt2FieldConvert, componentEvent);
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
        }
    }

    public enum BindType
    {
        OnWay,
        Revert
    }
}