using System;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using Framework.UI.Core;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Tool
{
    public static class ReflectionHelper
    {
        private const string IlRuntimeDllName = "ILRuntime";
        public static T CreateInstance<T>(params object[] args)
        {
            var type = typeof(T).GetCLRType();
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

        public static Type GetCLRType(this object obj)
        {
            //如果是继承了主项目的热更的类型
            if (obj is CrossBindingAdaptorType adaptor)
            {
                return adaptor.ILInstance.Type.ReflectionType;
            }
            //如果是热更的类型
            if (obj is ILTypeInstance ilInstance)
            {
                return ilInstance.Type.ReflectionType;
            }
            return obj.GetType();
        }

        public static Type GetCLRType(this Type type)
        {
            if (type is ILRuntimeType runtimeType)
                return runtimeType.ILType.ReflectionType;
            if (type is ILRuntimeWrapperType wrapperType)
                return wrapperType.RealType;
            return type;
        }
    }
}