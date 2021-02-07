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
    public class UIManager : ManagerBase<UIManager, UIAttribute>
    {
        private IRes _res;
       
        public Canvas Canvas { get; private set; }

        public override void Init()
        {
            if (Canvas == null) Canvas = CreateCanvas();
            Object.DontDestroyOnLoad(Canvas);
            foreach (UILevel level in (UILevel[]) Enum.GetValues(typeof(UILevel)))
            {
                _sortViews[level] = new List<View>();
            }

            _res = Res.Create();
            GameLoop.Ins.OnUpdate += Update;
        }

        private const int ViewDestroyTime = 5;
        private Dictionary<View,DateTime> _waitDestroyViews = new Dictionary<View, DateTime>();
        private Dictionary<int, View> _openedViews = new Dictionary<int, View>();
        private Dictionary<UILevel, List<View>> _sortViews = new Dictionary<UILevel, List<View>>();

        public IProgressResult<float, T> OpenAsync<T>(ViewModel viewModel = null) where T : View
        {
            ProgressResult<float, T> result = new ProgressResult<float, T>();
            var attr = typeof(T).GetCustomAttribute<UIAttribute>();
            InternalOpen(attr.IntTag, result, viewModel);
            return result;
        }

        public IProgressResult<float, View> OpenAsync(Enum uiIndex, ViewModel viewModel = null)
        {
            ProgressResult<float, View> result = new ProgressResult<float, View>();
            InternalOpen(uiIndex.GetHashCode(), result, viewModel);
            return result;
        }

        private void InternalOpen<T>(int uiIndex, ProgressResult<float, T> promise, ViewModel viewModel) where T : View
        {
            promise.Callbackable().OnCallback(progressResult =>
            {
                var _view = progressResult.Result;
                Sort(_view);
                _view.Show();
                if (_view.IsSingle)
                    _openedViews[uiIndex] = _view;
            });
            if (_openedViews.TryGetValue(uiIndex, out var view))
            {
                promise.UpdateProgress(1);
                promise.SetResult(view);
            }
            else
            {
                Executors.RunOnCoroutineNoReturn(CreateView(promise, uiIndex, viewModel));
            }
        }

        public IEnumerator CreateView<T>(IProgressPromise<float, T> promise, Enum index, ViewModel viewModel)
            where T : View
        {
            return CreateView(promise, index.GetHashCode(), viewModel);
        }

        public IEnumerator CreateView<T>(IProgressPromise<float, T> promise, int index, ViewModel viewModel)
            where T : View
        {
            var classData = GetClassData(index);
            var view = ReflectionHelper.CreateInstance(classData.Type) as T;
            var path = (classData.Attribute as UIAttribute).Path;
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
        
        public T Get<T>() where T : View
        {
            var view = Get(typeof(T).GetCustomAttribute<UIAttribute>().IntTag);
            return view as T;
        }

        public View Get(Enum index)
        {
            return Get(index.GetHashCode());
        }
        
        private View Get(int index)
        {
            if (_openedViews.TryGetValue(index, out var view))
                return view;
            return null;
        }
        
        public void Close<T>() where T : View
        {
            Close(typeof(T).GetCustomAttribute<UIAttribute>().IntTag);
        }

        public void Close(Enum index)
        {
            Close(index.GetHashCode());
        }
        
        private void Close(int index)
        {
            if (!_openedViews.TryGetValue(index, out var view))
                return;
            _openedViews.Remove(index);
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
            index = lastTrans == null ? 0 : lastTrans.GetSiblingIndex() + 1;
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
        
        private static Canvas CreateCanvas()
        {
            var obj = Resources.Load<Canvas>("Canvas");
            var canvas = Object.Instantiate(obj);
            return canvas;
        }
    }
}