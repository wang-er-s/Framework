using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace SF.UI.Core
{
    public abstract class ViewModelBase 
    {
        public ViewModelBase ParentViewModel { get; set; }
        public bool IsShow { get; private set; }
        
        public abstract void OnCreate();


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

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (value.Equals(field))
                return false;
            field = value;
            OnPropertyChanged(name, field);
            return true;
        }

        protected virtual void OnPropertyChanged<T>(string name, T value)
        {
            //存储Name对应的BindProperty，来设置OnValueChanged的回调
            Debug.Log($"{name}的值为{value}");
        }
    }
}