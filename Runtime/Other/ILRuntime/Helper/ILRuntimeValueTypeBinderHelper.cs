#if ILRUNTIME
using ILRuntime.Runtime.Enviorment;
using UnityEngine;

namespace Framework
{
    public class ILRuntimeValueTypeBinderHelper
    {
        public static void Register(AppDomain appdomain)
        {
            appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
        }
    }
}
#endif