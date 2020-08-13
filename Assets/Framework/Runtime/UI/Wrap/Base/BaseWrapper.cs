using System;
using UnityEngine;

namespace Framework.UI.Wrap.Base
{
    public abstract class BaseWrapper<T> : IFieldChangeCb<bool> where T : class
    {
        protected T View;

        protected BaseWrapper(T view)
        {
            this.View = view;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            if (View is Component component) return (value) => component.gameObject.SetActive(value);
            return null;
        }
    }
}