using Framework.Asynchronous;
using UnityEngine.Events;

namespace Framework
{
    public static class ILRuntimeGenericHelper
    {
        public static void RegisterGenericFunc()
        {
            ILRuntimeHelper.Appdomain.GetType(typeof(IAsyncResult<object>));
            ILRuntimeHelper.Appdomain.GetType(typeof(UnityEvent<object>));
            ILRuntimeHelper.Appdomain.GetType(typeof(ProgressResult<object, object>));
        }
    }
}