using System;

namespace Framework
{
    /// <summary>
    /// 标记热更初始化注册方法的标签
    /// 标记后会在热更dll加载完后自动调用注册重定向或者委托适配器等
    /// </summary>
    public class HotfixInitAttribute : Attribute
    {
        
    }
}