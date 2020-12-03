using System.Collections.Generic;
using Framework.UI.Core.Bind;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.UI.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour
    {
        private List<View> _subViews = new List<View>();
        private CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }
        private CanvasGroup _canvasGroup;
        public ViewModel ViewModel { get; private set; }
        protected UIBindFactory Binding = new UIBindFactory();
        public abstract UILevel UILevel { get; }

        public void SetVm(ViewModel vm)
        {
            if (ViewModel == vm) return;
            ViewModel = vm;
            Binding.Reset();
            if (ViewModel != null)
            {
                OnVmChange();
            }
        }

        public IAnimation EnterAnimation;

        public IAnimation ExitAnimation;

        public bool Visible => CanvasGroup.alpha == 1;

        #region 界面显示隐藏的调用和回调方法

        public void Show(bool ignoreAnimation = false)
        {
            SetCanvas(true);
            if (!ignoreAnimation)
                EnterAnimation?.Play();
            OnShow();
            _subViews.ForEach((subView) => subView.OnShow());
        }

        public void Hide(bool ignoreAnimation = false)
        {
            SetCanvas(false);
            if (!ignoreAnimation)
                ExitAnimation?.Play();
            OnHide();
            _subViews.ForEach((subView) => subView.OnHide());
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        public void Destroy()
        {
            if (!Addressables.ReleaseInstance(gameObject))
            {
                Destroy(gameObject);
            }
        }

        private void SetCanvas(bool visible)
        {
            CanvasGroup.interactable = visible;
            CanvasGroup.alpha = visible ? 1 : 0;
            CanvasGroup.blocksRaycasts = visible;
        }

        #endregion

        public void AddSubView(View view)
        {
            if (_subViews.Contains(view)) return;
            _subViews.Add(view);
        }

        protected abstract void OnVmChange();
        
        public abstract string ViewPath { get; }
    }
}