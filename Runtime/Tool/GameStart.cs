using System;

namespace Framework
{
    /// <summary>
    /// 游戏开始标签会在管理器初始化之前调用
    /// 必须是静态无参方法
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Class)]
    public class GameStartAttribute : Attribute
    {
    }
}