using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Nine.UI.Wrap
{
    public abstract class BaseWrapper<T> : IBindData<bool> where T : Component
    {
        protected T component;

        protected BaseWrapper(T _component)
        {
            component = _component;
        }

        Action<bool> IBindData<bool>.GetBindFieldFunc()
        {
            return(value) => component.gameObject.SetActive(value);
        }
    }

}
