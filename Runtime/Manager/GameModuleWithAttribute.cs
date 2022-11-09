using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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

    public interface IGameModule
    {
        void Init();
        void OnStart();
        void OnUpdate(float deltaTime);
        void OnGUI();
        void Shutdown();
    }

    public interface IGameModuleWithAttribute : IGameModule
    {
        void CheckType(Type type);
    }

    public abstract class GameModuleWithAttribute<T,V,I> : IGameModuleWithAttribute where T : GameModuleWithAttribute<T,V,I> 
        where V : ManagerAttribute
    {
        protected Dictionary<I, ClassData> ClassDataMap { get; } = new Dictionary<I, ClassData>();
        private PropertyInfo _indexProperty;
        private BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public;
        
        public virtual void CheckType(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(V), false);
            if(attrs.Length <= 0) return;
            Debug.Assert(attrs.Length == 1, $"{type.Name} has mul attribute {typeof(V)}");
            var attr = (V) attrs[0];
            if (attr != null)
            {
                if (_indexProperty == null)
                {
                    _indexProperty = typeof(V).GetProperty(attr.IndexName, _flags);
                }
                ClassDataMap[(I)_indexProperty.GetValue(attr)] = new ClassData {Attribute = attr, Type = type};
            }
        }

        private ClassData GetClassData(I tag)
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
            var classDatas = GetAllClassData();
            foreach (var value in classDatas)
            {
                if (value.Type == type)
                {
                    return value;
                }
            }
            return null;
        }
        
        public IEnumerable<ClassData> GetAllClassData()
        {
            return ClassDataMap.Values;
        }

        protected TIns CreateInstance<TIns>(I tag, params object[] args) where TIns : class
        {
            var cd = GetClassData(tag);
            if (cd == null)
            {
                Log.Error("没有找到:", tag, " -");
                return null;
            }
            return ReflectionHelper.CreateInstance(cd.Type, args) as TIns;
        }

        public virtual void Init()
        {
            
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }

        public virtual void OnGUI()
        {
        }

        public virtual void Shutdown()
        {
        }

        void IGameModuleWithAttribute.CheckType(Type type)
        {
            CheckType(type);
        }
    }
}