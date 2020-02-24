using System;
using Framework.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI.Core
{

    public class BindCommand<TComponent> where TComponent : class
    {
        private TComponent _component;
        private Action _command;
        private UnityEvent _componentEvent;
        private object _defaultBind;
        private Func<Action, Action> _wrapFunc;

        public BindCommand(TComponent component, Action command, UnityEvent componentEvent = null,
            Func<Action, Action> wrapFunc = null)
        {
            UpdateValue(component,command,componentEvent,wrapFunc);
            InitEvent();
        }

        public void UpdateValue(TComponent component, Action command, UnityEvent componentEvent,
            Func<Action, Action> wrapFunc)
        {
            _component = component;
            _command = command;
            _componentEvent = componentEvent;
            _wrapFunc = wrapFunc;
        }

        private void InitEvent()
        {
            if (_componentEvent == null)
                _componentEvent = (_component as IComponentEvent)?.GetComponentEvent();
            if (_componentEvent == null)
            {
                _defaultBind = BindTool.GetDefaultBind(_component);
                _componentEvent = (_defaultBind as IComponentEvent)?.GetComponentEvent();
            }
            if (_wrapFunc == null)
            {
                _componentEvent?.AddListener(() => _command());
            }
            else
            {
                _componentEvent?.AddListener(() => _wrapFunc(_command)());
            }
        }
    }

    public class BindCommandWithPara<TComponent, TData> where TComponent : class
    {
        private TComponent _component;
        private Action<TData> _command;
        private Func<Action<TData>, Action<TData>> _wrapFunc;
        private UnityEvent<TData> _componentEvent;
        private object _defaultBind;

        public BindCommandWithPara(TComponent component, Action<TData> command, UnityEvent<TData> componentEvent = null,
            Func<Action<TData>, Action<TData>> wrapFunc = null)
        {
            UpdateValue(component,command,componentEvent,wrapFunc);
            InitEvent();
        }

        public void UpdateValue(TComponent component, Action<TData> command, UnityEvent<TData> componentEvent,
            Func<Action<TData>, Action<TData>> wrapFunc)
        {
            _component = component;
            _command = command;
            _componentEvent = componentEvent;
            _wrapFunc = wrapFunc;
        }

        private void InitEvent()
        {
            _defaultBind = BindTool.GetDefaultBind(_component);
            if (_componentEvent == null)
                _componentEvent = (_defaultBind as IComponentEvent<TData>)?.GetComponentEvent();
            Debugger.Assert(_componentEvent != null);
            if (_wrapFunc == null)
            {
                _componentEvent.AddListener((value) => _command(value));
            }
            else
            {
                _componentEvent.AddListener((value) => _wrapFunc(_command)(value));
            }
        }
    }
}
