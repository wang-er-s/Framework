using System;
using UnityEngine;

namespace Framework
{
    public class BaseWrapper<T> : IWrapper, IFieldChangeCb<bool> where T : class
    {
        protected T Component;
        protected object Container;

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            if (Component is Component component) return (value) => component.gameObject.SetActive(value);
            if (Component is GameObject go) return value => go.SetActive(value);
            return null;
        }

        public virtual void Clear()
        {
            Component = null;
            Container = null;
        }

        public virtual void Init(object component, object container)
        {
            Component = component as T;
            Container = container;
        }
    }

    public interface IWrapper : IReference
    {
        void Init(object component, object container);
    }
}