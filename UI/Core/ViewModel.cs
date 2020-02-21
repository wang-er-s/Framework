using System.Collections.Generic;

namespace Framework.UI.Core
{
    public abstract class ViewModel
    {
        public BindableProperty<ViewState> IsShow { get; } = new BindableProperty<ViewState>(ViewState.Hide);

        protected ViewState _isShow
        {
            get => IsShow;
            set => ((IBindableProperty<ViewState>) IsShow).Value = value;
        } 

        public abstract string ViewPath { get;}

        public virtual void ShowView()
        {
            _isShow = ViewState.Show;
        }

        public virtual void HideView()
        {
            _isShow = ViewState.Hide;
        }

        public virtual void DestroyView()
        {
            _isShow = ViewState.Destroy;
        }
        
        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
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

    public enum ViewState
    {
        Show,
        Hide,
        Destroy,
    }
}