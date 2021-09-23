using System;
using Framework.UI.Wrap.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
{
    public class BindCommand<TComponent> : BaseBind
    {
        private TComponent _component;
        private Action _command;
        private UnityEvent _componentEvent;
        private object _defaultWrapper;
        private Func<Action, Action> _wrapFunc;

        public BindCommand(object container) : base(container)
        {
        }

        public void Reset(TComponent component, Action command, UnityEvent componentEvent,
            Func<Action, Action> wrapFunc)
        {
            SetValue(component, command, componentEvent, wrapFunc);
            InitEvent();
        }

        private void SetValue(TComponent component, Action command, UnityEvent componentEvent,
            Func<Action, Action> wrapFunc)
        {
            this._component = component;
            this._command = ()=> command?.Invoke();
            this._componentEvent = componentEvent;
            this._wrapFunc = wrapFunc;
        }

        private void InitEvent()
        {
            _defaultWrapper = BindTool.GetDefaultWrapper(Container, _component);
            _componentEvent = _componentEvent ?? (_component as IComponentEvent)?.GetComponentEvent() ??
                (_defaultWrapper as IComponentEvent)?.GetComponentEvent();
            Debug.Assert(_componentEvent != null, "componentEvent can not be null");
            _componentEvent.AddListener(Listener);
        }
        
        private void Listener()
        {
            if (_wrapFunc != null)
                _wrapFunc(_command)();
            else
                _command();
        }

        public override void ClearView()
        {
            _componentEvent.RemoveListener(Listener);
        }

        public override void ClearModel()
        {
            
        }
    }

    public class BindCommandWithPara<TComponent, TData> : BaseBind
    {
        private TComponent _component;
        private Action<TData> _command;
        private Func<Action<TData>, Action<TData>> _wrapFunc;
        private UnityEvent<TData> _componentEvent;
        private object _defaultWrapper;

        public BindCommandWithPara(object container) : base(container)
        {
            
        }

        public void Reset(TComponent component, Action<TData> command, UnityEvent<TData> componentEvent,
            Func<Action<TData>, Action<TData>> wrapFunc)
        {
            SetValue(component, command, componentEvent, wrapFunc);
            InitEvent();
        }
        
        private void SetValue(TComponent component, Action<TData> command, UnityEvent<TData> componentEvent,
            Func<Action<TData>, Action<TData>> wrapFunc)
        {
            this._component = component;
            this._command = data=> command?.Invoke(data);
            this._componentEvent = componentEvent;
            this._wrapFunc = wrapFunc;
        }

        private void InitEvent()
        {
            _defaultWrapper = BindTool.GetDefaultWrapper(Container, _component);
            if (_componentEvent == null)
            {
                IComponentEvent<TData> changeCb = _defaultWrapper as IComponentEvent<TData>;
                if (_component is IComponentEvent<TData> cb)
                {
                    changeCb = cb;
                }
                _componentEvent = changeCb?.GetComponentEvent();
            }
            Debug.Assert(_componentEvent != null,
                $" can not found wrapper , check if the folder(Runtime/UI/Wrap) has {typeof(TComponent).Name} wrapper or {typeof(TComponent).Name} implements IComponentEvent<{typeof(TData).Name}> interface");
            _componentEvent.AddListener(Listener);
        }

        private void Listener(TData data)
        {
            if (_wrapFunc != null)
                _wrapFunc(_command)(data);
            else
                _command(data);
        }

        public override void ClearView()
        {
            _componentEvent.RemoveListener(Listener);
        }

        public override void ClearModel()
        {

        }
    }
}