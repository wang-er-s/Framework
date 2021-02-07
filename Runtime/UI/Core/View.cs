using System.Collections.Generic;
using System.Reflection;
using Framework.Assets;
using Framework.Asynchronous;
using Framework.Execution;
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
        FullScreen,
    }
    
    public abstract class View
    {
        private List<View> _subViews;
        private CanvasGroup _canvasGroup;
        private IRes _res;
        public GameObject Go { get; private set; }
        public ViewModel ViewModel { get; private set; }
        protected readonly UIBindFactory Binding;

        public View()
        {
            _subViews = new List<View>();
            Binding = new UIBindFactory();
            _res = Res.Create();
        }

        public void SetGameObject(GameObject obj)
        {
            Go = obj;
            _canvasGroup = Go.GetOrAddComponent<CanvasGroup>();
            SetComponent();
            Start();
            GameLoop.Ins.OnUpdate += Update;
        }

        private void SetComponent()
        {
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields)
            {
                var path = fieldInfo.GetCustomAttribute<TransformPath>();
                if (path != null)
                {
                    fieldInfo.SetValue(this, Go.transform.Find(path.Path).GetComponent(fieldInfo.FieldType));
                }
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

        #region 界面显示隐藏的调用和回调方法

        protected virtual async void Start()
        {
            
        }

        protected virtual void Update()
        {
        }

        public void Show()
        {
            Visible(true);
            OnShow();
        }

        public void Hide()
        {
            Visible(false);
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

        public void Visible(bool visible)
        {
            _canvasGroup.interactable = visible;
            _canvasGroup.alpha = visible ? 1 : 0;
            _canvasGroup.blocksRaycasts = visible;
        }

        #endregion

        public IProgressResult<float, T> AddSubView<T>(ViewModel viewModel = null) where T : View
        {
            foreach (var subView in _subViews)
            {
                if(subView is T)
                    return null;
            }
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            Executors.RunOnCoroutineNoReturn(UIManager.Ins.CreateView(progressResult, typeof(T), viewModel));
            progressResult.Callbackable().OnCallback((result => _subViews.Add(result.Result)));
            return progressResult;
        }

        public void AddSubView(View view)
        {
            if(_subViews.Contains(view))
                return;
            _subViews.Add(view);
        }

        public T GetSubView<T>() where T : View
        {
            foreach (var subView in _subViews)
            {
                if (subView is T view)
                    return view;
            }
            return null;
        }

        public void Close()
        {
            UIManager.Ins.Close(GetType());
        }

        protected abstract void OnVmChange();
        public virtual UILevel UILevel { get; } = UILevel.Common;
        public abstract string Path { get; }
        public virtual bool IsSingle { get; } = true;
    }
}