namespace SF.UI.Core
{
    public abstract class ViewModelBase
    {
        private bool _isInitialized;
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

        public virtual void OnDestory()
        {
            
        }
    }
}