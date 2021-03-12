using System;
using System.Collections.Generic;
using ILRuntime.Runtime.Enviorment;
using Tool;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework
{
    public static class ILRuntimeAdapterHelper
    {
        public static void AddAdaptor(Type t)
        {
            if(!t.IsSubclassOf(typeof(CrossBindingAdaptor))) return;
            _adaptors.Add((CrossBindingAdaptor) ReflectionHelper.CreateInstance(t));
        }

        private static List<CrossBindingAdaptor> _adaptors = new List<CrossBindingAdaptor>();

        public static void RegisterCrossBindingAdaptor(AppDomain appdomain)
        {
            foreach (var adaptor in _adaptors)
            {
                appdomain.RegisterCrossBindingAdaptor(adaptor);
            }
        }
    }
}
