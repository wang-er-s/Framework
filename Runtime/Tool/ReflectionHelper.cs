using System;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using Framework.UI.Core;
using ILRuntime.Runtime.Intepreter;

namespace Tool
{
    public class ReflectionHelper
    {
        private const string IlRuntimeDllName = "ILRuntime";
        public static T CreateInstance<T>(Type type, params object[] args)
        {
            T result;
            if (type.Assembly.GetName().Name == IlRuntimeDllName)
            {
                result = ILRuntimeHelper.Appdomain.Instantiate<T>(type.FullName, args);
            }
            else
            {
                result = (T)Activator.CreateInstance(typeof(T), args);
            }
            return result;
        }
        
        public static object CreateInstance(Type type, params object[] args)
        {
            object result;
            if (type.Assembly.GetName().Name == IlRuntimeDllName)
            {
                ILTypeInstance ins = ILRuntimeHelper.Appdomain.Instantiate(type.FullName, args);
                return ins.CLRInstance;
            }
            else
            {
                result = Activator.CreateInstance(type, args);
            }
            return result;
        }
    }
}