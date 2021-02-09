using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;

namespace Framework
{
    public class ManagerAttribute : Attribute
    {
        public int IntTag { get; private set; } = -1;
        
        public ManagerAttribute(int intTag)
        {
            this.IntTag = intTag;
        }

        public virtual string IndexName { get; } = nameof(IntTag);
    }
    
    public class ManagerBase<T,V,I> : IManager  where T : IManager, new() 
        where V : ManagerAttribute
    {
        
        private static T ins;

        public static T Ins
        {
            get
            {
                if (ins == null)
                {
                    ins = new T();
                }

                return ins;
            }
        }

        protected Dictionary<I, ClassData> ClassDataMap { get; } = new Dictionary<I, ClassData>();
        
        public virtual void Init()
        {
            
        }

        public virtual void Start()
        {
            
        }

        private PropertyInfo _indexProperty;
        
        public virtual void CheckType(Type type)
        {
            var attr = type.GetCustomAttribute<V>();
            
            if (attr != null)
            {
                if (_indexProperty == null)
                {
                    _indexProperty = typeof(V).GetProperty(attr.IndexName);
                }
                ClassDataMap[(I)_indexProperty.GetValue(attr)] = new ClassData {Attribute = attr, Type = type};
            }
        }
        
        public ClassData GetClassData(I tag)
        {
            this.ClassDataMap.TryGetValue(tag, out var classData);
            return classData;
        }
        
        public ClassData GetClassData<TN>()
        {
            return GetClassData(typeof(TN));
        }
        
        public ClassData GetClassData(Type type)
        {
            var classDatas = GetAllClassDatas();
            foreach (var value in classDatas)
            {
                if (value.Type == type)
                {
                    return value;
                }
            }
            return null;
        }
        
        public IEnumerable<ClassData> GetAllClassDatas()
        {
            return ClassDataMap.Values;
        }
        
        public T2 CreateInstance<T2>(ClassData cd, params object[] args) where T2 : class
        {
            if (cd.Type != null)
            {
                if (args.Length == 0)
                {
                    return Activator.CreateInstance(cd.Type) as T2;
                }
                else
                {
                    return Activator.CreateInstance(cd.Type, args) as T2;
                }
            }
            else
            {
                return null;
            }
        }
        
        public T2 CreateInstance<T2>(I tag, params object[] args) where T2 : class
        {
            var cd = GetClassData(tag);
            if (cd == null)
            {
                Log.Error("没有找到:", tag, " -", typeof(T2).Name);
                return null;
            }

            return CreateInstance<T2>(cd, args);
        }
    }
}