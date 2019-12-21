using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;

namespace AD.UI.Core
{

    public class BindCommand<TComponent> where TComponent : Component
    {
        private TComponent component;
        private Action vmFunc;
        private UnityEvent componentFunc;
        private BaseWrapper<TComponent> wrapper;
        private Func<Action, Action> wrapFunc;

        public BindCommand(TComponent _component, Action _vmFunc, UnityEvent _componentFunc = null,
            Func<Action, Action> _wrapFunc = null)
        {
            component = _component;
            vmFunc = _vmFunc;
            componentFunc = _componentFunc;
            wrapFunc = _wrapFunc;
            InitEvent();
        }

        private void InitEvent()
        {
            wrapper = WrapTool.GetWrapper(component);
            componentFunc = componentFunc ?? (wrapper as IBindCommand)?.GetBindCommandFunc();
            if (wrapFunc == null)
            {
                componentFunc?.AddListener(() => vmFunc());
            }
            else
            {
                componentFunc?.AddListener(() => wrapFunc(vmFunc)());
            }
        }
    }

    public class BindCommandWithPara<TComponent, TData> where TComponent : Component
    {
        private TComponent component;
        private Action<TData> vmFunc;
        private Func<Action<TData>, Action<TData>> wrapFunc;
        private UnityEvent<TData> componentFunc;
        private BaseWrapper<TComponent> wrapper;

        public BindCommandWithPara(TComponent _component, Action<TData> _vmFunc, UnityEvent<TData> _componentFunc = null,
            Func<Action<TData>, Action<TData>> _wrapFunc = null)
        {
            component = _component;
            vmFunc = _vmFunc;
            componentFunc = _componentFunc;
            wrapFunc = _wrapFunc;
        }

        public void InitBind()
        {
            wrapper = WrapTool.GetWrapper(component);
            componentFunc = componentFunc ?? (wrapper as IBindCommand<TData>)?.GetBindCommandFunc();
            if (wrapFunc == null)
            {
                componentFunc?.AddListener((value) => vmFunc(value));
            }
            else
            {
                componentFunc?.AddListener((value) => wrapFunc(vmFunc)(value));
            }
        }
    }
}
