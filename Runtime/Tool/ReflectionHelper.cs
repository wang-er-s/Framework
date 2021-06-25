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
        private static readonly bool useHotfix;
        /// <summary>
        /// 不使用ilruntime时，game@hotfix.asmdef的程序集
        /// </summary>
        private static Assembly curHotfixAssembly;
        private static Assembly curGameAssembly;
        private const string IlRuntimeDllName = "ILRuntime";

        static ReflectionHelper()
        {
            var runtimeConfig = ConfigBase.Load<FrameworkRuntimeConfig>();
            useHotfix = runtimeConfig.ILRConfig.UseHotFix;
            curHotfixAssembly = AssemblyManager.GetAssembly(runtimeConfig.ILRConfig.DllName);
            curGameAssembly = AssemblyManager.GetAssembly(runtimeConfig.GameDllName);
        }
        
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

        public static object CreateInstance(string type, params object[] args)
        {
            return CreateInstance(GetType(type), args);
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

        public static Type GetCLRType(this IType type)
        {
            if (type is CLRType clrType)
            {
                return clrType.TypeForCLR;
            }
            return type.ReflectionType;
        }

        public static Type GetType(string type)
        {
            var result = useHotfix ? ILRuntimeHelper.GetType(type) : curHotfixAssembly.GetType(type);
            if (result == null)
            {
                result = curGameAssembly.GetType(type);
            }
            return result;
        }
    }
}