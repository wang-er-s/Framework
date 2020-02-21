using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour, IView
    {
        private List<View> _subViews;
        private CanvasGroup _canvasGroup;
        private ViewModel _viewModel;
        public ViewModel ViewModel
        {
            get => _viewModel;
            private set
            {
                if (_viewModel == value)
                {
                    return;
                }
                _viewModel = value;
                if (_viewModel != null)
                    OnVmChange();
            }
        }
        public Transform Transform { get; private set; }
        public UIManager UiManager { get; private set; }
        public abstract UILevel UILevel { get; }
        public virtual bool SingleInstance { get; }

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
            _canvasGroup = GetComponent<CanvasGroup>();
            _subViews = new List<View>();
            SetCanvas(false);
            Transform = transform;
        }

        #region 界面显示隐藏的调用和回调方法

        public void Show()
        {
            SetCanvas(true);
            OnShow();
            _subViews.ForEach((subView) => subView.OnShow());
        }

        public void Hide()
        {
            SetCanvas(false);
            OnHide();
            _subViews.ForEach((subView) => subView.OnHide());
        }
        
        protected virtual void OnShow(){}
        
        protected virtual void OnHide(){}

        private void SetCanvas(bool visible)
        {
            _canvasGroup.interactable = visible;
            _canvasGroup.alpha = visible ? 1 : 0;
            _canvasGroup.blocksRaycasts = !visible;
        }

        #endregion

        protected void AddSubView(View view)
        {
            if (_subViews.Contains(view)) return;
            _subViews.Add(view);
        }

        protected abstract void OnVmChange();
        
    }

}