using System;

namespace Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : BaseAttribute
    {
        public string Path { get; }

        public ConfigAttribute(string path)
        {
            Path = path;
        }
    }
}