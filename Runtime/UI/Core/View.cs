using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Framework.Assets;
using Framework.Asynchronous;
using Framework.Execution;
using Framework.UI.Core.Bind;
using Tool;
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
    
    public abstract class View
    {
        private List<View> _subViews;
        private CanvasGroup _canvasGroup;
        public GameObject Go { get; private set; }
        public ViewModel ViewModel { get; private set; }
        protected readonly UIBindFactory Binding;
        protected IRes Res;

        public View()
        {
            _subViews = new List<View>();
            Binding = new UIBindFactory();
            Res = Assets.Res.Create();
        }

        public void SetGameObject(GameObject obj)
        {
            Go = obj;
            _canvasGroup = Go.GetOrAddComponent<CanvasGroup>();
            SetComponent();
            Start();
            GameLoop.Ins.OnUpdate += Update;
            GameLoop.Ins.OnLateUpdate += LateUpdate;
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
            if (ViewModel != null)
            {
                OnVmChange();
            }
        }

        #region 界面显示隐藏的调用和回调方法

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
        }

        protected virtual void LateUpdate()
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

        public virtual void Destroy()
        {
            Hide();
            Res.Release();
            GameLoop.Ins.OnUpdate -= Update;
            GameLoop.Ins.OnLateUpdate -= LateUpdate;
            _subViews.ForEach(subView => subView.Destroy());
            Object.Destroy(Go.gameObject);
            ViewModel?.OnViewDestroy();
        }

        public void Visible(bool visible)
        {
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


        public void AddSubView(View view)
        {
            view.Show();
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

        public void Close()
        {
            UIManager.Ins.Close(this.GetCLRType());
        }

        protected abstract void OnVmChange();
        public virtual UILevel UILevel { get; } = UILevel.Common;
        public virtual bool IsSingle { get; } = true;
    }
}