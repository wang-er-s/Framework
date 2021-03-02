using Framework.UI.Core.Bind;

namespace Framework.UI.Core
{
    public abstract class ViewModel
    {
        public virtual void OnViewDestroy()
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