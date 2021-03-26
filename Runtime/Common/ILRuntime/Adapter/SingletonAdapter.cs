using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework
{
    public class SingletonAdapter : CrossBindingAdaptor
    {
        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public override Type BaseCLRType => typeof(Singleton<ILTypeInstance>);
        public override Type AdaptorType => typeof(Adapter);

        public class Adapter : Singleton<ILTypeInstance>, CrossBindingAdaptorType
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
            
        }
    }
}