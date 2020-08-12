using System;
using UnityEngine;

namespace Framework.UI.Wrap.Base
{
    public abstract class BaseWrapper<T> : IFieldChangeCb<bool> where T : class
    {
        protected T view;

        protected BaseWrapper(T view)
        {
            this.view = view;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            if (view is Component component) return (value) => component.gameObject.SetActive(value);
            return null;
        }
    }
}