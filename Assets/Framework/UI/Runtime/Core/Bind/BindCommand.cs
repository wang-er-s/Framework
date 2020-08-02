using System;
using Framework.UI.Wrap.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI.Core.Bind
{
    public class BindCommand<TComponent>
    {
        private TComponent component;
        private Action command;
        private UnityEvent componentEvent;
        private object defaultWrapper;
        private Func<Action, Action> wrapFunc;

        public BindCommand(TComponent component, Action command, UnityEvent componentEvent = null,
            Func<Action, Action> wrapFunc = null)
        {
            UpdateValue(component, command, componentEvent, wrapFunc);
            InitEvent();
        }

        public void UpdateValue(TComponent component, Action command, UnityEvent componentEvent,
            Func<Action, Action> wrapFunc)
        {
            this.component = component;
            this.command = command;
            this.componentEvent = componentEvent;
            this.wrapFunc = wrapFunc;
        }

        private void InitEvent()
        {
            if (componentEvent == null)
            {
                componentEvent = (component as IComponentEvent)?.GetComponentEvent();
            }
            if (componentEvent == null)
            {
                defaultWrapper = BindTool.GetDefaultWrapper(component);
                componentEvent = (defaultWrapper as IComponentEvent)?.GetComponentEvent();
            }
            Log.Assert(componentEvent != null);
            if(componentEvent == null) return;
            if (wrapFunc == null)
                componentEvent.AddListener(() => command());
            else
                componentEvent.AddListener(() => wrapFunc(command)());
        }
    }

    public class BindCommandWithPara<TComponent, TData>
    {
        private TComponent component;
        private Action<TData> command;
        private Func<Action<TData>, Action<TData>> wrapFunc;
        private UnityEvent<TData> componentEvent;
        private object defaultWrapper;

        public BindCommandWithPara(TComponent component, Action<TData> command, UnityEvent<TData> componentEvent = null,
            Func<Action<TData>, Action<TData>> wrapFunc = null)
        {
            UpdateValue(component, command, componentEvent, wrapFunc);
            InitEvent();
        }

        public void UpdateValue(TComponent component, Action<TData> command, UnityEvent<TData> componentEvent,
            Func<Action<TData>, Action<TData>> wrapFunc)
        {
            this.component = component;
            this.command = command;
            this.componentEvent = componentEvent;
            this.wrapFunc = wrapFunc;
        }

        private void InitEvent()
        {
            defaultWrapper = BindTool.GetDefaultWrapper(component);
            if (componentEvent == null)
            {
                componentEvent = (defaultWrapper as IComponentEvent<TData>)?.GetComponentEvent();
            }
            Log.Assert(componentEvent != null);
            if(componentEvent == null) return;
            if (wrapFunc == null)
                componentEvent.AddListener((value) => command(value));
            else
                componentEvent.AddListener((value) => wrapFunc(command)(value));
        }
    }
}