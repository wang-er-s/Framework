using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.SF.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.SF.UI.Core
{

    public class BindFunc<TComponent> where TComponent : Component
    {

        private TComponent component;
        private Action vmFunc;
        private UnityEvent cmpFunc;
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

        public void Init()
        {
            bindCommand = WrapTool.GetBindCommand(component);
            if (cmpFunc == null)
                cmpFunc = bindCommand.GetBindCommandFunc(); 
            if (wrapFunc == null)
            {
                cmpFunc.AddListener(() => vmFunc());
            }
            else
            {
                cmpFunc.AddListener(() => wrapFunc(vmFunc)());
            }
        }

    }

    public class BindFuncWithPara<TComponent, TValue> where TComponent : Component
    {
        private TComponent component;
        private Action<TValue> vmFunc;
        private Func<Action<TValue>, Action<TValue>> wrapFunc;
        private UnityEvent<TValue> cmpFunc;
        private IBindCommand<TValue> bindCommand;

        //public BindFuncWithPara(TComponent component, Action<TValue> vmFunc)
        //{
        //    this.component = component;
        //    this.vmFunc = vmFunc;
        //}

        public BindFuncWithPara(TComponent component, Action<TValue> vmFunc)
        {
            this.component = component;
            //this.vmFunc = vmFunc;
        }

        public BindFuncWithPara<TComponent, TValue> For(UnityEvent<TValue> cmpFunc)
        {
            this.cmpFunc = cmpFunc;
            return this;
        }

        public BindFuncWithPara<TComponent, TValue> Wrap(Func<Action<TValue>, Action<TValue>> wrapFunc)
        {
            this.wrapFunc = wrapFunc;
            return this;
        }

        public void Init()
        {
            bindCommand = WrapTool.GetBindCommand<TValue>(component);
            if (cmpFunc == null)
                cmpFunc = bindCommand.GetBindCommandFunc();
            if (wrapFunc == null)
            {
                cmpFunc.AddListener((value) => vmFunc(value));
            }
            else
            {
                cmpFunc.AddListener((value) => wrapFunc(vmFunc)(value));
            }
        }
    }
}
