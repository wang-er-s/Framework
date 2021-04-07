using Framework.Asynchronous;
using ILRuntime.Runtime.Enviorment;
using UnityEngine.Events;

namespace Framework
{
    public static class ILRuntimeGenericHelper
    {
        public static void RegisterGenericFunc(AppDomain appDomain)
        {
            appDomain.GetType(typeof(IAsyncResult<object>));
            appDomain.GetType(typeof(UnityEvent<object>));
            appDomain.GetType(typeof(ProgressResult<object, object>));
        }
    }
}