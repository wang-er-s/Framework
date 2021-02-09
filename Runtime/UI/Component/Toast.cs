using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Contexts;
using Framework.Execution;
using Framework.UI.Core;
using UnityEngine;

namespace Framework.Runtime.UI.Component
{
    public class Toast
    {

        private static UIManager GetUIViewLocator()
        {
            return UIManager.Ins;
        }

        public static async Task<Toast> Show(string text, float duration = 3f, Action callback = null)
        {
            UIManager locator = GetUIViewLocator();
            ToastView view = await locator.OpenAsync<ToastView>();
            if (view == null)
                throw new FileNotFoundException("Not found the \"ToastView\".");

            Toast toast = new Toast(view, text, duration);
            toast.Show();
            return toast;
        }

        private readonly float duration;
        private readonly string text;
        private readonly ToastView view;
        private readonly Action callback;
        private readonly bool autoHide;
        
        protected Toast(ToastView view, string text, float duration = 3, Action callback = null, bool autoHide = true)
        {
            this.view = view;
            this.text = text;
            this.duration = duration;
            this.callback = callback;
            this.autoHide = autoHide;
        }

        public float Duration => this.duration;

        public string Text => this.text;

        public ToastView View => this.view;

        public void Cancel()
        {
            if (this.view == null)
                return;
            view.Destroy();
            this.DoCallback();
        }

        public void Show()
        {
            this.view.Show();
            this.view.Text.text = this.text;
            if (autoHide)
                Executors.RunOnCoroutineNoReturn(DelayDismiss(duration));
        }

        protected IEnumerator DelayDismiss(float duration)
        {
            yield return new WaitForSeconds(duration);
            this.Cancel();
        }

        protected void DoCallback()
        {
            try
            {
                if (this.callback != null)
                    this.callback();
            }
            catch (Exception)
            {
            }
        }
    }
}