using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour
    {
        private List<View> subViews;
        private CanvasGroup canvasGroup;
        public ViewModel ViewModel { get; private set; }
        public Transform Transform { get; private set; }
        public abstract UILevel UILevel { get; }

        internal void SetVM(ViewModel vm)
        {
            if (ViewModel == vm) return;
            ViewModel = vm;
            if (ViewModel != null)
                OnVmChange();
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            subViews = new List<View>();
            Transform = transform;
        }

        #region 界面显示隐藏的调用和回调方法

        public void Show()
        {
            SetCanvas(true);
            OnShow();
            subViews.ForEach((subView) => subView.OnShow());
        }

        public void Hide()
        {
            SetCanvas(false);
            OnHide();
            subViews.ForEach((subView) => subView.OnHide());
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        private void SetCanvas(bool visible)
        {
            canvasGroup.interactable = visible;
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.blocksRaycasts = visible;
        }

        #endregion

        protected void AddSubView(View view)
        {
            if (subViews.Contains(view)) return;
            subViews.Add(view);
        }

        protected abstract void OnVmChange();
    }
}