#if ILRUNTIME
using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Adapter
{   
    public class IEquatable_1_ILTypeInstanceAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<Adapter, System.Boolean> mEquals_0 = new CrossBindingFunctionInfo<Adapter, System.Boolean>("Equals");
        static readonly CrossBindingFunctionInfo<Int32> mGetHashCode = new CrossBindingFunctionInfo< System.Int32>("GetHashCode");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.IEquatable<ILRuntime.Runtime.Intepreter.ILTypeInstance>);
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

        public class Adapter : System.IEquatable<Adapter>, CrossBindingAdaptorType
        {
            readonly ILTypeInstance instance;
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

            public System.Boolean Equals(Adapter other)
            {
                return mEquals_0.Invoke(instance, other);
            }
            
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Adapter) obj);
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

            public override int GetHashCode()
            {
                return mGetHashCode.Invoke(instance);
            }
        }
    }
}

#endif