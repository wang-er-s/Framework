namespace Framework
{
    public interface IUIAnimation
    {
        IAsyncResult OnShowAnim();
        IAsyncResult OnHideAnim();
    }
}