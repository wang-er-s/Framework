using System;
using System.Collections.Generic;

namespace Framework
{
    public class Context : IDisposable
    {
        private Context _contextBase;
        private Dictionary<string, object> _attributes;

        public Context() : this(null)
        {
        }

        public Context(Context contextBase)
        {
            _attributes = new Dictionary<string, object>();
            _contextBase = contextBase;
        }

        public virtual bool Contains(string name, bool cascade = true)
        {
            if (_attributes.ContainsKey(name))
            {
                return true;
            }

            if (cascade && _contextBase != null)
            {
                return _contextBase.Contains(name, cascade);
            }

            return false;
        }

        public virtual bool Contains<T>(bool cascade = true)
        {
            return Contains(typeof(T).Name, cascade);
        }

        public virtual object Get(string name, bool cascade = true)
        {
            return Get<object>(name, cascade);
        }

        public virtual T Get<T>(bool cascade = true)
        {
            return Get<T>(typeof(T).Name, cascade);
        }

        public virtual T Get<T>(string name, bool cascade = true)
        {
            object v;
            if (_attributes.TryGetValue(name, out v))
            {
                return (T)v;
            }

            if (cascade && _contextBase != null)
            {
                return _contextBase.Get<T>(name, cascade);
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

        public virtual void Dispose()
        {
        }
    }
}