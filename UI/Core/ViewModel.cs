using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework.UI.Core
{
    public abstract class ViewModel 
    {
        
        public bool IsShow { get; private set; }

        public virtual void OnShow()
        {
            IsShow = true;
        }

        public virtual void OnHide()
        {
            IsShow = false;
        }

        public virtual void OnDestroy()
        {
            
        }

        /// <summary>
        /// 当不同的view换绑vm的时候的reset方法
        /// </summary>
        public virtual void Reset()
        {
            
        }

    }
}