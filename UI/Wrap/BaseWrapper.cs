using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.UI.Wrap
{
    public abstract class BaseWrapper<T> : IFieldChangeCb<bool> where T : Component
    {
        protected T component;

        protected BaseWrapper(T _component)
        {
            component = _component;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            return(value) => component.gameObject.SetActive(value);
        }
    }

}
