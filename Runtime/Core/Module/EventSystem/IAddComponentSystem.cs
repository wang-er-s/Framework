using System;

namespace Framework
{
    public interface IAddComponentSystem : ISystemType
    {
        void OnAddComponent(Entity component);
    }
}