using System;
using Framework;
using Framework.UI.Core;

namespace Tool
{
    public class ReflectionHelper
    {
        private const string ILRuntimeDllName = "ILRuntime";
        
        public static object CreateInstance(Type type)
        {
            object result;
            if (type.Assembly.GetName().Name == ILRuntimeDllName)
            {
                result = ILRuntimeHelper.appdomain.Instantiate(type.FullName);
            }
            else
            {
                result = Activator.CreateInstance(type);
            }
            return result;
        }
    }
}