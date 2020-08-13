using System;
using System.Collections.Generic;
using Framework.Services;

namespace Framework.Context
{
    public class Context
    {
        private bool _innerContainer;
        private Context _contextBase;
        private IServiceContainer _container;
        private Dictionary<string, object> _attributes;

        public Context() : this(null, null)
        {
        }

        public Context(IServiceContainer container, Context contextBase)
        {
            _attributes = new Dictionary<string, object>();
            _contextBase = contextBase;
            _container = container;
            if (_container == null)
            {
               _innerContainer = true;
               _container = new ServiceContainer();
            }
        }
        
        public virtual bool Contains(string name, bool cascade = true)
        {
            if (this._attributes.ContainsKey(name))
                return true;

            if (cascade && this._contextBase != null)
                return this._contextBase.Contains(name, cascade);

            return false;
        }

        public virtual object Get(string name, bool cascade = true)
        {
            return this.Get<object>(name, cascade);
        }

        public virtual T Get<T>(string name, bool cascade = true)
        {
            object v;
            if (this._attributes.TryGetValue(name, out v))
                return (T)v;

            if (cascade && this._contextBase != null)
                return this._contextBase.Get<T>(name, cascade);

            return default(T);
        }

        public virtual void Set(string name, object value)
        {
            this.Set<object>(name, value);
        }

        public virtual void Set<T>(string name, T value)
        {
            this._attributes[name] = value;
        }

        public virtual object Remove(string name)
        {
            return this.Remove<object>(name);
        }

        public virtual T Remove<T>(string name)
        {
            if (!this._attributes.ContainsKey(name))
                return default(T);

            object v = this._attributes[name];
            this._attributes.Remove(name);
            return (T)v;
        }

        public virtual IServiceContainer GetContainer()
        {
            return _container;
        }

        public virtual object GetService(Type type)
        {
            object result = _container.Resolve(type);
            if (result != null)
                return result;

            if (this._contextBase != null)
                return _contextBase.GetService(type);

            return null;
        }
        
        public virtual object GetService(string name)
        {
            object result = _container.Resolve(name);
            if (result != null)
                return result;

            if (_contextBase != null)
                return _contextBase.GetService(name);

            return null;
        }

        public virtual T GetService<T>()
        {
            T result = _container.Resolve<T>();
            if (result != null)
                return result;

            if (_contextBase != null)
                return _contextBase.GetService<T>();

            return default(T);
        }

        public virtual T GetService<T>(string name)
        {
            T result = _container.Resolve<T>(name);
            if (result != null)
                return result;

            if (_contextBase != null)
                return _contextBase.GetService<T>(name);

            return default(T);
        }

        public virtual void Register<T>(T target)
        {
            Register(typeof(T).Name, target);
        }

        public virtual void Register<T>(Func<T> factory)
        {
            Register(typeof(T).Name, factory);
        }

        public virtual void Register<T>(string name, T target)
        {
            _container.Register(name, target);
        }

        public virtual void Register<T>(string name, Func<T> factory)
        {
            _container.Register(name, factory);
        }

        public virtual void UnRegister<T>()
        {
            _container.Unregister<T>();
        }

        public virtual void UnRegister(Type type)
        {
            _container.Unregister(type);
        }

        public virtual void UnRegister(string name)
        {
            _container.Unregister(name);
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this._innerContainer && this._container != null)
                    {
                        IDisposable dis = this._container as IDisposable;
                        if (dis != null)
                            dis.Dispose();
                    }
                }
                disposed = true;
            }
        }

        ~Context()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}