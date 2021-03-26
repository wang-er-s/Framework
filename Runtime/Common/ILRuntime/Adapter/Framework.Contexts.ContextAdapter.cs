using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Adapter
{   
    public class ContextAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.String, System.Boolean, System.Boolean> mContains_0 = new CrossBindingFunctionInfo<System.String, System.Boolean, System.Boolean>("Contains");
        static CrossBindingFunctionInfo<System.Boolean, System.Boolean> mContains_1 = new CrossBindingFunctionInfo<System.Boolean, System.Boolean>("Contains");
        static CrossBindingFunctionInfo<System.String, System.Boolean, System.Object> mGet_2 = new CrossBindingFunctionInfo<System.String, System.Boolean, System.Object>("Get");
        static CrossBindingMethodInfo<System.String, System.Object> mSet_5 = new CrossBindingMethodInfo<System.String, System.Object>("Set");
        static CrossBindingFunctionInfo<System.String, System.Object> mRemove_8 = new CrossBindingFunctionInfo<System.String, System.Object>("Remove");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Framework.Contexts.Context);
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

        public class Adapter : Framework.Contexts.Context, CrossBindingAdaptorType
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

            public override System.Boolean Contains(System.String name, System.Boolean cascade)
            {
                if (mContains_0.CheckShouldInvokeBase(this.instance))
                    return base.Contains(name, cascade);
                else
                    return mContains_0.Invoke(this.instance, name, cascade);
            }

            public override System.Object Get(System.String name, System.Boolean cascade)
            {
                if (mGet_2.CheckShouldInvokeBase(this.instance))
                    return base.Get(name, cascade);
                else
                    return mGet_2.Invoke(this.instance, name, cascade);
            }

            public override void Set(System.String name, System.Object value)
            {
                if (mSet_5.CheckShouldInvokeBase(this.instance))
                    base.Set(name, value);
                else
                    mSet_5.Invoke(this.instance, name, value);
            }

            public override System.Object Remove(System.String name)
            {
                if (mRemove_8.CheckShouldInvokeBase(this.instance))
                    return base.Remove(name);
                else
                    return mRemove_8.Invoke(this.instance, name);
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

