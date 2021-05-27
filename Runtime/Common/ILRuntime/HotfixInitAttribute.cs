using System;

namespace Framework
{
    /// <summary>
    /// 标记热更初始化注册方法的标签
    /// 标记后会在热更dll加载完后自动调用注册重定向或者委托适配器等
    /// 方法的参数必须是AppDomain
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Class)]
    public class HotfixInitAttribute : Attribute
    {
        
    }
}