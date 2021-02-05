using System.Collections.Generic;
using System.Reflection;
using Framework.UI.Core.Bind;
using Sirenix.Utilities;
using Tool;
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
        FullScreen,
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
        }
        
        public void SetGameObject(GameObject obj)
        {
            Go = obj;
            _canvasGroup = Go.GetOrAddComponent<CanvasGroup>();
            SetComponentsValue();
            Start();
            GameLoop.Ins.OnUpdate += Update;
        }

        private void SetComponentsValue()
        {
            var memberInfos = ReflectionHelper.GetCacheMember(GetType(), info => info.GetCustomAttribute<TransformPath>() != null, BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var mi in memberInfos.Values)
            {
                var path = mi.GetCustomAttribute<TransformPath>();
                var trans = Go.transform.Find(path.Path);
                mi.SetMemberValue(this, trans.GetComponent(mi.GetReturnType()));
            }
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

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
        }

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
            Hide();
            GameLoop.Ins.OnUpdate -= Update;
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

        protected void Close()
        {
            UIManager.Ins.Close(GetType());
        }

        protected abstract void OnVmChange();
        public virtual UILevel UILevel { get; } = UILevel.Common;
        public abstract string Path { get; }
        public virtual bool IsSingle { get; } = true;
    }
}