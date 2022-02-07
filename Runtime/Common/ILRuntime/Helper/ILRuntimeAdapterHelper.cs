#if ILRUNTIME
using System;
using System.Collections.Generic;
using ILRuntime.Runtime.Enviorment;
using Tool;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework
{
    public static class ILRuntimeAdapterHelper
    {
        public static bool AddAdaptor(Type t)
        {
            if(!t.IsSubclassOf(typeof(CrossBindingAdaptor))) return false;
            if (!_adaptors.Contains(t))
                _adaptors.Add(t);
            return true;
        }

        private static List<Type> _adaptors = new List<Type>();

        public static void RegisterCrossBindingAdaptor(AppDomain appdomain)
        {
            foreach (var adaptor in _adaptors)
            {
                appdomain.RegisterCrossBindingAdaptor((CrossBindingAdaptor) ReflectionHelper.CreateInstance(adaptor));
            }
        }
    }
}
#endif