using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace AD.UI.Core
{
    public abstract class ViewModel
    {
        private Dictionary<string, object> binds;
        public ViewModel ParentViewModel { get; set; }
        public bool IsShow { get; private set; }

        protected ViewModel()
        {
            OnCreate();
            binds = new Dictionary<string, object>();
        }

        public abstract void OnCreate ();


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

    }
}