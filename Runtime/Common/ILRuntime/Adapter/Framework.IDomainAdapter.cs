using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Framework
{   
    public class IDomainAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Int32> mget_Name_0 = new CrossBindingFunctionInfo<System.Int32>("get_Name");
        static CrossBindingMethodInfo<System.Int32> mset_Name_0 = new CrossBindingMethodInfo<System.Int32>("set_Name");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsLoad_1 = new CrossBindingFunctionInfo<System.Boolean>("get_IsLoad");
        static CrossBindingMethodInfo<System.Boolean> mgset_IsLoad_1 = new CrossBindingMethodInfo<System.Boolean>("set_IsLoad");
        static CrossBindingMethodInfo<Object> mBeginEnter_2 = new CrossBindingMethodInfo<Object>("BeginEnter");
        static CrossBindingMethodInfo mBeginExit_3 = new CrossBindingMethodInfo("BeginExit");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Framework.IDomain);
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

        public class Adapter : Framework.IDomain, CrossBindingAdaptorType
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

            public void BeginEnter(object data)
            {
                mBeginEnter_2.Invoke(this.instance, data);
            }

            public void BeginExit()
            {
                mBeginExit_3.Invoke(this.instance);
            }

            public System.Int32 Name
            {
            get => mget_Name_0.Invoke(this.instance);
            set => mset_Name_0.Invoke(instance, value);
            }

            public System.Boolean IsLoad
            {
            get
            {
                return mget_IsLoad_1.Invoke(this.instance);

            }
            set => mgset_IsLoad_1.Invoke(instance, value);
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

