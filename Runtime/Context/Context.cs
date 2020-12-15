/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using Framework.Services;

namespace Framework.Contexts
{
    public class Context
    {
        private static ApplicationContext context = new ApplicationContext();
        private static Dictionary<string, Context> contexts = new Dictionary<string, Context>();
        public static ApplicationContext GetApplicationContext()
        {
            return Context.context;
        }

        public static void SetApplicationContext(ApplicationContext context)
        {
            Context.context = context;
        }
        
        public static Context GetContext(string key)
        {
            Context context = null;
            contexts.TryGetValue(key, out context);
            return context;
        }

        public static T GetContext<T>(string key) where T : Context
        {
            return (T)GetContext(key);
        }

        public static void AddContext(string key, Context context)
        {
            contexts.Add(key, context);
        }

        public static void RemoveContext(string key)
        {
            contexts.Remove(key);
        }
        
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