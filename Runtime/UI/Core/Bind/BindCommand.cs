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

        public BindCommand(object container, TComponent component, Action command, UnityEvent componentEvent = null,
            Func<Action, Action> wrapFunc = null) : base(container)
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
            Log.Assert(_componentEvent != null, "componentEvent can not be null");
            if (_wrapFunc == null)
                _componentEvent.AddListener(() => _command());
            else
                _componentEvent.AddListener(() => _wrapFunc(_command)());
        }

        public override void ClearBind()
        {
            _componentEvent.RemoveAllListeners();
        }
    }

    public class BindCommandWithPara<TComponent, TData> : BaseBind
    {
        private TComponent _component;
        private Action<TData> _command;
        private Func<Action<TData>, Action<TData>> _wrapFunc;
        private UnityEvent<TData> _componentEvent;
        private object _defaultWrapper;

        public BindCommandWithPara(object container, TComponent component, Action<TData> command, UnityEvent<TData> componentEvent = null,
            Func<Action<TData>, Action<TData>> wrapFunc = null) : base(container)
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
            Log.Assert(_componentEvent != null,
                $" can not found wrapper , check if the folder(Runtime/UI/Wrap) has {typeof(TComponent).Name} wrapper or {typeof(TComponent).Name} implements IComponentEvent<{typeof(TData).Name}> interface");
            if (_wrapFunc == null)
                _componentEvent.AddListener((value) => _command(value));
            else
                _componentEvent.AddListener((value) => _wrapFunc(_command)(value));
        }

        public override void ClearBind()
        {
            _componentEvent.RemoveAllListeners();
        }
    }
}