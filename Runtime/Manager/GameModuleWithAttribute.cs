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

    public abstract class GameModule<T> : MonoBehaviour where T : GameModule<T>
    {
        public static T Ins { get; private set; }
        public virtual void Init()
        {
        }

        public virtual void OnStart()
        {
        }
        
        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal virtual int Priority => 0;
        
        /// <summary>
        /// 游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal abstract void OnUpdate(float elapseSeconds, float realElapseSeconds);
        
        /// <summary>
        /// 关闭并清理游戏框架模块。
        /// </summary>
        internal abstract void Shutdown();
    }

    public abstract class GameModuleWithAttribute<T,V,I> : GameModule<T>  where T : GameModuleWithAttribute<T,V,I>, new() 
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
    }
}