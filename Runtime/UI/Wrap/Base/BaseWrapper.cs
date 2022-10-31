using System;
using UnityEngine;

namespace Framework
{
    public class BaseWrapper<T> : IFieldChangeCb<bool> where T : class
    {
        protected T Component;
        protected object Container;

        protected BaseWrapper(T component, object container)
        {
            this.Component = component;
            this.Container = container;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            if (Component is Component component) return (value) => component.gameObject.SetActive(value);
            if (Component is GameObject go) return value => go.SetActive(value);
            return null;
        }
    }
}