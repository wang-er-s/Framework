using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Contexts;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Google.Protobuf;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ProtoBuf.Meta;
using Tool;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// 有参无返回值实例泛型方法 指针从1-n表示第n个参数，最后一个表示this指针 , 返回值使用this指针
    /// 无参则第一个为this指针
    ///
    /// 泛型Type  type is CLRType 是主项目 type.TypeForCLR
    ///             else type.ReflectionType 
    ///
    /// object is ILTypeInstance 是热更实例
    ///
    /// 有返回值时，push的指针如果是ins则使用ins，否则使用esp
    /// </summary>
    public static class ILRuntimeRedirectHelper
    {
        public static unsafe void RegisterMethodRedirection(AppDomain appdomain)
        {
            Type[] args;
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
                                BindingFlags.DeclaredOnly;

            Type log = typeof(Log);
            foreach (var methodInfo in log.GetMethods(flag))
            {
                switch (methodInfo.Name)
                {
                    case "Msg":
                        appdomain.RegisterCLRMethodRedirection(methodInfo, (intp, esp, stack, method, obj) =>
                            LogMsg(intp, esp, stack, method, obj, Log.Msg));
                        break;
                    case "Error":
                        appdomain.RegisterCLRMethodRedirection(methodInfo, (intp, esp, stack, method, obj) =>
                            LogMsg(intp, esp, stack, method, obj, Log.Error));
                        break;
                    case "Warning":
                        appdomain.RegisterCLRMethodRedirection(methodInfo, (intp, esp, stack, method, obj) =>
                            LogMsg(intp, esp, stack, method, obj, Log.Warning));
                        break;
                    case "Assert":
                        appdomain.RegisterCLRMethodRedirection(methodInfo, LogAssert);
                        break;
                    case "MsgWithGo":
                        appdomain.RegisterCLRMethodRedirection(methodInfo, LogMsgWithGo);
                        break;
                }
            }

            foreach (MethodInfo methodInfo in typeof(UIManager).GetMethods(flag))
            {
                switch (methodInfo.Name)
                {
                    case "OpenAsync" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, UIOpenAsync);
                        break;
                    case "Close" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, UIClose);
                        break;
                    case "Get" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, UIGet);
                        break;
                    case "Get" when !methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, UIGet2);
                        break;
                }
            }

            Type uiBindFactory = typeof(UIBindFactory);

            foreach (var methodInfo in uiBindFactory.GetMethods(flag))
            {
                switch (methodInfo.Name)
                {
                    case "BindViewList" when methodInfo.GetGenericArguments().Length == 2:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, BindViewList);
                        break;
                    case "BindIpairs" when methodInfo.GetGenericArguments().Length == 2:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, BindIpairs);
                        break;
                }
            }

            var addSubView = typeof(View).GetMethods(flag)
                .First((info => info.Name == "AddSubView" && info.IsGenericMethod));
            appdomain.RegisterCLRMethodRedirection(addSubView, UIAddSubview);

            foreach (var methodInfo in typeof(Context).GetMethods(flag))
            {
                switch (methodInfo.Name)
                {
                    case "Contains" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, ContextContains);
                        break;
                    case "Get" when methodInfo.IsGenericMethod:
                    {
                        var paras = methodInfo.GetParameters();
                        if (paras.Length == 1)
                        {
                            appdomain.RegisterCLRMethodRedirection(methodInfo, ContextGet1);
                        }
                        else
                        {
                            appdomain.RegisterCLRMethodRedirection(methodInfo, ContextGet2);
                        }
                        break;
                    }
                    case "Set" when methodInfo.IsGenericMethod:
                    {
                        var paras = methodInfo.GetParameters();
                        if (paras.Length == 1)
                        {
                            appdomain.RegisterCLRMethodRedirection(methodInfo, ContextSet1);
                        }
                        break;
                    }
                    case "Remove" when methodInfo.IsGenericMethod:
                    {
                        var paras = methodInfo.GetParameters();
                        if (paras.Length == 1)
                        {
                            appdomain.RegisterCLRMethodRedirection(methodInfo, ContextRemove1);
                        }
                        else
                        {
                            appdomain.RegisterCLRMethodRedirection(methodInfo, ContextRemove0);
                        }
                        break;
                    }
                    case "GetContext" when methodInfo.IsGenericMethod:
                    {
                        var paras = methodInfo.GetParameters();
                        if (paras.Length == 0)
                        {
                            appdomain.RegisterCLRMethodRedirection(methodInfo, ContextGetContext);
                        }
                        break;
                    }
                    case "RemoveContext" when methodInfo.IsGenericMethod:
                    {
                        appdomain.RegisterCLRMethodRedirection(methodInfo, ContextRemoveContext);
                        break;
                    }
                }
            }

            //注册Add Component
            Type gameObjectType = typeof(GameObject);
            foreach (var methodInfo in gameObjectType.GetMethods(flag))
            {
                switch (methodInfo.Name)
                {
                    case "AddComponent" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, AddComponent);
                        break;
                    case "GetComponent" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, GetComponent);
                        break;
                    case "GetComponent" when !methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, GetComponent_1);
                        break;
                }
            }

            Type componentType = typeof(Component);
            foreach (var methodInfo in componentType.GetMethods(flag))
            {
                switch (methodInfo.Name)
                {
                    case "GetComponent" when methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, GetComponent);
                        break;
                    case "GetComponent" when !methodInfo.IsGenericMethod:
                        appdomain.RegisterCLRMethodRedirection(methodInfo, GetComponent_1);
                        break;
                }
            }

            args = new[] {typeof(GameObject)};
            var getOrAdd1 = typeof(GameObjectExtension).GetMethod("GetOrAddComponent", flag, null, args, null);
            appdomain.RegisterCLRMethodRedirection(getOrAdd1, GetOrAddComponent);

            //注册pb反序列化
            Type pbSerializeType = typeof(ProtoBuf.Serializer);
            args = new Type[] {typeof(System.Type), typeof(System.IO.Stream)};
            var pbDeserializeMethod = pbSerializeType.GetMethod("Deserialize", flag, null, args, null);
            appdomain.RegisterCLRMethodRedirection(pbDeserializeMethod, Deserialize_1);
            args = new Type[] {typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance)};
            Dictionary<string, List<MethodInfo>> genericMethods = new Dictionary<string, List<MethodInfo>>();
            List<MethodInfo> lst = null;
            if (genericMethods.TryGetValue("Deserialize", out lst))
            {
                foreach (var m in lst)
                {
                    if (m.MatchGenericParameters(args, typeof(ILRuntime.Runtime.Intepreter.ILTypeInstance),
                        typeof(System.IO.Stream)))
                    {
                        var method = m.MakeGenericMethod(args);
                        appdomain.RegisterCLRMethodRedirection(method, Deserialize_2);
                        break;
                    }
                }
            }
            
            Type JsonFormatter = typeof(JsonFormatter);
            appdomain.RegisterCLRMethodRedirection(JsonFormatter.GetMethod("ToDiagnosticString"),
                ToDiagnosticString);

            args = new[] {typeof(Enum)};
            appdomain.RegisterCLRMethodRedirection(typeof(DomainManager).GetMethod("BeginNavTo", args), BeginNavTo);
        }

        private static unsafe StackObject* BeginNavTo(ILIntepreter intp, StackObject* esp, IList<object> mstack, CLRMethod method, bool isnewobj)
        {
            AppDomain domain = intp.AppDomain;
            StackObject* ret = ILIntepreter.Minus(esp, 1);
            var ptr_msg1 = ILIntepreter.Minus(esp, 1);
            ILTypeInstance obj = (ILTypeInstance)StackObject.ToObject(ptr_msg1, domain, mstack);
            DomainManager.Ins.BeginNavTo(obj.Fields[0].Value);
            intp.Free(ptr_msg1);
            return ret;
        }

        private static unsafe StackObject* ToDiagnosticString(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod method, bool isnewobj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            StackObject* ptr2 = ILIntepreter.Minus(__esp, 1);
            object msg = StackObject.ToObject(ptr2, __domain, __mStack);
            JsonFormatter.ToDiagnosticString((IMessage) ((ILTypeInstance) msg).CLRInstance);
            __intp.Free(ptr2);
            return __ret;
        }
        
        #region Log

        private static unsafe StackObject* LogMsg(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj, Action<object[]> action)
        {
            AppDomain domain = intp.AppDomain;
            StackObject* ret = ILIntepreter.Minus(esp, 1);
            var ptr_msg = ILIntepreter.Minus(esp, 1);
            object[] message = (object[]) StackObject.ToObject(ptr_msg, domain, mStack);
            intp.Free(ptr_msg);
            string stackTrace = domain.DebugService.GetStackTrace(intp);
            Array.Resize(ref message, message.Length + 1);
            message[message.Length - 1] = "\n" + stackTrace;
            action(message);
            return ret;
        }

        private static unsafe StackObject* LogAssert(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            AppDomain domain = intp.AppDomain;
            StackObject* ret = ILIntepreter.Minus(esp, 1);
            var ptr_msg1 = ILIntepreter.Minus(esp, 1);
            bool condition = (int) StackObject.ToObject(ptr_msg1, domain, mStack) == 1;
            var ptr_msg2 = ILIntepreter.Minus(esp, 2);
            string message = (string) StackObject.ToObject(ptr_msg2, domain, mStack);
            var ptr_msg3 = ILIntepreter.Minus(esp, 3);
            Object context = (Object) StackObject.ToObject(ptr_msg3, domain, mStack);
            intp.Free(ptr_msg1);
            intp.Free(ptr_msg2);
            intp.Free(ptr_msg3);
            string stackTrace = domain.DebugService.GetStackTrace(intp);
            Log.Assert(condition, message + "/n" + stackTrace, context);
            return ret;
        }

        private static unsafe StackObject* LogMsgWithGo(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            AppDomain domain = intp.AppDomain;
            StackObject* ret = ILIntepreter.Minus(esp, 1);
            var ptr_msg = ILIntepreter.Minus(esp, 1);
            var ptr_go = ILIntepreter.Minus(esp, 2);
            object message = StackObject.ToObject(ptr_msg, domain, mStack);
            Object go = (Object) StackObject.ToObject(ptr_go, domain, mStack);
            intp.Free(ptr_msg);
            intp.Free(ptr_go);
            string stackTrace = domain.DebugService.GetStackTrace(intp);
            Log.MsgWithGo(message + "/n" + stackTrace, go);
            return ret;
        }

        #endregion

        #region UI

        static unsafe StackObject* UIOpenAsync(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //只有一个参数，所以返回指针就是当前栈指针ESP - 1
            StackObject* ret = ILIntepreter.Minus(esp, 1);
            //第一个参数为ESP -1， 第二个参数为ESP - 2，以此类推
            StackObject* ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            object vm = StackObject.ToObject(ptr_of_this_method, domain, mStack);
            intp.Free(ptr_of_this_method);
            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var t = genericArguments[0];
                    if (t is ILType) //如果T是热更DLL里的类型
                    {
                        var result = UIManager.Ins.OpenAsync(t.ReflectionType,
                            (ViewModel) ((ILTypeInstance) vm)?.CLRInstance);
                        return ILIntepreter.PushObject(ret, mStack, result);
                    }
                    else
                    {
                        var result = UIManager.Ins.OpenAsync(t.TypeForCLR, (ViewModel) vm);
                        return ILIntepreter.PushObject(ret, mStack, result);
                    }
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return ret;
        }

        static unsafe StackObject* UIAddSubview(ILIntepreter intp, StackObject* esp, IList<object> mstack,
            CLRMethod method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //成员方法的第一个参数
            StackObject* ptr1 = ILIntepreter.Minus(esp, 1);
            var vm = StackObject.ToObject(ptr1, domain, mstack);
            StackObject* ptr2 = ILIntepreter.Minus(esp, 2);
            var ins = StackObject.ToObject(ptr2, domain, mstack);
            intp.Free(ptr1);
            intp.Free(ptr2);
            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    object result = null;
                    if (ins is ILTypeInstance typeInstance)
                    {
                        result = ((View) typeInstance.CLRInstance).AddSubView(genericArguments[0].ReflectionType,
                            ((ILTypeInstance) vm).CLRInstance as ViewModel);
                    }
                    else
                    {
                        result = ((View) ins).AddSubView(genericArguments[0].TypeForCLR, vm as ViewModel);
                    }
                    return ILIntepreter.PushObject(ptr2, mstack, result);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return esp;
        }

        static unsafe StackObject* UIGet(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method,
            bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //只有一个参数，所以返回指针就是当前栈指针ESP - 1
            StackObject* ret = ILIntepreter.Minus(esp, 1);
            intp.Free(ret);
            //close只能有一个泛型
            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var t = genericArguments[0];
                    object result = UIManager.Ins.Get(t is ILType ? t.ReflectionType : t.TypeForCLR);
                    if (result is CrossBindingAdaptorType adaptorType)
                    {
                        result = adaptorType.ILInstance;
                    }
                    return ILIntepreter.PushObject(ret, mStack, result);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return ret;
        }

        static unsafe StackObject* UIGet2(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method,
            bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            //只有一个参数，所以返回指针就是当前栈指针ESP - 1
            StackObject* ptr1 = ILIntepreter.Minus(esp, 1);
            var viewType = (Type) StackObject.ToObject(ptr1, domain, mStack);
            StackObject* ret = ILIntepreter.Minus(esp, 2);
            intp.Free(ret);
            intp.Free(ptr1);
            try
            {
                object result = UIManager.Ins.Get(viewType);
                if (result is CrossBindingAdaptorType adaptorType)
                {
                    result = adaptorType.ILInstance;
                }
                return ILIntepreter.PushObject(ret, mStack, result);
            }
            catch (Exception e)
            {
                string stackTrace = domain.DebugService.GetStackTrace(intp);
                Log.Error(e, stackTrace);
            }

            return ret;
        }

        static unsafe StackObject* UIClose(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method,
            bool isNewObj)
        {
            AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //只有一个参数，所以返回指针就是当前栈指针ESP - 1
            StackObject* ret = ILIntepreter.Minus(esp, 1);
            intp.Free(ret);
            //close只能有一个泛型
            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var t = genericArguments[0];
                    UIManager.Ins.Close(t is ILType ? t.ReflectionType : t.TypeForCLR);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return ret;
        }

        #endregion

        #region Context

        static unsafe StackObject* ContextContains(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //成员方法的第一个参数
            StackObject* ptr_this = ILIntepreter.Minus(esp, 1);
            var cascade = StackObject.ToObject(ptr_this, domain, mStack);
            //第二个参数为ESP - 2，以此类推
            StackObject* ret = ILIntepreter.Minus(esp, 2);
            object ins = StackObject.ToObject(ret, domain, mStack);
            intp.Free(ptr_this);
            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var t = genericArguments[0];
                    var result = ((Context) ins).Contains(t.GetCLRType().Name, (int) cascade == 1);
                    return ILIntepreter.PushObject(ret, mStack, result);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return ret;
        }

        private static unsafe StackObject* ContextRemove0(ILIntepreter intp, StackObject* esp, IList<object> mstack,
            CLRMethod method, bool isnewobj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //成员方法的第一个参数
            StackObject* ret = ILIntepreter.Minus(esp, 1);
            //调用方 this
            object ins = StackObject.ToObject(ret, domain, mstack);
            intp.Free(ret);
            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var t = genericArguments[0];
                    var result = ((Context) ins).Remove(t.GetCLRType().Name);
                    if (result is CrossBindingAdaptorType adaptorType)
                    {
                        return ILIntepreter.PushObject(ret, mstack, adaptorType.ILInstance);
                    }
                    return ILIntepreter.PushObject(ret, mstack, result);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return esp;
        }

        private static unsafe StackObject* ContextRemove1(ILIntepreter intp, StackObject* esp, IList<object> mstack,
            CLRMethod method, bool isnewobj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //成员方法的第一个参数
            StackObject* ptr1 = ILIntepreter.Minus(esp, 1);
            var name = StackObject.ToObject(ptr1, domain, mstack);
            StackObject* ptr2 = ILIntepreter.Minus(esp, 2);
            var ins = StackObject.ToObject(ptr2, domain, mstack);
            intp.Free(ptr1);
            intp.Free(ptr2);

            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var result = ((Context) ins).Remove((string) name);
                    if (result is CrossBindingAdaptorType adaptorType)
                    {
                        return ILIntepreter.PushObject(ptr2, mstack, adaptorType.ILInstance);
                    }
                    return ILIntepreter.PushObject(ptr2, mstack, result);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return esp;
        }


        private static unsafe StackObject* ContextGetContext(ILIntepreter intp, StackObject* esp, IList<object> mstack,
            CLRMethod method, bool isnewobj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var t = genericArguments[0];
                    string name = t.GetCLRType().Name;
                    var result = Context.GetContext(name);
                    if (result is CrossBindingAdaptorType adaptorType)
                    {
                        return ILIntepreter.PushObject(esp, mstack, adaptorType.ILInstance);
                    }
                    return ILIntepreter.PushObject(esp, mstack, result);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return esp;
        }

        private static unsafe StackObject* ContextRemoveContext(ILIntepreter intp, StackObject* esp,
            IList<object> mstack, CLRMethod method, bool isnewobj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var t = genericArguments[0];
                    string name = t.GetCLRType().Name;
                    Context.RemoveContext(name);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return esp;
        }

        private static unsafe StackObject* ContextSet1(ILIntepreter intp, StackObject* esp, IList<object> mstack,
            CLRMethod method, bool isnewobj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //成员方法的第一个参数
            StackObject* ptr_this = ILIntepreter.Minus(esp, 1);
            var value = StackObject.ToObject(ptr_this, domain, mstack);
            //第二个参数为ESP - 2，以此类推
            StackObject* ptr_of_this_method = ILIntepreter.Minus(esp, 2);
            //调用方 this
            object ins = StackObject.ToObject(ptr_of_this_method, domain, mstack);
            intp.Free(ptr_this);
            intp.Free(ptr_of_this_method);
            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var t = genericArguments[0];
                    ((Context) ins).Set(t.GetCLRType().Name, value);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return esp;
        }

        private static unsafe StackObject* ContextGet2(ILIntepreter intp, StackObject* esp, IList<object> mstack,
            CLRMethod method, bool isnewobj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //成员方法的第一个参数
            StackObject* ptr1 = ILIntepreter.Minus(esp, 1);
            var cascade = StackObject.ToObject(ptr1, domain, mstack);
            StackObject* ptr2 = ILIntepreter.Minus(esp, 2);
            var name = StackObject.ToObject(ptr2, domain, mstack);
            StackObject* ptr3 = ILIntepreter.Minus(esp, 3);
            var ins = StackObject.ToObject(ptr3, domain, mstack);
            intp.Free(ptr1);
            intp.Free(ptr2);
            intp.Free(ptr3);

            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    object result = null;
                    if (ins is ILTypeInstance typeInstance)
                    {
                        result = ((Context) typeInstance.CLRInstance).Get((string) name, (int) cascade == 1);
                    }
                    else
                    {
                        result = ((Context) ins).Get((string) name, (int) cascade == 1);
                    }
                    return ILIntepreter.PushObject(ptr3, mstack, result);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return esp;
        }

        private static unsafe StackObject* ContextGet1(ILIntepreter intp, StackObject* esp, IList<object> mstack,
            CLRMethod method, bool isnewobj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            //获取泛型参数<T>的实际类型
            IType[] genericArguments = method.GenericArguments;
            //成员方法的第一个参数
            StackObject* ptr1 = ILIntepreter.Minus(esp, 1);
            var cascade = StackObject.ToObject(ptr1, domain, mstack);
            StackObject* ptr2 = ILIntepreter.Minus(esp, 2);
            var ins = StackObject.ToObject(ptr2, domain, mstack);
            intp.Free(ptr1);
            intp.Free(ptr2);

            if (genericArguments != null && genericArguments.Length == 1)
            {
                try
                {
                    var t = genericArguments[0];
                    object result;
                    if (ins is ILTypeInstance ilTypeInstance)
                    {
                        result = ((Context) ilTypeInstance.CLRInstance).Get(t.GetCLRType().Name, (int) cascade == 1);
                    }
                    else
                    {
                        result = ((Context) ins).Get(t.GetCLRType().Name, (int) cascade == 1);
                    }
                    return ILIntepreter.PushObject(ptr2, mstack, result);
                }
                catch (Exception e)
                {
                    string stackTrace = domain.DebugService.GetStackTrace(intp);
                    Log.Error(e, stackTrace);
                }
            }

            return esp;
        }

        #endregion

        #region Component

        /// <summary>
        /// Get的字符串参数重定向
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        private static unsafe StackObject* GetOrAddComponent(ILIntepreter __intp, StackObject* __esp,
            IList<object> __mStack, CLRMethod __method, bool isNewObj)
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
                try
                {
                    var res = getComponent(ins, genericArgument[0]);
                    //没有找到组件
                    if (res == null || res.Equals(null))
                    {
                        //添加组件
                        res = addComponent(ins as GameObject, genericArgument[0], __domain);
                    }
                    return ILIntepreter.PushObject(ptr, __mStack, res);
                }
                catch (Exception e)
                {
                    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                    Log.Error(e, stackTrace);
                }
            }

            return __esp;
        }

        /// <summary>
        /// AddComponent 实现
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                try
                {
                    object res = addComponent(instance, genericArgument[0], __domain);
                    return ILIntepreter.PushObject(ptr, __mStack, res);
                }
                catch (Exception e)
                {
                    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                    Log.Error(e, stackTrace);
                }
            }

            return __esp;
        }

        private static object addComponent(GameObject instance, IType type, AppDomain __domain)
        {

            object res;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.AddComponent(type.TypeForCLR);
            }
            else
            {
                //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                ILTypeInstance ilInstance = new ILTypeInstance(type as ILType, false);
                Type t = type.ReflectionType; //获取实际属性
                bool isMonoAdapter = t.BaseType?.FullName == typeof(MonoBehaviourAdapter.Adaptor).FullName;

                if (!isMonoAdapter && Type.GetType(t.BaseType.FullName) != null)
                {
                    Type adapterType = Type.GetType(t.BaseType?.FullName);
                    if (adapterType == null)
                    {
                        Log.Error(t.FullName, "need to generate adapter");
                        return null;
                    }

                    //直接反射赋值一波了
                    var clrInstance = instance.AddComponent(adapterType);
                    var ILInstance = t.GetField("instance",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    var AppDomain = t.GetField("appdomain",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    ILInstance.SetValue(clrInstance, ilInstance);
                    AppDomain.SetValue(clrInstance, __domain);
                    ilInstance.CLRInstance = clrInstance;
                    bool activated = false;
                    //不管是啥类型，直接invoke这个awake方法
                    var awakeMethod = clrInstance.GetType().GetMethod("Awake",
                        BindingFlags.Default | BindingFlags.Public
                                             | BindingFlags.Instance | BindingFlags.FlattenHierarchy |
                                             BindingFlags.NonPublic | BindingFlags.Static);
                    if (awakeMethod == null)
                    {
                        awakeMethod = t.GetMethod("Awake",
                            BindingFlags.Default | BindingFlags.Public
                                                 | BindingFlags.Instance | BindingFlags.FlattenHierarchy |
                                                 BindingFlags.NonPublic | BindingFlags.Static);
                    }
                    else
                    {
                        awakeMethod.Invoke(clrInstance, null);
                        activated = true;
                    }

                    if (awakeMethod == null)
                    {
                        Log.Error($"{t.FullName}不包含Awake方法，无法激活，已跳过");
                    }
                    else if (!activated)
                    {
                        awakeMethod.Invoke(t, null);
                    }
                }
                else
                {
                    //接下来创建Adapter实例
                    var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                    //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                    clrInstance.ILInstance = ilInstance;
                    clrInstance.AppDomain = __domain;
                    //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                    ilInstance.CLRInstance = clrInstance;
                    clrInstance.Awake(); //因为Unity调用这个方法时还没准备好所以这里补调一次
                }

                res = ilInstance;

                var m = type.GetConstructor(ILRuntime.CLR.Utils.Extensions.EmptyParamList);
                if (m != null)
                {
                    __domain.Invoke(m, res, null);
                }
            }
            return res;
        }


        /// <summary>
        /// Get的字符串参数重定向
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        private static unsafe StackObject* GetComponent_1(ILIntepreter __intp, StackObject* __esp,
            IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            String type = (String) StackObject.ToObject(ptr_of_this_method, __domain, __mStack);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GameObject instance_of_this_method =
                (GameObject) (StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            object result_of_this_method = null;
            try
            {
                result_of_this_method = instance_of_this_method.GetComponent(type); //先从本地匹配

                if (result_of_this_method == null) //本地没再从热更匹配
                {
                    var typeName = __domain.LoadedTypes.Keys.ToList().Find(k => k.EndsWith(type));
                    if (typeName != null) //如果有这个热更类型
                    {
                        //适配器全查找出来，匹配ILTypeInstance的真实类型的FullName
                        var clrInstances = instance_of_this_method.GetComponents<CrossBindingAdaptorType>();
                        for (int i = 0; i < clrInstances.Length; i++)
                        {
                            var clrInstance = clrInstances[i];
                            if (clrInstance.ILInstance != null) //ILInstance为null, 表示是无效的MonoBehaviour，要略过
                            {
                                if (clrInstance.ILInstance.Type.ReflectionType.FullName == typeName)
                                {
                                    result_of_this_method = clrInstance.ILInstance; //交给ILRuntime的实例应该为ILInstance
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                Log.Error(e, stackTrace);
            }

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        /// <summary>
        /// GetComponent 的实现
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            AppDomain __domain = __intp.AppDomain;

            var ptr = ILIntepreter.Minus(__esp, 1);
            //成员方法的第一个参数为this
            var ins = StackObject.ToObject(ptr, __domain, __mStack);
            if (ins == null)
                throw new NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                try
                {
                    var res = getComponent(ins, genericArgument[0]);
                    if (res != null)
                        return ILIntepreter.PushObject(ptr, __mStack, res);
                }
                catch (Exception e)
                {
                    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                    Log.Error(e, stackTrace);
                }
            }

            return __esp;
        }

        private static object getComponent(object ins, IType type)
        {
            GameObject instance;

            if (ins is GameObject)
            {
                instance = ins as GameObject;
            }
            else if (ins is Component)
            {
                instance = ((Component) ins).gameObject;
            }
            else if (ins is ILTypeInstance)
            {
                instance = FindGOFromHotClass(((ILTypeInstance) ins));
            }
            else
            {
                Debug.LogError($"[GetComponent错误] 不支持的参数类型：{ins.GetType().FullName}，" +
                               "请传参GameObject或继承MonoBehaviour的对象");
                return null;
            }

            object res = null;
            if (type is CLRType)
            {
                //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                res = instance.GetComponent(type.TypeForCLR);
            }
            else
            {
                //因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                var clrInstances = instance.GetComponents<CrossBindingAdaptorType>();
                for (int i = 0; i < clrInstances.Length; i++)
                {
                    var clrInstance = clrInstances[i];
                    if (clrInstance.ILInstance != null) //ILInstance为null, 表示是无效的MonoBehaviour，要略过
                    {
                        if (clrInstance.ILInstance.Type == type ||
                            clrInstance.ILInstance.Type.ReflectionType.IsSubclassOf(type.ReflectionType))
                        {
                            res = clrInstance.ILInstance; //交给ILRuntime的实例应该为ILInstance
                            break;
                        }
                    }
                }
            }
            return res;
        }

        private static GameObject FindGOFromHotClass(ILTypeInstance instance)
        {
            var returnType = instance.Type;
            if (returnType.ReflectionType == typeof(MonoBehaviour))
            {
                var pi = returnType.ReflectionType.GetProperty("gameObject");
                return pi.GetValue(instance.CLRInstance) as GameObject;
            }

            if (returnType.ReflectionType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                var pi = returnType.ReflectionType.BaseType.GetProperty("gameObject");
                return pi.GetValue(instance.CLRInstance) as GameObject;
            }
            return null;
        }

        #endregion

        unsafe static StackObject* BindViewList(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            AppDomain __domain = __intp.AppDomain;

            var ptr1 = ILIntepreter.Minus(__esp, 1);
            var root = StackObject.ToObject(ptr1, __domain, __mStack);
            var ptr2 = ILIntepreter.Minus(__esp, 2);
            var list = StackObject.ToObject(ptr2, __domain, __mStack);
            var ptr3 = ILIntepreter.Minus(__esp, 3);
            var ins = StackObject.ToObject(ptr3, __domain, __mStack);
            __intp.Free(ptr1);
            __intp.Free(ptr2);
            __intp.Free(ptr3);

            var genericArgument = __method.GenericArguments;

            if (genericArgument != null && genericArgument.Length == 2)
            {
                try
                {
                    var viewType = genericArgument[1];
                    Type type = null;
                    if (viewType is CLRType)
                    {
                        type = viewType.TypeForCLR;
                    }
                    else
                    {
                        type = viewType.ReflectionType;
                    }
                    ((UIBindFactory) ins).BindViewList((ObservableList<ViewModelAdapter.Adapter>) list,
                        (Transform) root, type);
                }
                catch (Exception e)
                {
                    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                    Log.Error(e, stackTrace);
                }
            }

            return __esp;
        }

        unsafe static StackObject* BindIpairs(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            AppDomain __domain = __intp.AppDomain;

            var ptr1 = ILIntepreter.Minus(__esp, 1);
            var pattern = StackObject.ToObject(ptr1, __domain, __mStack);
            var ptr2 = ILIntepreter.Minus(__esp, 2);
            var root = StackObject.ToObject(ptr2, __domain, __mStack);
            var ptr3 = ILIntepreter.Minus(__esp, 3);
            var list = StackObject.ToObject(ptr3, __domain, __mStack);
            var ptr4 = ILIntepreter.Minus(__esp, 4);
            var ins = StackObject.ToObject(ptr4, __domain, __mStack);
            __intp.Free(ptr1);
            __intp.Free(ptr2);
            __intp.Free(ptr3);
            __intp.Free(ptr4);

            var genericArgument = __method.GenericArguments;

            if (genericArgument != null && genericArgument.Length == 2)
            {
                try
                {
                    var viewType = genericArgument[1];
                    Type type = null;
                    if (viewType is CLRType)
                    {
                        type = viewType.TypeForCLR;
                    }
                    else
                    {
                        type = viewType.ReflectionType;
                    }

                    ((UIBindFactory) ins).BindIpairs((ObservableList<ViewModelAdapter.Adapter>) list, (Transform) root,
                        (string) pattern, type);
                }
                catch (Exception e)
                {
                    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                    Log.Error(e, stackTrace);
                }
            }

            return __esp;
        }

        /// <summary>
        /// pb net 反序列化重定向
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        private static unsafe StackObject* Deserialize_1(ILIntepreter __intp, StackObject* __esp,
            IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.IO.Stream @source =
                (System.IO.Stream) typeof(System.IO.Stream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method,
                    __domain,
                    __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Type @type =
                (System.Type) typeof(System.Type).CheckCLRTypes(
                    StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            var result_of_this_method = RuntimeTypeModel.Default.Deserialize(source, null, type);
            object obj_result_of_this_method = result_of_this_method;
            if (obj_result_of_this_method is CrossBindingAdaptorType)
            {
                return ILIntepreter.PushObject(__ret, __mStack,
                    ((CrossBindingAdaptorType) obj_result_of_this_method).ILInstance, true);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method, true);
        }

        /// <summary>
        /// pb net 反序列化重定向
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        private static unsafe StackObject* Deserialize_2(ILIntepreter __intp, StackObject* __esp,
            IList<object> __mStack,
            CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.IO.Stream @source =
                (System.IO.Stream) typeof(System.IO.Stream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method,
                    __domain,
                    __mStack));
            __intp.Free(ptr_of_this_method);
            var genericArgument = __method.GenericArguments;
            var type = genericArgument[0];
            var @realType = type is CLRType ? type.TypeForCLR : type.ReflectionType;
            var result_of_this_method = RuntimeTypeModel.Default.Deserialize(source, null, realType);
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }
    }
}