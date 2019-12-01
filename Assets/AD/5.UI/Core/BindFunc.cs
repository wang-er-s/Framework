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

    public class BindFunc<TComponent> where TComponent : Component
    {

        private TComponent component;
        private Action vmFunc;
        private UnityEvent cmpFunc;
        private BaseWrapper<TComponent> baseWrapper;
        private Func<Action, Action> wrapFunc;
        private IBindCommand bindCommand;

        public BindFunc(TComponent component, Action vmFunc)
        {
            this.component = component;
            this.vmFunc = vmFunc;
        }

        public BindFunc<TComponent> For(UnityEvent cmpFunc)
        {
            this.cmpFunc = cmpFunc;
            return this;
        }

        public BindFunc<TComponent> Wrap(Func<Action, Action> wrapFunc)
        {
            this.wrapFunc = wrapFunc;
            return this;
        }

        public void OneWay()
        {
            baseWrapper = WrapTool.GetWrapper(component);
            bindCommand = baseWrapper as IBindCommand;
            if (cmpFunc == null)
                cmpFunc = bindCommand?.GetBindCommandFunc(); 
            if (wrapFunc == null)
            {
                cmpFunc?.AddListener(() => vmFunc());
            }
            else
            {
                cmpFunc?.AddListener(() => wrapFunc(vmFunc)());
            }
        }

    }

    public class BindFuncWithPara<TComponent, TData> where TComponent : Component
    {
        private TComponent component;
        private Action<TData> vmFunc;
        private Func<Action<TData>, Action<TData>> wrapFunc;
        private UnityEvent<TData> cmpFunc;
        private BaseWrapper<TComponent> baseWrapper;
        private IBindCommand<TData> bindCommand;

        public BindFuncWithPara(TComponent component, Action<TData> vmFunc)
        {
            this.component = component;
            this.vmFunc = vmFunc;
        }

        public BindFuncWithPara<TComponent, TData> For(UnityEvent<TData> cmpFunc)
        {
            this.cmpFunc = cmpFunc;
            return this;
        }

        public BindFuncWithPara<TComponent, TData> Wrap(Func<Action<TData>, Action<TData>> wrapFunc)
        {
            this.wrapFunc = wrapFunc;
            return this;
        }

        public void OneWay()
        {
            baseWrapper = WrapTool.GetWrapper(component);
            bindCommand = baseWrapper as IBindCommand<TData>;
            if (cmpFunc == null)
                cmpFunc = bindCommand?.GetBindCommandFunc();
            if (wrapFunc == null)
            {
                cmpFunc?.AddListener((value) => vmFunc(value));
            }
            else
            {
                cmpFunc?.AddListener((value) => wrapFunc(vmFunc)(value));
            }
        }
    }
}
