using System;
using System.Collections.Generic;
using System.Linq;
using Framework.UI.Core;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using Object = UnityEngine.Object;

namespace Framework
{
    public class ILRuntimeRedirectHelper
    {
        public static unsafe void RegisterMethodRedirection(AppDomain appdomain)
        {
            appdomain.RegisterCLRMethodRedirection(typeof(Debug).GetMethod("Log", new[] {typeof(object)}), LogOnePara);
            appdomain.RegisterCLRMethodRedirection(
                typeof(Debug).GetMethod("Log", new[] {typeof(object), typeof(Object)}), LogTwoPara);
            //TODO 泛型重定向
            appdomain.RegisterCLRMethodRedirection(typeof(UIManager).GetMethod("OpenAsync", new []{typeof(ViewModel)}), UILoad);
        }

        private static unsafe StackObject* LogTwoPara(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            AppDomain __domain = __intp.AppDomain;
            //只有一个参数，所以返回指针就是当前栈指针ESP - 1
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            //第一个参数为ESP -1， 第二个参数为ESP - 2，以此类推
            var ptr_obj = ILIntepreter.Minus(__esp, 1);
            var ptr_msg = ILIntepreter.Minus(__esp, 2);
            //获取参数message的值
            object message = StackObject.ToObject(ptr_msg, __domain, __mStack);
            object obj = StackObject.ToObject(ptr_obj, __domain, __mStack);
            //需要清理堆栈
            __intp.Free(ptr_obj);
            __intp.Free(ptr_msg);
            //如果参数类型是基础类型，例如int，可以直接通过int param = ptr_of_this_method->Value获取值，
            //关于具体原理和其他基础类型如何获取，请参考ILRuntime实现原理的文档。
            //通过ILRuntime的Debug接口获取调用热更DLL的堆栈
            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            Debug.Log($"{message}\n{stackTrace}", obj as Object);
            return __ret;
        }

        private static unsafe StackObject* LogOnePara(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            var ptr_msg = ILIntepreter.Minus(__esp, 1);
            object message = StackObject.ToObject(ptr_msg, __domain, __mStack);
            __intp.Free(ptr_msg);
            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            Debug.Log($"{message}\n{stackTrace}");
            return __ret;
        }

        unsafe static StackObject* UILoad(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            var ins = StackObject.ToObject(ptr, __domain, __mStack);
            if (ins == null)
                throw new NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {

                if (ins is GameObject)
                {
                    Debug.Log(11);
                }
                else if (ins is Component)
                {
                    Debug.Log(22);
                }
                else if (ins is ILTypeInstance)
                {
                    Debug.Log(33);
                }
                else
                {
                    Debug.Log(44);
                }


                var type = genericArgument[0];
                object res = null;
                if (type is CLRType)
                {
                    Debug.Log(55);
                }
                else
                {
                    Debug.Log(55);
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }

    }
}