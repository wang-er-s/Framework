using System;
using UnityEngine.Events;

namespace Framework
{
    public class ILRuntimeDelegateHelper
    {
        //跨域委托调用，注册委托的适配器
        public static void RegisterDelegate(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            //Button 点击事件的委托注册
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction>((act) =>
            {
                return new UnityAction(() => { ((Action) act)(); });
            });
            appdomain.DelegateManager.RegisterMethodDelegate<string>();
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction<String>>((act) =>
            {
                return new UnityAction<String>((arg0) => { ((Action<string>) act)(arg0); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction<bool>>((act) =>
            {
                return new UnityAction<bool>((arg0) => { ((Action<Boolean>) act)(arg0); });
            });
            appdomain.DelegateManager.RegisterMethodDelegate<bool>();
            appdomain.DelegateManager.RegisterMethodDelegate<object>();
            appdomain.DelegateManager.RegisterFunctionDelegate<string, string>();
            // new thread
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.ParameterizedThreadStart>((act) =>
            {
                return new System.Threading.ParameterizedThreadStart((obj) =>
                {
                    ((Action<System.Object>)act)(obj);
                });
            });
            appdomain.DelegateManager
                .RegisterMethodDelegate<Asynchronous.IProgressResult<System.Single, System.String>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean>();

        }
    }
}