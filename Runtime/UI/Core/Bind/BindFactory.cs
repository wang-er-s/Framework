using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Framework.UI.Core.Bind
{
    public class BindFactory
    {
        protected List<IClearable> clearables = new List<IClearable>();
        protected Queue<BaseBind> CacheBinds = new Queue<BaseBind>();
        protected object Container;

        public BindFactory(object container)
        {
            Container = container;
        }
        
        //单向绑定
        public void Bind<TComponent, TData>
        (TComponent component, ObservableProperty<TData> property, Action<TData> fieldChangeCb = null,
            Func<TData, TData> prop2CpntWrap = null) where TComponent : class
        {
            BindField<TComponent, TData> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindField<TComponent, TData>) CacheBinds.Dequeue();
            }
            else
            {
                bind = new BindField<TComponent, TData>(Container);
            }

            bind.Reset(component, property, fieldChangeCb, null, BindType.OnWay, prop2CpntWrap, null);
            AddClearable(bind);
        }

        //反向绑定
        public void RevertBind<TComponent, TData>
        (TComponent component, ObservableProperty<TData> property,
            UnityEvent<TData> componentEvent = null,
            Func<TData, TData> cpnt2PropWrap = null) where TComponent : class
        {
            BindField<TComponent, TData> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindField<TComponent, TData>) CacheBinds.Dequeue();
                
            }
            else
            {
                bind = new BindField<TComponent, TData>(Container);
            }
            bind.Reset(component, property, null, componentEvent, BindType.Revert, null, cpnt2PropWrap);
            AddClearable(bind);
        }

        //同类型双向绑定
        public void TwoWayBind<TComponent, TData>
        (TComponent component, ObservableProperty<TData> property,
            UnityEvent<TData> componentEvent = null,
            Action<TData> fieldChangeCb = null,
            Func<TData, TData> cpnt2PropWrap = null,
            Func<TData, TData> prop2CpntWrap = null) where TComponent : class
        {
            Bind(component, property, fieldChangeCb, prop2CpntWrap);
            RevertBind(component, property, componentEvent, cpnt2PropWrap);
        }

        //wrap不同类型单向绑定
        public void Bind<TComponent, TData, TResult>(TComponent component,
            ObservableProperty<TData> property, Func<TData, TResult> field2CpntConvert,
            Action<TResult> fieldChangeCb = null) where TComponent : class
        {
            ConvertBindField<TComponent, TData, TResult> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (ConvertBindField<TComponent, TData, TResult>) CacheBinds.Dequeue();
                
            }
            else
            {
                bind = new ConvertBindField<TComponent, TData, TResult>(Container);
            }
            bind.Reset(component, property, fieldChangeCb, field2CpntConvert, null, null);
            AddClearable(bind);
        }

        //wrap不同类型反向绑定
        public void RevertBind<TComponent, TData, TResult>(TComponent component,
            ObservableProperty<TData> property,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent = null) where TComponent : class
        {
            ConvertBindField<TComponent, TData, TResult> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (ConvertBindField<TComponent, TData, TResult>) CacheBinds.Dequeue();
                
            }
            else
            {
                bind = new ConvertBindField<TComponent, TData, TResult>(Container);
            }
            bind.Reset(component, property, null, null, cpnt2FieldConvert, componentEvent);
            AddClearable(bind);
        }

        //不同类型双向绑定
        public void TwoWayBind<TComponent, TData, TViewEvent>
        (TComponent component, ObservableProperty<TData> property,
            Func<TViewEvent, TData> cpnt2FieldConvert, Func<TData, TViewEvent> field2CpntConvert,
            UnityEvent<TViewEvent> componentEvent = null, Action<TViewEvent> fieldChangeCb = null)
            where TComponent : class
        {
            Bind(component, property, field2CpntConvert, fieldChangeCb);
            RevertBind(component, property, cpnt2FieldConvert, componentEvent);
        }

        //绑定两个field
        public void Bind<TComponent, TData1, TData2, TResult>
        (TComponent component, ObservableProperty<TData1> property1, ObservableProperty<TData2> property2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> filedChangeCb = null)
            where TComponent : class
        {
            BindField<TComponent, TData1, TData2, TResult> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindField<TComponent, TData1, TData2, TResult>) CacheBinds.Dequeue();
                
            }
            else
            {
                bind = new BindField<TComponent, TData1, TData2, TResult>(Container);
            }
            bind.Reset(component, property1, property2, wrapFunc, filedChangeCb);
            AddClearable(bind);
        }

        public void BindData<TData>(ObservableProperty<TData> property, Action<TData> cb)
        {
            AddClearable(property.AddListener(cb));
        }
        
        public void BindData<TData1,TData2>(ObservableProperty<TData1> property,ObservableProperty<TData2> property2, Action<TData1, TData2> cb)
        {
            AddClearable(property.AddListener(data1 => cb?.Invoke(data1, property2)));
            AddClearable(property2.AddListener(data2 => cb?.Invoke(property, data2)));
        }
        
        public void BindData<TData1,TData2,TData3>(ObservableProperty<TData1> property,ObservableProperty<TData2> property2,ObservableProperty<TData3> property3, Action<TData1, TData2, TData3> cb)
        {
            AddClearable(property.AddListener((data1) =>  cb?.Invoke(data1, property2, property3)));
            AddClearable(property2.AddListener((data2) =>  cb?.Invoke(property, data2, property3)));
            AddClearable(property3.AddListener((data3) =>  cb?.Invoke(property, property2, data3)));
        }

        //绑定command
        public void BindCommand<TComponent>
        (TComponent component, Action command, UnityEvent componentEvent = null,
            Func<Action, Action> wrapFunc = null) where TComponent : class
        {
            BindCommand<TComponent> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindCommand<TComponent>) CacheBinds.Dequeue();
                
            }
            else
            {
                bind = new BindCommand<TComponent>(Container);
            }
            bind.Reset(component, command, componentEvent, wrapFunc);
            AddClearable(bind);
        }

        //绑定带参数的command
        public void BindCommand<TComponent, TData>
        (TComponent component, Action<TData> command, UnityEvent<TData> componentEvent = null,
            Func<Action<TData>, Action<TData>> wrapFunc = null) where TComponent : class
        {
            BindCommandWithPara<TComponent, TData> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindCommandWithPara<TComponent, TData>) CacheBinds.Dequeue();
                
            }
            else
            {
                bind = new BindCommandWithPara<TComponent, TData>(Container);
            }
            bind.Reset(component, command, componentEvent, wrapFunc);
            AddClearable(bind);
        }

        public void BindList<TComponent, TData>(TComponent component, ObservableList<TData> property,
            Action<TComponent, TData> onCreate = null, Action<TComponent, TData> onDestroy = null) where TComponent : Object
        {
            BindList<TComponent, TData> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindList<TComponent, TData>) CacheBinds.Dequeue();
                
            }
            else
            {
                bind = new BindList<TComponent, TData>(Container);
            }
            bind.Reset(component, property, onCreate, onDestroy);
            AddClearable(bind);
        }

        public void Reset()
        {
            foreach (var clearable in clearables)
            {
                clearable.Clear();
                if (clearable is BaseBind bind)
                {
                    CacheBinds.Enqueue(bind);
                }
            }
            clearables.Clear();
        }

        public void AddClearable(IClearable clearable)
        {
            //viewModel.OnClearModelBinding += clearable.ClearModel;
            clearables.Add(clearable);
        }
    }

    public enum BindType
    {
        OnWay,
        Revert
    }
}