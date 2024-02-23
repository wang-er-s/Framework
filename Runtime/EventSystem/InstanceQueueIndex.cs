using System;
using System.Collections.Generic;

namespace Framework
{
    public enum InstanceQueueIndex
    {
        None = -1,
        Update,
        LateUpdate,
        FixedUpdate,
        RendererUpdate,
        ParkourUpdate,
        ParkourRendererUpdate,
        ParkourLateUpdate,
        Max,
    }

    public static class InstanceQueueMap
    {
        public static Dictionary<Type, InstanceQueueIndex> InstanceQueueMapDic =
            new Dictionary<Type, InstanceQueueIndex>()
            {
                { typeof(IUpdateSystem), InstanceQueueIndex.Update },
                { typeof(ILateUpdateSystem), InstanceQueueIndex.LateUpdate },
                { typeof(IRendererUpdateSystem), InstanceQueueIndex.RendererUpdate },
                { typeof(IFixedUpdateSystem), InstanceQueueIndex.FixedUpdate },
            };
    }
}