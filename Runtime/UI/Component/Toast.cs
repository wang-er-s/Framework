using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Framework
{
    public class Toast
    {
        private static ToastContent toastContent;

        public static async IAsyncResult<ToastWindow> Show(string text, int fontSize = 36, float duration = 3f,
            bool fly = true, Action callback = null)
        {
            if (toastContent == null)
            {
                toastContent =
                    await Root.Instance.Scene.GetComponent<UIComponent>().CreateWindow<ToastContent>(null);
            }
            else if (toastContent.GameObject == null)
            {
                toastContent =
                    await Root.Instance.Scene.GetComponent<UIComponent>().CreateWindow<ToastContent>(null);
            }

            await toastContent.Show();

            var result = toastContent.AddSubView<ToastWindow>();
            await result;
            ToastWindow window = result.Result;
            if (window == null)
                throw new FileNotFoundException("Not found the \"ToastView\".");

            Toast toast = new Toast(window, text, fontSize, duration, fly, callback);
            toast.Show();
            return window;
        }

        private float duration;
        private string text;
        private ToastWindow window;
        private CanvasGroup canvasGroup;
        private Action callback;
        private bool autoHide;
        private int fontSize;
        private bool fly;

        private Toast(ToastWindow window, string text, int fontSize, float duration, bool fly, Action callback = null,
            bool autoHide = true)
        {
            this.fontSize = fontSize;
            this.window = window;
            this.text = text;
            this.fly = fly;
            this.duration = duration;
            this.callback = callback;
            this.autoHide = autoHide;
        }

        private const float flyTime = 1;
        private const float flyDis = 500;

        private void Cancel()
        {
            if (this.window == null)
                return;
            Object.Destroy(window.GameObject);
            this.DoCallback();
        }

        private void Show()
        {
            canvasGroup = window.GameObject.GetComponent<CanvasGroup>();
            this.window.Show();
            this.window.Text.text = this.text;
            window.Text.fontSize = fontSize;
            if (autoHide)
                Executors.RunOnCoroutineNoReturn(DelayDismiss(duration));
        }

        IEnumerator DelayDismiss(float duration)
        {
            yield return new WaitForSeconds(duration);
            float time = 0;
            float oldPosY = window.GameObject.transform.position.y;
            window.LayoutElement.ignoreLayout = true;
            if (fly)
            {
                while (time < flyTime)
                {
                    time += Time.deltaTime;
                    var delta = time / flyTime;
                    canvasGroup.alpha = 1 - delta;
                    window.GameObject.transform.SetPositionY(oldPosY + delta * flyDis);
                    yield return null;
                }
            }
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
    
    [UI("Assets/Framework/Runtime/UI/Res/ToastContent.prefab", false, false)]
    public class ToastContent : Window
    {
        protected override void OnVmChange()
        {
        }

        public override UILevel UILevel { get; } = UILevel.Toast;
    }
    
    [UI("Assets/Framework/Runtime/UI/Res/Toast.prefab", false, false)]
    public class ToastWindow : Window
    {
        public Text Text;
        public override UILevel UILevel { get; } = UILevel.Toast;

        public LayoutElement LayoutElement { get; private set; }

        protected override void OnCreated()
        {
            base.OnCreated();
            LayoutElement = GameObject.GetComponent<LayoutElement>();
        }

        protected override void OnVmChange()
        {
             
        }
    }
}