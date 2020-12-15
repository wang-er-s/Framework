using ILRuntime.Runtime.Enviorment;

namespace Framework
{
    public class ILRuntimeAdapterHelper
    {
        public static void RegisterCrossBindingAdaptor(AppDomain appdomain)
        {
            appdomain.RegisterCrossBindingAdaptor(new VMAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ViewAdapter());
            appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());
        }
    }
}