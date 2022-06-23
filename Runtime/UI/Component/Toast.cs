using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Execution;
using Framework.UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Framework.Runtime.UI.Component
{
    public class Toast
    {

        private static UIManager GetUIViewLocator()
        {
            return UIManager.Ins;
        }

        private static ToastContent toastContent;
        
        public static async Task<IAsyncResult<View>> Show(string text,int fontSize = 36, float duration = 3f,bool fly = true, Action callback = null)
        {
            if (toastContent == null)
            {
                toastContent = await UIManager.Ins.OpenAsync<ToastContent>() as ToastContent;
            }
            else if (toastContent.Go == null)
            {
                toastContent = await UIManager.Ins.OpenAsync<ToastContent>() as ToastContent;
            }
            var result = toastContent.AddSubView<ToastView>();
            result.Callbackable().OnCallback(progressResult =>
            {
                ToastView view = progressResult.Result as ToastView;
                if (view == null)
                    throw new FileNotFoundException("Not found the \"ToastView\".");
            
                Toast toast = new Toast(view, text, fontSize, duration, fly, callback);
                toast.Show();
            });
            return result;
        }

        private float duration;
        private string text;
        private ToastView view;
        private CanvasGroup canvasGroup;
        private Action callback;
        private bool autoHide;
        private int fontSize;
        private bool fly;

        private Toast(ToastView view, string text, int fontSize, float duration,bool fly,  Action callback = null, bool autoHide = true)
        {
            this.fontSize = fontSize;
            this.view = view;
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
            if (this.view == null)
                return;
            Object.Destroy(view.Go);
            this.DoCallback();
        }

        private void Show()
        {
            canvasGroup = view.Go.GetComponent<CanvasGroup>();
            this.view.Show();
            this.view.Text.text = this.text;
            view.Text.fontSize = fontSize;
            if (autoHide)
                Executors.RunOnCoroutineNoReturn(DelayDismiss(duration));
        }

        IEnumerator DelayDismiss(float duration)
        {
            yield return new WaitForSeconds(duration);
            float time = 0;
            float oldPosY = view.Go.transform.position.y;
            view.LayoutElement.ignoreLayout = true;
            if (fly)
            {
                while (time < flyTime)
                {
                    time += Time.deltaTime;
                    var delta = time / flyTime;
                    canvasGroup.alpha = 1 - delta;
                    view.Go.transform.PositionY(oldPosY + delta * flyDis);
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
    
    [UI("Assets/Framework/Runtime/UI/Res/ToastContent.prefab")]
    public class ToastContent : View
    {
        protected override void OnVmChange()
        {
        }

        public override UILevel UILevel { get; } = UILevel.Toast;
    }
    
    [UI("Assets/Framework/Runtime/UI/Res/Toast.prefab")]
    public class ToastView : View
    {
        [TransformPath("content/Text")]
        public Text Text;
        public override UILevel UILevel { get; } = UILevel.Toast;
        public override bool IsSingle { get; } = false;

        public LayoutElement LayoutElement { get; private set; }

        protected override void Start()
        {
            LayoutElement = Go.GetComponent<LayoutElement>();
        }

        protected override void OnVmChange()
        {
             
        }
    }
}