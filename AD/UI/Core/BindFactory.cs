using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace AD.UI.Core
{
    public class BindFactory<TView, TVm>
        where TView : View
        where TVm : ViewModel
    {
        private TView view;
        private TVm vm;
        private Dictionary<Component, List<object>> cacheBinder;

        public BindFactory(TView _view, TVm _vm)
        {
            view = _view;
            vm = _vm;
            cacheBinder = new Dictionary<Component, List<object>>();
        }

        //单向绑定
        public BindField<TComponent, TData> Bind<TComponent, TData>
        (TComponent component, IBindableField<TData> filed, Action<TData> fileValueChangeEvent = null,
            Func<TData, TData> wrapFunc = null) where TComponent : Component
        {
            return new BindField<TComponent, TData>(component, filed, _wrapFunc: wrapFunc,
                _valueChangeEvent: fileValueChangeEvent);
        }

        //反向绑定
        public BindField<TComponent, TData> RevertBind<TComponent, TData>
        (TComponent component, IBindableField<TData> filed,
            UnityEvent<TData> componentValueChangeEvent = null,
            Func<TData, TData> component2FieldWrapFunc = null) where TComponent : Component
        {
            return new BindField<TComponent, TData>(component, filed, componentValueChangeEvent,
                component2FieldWrapFunc);
        }

        //wrap不同类型单向绑定
        public BindFieldWrap<TComponent, TData, TResult> Bind<TComponent, TData, TResult>(TComponent component,
            IBindableField<TData> filed,
            Func<TData, TResult> fieldWrapFunc, Action<TResult> fieldValueChangeEvent = null) where TComponent : Component
        {
            return new BindFieldWrap<TComponent, TData, TResult>(component, filed, fieldWrapFunc, fieldValueChangeEvent);
        }
        
        //wrap不同类型反向绑定
        public BindFieldWrap<TComponent, TData, TResult> RevertBind<TComponent, TData, TResult>(TComponent component,
            IBindableField<TData> filed,
            Func<TResult, TData> componentWrapFunc,
            UnityEvent<TResult> _componentValueChangeEvent = null) where TComponent : Component
        {
            return new BindFieldWrap<TComponent, TData, TResult>(component, filed, componentWrapFunc,_componentValueChangeEvent);
        }

        

        public BindField<TComponent, TData1, TData2, TResult> Bind<TComponent, TData1, TData2, TResult>
        (TComponent component, IBindableField<TData1> field1, IBindableField<TData2> field2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> filedValueChangeEvent = null)
            where TComponent : Component
        {
            return new BindField<TComponent, TData1, TData2, TResult>(component, field1, field2, wrapFunc,
                filedValueChangeEvent);
        }

        public void BindData<TData>(IBindableField<TData> property, Action<TData> cb)
        {
            Core.BindData.Bind(property, cb);
        }

        public BindCommand<TComponent> Bind<TComponent>
        (TComponent component, Action command, UnityEvent componentFunc = null,
            Func<Action, Action> wrapFunc = null) where TComponent : Component
        {
            return new BindCommand<TComponent>(component, command, componentFunc, wrapFunc);
        }

        public BindCommandWithPara<TComponent, TData> Bind<TComponent, TData>
        (TComponent component, Action<TData> command, UnityEvent<TData> componentFunc = null,
            Func<Action<TData>, Action<TData>> wrapFunc = null) where TComponent : Component
        {
            return new BindCommandWithPara<TComponent, TData>(component, command, componentFunc, wrapFunc);
        }

        public BindList<TItemVm> BindList<TItemVm>
            (BindableList<TItemVm> list, params View[] views) where TItemVm : ViewModel
        {
            return new BindList<TItemVm>(list, views);
        }

        public BindIpairsView<TItemVm> BindIpairs<TItemVm>
            (BindableList<TItemVm> list, string pattern) where TItemVm : ViewModel
        {
            return new BindIpairsView<TItemVm>(ref list, pattern, view.transform);
        }

        //private object TryGetBinder(Component component)
    }
}