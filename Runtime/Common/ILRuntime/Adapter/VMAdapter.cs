using System;
using Framework.UI.Core;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Framework
{
    public class ViewModelAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mOnViewDestroy_0 = new CrossBindingMethodInfo("OnViewDestroy");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Framework.UI.Core.ViewModel);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : Framework.UI.Core.ViewModel, CrossBindingAdaptorType
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

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void OnViewDestroy()
            {
                if (mOnViewDestroy_0.CheckShouldInvokeBase(this.instance))
                    base.OnViewDestroy();
                else
                    mOnViewDestroy_0.Invoke(this.instance);
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
        }
    }
}