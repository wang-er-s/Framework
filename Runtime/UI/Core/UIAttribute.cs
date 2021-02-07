using System;

namespace Framework.UI.Core
{
    public class UIAttribute : ManagerAttribute
    {
        public string Path { get; private set; }
        
        public UIAttribute(int intTag, string path) : base(intTag)
        {
            Path = path;
        }
    }
}