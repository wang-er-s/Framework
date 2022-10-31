using System;

namespace Framework
{
    public class UIAttribute : ManagerAttribute
    {
        public string Path { get; private set; }

        public UIAttribute(string path) : base(-1)
        {
            Path = path;
        }
    }
}