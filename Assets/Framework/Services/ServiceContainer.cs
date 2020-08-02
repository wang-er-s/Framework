using System;
using System.Collections.Generic;

namespace Framework.Services
{
    public class ServiceContainer : IServiceContainer, IDisposable
    {
        private readonly Dictionary<string, IFactory> _services = new Dictionary<string, IFactory>();

        public virtual object Resolve(Type type)
        {
            return Resolve<object>(type.Name);
        }

        public virtual T Resolve<T>()
        {
            return Resolve<T>(typeof(T).Name);
        }

        public virtual object Resolve(string name)
        {
            return Resolve<object>(name);
        }

        public virtual T Resolve<T>(string name)
        {
            if (_services.TryGetValue(name, out var factory))
                return (T) factory.Create();
            return default;
        }

        public virtual void Register<T>(Func<T> factory)
        {
            Register(typeof(T).Name, factory);
        }

        public virtual void Register(Type type, object target)
        {
            Register<object>(type.Name, target);
        }

        public virtual void Register(string name, object target)
        {
            Register<object>(name, target);
        }

        public virtual void Register<T>(T target)
        {
            Register(typeof(T).Name, target);
        }

        public virtual void Register<T>(string name, Func<T> factory)
        {
            if (_services.ContainsKey(name))
                throw new DuplicateRegisterServiceException($"Duplicate key {name}");

            _services.Add(name, new GenericFactory<T>(factory));
        }

        public virtual void Register<T>(string name, T target)
        {
            if (_services.ContainsKey(name))
                throw new DuplicateRegisterServiceException($"Duplicate key {name}");

            _services.Add(name, new SingleInstanceFactory(target));
        }

        public virtual void Unregister(Type type)
        {
            Unregister(type.Name);
        }

        public virtual void Unregister<T>()
        {
            Unregister(typeof(T).Name);
        }

        public virtual void Unregister(string name)
        {
            if (_services.TryGetValue(name, out var factory))
                factory.Dispose();

            _services.Remove(name);
        }

        #region IDisposable Support

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                foreach (var kv in _services)
                    kv.Value.Dispose();

                _services.Clear();
            }

            _disposed = true;
        }

        ~ServiceContainer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        internal interface IFactory : IDisposable
        {
            object Create();
        }

        internal class GenericFactory<T> : IFactory
        {
            private Func<T> _func;

            public GenericFactory(Func<T> func)
            {
                _func = func;
            }

            public virtual object Create()
            {
                return _func();
            }

            public void Dispose()
            {
            }
        }

        internal class SingleInstanceFactory : IFactory
        {
            private object _target;

            public SingleInstanceFactory(object target)
            {
                _target = target;
            }

            public virtual object Create()
            {
                return _target;
            }

            #region IDisposable Support

            private bool _disposed;

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed) return;
                if (disposing)
                {
                    var disposable = _target as IDisposable;
                    disposable?.Dispose();
                    _target = null;
                }

                _disposed = true;
            }

            ~SingleInstanceFactory()
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
}
