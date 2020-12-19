using Sirenix.OdinInspector;

namespace Framework
{
    [ShowOdinSerializedPropertiesInInspector]
    public class UIConfig : ConfigBase
    {
        [FolderPath(AbsolutePath = false)]
        public string GenUIScriptsPath;
    }
}