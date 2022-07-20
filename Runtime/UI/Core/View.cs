using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Framework.Assets;
using Framework.Asynchronous;
using Framework.Execution;
using Framework.UI.Core.Bind;
using UnityEngine;
using Object = UnityEngine.Object;

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
    
    public abstract class View : ICustomRes
    {
        private List<View> _subViews;
        private CanvasGroup _canvasGroup;
        public GameObject Go { get; private set; }
        public ViewModel ViewModel { get; private set; }
        protected readonly UIBindFactory Binding;
        internal event Action OnDestroy;
        public IRes Res { get; }

        public View()
        {
            _subViews = new List<View>();
            Binding = new UIBindFactory(this);
            Res = Assets.Res.Create();
        }

        public void SetGameObject(GameObject obj)
        {
            Go = obj;
            _canvasGroup = Go.GetOrAddComponent<CanvasGroup>();
            SetComponent();
            Start();
        }

        private static Dictionary<Type, List<Tuple<FieldInfo, string>>> _type2TransPath =
            new Dictionary<Type, List<Tuple<FieldInfo, string>>>();

        private void SetComponent()
        {
            var type = this.GetCLRType();
            if (!_type2TransPath.TryGetValue(type, out var paths))
            {
                paths = new List<Tuple<FieldInfo, string>>();
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var fieldInfo in fields)
                {
                    var attributes = fieldInfo.GetCustomAttributes(typeof(TransformPath), false);
                    if (attributes.Length <= 0)
                    {
                        continue;
                    }
                    paths.Add(new Tuple<FieldInfo, string>(fieldInfo, ((TransformPath)attributes[0]).Path));
                }
                _type2TransPath[type] = paths;
            }
            foreach (var tuple in paths)
            {
                try
                {
                    if (tuple.Item1.FieldType == typeof(GameObject))
                    {
                        tuple.Item1.SetValue(this, Go.transform.Find(tuple.Item2).gameObject);
                    }
                    else
                    {
                        //热更IlRuntimeFieldInfo中的FieldType是ILRuntimeWrapperType类型，直接get unity会蹦掉
                        tuple.Item1.SetValue(this, Go.transform.Find(tuple.Item2).GetComponent(tuple.Item1.FieldType.Name));
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Debug.LogError(tuple.Item2 + " not found", Go);
                }
            }
        }
        
        public void SetVm(ViewModel vm)
        {
            if (vm == null || ViewModel == vm) return;
            ViewModel = vm;
            Binding.Reset();
            Res.Release();
            if (ViewModel != null)
            {
                OnVmChange();
            }
        }

        #region 界面显示隐藏的调用和回调方法

        protected virtual void Start()
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
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnClose()
        {
        }

        public void Visible(bool visible)
        {
            if (Go == null) return;
            _canvasGroup.interactable = visible;
            _canvasGroup.alpha = visible ? 1 : 0;
            _canvasGroup.blocksRaycasts = visible;
        }

        #endregion

        public IProgressResult<float, View> AddSubView<T>(ViewModel viewModel = null) where T : View
        {
            var progressResult = UIManager.Ins.CreateView(typeof(T), viewModel);
            progressResult.Callbackable().OnCallback((result => AddSubView(result.Result)));
            return progressResult;
        }
        
        public IProgressResult<float, View> AddSubView(Type type, ViewModel viewModel = null)
        {
            var progressResult = UIManager.Ins.CreateView(type, viewModel);
            progressResult.Callbackable().OnCallback((result => AddSubView(result.Result)));
            return progressResult;
        }

        protected void RemoveSubView(View view)
        {
            _subViews.TryRemove(view);
        }
        
        public void AddSubView(View view)
        {
            view.OnDestroy += () => RemoveSubView(view);
            view.Go.transform.SetParent(Go.transform, false);
            _subViews.Add(view);
        }

        protected T GetSubView<T>() where T : View
        {
            foreach (var subView in _subViews)
            {
                if (subView is T view)
                    return view;
            }
            return null;
        }

        protected void Close()
        {
            UIManager.Ins.Close(this);
        }

        public void Dispose()
        {
            Hide();
            Res.Release();
            OnClose();
            for (int i = 0; i < _subViews.Count; i++)
            {
                _subViews[i].OnClose();
            }
            Binding.Reset();
            OnDestroy?.Invoke();
            ViewModel?.OnViewDestroy();
            if (Go != null)
                Object.Destroy(Go.gameObject);
        }

        protected abstract void OnVmChange();
        public virtual UILevel UILevel { get; } = UILevel.Common;
        public virtual bool IsSingle { get; } = true;
        public IRes GetRes()
        {
            return Res;
        }
    }
}