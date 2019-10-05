using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace SF.UI.Core
{
    public abstract class ViewModelBase
    {
        private Dictionary<string, object> binds;
        public ViewModelBase ParentViewModel { get; set; }
        public bool IsShow { get; private set; }

        protected ViewModelBase()
        {
            OnCreate();
            binds = new Dictionary<string, object>();
        }

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
            if (Equals(field,value))
                return false;
            field = value;
            OnPropertyChanged(name, field);
            return true;
        }

        protected void OnPropertyChanged<T>(string name, T value)
        {
            BindableProperty<T> property = GetBindingAbleProperty<T>(name);
            property.Value = value;
        }

        public BindableProperty<T> GetBindingAbleProperty<T>(string name)
        {
            BindableProperty<T> property;

            if (binds.TryGetValue(name,out object obj))
            {
                property = (BindableProperty<T>)obj;
            }
            else
            {
                property = CreateBindableProperty<T>(name);
            }
            return property;
        }

        private BindableProperty<T> CreateBindableProperty<T>(string name)
        {
            var property = new BindableProperty<T>();
            binds.Add(name, property);
            var propertyInfo = GetType().GetProperty(name, typeof(T));
            property.AddChangeEvent((value) => propertyInfo.SetValue(this, value));
            return property;
        }

    }
}