using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using UnityEngine;

namespace Framework
{
    public class ILRuntimeRedirectHelper
    {
        public static unsafe void RegisterMethodRedirection(AppDomain appdomain)
        {
            appdomain.RegisterCLRMethodRedirection(typeof(Debug).GetMethod("Log", new[] {typeof(object)}), LogOnePara);
            appdomain.RegisterCLRMethodRedirection(
                typeof(Debug).GetMethod("Log", new[] {typeof(object), typeof(Object)}), LogTwoPara);
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
    }
}