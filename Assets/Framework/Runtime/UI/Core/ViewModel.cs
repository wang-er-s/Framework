using Framework.UI.Core.Bind;

namespace Framework.UI.Core
{
    public abstract class ViewModel
    {
        public ObservableProperty<ViewState> IsShow { get; } = new ObservableProperty<ViewState>(ViewState.Hide);
        

        public abstract string ViewPath { get; }

        public virtual void ShowView()
        {
            IsShow.Value = ViewState.Show;
        }

        public virtual void HideView()
        {
            IsShow.Value = ViewState.Hide;
        }

        public virtual void DestroyView()
        {
            IsShow.Value = ViewState.Destroy;
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