using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
{
    public class BindFactory<TView, TVm>
    {
        protected TView View;
        protected TVm Vm;
        /// <summary>
        /// 增加缓存，在换绑的时候，
        /// 把新vm的方法绑定到现有的BindCommand里面的Button绑定的方法
        /// 这样不会出现按钮还能触发上个vm方法的情况
        /// </summary>
        protected readonly Dictionary<int, object> CacheBinder;
        /// <summary>
        /// 换绑时候清除之前的bindableProperty
        /// </summary>
        protected readonly List<IClearListener> CanClearListeners;
        protected int Index;

        public BindFactory(TView view, TVm vm)
        {
            this.View = view;
            this.Vm = vm;
            Index = 0;
            CacheBinder = new Dictionary<int, object>();
            CanClearListeners = new List<IClearListener>();
        }

        //单向绑定
        public void Bind<TComponent, TData>
        (TComponent component, ObservableProperty<TData> property, Action<TData> fileChangeCb = null,
            Func<TData, TData> prop2CpntWrap = null) where TComponent : class
        {
            CanClearListeners.TryAdd(property);
            var bindField = new BindField<TComponent, TData>(component, property, fileChangeCb, null, BindType.OnWay,
                prop2CpntWrap, null);
        }

        //反向绑定
        public void RevertBind<TComponent, TData>
        (TComponent component, ObservableProperty<TData> property,
            UnityEvent<TData> componentEvent = null,
            Func<TData, TData> cpnt2PropWrap = null) where TComponent : class
        {
            Index++;
            CanClearListeners.TryAdd(property);
            if (!TryGetBinder<BindField<TComponent, TData>>(out var result,
                (bind) =>
                {
                    bind.UpdateValue(component, property, null, componentEvent, BindType.Revert,
                        null, cpnt2PropWrap);
                }))
            {
                result = new BindField<TComponent, TData>(component, property, null, componentEvent, BindType.Revert,
                    null, cpnt2PropWrap);
                CacheBinder[Index] = result;
            }
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
            CanClearListeners.TryAdd(property);
            var convertBindField = new ConvertBindField<TComponent, TData, TResult>(component, property, fieldChangeCb,
                field2CpntConvert, null, null);
        }

        //wrap不同类型反向绑定
        public void RevertBind<TComponent, TData, TResult>(TComponent component,
            ObservableProperty<TData> property,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent = null) where TComponent : class
        {
            Index++;
            CanClearListeners.TryAdd(property);
            if (!TryGetBinder<ConvertBindField<TComponent, TData, TResult>>(out var result,
                (bind) =>
                {
                    bind.UpdateValue(component, property, null, null, cpnt2FieldConvert,
                        componentEvent);
                }))
            {
                result = new ConvertBindField<TComponent, TData, TResult>(component, property, null, null,
                    cpnt2FieldConvert,
                    componentEvent);
                CacheBinder[Index] = result;
            }
        }
        
        //不同类型双向绑定
        public void TwoWayBind<TComponent, TData, TViewEvent>
        (TComponent component, ObservableProperty<TData> property,
            Func<TViewEvent, TData> cpnt2FieldConvert, Func<TData, TViewEvent> field2CpntConvert,
            UnityEvent<TViewEvent> componentEvent = null, Action<TViewEvent> fileChangeCb = null) where TComponent : class
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
            CanClearListeners.TryAdd(property1);
            CanClearListeners.TryAdd(property2);
            var bindField = new BindField<TComponent, TData1, TData2, TResult>(component, property1, property2, wrapFunc,
                filedChangeCb);
        }

        public void BindData<TData>(ObservableProperty<TData> property, Action<TData> cb)
        {
            CanClearListeners.TryAdd(property);
            cb?.Invoke(property);
            property.AddListener(cb);
        }

        //绑定command
        public void BindCommand<TComponent>
        (TComponent component, Action command, UnityEvent componentEvent = null,
            Func<Action, Action> wrapFunc = null) where TComponent : class
        {
            Index++;
            if (!TryGetBinder<BindCommand<TComponent>>(out var result,
                (bind) => { bind.UpdateValue(component, command, componentEvent, wrapFunc); }))
            {
                result = new BindCommand<TComponent>(component, command, componentEvent, wrapFunc);
                CacheBinder[Index] = result;
            }
        }

        //绑定带参数的command
        public void BindCommand<TComponent, TData>
        (TComponent component, Action<TData> command, UnityEvent<TData> componentEvent = null,
            Func<Action<TData>, Action<TData>> wrapFunc = null) where TComponent : class
        {
            Index++;
            if (!TryGetBinder<BindCommandWithPara<TComponent, TData>>(out var result,
                (bind) => { bind.UpdateValue(component, command, componentEvent, wrapFunc); }))
            {
                result = new BindCommandWithPara<TComponent, TData>(component, command, componentEvent, wrapFunc);
                CacheBinder[Index] = result;
            }
        }

        public void BindList<TComponent, TData>(TComponent component, ObservableList<TData> property)
        {
            var bindList = new BindList<TComponent,TData>(component, property);
        }
        
        protected bool TryGetBinder<T>(out T result, Action<T> updateFunc) where T : class
        {
            result = null;
            if (!CacheBinder.TryGetValue(Index, out var bind)) return false;
            result = bind as T;
            updateFunc(result);
            return true;
        }

        public void UpdateVm(TVm vm)
        {
            foreach (var canClearListener in CanClearListeners) canClearListener.ClearListener(View);
            Index = 0;
            this.Vm = vm;
        }
    }

    public enum BindType
    {
        OnWay,
        Revert
    }
}