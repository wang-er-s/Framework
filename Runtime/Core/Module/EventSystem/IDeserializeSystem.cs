using System;

namespace Framework
{

    /// <summary>
    /// 反序列化后执行的System
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeserializeSystem : ISystemType
    {
        void OnDeserialize();
    }

}