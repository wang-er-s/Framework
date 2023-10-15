using System;

namespace Framework
{
    public interface IRendererUpdateSystem : ISystemType
    {
        void RenderUpdate(float deltaTime);
    }
}