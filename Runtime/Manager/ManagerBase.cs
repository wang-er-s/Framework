using System;
using System.Collections.Generic;

namespace Framework
{
    public class ManagerAttribute : Attribute
    {
        public int IntTag { get; private set; } = -1;
        
        public ManagerAttribute(int intTag)
        {
            this.IntTag = intTag;
        }
    }
    
    public class ManagerBase<T,V> : IManager  where T : IManager, new() 
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

        private Dictionary<int, ClassData> ClassDataMap_IntKey { get; } = new Dictionary<int, ClassData>();
        
        public virtual void Init()
        {
            
        }

        public virtual void Start()
        {
            
        }

        private Type vType;

        public virtual void CheckType(Type type)
        {
            if (vType == null)
            {
                vType = typeof(V);
            }

            var attrs = type.GetCustomAttributes(vType, false);
            if (attrs.Length > 0)
            {
                var attr = attrs[0];
                if (attr is V attr1)
                {
                    ClassDataMap_IntKey[attr1.IntTag] = new ClassData {Attribute = attr1, Type = type};
                }
            }
        }
        
        public ClassData GetClassData(int tag)
        {
            this.ClassDataMap_IntKey.TryGetValue(tag, out var classData);
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
            return ClassDataMap_IntKey.Values;
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
        
        public T2 CreateInstance<T2>(int tag, params object[] args) where T2 : class
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