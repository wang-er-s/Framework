using System;

namespace Framework
{
    public abstract class ViewModel
    {
        public virtual void OnViewHide()
        {
        }
        
        public virtual void OnViewDestroy()
        {
        }
    }
}