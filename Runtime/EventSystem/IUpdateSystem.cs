using System;

namespace Framework
{
    // 逻辑计算update 会以较低的帧率来跑
    public interface IUpdateSystem : ISystemType
    {
        void Update(float deltaTime);
    }
}