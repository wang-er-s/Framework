using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.SF.UI.Wrap
{
    public abstract class BaseWrapper<T>  where T: Component
    {
        protected T component;
    }
}
