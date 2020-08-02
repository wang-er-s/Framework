using Framework.UI.Core.Bind;

namespace Framework.UI.Core
{
    public abstract class ViewModel
    {
        public BindableProperty<ViewState> IsShow { get; } = new BindableProperty<ViewState>(ViewState.Hide);

        protected ViewState isShow
        {
            get => IsShow;
            set => ((BindableProperty<ViewState>) IsShow).Value = value;
        }

        public abstract string ViewPath { get; }

        public virtual void ShowView()
        {
            isShow = ViewState.Show;
        }

        public virtual void HideView()
        {
            isShow = ViewState.Hide;
        }

        public virtual void DestroyView()
        {
            isShow = ViewState.Destroy;
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
    }

    public enum ViewState
    {
        Show,
        Hide,
        Destroy
    }
}