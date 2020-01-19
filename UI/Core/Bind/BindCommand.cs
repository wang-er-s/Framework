using System;
using Framework.UI.Wrap;
using UnityEngine.Events;

namespace Framework.UI.Core
{

    public class BindCommand<TComponent> where TComponent : class
    {
        private TComponent component;
        private Action command;
        private UnityEvent componentEvent;
        private object defaultBind;
        private Func<Action, Action> wrapFunc;

        public BindCommand(TComponent _component, Action _command, UnityEvent _componentEvent = null,
            Func<Action, Action> _wrapFunc = null)
        {
            UpdateValue(_component,_command,_componentEvent,_wrapFunc);
            InitEvent();
        }

        public void UpdateValue(TComponent _component, Action _command, UnityEvent _componentEvent,
            Func<Action, Action> _wrapFunc)
        {
            component = _component;
            command = _command;
            componentEvent = _componentEvent;
            wrapFunc = _wrapFunc;
        }

        private void InitEvent()
        {
            componentEvent = componentEvent ?? (component as IComponentEvent)?.GetComponentEvent();
            if (componentEvent == null)
            {
                defaultBind = BindTool.GetDefaultBind(component);
                componentEvent = (defaultBind as IComponentEvent)?.GetComponentEvent();
            }
            if (wrapFunc == null)
            {
                componentEvent?.AddListener(() => command());
            }
            else
            {
                componentEvent?.AddListener(() => wrapFunc(command)());
            }
        }
    }

    public class BindCommandWithPara<TComponent, TData> where TComponent : class
    {
        private TComponent component;
        private Action<TData> command;
        private Func<Action<TData>, Action<TData>> wrapFunc;
        private UnityEvent<TData> componentEvent;
        private object defaultBind;

        public BindCommandWithPara(TComponent _component, Action<TData> _command, UnityEvent<TData> _componentEvent = null,
            Func<Action<TData>, Action<TData>> _wrapFunc = null)
        {
            UpdateValue(_component,_command,_componentEvent,_wrapFunc);
            InitEvent();
        }

        public void UpdateValue(TComponent _component, Action<TData> _command, UnityEvent<TData> _componentEvent,
            Func<Action<TData>, Action<TData>> _wrapFunc)
        {
            component = _component;
            command = _command;
            componentEvent = _componentEvent;
            wrapFunc = _wrapFunc;
        }

        private void InitEvent()
        {
            defaultBind = BindTool.GetDefaultBind(component);
            componentEvent = componentEvent ?? (defaultBind as IComponentEvent<TData>)?.GetComponentEvent();
            if (wrapFunc == null)
            {
                componentEvent?.AddListener((value) => command(value));
            }
            else
            {
                componentEvent?.AddListener((value) => wrapFunc(command)(value));
            }
        }
    }
}
