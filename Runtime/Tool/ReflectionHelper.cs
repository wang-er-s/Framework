using System;
using System.Collections.Generic;
using System.Reflection;
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
        
        private static Dictionary<Type, Dictionary<string, MemberInfo>> MemberCahce =
            new Dictionary<Type, Dictionary<string, MemberInfo>>();

        /// <summary>
        /// 仅在每个类型每次获取一样字段的地方使用
        /// 如View中获取TransformPath 
        /// </summary>
        public static Dictionary<string, MemberInfo> GetCacheMember(Type type, Predicate<MemberInfo> filter = null,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (MemberCahce.TryGetValue(type, out var result)) return result;
            var fields = type.GetFields(flags);
            var properties = type.GetProperties(flags);
            result = new Dictionary<string, MemberInfo>(fields.Length + properties.Length);
            foreach (var fieldInfo in fields)
            {
                if (filter == null)
                {
                    result[fieldInfo.Name] = fieldInfo;
                }
                else if (filter(fieldInfo))
                {
                    result[fieldInfo.Name] = fieldInfo;
                }
            }

            foreach (var propertyInfo in properties)
            {
                if (filter != null && filter(propertyInfo))
                {
                    result[propertyInfo.Name] = propertyInfo;
                }
                else
                {
                    result[propertyInfo.Name] = propertyInfo;
                }
            }

            MemberCahce[type] = result;
            return result;
        }
    }
}