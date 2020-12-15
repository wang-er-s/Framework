using System.Collections.Generic;
using Framework.UI.Core.Bind;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.UI.Core
{
    public enum UILevel
    {
        None,
        Bg,
        Common,
        Pop,
        Toast,
        Guide,
    }
    
    public abstract class View
    {
        private List<View> _subViews;
        private CanvasGroup _canvasGroup;

        public GameObject Go { get; private set; }
        public ViewModel ViewModel { get; private set; }
        protected readonly UIBindFactory Binding;

        public View()
        {
            _subViews = new List<View>();
            Binding = new UIBindFactory();
            _cacheComponent = new Dictionary<string, object>();
        }

        private Dictionary<string, object> _cacheComponent;
        
        protected T Find<T>(string name) where T  : Component
        {
            if (_cacheComponent.TryGetValue(name, out var com))
            {
                return com as T;
            }

            var tCom = Go.transform.Find(name).GetComponent<T>();
            _cacheComponent[name] = tCom;
            return tCom;
        }

        public void SetGameObject(GameObject obj)
        {
            Go = obj;
            _canvasGroup = Go.GetOrAddComponent<CanvasGroup>();
        }
        
        public void SetVm(ViewModel vm)
        {
            if (vm == null || ViewModel == vm) return;
            ViewModel = vm;
            Binding.Reset();
            if (ViewModel != null)
            {
                OnVmChange();
            }
        }

        public bool Visible => Go != null && _canvasGroup.alpha == 1;

        #region 界面显示隐藏的调用和回调方法

        public void Show()
        {
            SetCanvas(true);
            OnShow();
            _subViews.ForEach(subView => subView.OnShow());
        }

        public void Hide()
        {
            SetCanvas(false);
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
            Object.Destroy(Go.gameObject);
        }

        private void SetCanvas(bool visible)
        {
            _canvasGroup.interactable = visible;
            _canvasGroup.alpha = visible ? 1 : 0;
            _canvasGroup.blocksRaycasts = visible;
        }

        #endregion

        public void AddSubView(View view)
        {
            if (_subViews.Contains(view)) return;
            _subViews.Add(view);
        }

        protected abstract void OnVmChange();
        public abstract UILevel UILevel { get; }
        public abstract string Path { get; }
        public virtual bool IsSingle { get; } = true;
    }
}