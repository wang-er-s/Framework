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
            if (!_adaptors.Contains(t))
                _adaptors.Add(t);
        }

        private static List<Type> _adaptors = new List<Type>()
        {
            typeof(CoroutineAdapter),
            typeof(IDomainAdapter),
            typeof(IAsyncStateMachineAdaptor),
            typeof(ViewAdapter),
            typeof(ViewModelAdapter),
            typeof(ExceptionAdapter)
        };

        public static void RegisterCrossBindingAdaptor(AppDomain appdomain)
        {
            foreach (var adaptor in _adaptors)
            {
                appdomain.RegisterCrossBindingAdaptor((CrossBindingAdaptor) ReflectionHelper.CreateInstance(adaptor));
            }
        }
    }
}
