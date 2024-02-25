using System;
using System.Collections.Generic;

namespace Framework
{
    public class Context : Entity, IAwakeSystem, IDestroySystem
    {
        private Dictionary<string, object> _attributes ;

        public virtual void Awake()
        {
            _attributes = ReferencePool.Allocate<Dictionary<string,object>>();
        }

        public virtual bool Contains(string name)
        {
            if (_attributes.ContainsKey(name))
            {
                return true;
            }

            return false;
        }

        public virtual bool Contains<T>()
        {
            return Contains(typeof(T).Name);
        }

        public virtual object Get(string name)
        {
            return Get<object>(name);
        }

        public virtual T Get<T>()
        {
            return Get<T>(typeof(T).Name);
        }

        public virtual T Get<T>(string name)
        {
            object v;
            if (_attributes.TryGetValue(name, out v))
            {
                return (T)v;
            }

            return default;
        }

        public virtual void Set(string name, object value)
        {
            _attributes[name] = value;
        }

        public virtual void Set<T>(T value)
        {
            Set(typeof(T).Name, value);
        }

        public virtual object Remove(string name)
        {
            return Remove<object>(name);
        }

        public virtual T Remove<T>()
        {
            return Remove<T>(typeof(T).Name);
        }

        public virtual T Remove<T>(string name)
        {
            if (!_attributes.ContainsKey(name))
            {
                return default;
            }

            object v = _attributes[name];
            _attributes.Remove(name);
            return (T)v;
        }

        public virtual void OnDestroy()
        {
            _attributes.Clear();
            ReferencePool.Free(_attributes);
        }
    }
}