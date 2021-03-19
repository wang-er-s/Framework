using Framework.Asynchronous;
using Framework.UI.Core;
using UnityEngine.Events;

namespace Framework
{
    public static class ILRuntimeGenericHelper
    {
        public static void RegisterGenericFunc()
        {
            ILRuntimeHelper.Appdomain.GetType(typeof(IAsyncResult<object>));
            ILRuntimeHelper.Appdomain.GetType(typeof(UnityEvent<object>));
            ILRuntimeHelper.Appdomain.GetType(typeof(ProgressResult<float, View>));
        }
    }
}