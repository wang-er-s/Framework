using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour, IView
    {

        private List<View> subViews;
        private CanvasGroup canvasGroup;
        private ViewModel viewModel;
        public ViewModel ViewModel
        {
            get => viewModel;
            private set
            {
                if (viewModel == value)
                {
                    return;
                }
                viewModel = value;
                if (viewModel != null)
                    OnVmChange();
            }
        }
        public Transform Transform { get; private set; }
        public UIManager UiManager { get; private set; }
        public abstract UILevel UILevel { get; }

        public void SetVM(ViewModel vm)
        {
            ViewModel = vm;
        }

        public void SetUIManager(UIManager uiManager)
        {
            UiManager = uiManager;
        }

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            subViews = new List<View>();
            SetCanvas(false);
            Transform = transform;
        }

        #region 界面显示隐藏的调用和回调方法

        public void Show()
        {
            SetCanvas(true);
            ViewModel?.OnShow();
            OnShow();
            subViews.ForEach((subView) => subView.OnShow());
        }

        public void Hide()
        {
            SetCanvas(false);
            ViewModel?.OnHide();
            OnHide();
            subViews.ForEach((subView) => subView.OnHide());
        }
        
        protected virtual void OnShow(){}

        protected virtual void OnDestroy()
        {
            ViewModel?.OnDestroy();
        }

        protected virtual void OnHide(){}

        private void SetCanvas(bool visible)
        {
            canvasGroup.interactable = visible;
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.blocksRaycasts = !visible;
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