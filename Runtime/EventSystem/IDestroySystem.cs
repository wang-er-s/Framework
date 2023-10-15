using System;

namespace Framework
{
    public interface IDestroySystem : ISystemType
    {
        void OnDestroy();
    }
}