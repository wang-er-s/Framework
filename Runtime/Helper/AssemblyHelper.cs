using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework
{
    public static class AssemblyHelper
    {
        public static Dictionary<string, Type> GetAssemblyTypes(params Assembly[] args)
        {
            Dictionary<string, Type> types = new Dictionary<string, Type>();

            foreach (Assembly ass in args)
            {
                foreach (Type type in ass.GetTypes())
                {
                    types[type.FullName] = type;
                }
            }

            return types;
        }
        
        /// <summary>
        /// 获取Assembly
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("JetBrains")) continue;
                if (assembly.GetName().Name.Equals(name))
                {
                    return assembly;
                }
            }

            return null;
        }
    }
}