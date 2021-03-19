using System;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using Framework.UI.Core;
using ILRuntime.Runtime.Enviorment;
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
                result = (T)Activator.CreateInstance(type, args);
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

        public static Type GetType(object obj)
        {
            //如果是继承了主项目的热更的类型
            if (obj is CrossBindingAdaptorType adaptor)
            {
                return adaptor.ILInstance.Type.ReflectionType;
            }
            //如果是热更的类型
            if (obj is ILTypeInstance ilInstance)
            {
                return ilInstance.GetType().ReflectedType;
            }
            return obj.GetType();
        }
    }
}