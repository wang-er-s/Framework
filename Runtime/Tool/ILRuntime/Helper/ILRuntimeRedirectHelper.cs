using System;
using System.Collections.Generic;
using System.Reflection;
using Framework.UI.Core;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using Object = UnityEngine.Object;

namespace Framework
{
    public static class ILRuntimeRedirectHelper
    {
        public static unsafe void RegisterMethodRedirection(AppDomain appdomain)
        {
            Type log = typeof(Log);
            appdomain.RegisterCLRMethodRedirection(log.GetMethod("Msg"),
                (intp, esp, stack, method, obj) =>
                    LogMsg(intp, esp, stack, method, obj, Log.Msg));
            appdomain.RegisterCLRMethodRedirection(log.GetMethod("Error"),
                (intp, esp, stack, method, obj) =>
                    LogMsg(intp, esp, stack, method, obj, Log.Msg));
            appdomain.RegisterCLRMethodRedirection(log.GetMethod("Warning"),
                (intp, esp, stack, method, obj) =>
                    LogMsg(intp, esp, stack, method, obj, Log.Msg));
            appdomain.RegisterCLRMethodRedirection(log.GetMethod("Assert"), LogAssert);
            appdomain.RegisterCLRMethodRedirection(typeof(Log).GetMethod("MsgWithGo"), LogMsgWithGo);
            
            
            foreach (MethodInfo methodInfo in typeof(UIManager).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (methodInfo.Name == "OpenAsync")
                {
                    appdomain.RegisterCLRMethodRedirection(methodInfo, UIOpenAsync);
                }else if (methodInfo.Name == "Close" && methodInfo.IsGenericMethod)
                {
                    appdomain.RegisterCLRMethodRedirection(methodInfo, UIClose);
                }else if (methodInfo.Name == "Get" && methodInfo.IsGenericMethod)
                {
                    appdomain.RegisterCLRMethodRedirection(methodInfo, UIGet);
                }
            }
        }

        private static unsafe StackObject* LogMsg(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj, Action<object[]> action)
        {
            AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            var ptr_msg = ILIntepreter.Minus(__esp, 1);
            object[] message = (object[])StackObject.ToObject(ptr_msg, __domain, __mStack);
            __intp.Free(ptr_msg);
            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            Array.Resize(ref message, message.Length + 1);
            message[message.Length - 1] = "\n" + stackTrace;
            action(message);
            return __ret;
        }
        
        private static unsafe StackObject* LogAssert(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            var ptr_msg1 = ILIntepreter.Minus(__esp, 1);
            bool condition = (bool)StackObject.ToObject(ptr_msg1, __domain, __mStack);
            var ptr_msg2 = ILIntepreter.Minus(__esp, 2);
            string message = (string)StackObject.ToObject(ptr_msg2, __domain, __mStack);
            var ptr_msg3 = ILIntepreter.Minus(__esp, 3);
            Object context = (Object)StackObject.ToObject(ptr_msg3, __domain, __mStack);
            __intp.Free(ptr_msg1);
            __intp.Free(ptr_msg2);
            __intp.Free(ptr_msg3);
            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            Log.Assert(condition, message + "/n" + stackTrace, context);
            return __ret;
        }

        private static unsafe StackObject* LogMsgWithGo(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            var ptr_msg = ILIntepreter.Minus(__esp, 1);
            var ptr_go = ILIntepreter.Minus(__esp, 2);
            object message = StackObject.ToObject(ptr_msg, __domain, __mStack);
            Object go = (Object)StackObject.ToObject(ptr_go, __domain, __mStack);
            __intp.Free(ptr_msg);
            __intp.Free(ptr_go);
            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            Log.MsgWithGo(message + "/n" + stackTrace, go);
            return __ret;
        }
        
        static unsafe StackObject* UIOpenAsync(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = __method.GenericArguments;
            //只有一个参数，所以返回指针就是当前栈指针ESP - 1
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            //第一个参数为ESP -1， 第二个参数为ESP - 2，以此类推
            StackObject* ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            object vm = StackObject.ToObject(ptr_of_this_method, __domain, __mStack);
            __intp.Free(ptr_of_this_method);
            //load只能有一个泛型
            if (genericArguments != null && genericArguments.Length == 1)
            {
                var t = genericArguments[0];
                if (t is ILType)//如果T是热更DLL里的类型
                {
                    var result = UIManager.Ins.OpenAsync(t.ReflectionType, (ViewModel) vm);
                    return ILIntepreter.PushObject(__ret, __mStack, result);
                }
                else
                {
                    var result = UIManager.Ins.OpenAsync(t.TypeForCLR, (ViewModel) vm);
                    return ILIntepreter.PushObject(__ret, __mStack, result);
                }
            }

            return __ret;
        }

        static unsafe StackObject* UIGet(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = __method.GenericArguments;
            //只有一个参数，所以返回指针就是当前栈指针ESP - 1
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            //close只能有一个泛型
            if (genericArguments != null && genericArguments.Length == 1)
            {
                var t = genericArguments[0];
                var result = UIManager.Ins.Get(t is ILType ? t.ReflectionType : t.TypeForCLR);
                return ILIntepreter.PushObject(__ret, __mStack, result);
            }

            return __ret;
        }
        
        static unsafe StackObject* UIClose(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            AppDomain __domain = __intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = __method.GenericArguments;
            //只有一个参数，所以返回指针就是当前栈指针ESP - 1
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            //close只能有一个泛型
            if (genericArguments != null && genericArguments.Length == 1)
            {
                var t = genericArguments[0];
                UIManager.Ins.Close(t is ILType ? t.ReflectionType : t.TypeForCLR);
            }

            return __ret;
        }
    }
}