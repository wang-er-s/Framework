using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI.Core
{

    public abstract class BindFactory<TView, TVm>
        where TView : class
        where TVm : ViewModel
    {
        protected TView _view;
        protected TVm _vm;
        protected Dictionary<int, object> _cacheBinder;
        protected List<IClearListener> _canClearListeners;
        protected int _index;

        public BindFactory(TView view, TVm vm)
        {
            _view = view;
            _vm = vm;
            _index = 0;
            _cacheBinder = new Dictionary<int, object>();
            _canClearListeners = new List<IClearListener>();
        }

        //单向绑定
        public BindField<TComponent, TData> Bind<TComponent, TData>
        (TComponent component, BindableProperty<TData> property, Action<TData> fileChangeCb = null,
            Func<TData, TData> prop2CpntWrap = null) where TComponent : class
        {
            _canClearListeners.TryAdd(property);
            return new BindField<TComponent, TData>(component, property, fileChangeCb, null, BindType.OnWay,
                prop2CpntWrap, null);
        }

        //反向绑定
        public BindField<TComponent, TData> RevertBind<TComponent, TData>
        (TComponent component, BindableProperty<TData> property,
            UnityEvent<TData> componentEvent = null,
            Func<TData, TData> cpnt2PropWrap = null) where TComponent : class
        {
            _index++;
            _canClearListeners.TryAdd(property);
            if (!TryGetBinder<BindField<TComponent, TData>>(out var result,
                (bind) =>
                {
                    bind.UpdateValue(component, property, null, componentEvent, BindType.Revert,
                        null, cpnt2PropWrap);
                }))
            {
                result = new BindField<TComponent, TData>(component, property, null, componentEvent, BindType.Revert,
                    null, cpnt2PropWrap);
                _cacheBinder[_index] = result;
            }
            return result;
        }

        //wrap不同类型单向绑定
        public ConvertBindField<TComponent, TData, TResult> Bind<TComponent, TData, TResult>(TComponent component,
            BindableProperty<TData> property,
            Func<TData, TResult> field2CpntConvert, Action<TResult> _fieldChangeCb = null) where TComponent : class
        {
            _canClearListeners.TryAdd(property);
            return new ConvertBindField<TComponent, TData, TResult>(component, property, _fieldChangeCb, field2CpntConvert,
                null, null);
        }
        
        //wrap不同类型反向绑定
        public ConvertBindField<TComponent, TData, TResult> RevertBind<TComponent, TData, TResult>(TComponent component,
            BindableProperty<TData> property,
            Func<TResult, TData> cpnt2FieldConvert,
            UnityEvent<TResult> componentEvent = null) where TComponent : class
        {
            _index++;
            _canClearListeners.TryAdd(property);
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
                _cacheBinder[_index] = result;
            }
            return result;
        }

        //绑定两个field
        public BindField<TComponent, TData1, TData2, TResult> Bind<TComponent, TData1, TData2, TResult>
        (TComponent component, BindableProperty<TData1> property1, BindableProperty<TData2> property2,
            Func<TData1, TData2, TResult> wrapFunc, Action<TResult> filedChangeCb = null)
            where TComponent : class
        {
            _canClearListeners.TryAdd(property1);
            _canClearListeners.TryAdd(property2);
            return new BindField<TComponent, TData1, TData2, TResult>(component, property1, property2, wrapFunc,
                filedChangeCb);
        }

        public void BindData<TData>(BindableProperty<TData> property, Action<TData> cb)
        {
            _canClearListeners.TryAdd(property);
            cb?.Invoke(property);
            property.AddListener(cb);
        }

        //绑定command
        public BindCommand<TComponent> Bind<TComponent>
        (TComponent component, Action command, UnityEvent componentEvent = null,
            Func<Action, Action> wrapFunc = null) where TComponent : class
        {
            _index++;
            if (!TryGetBinder<BindCommand<TComponent>>(out var result,
                (bind) => { bind.UpdateValue(component, command, componentEvent, wrapFunc); }))
            {
                result = new BindCommand<TComponent>(component, command, componentEvent, wrapFunc);
                _cacheBinder[_index] = result;
            }
            return result;
        }

        //绑定带参数的command
        public BindCommandWithPara<TComponent, TData> Bind<TComponent, TData>
        (TComponent component, Action<TData> command, UnityEvent<TData> componentEvent = null,
            Func<Action<TData>, Action<TData>> wrapFunc = null) where TComponent : class
        {
            _index++;
            if (!TryGetBinder<BindCommandWithPara<TComponent, TData>>( out var result,
                (bind) => { bind.UpdateValue(component, command, componentEvent, wrapFunc); }))
            {
                result = new BindCommandWithPara<TComponent, TData>(component, command, componentEvent, wrapFunc);
                _cacheBinder[_index] = result;
            }
            return result;
        }
        

        protected bool TryGetBinder<T>(out T result, Action<T> updateFunc) where T : class
        {
            result = null;
            if (!_cacheBinder.TryGetValue(_index, out var bind))
            {
                return false;
            }
            result = bind as T;
            updateFunc(result);
            return true;
        }
        
        public void UpdateVm()
        {
            foreach (var canClearListener in _canClearListeners)
            {
                canClearListener.ClearListener();
            }
            _index = 0;
            _vm.Reset();
        }
    }
    
    public enum BindType
    {
        OnWay,
        Revert,
    }
}