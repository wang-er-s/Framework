using Framework.Asynchronous;
using Framework.UI.Core;

namespace Framework
{
    public static class ILRuntimeGenericHelper
    {
        public static void RegisterGenericFunc() => UnityEvent();

        private static void UnityEvent()
        {
            ABool((UnityEngine.Events.UnityEvent<bool>) null);
            AString((UnityEngine.Events.UnityEvent<string>) null);
            ALogin(null);

            void ABool(UnityEngine.Events.UnityEvent<bool> b)
            {
            }

            void AString(UnityEngine.Events.UnityEvent<string> b)
            {
            }

            void ALogin(ProgressResult<float, View> a)
            {
            }
        }
    }
}