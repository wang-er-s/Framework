using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace SF.UI.Core
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
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

        protected bool Set<T>(ref T field, T value)
        {
            if (field.Equals(value))
                return false;
            field = value;
            OnPropertyChanged(nameof(field));
            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}