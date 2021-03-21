using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework.Assets;
using Framework.Asynchronous;
using Framework.Execution;
using Sirenix.Utilities;
using Tool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Core
{
    public class UIManager : ManagerBase<UIManager, UIAttribute, Type>
    {
        private IRes _res;
       
        public Canvas Canvas { get; private set; }

        public override void Init()
        {
            Canvas = GameObject.Find("UIRoot").GetComponent<Canvas>();
            Object.DontDestroyOnLoad(Canvas);
            foreach (UILevel level in (UILevel[]) Enum.GetValues(typeof(UILevel)))
            {
                _sortViews[level] = new List<View>();
            }

            _res = Res.Create();
            GameLoop.Ins.OnUpdate += Update;
        }

        public override void CheckType(Type type)
        {
            var attr = type.GetCustomAttribute<UIAttribute>();
            
            if (attr != null)
            {
                ClassDataMap[type] = new ClassData {Attribute = attr, Type = type};
            }
        }

        private const int ViewDestroyTime = 5;
        private Dictionary<View,DateTime> _waitDestroyViews = new Dictionary<View, DateTime>();
        private Dictionary<Type, View> _openedViews = new Dictionary<Type, View>();
        private Dictionary<UILevel, List<View>> _sortViews = new Dictionary<UILevel, List<View>>();

        public IProgressResult<float, View> OpenAsync<T>(ViewModel viewModel = null) where T : View
        {
            ProgressResult<float, View> result = new ProgressResult<float, View>();
            InternalOpen(typeof(T), result, viewModel);
            return result;
        }

        public IProgressResult<float, View> OpenAsync(Type type, ViewModel viewModel = null)
        {
            ProgressResult<float, View> result = new ProgressResult<float, View>();
            InternalOpen(type, result, viewModel);
            return result;
        }

        private void InternalOpen<T>(Type type, ProgressResult<float, T> promise, ViewModel viewModel) where T : View
        {
            promise.Callbackable().OnCallback(progressResult =>
            {
                var _view = progressResult.Result;
                Sort(_view);
                _view.Show();
                if (_view.IsSingle)
                    _openedViews[type] = _view;
            });
            if (_openedViews.TryGetValue(type, out var view))
            {
                promise.UpdateProgress(1);
                promise.SetResult(view);
            }
            else
            {
                Executors.RunOnCoroutineNoReturn(CreateView(promise, type, viewModel));
            }
        }

        public IProgressResult<float, T> CreateView<T>(ViewModel vm) where T : View
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            Executors.RunOnCoroutineNoReturn(CreateView(progressResult, typeof(T), vm));
            return progressResult;
        }
        
        public IProgressResult<float, View> CreateView(Type type, ViewModel vm)
        {
            ProgressResult<float, View> progressResult = new ProgressResult<float, View>();
            Executors.RunOnCoroutineNoReturn(CreateView(progressResult, type, vm));
            return progressResult;
        }

        private IEnumerator CreateView<T>(IProgressPromise<float, T> promise,Type type, ViewModel viewModel)
            where T : View
        {
            if (_openedViews.TryGetValue(type, out var view))
            {
                promise.UpdateProgress(1f);
                promise.SetResult(view);
                yield break;
            }
            view = ReflectionHelper.CreateInstance<View>(type);
            var path = (GetClassData(type).Attribute as UIAttribute).Path;
            var request = _res.LoadAssetAsync<GameObject>(path);
            while (!request.IsDone)
            {
                promise.UpdateProgress(request.Progress);
                yield return null;
            }
            GameObject viewTemplateGo = request.Result;
            if (viewTemplateGo == null)
            {
                promise.UpdateProgress(1f);
                Log.Error($"Not found the window path = \"{path}\".");
                promise.SetException(new FileNotFoundException(path));
                yield break;
            }
            GameObject go = Object.Instantiate(viewTemplateGo);
            go.name = viewTemplateGo.name;
            view.SetGameObject(go);
            view.SetVm(viewModel);
            view.Visible(false);
            promise.UpdateProgress(1f);
            promise.SetResult(view);
        }

        [Obsolete("use LoadViewAsync replace", true)]
        public View Open(string path, ViewModel viewModel = null)
        {
            var view = CreateView(path);
            view.SetVm(viewModel);
            view.Show();
            return view;
        }

        [Obsolete("", true)]
        private View CreateView(string panelName)
        {
            var loadGo = _res.LoadAsset<GameObject>(panelName);
            var view = loadGo.GetComponent<View>();
            Sort(view);
            return view;
        }

        public void Close<T>()
        {
            Close(typeof(T));
        }

        public T Get<T>() where T : View
        {
            var view = Get(typeof(T));
            return view as T;
        }

        public View Get(Type type)
        {
            if (_openedViews.TryGetValue(type, out var view))
                return view;
            return null;
        }

        public void Close(Type type)
        {
            if (!_openedViews.TryGetValue(type, out var view))
                return;
            _openedViews.Remove(type);
            //view.Hide();
            view.Destroy();
            //_waitDestroyViews[view] = DateTime.Now.AddSeconds(ViewDestroyTime);
        }

        private void Sort(View view)
        {
            var viewTransform = view.Go.transform;
            Transform lastTrans = null;
            int index = 0;
            foreach (UILevel level in (UILevel[]) Enum.GetValues(typeof(UILevel)))
            {
                if (level > view.UILevel)
                    break;
                var views = _sortViews[level];
                for (int i = 0; i < views.Count; i++)
                {
                    if (views[i].Go != null)
                    {
                        lastTrans = views[i].Go.transform;
                        continue;
                    }
                    views.RemoveAt(i);
                    i--;
                }
            }
            index = lastTrans == null ? Canvas.transform.childCount : lastTrans.GetSiblingIndex() + 1;
            viewTransform.SetParent(Canvas.transform, false);
            viewTransform.SetSiblingIndex(index);
            _sortViews[view.UILevel].Add(view);
        }

        private List<View> _needDestroyViews = new List<View>();
        private void Update()
        {
            if(_waitDestroyViews.Count <= 0) return;
            var nowTime = DateTime.Now;
            foreach (var waitDestroyView in _waitDestroyViews)
            {
                if (waitDestroyView.Value <= nowTime)
                {
                    waitDestroyView.Key.Destroy();
                    _needDestroyViews.Add(waitDestroyView.Key);
                }
            }
            foreach (var view in _needDestroyViews)
            {
                _waitDestroyViews.Remove(view);
            }
            _needDestroyViews.Clear();
        }
    }
}