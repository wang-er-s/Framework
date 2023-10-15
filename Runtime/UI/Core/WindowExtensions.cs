using System;

namespace Framework
{
    public static class WindowExtensions
    {
        /// <summary>
        /// wait until the window is dismissed
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static IAsyncResult WaitDismissed(this Window window)
        {
            AsyncResult result = AsyncResult.Create();
            Action<Window> handler = null;
            handler = (sender) =>
            {
                window.OnDismissed -= handler;
                result.SetResult();
            };
            window.OnDismissed += handler;
            return result;
        }

        /// <summary>
        /// wait until the view visibility changed.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static IAsyncResult WaitShow(this View view)
        {
            AsyncResult result = AsyncResult.Create();
            Action<View> handler = null;
            handler = (sender) =>
            {
                view.VisibilityChanged -= handler;
                result.SetResult(null);
            };
            view.VisibilityChanged += handler;
            return result;
        } 
    }
}