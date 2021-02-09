using System;
using Framework.UI.Core;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Framework
{
    public class ViewAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<UILevel> get_UILevel = new CrossBindingFunctionInfo<UILevel>("get_UILevel");
        static CrossBindingFunctionInfo<string> get_Path = new CrossBindingFunctionInfo<string>("get_Path");
        static CrossBindingMethodInfo show = new CrossBindingMethodInfo("OnShow");
        static CrossBindingMethodInfo hide = new CrossBindingMethodInfo("OnHide");
        static CrossBindingMethodInfo vmChange = new CrossBindingMethodInfo("OnVmChange");

        public override Type BaseCLRType
        {
            get { return typeof(View); }
        }

        public override Type AdaptorType
        {
            get { return typeof(Adapter); }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain,
            ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : View, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {
            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance
            {
                get { return instance; }
            }

            protected override void OnShow()
            {
                if (show.CheckShouldInvokeBase(this.instance))
                    base.OnShow();
                else
                    show.Invoke(this.instance);
            }

            protected override void OnHide()
            {
                if (hide.CheckShouldInvokeBase(this.instance))
                    base.OnHide();
                else
                    hide.Invoke(this.instance);
            }

            protected override void OnVmChange()
            {
                vmChange.Invoke(this.instance);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }

            public override UILevel UILevel
            {
                get { return get_UILevel.Invoke(this.instance); }
            }
        }
    }
}