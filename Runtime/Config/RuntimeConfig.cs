using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework
{
    [ShowOdinSerializedPropertiesInInspector]
    public class RuntimeConfig : ConfigBase
    {
        [LabelText("资源加载类型")]
        public ResType LoadType = ResType.Addressable;
        
        public enum ResType
        {
            Resources,
            Addressable,
        }
    }
}