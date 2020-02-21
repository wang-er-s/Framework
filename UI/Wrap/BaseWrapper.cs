using System;
using UnityEngine;

namespace Framework.UI.Wrap
{
    public abstract class BaseWrapper<T> : IFieldChangeCb<bool> where T : class
    {
        protected T _view;

        protected BaseWrapper(T view)
        {
            _view = view;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            if (_view is Component component)
            {
                return(value) => component.gameObject.SetActive(value);
            }
            return null;
        }
    }

}
