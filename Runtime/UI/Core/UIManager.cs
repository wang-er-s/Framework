using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework.Assets;
using Framework.Asynchronous;
using Framework.Execution;
using Tool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Core
{
    public class UIManager : Singleton<UIManager>
    {
        private IRes _res;
       
        public Canvas Canvas { get; }

        public UIManager()
        {
            if (Canvas == null) Canvas = CreateCanvas();
            Object.DontDestroyOnLoad(Canvas);
            foreach (UILevel level in (UILevel[]) Enum.GetValues(typeof(UILevel)))
            {
                _sortViews[level] = new List<View>();
            }

            _res = Res.Default;
            GameLoop.Ins.OnUpdate += Update;
        }

        private const int ViewDestroyTime = 5;
        private Dictionary<View,DateTime> _waitDestroyViews = new Dictionary<View, DateTime>();
        private Dictionary<string, View> _openedViews = new Dictionary<string, View>();
        private Dictionary<UILevel, List<View>> _sortViews = new Dictionary<UILevel, List<View>>();

        public IProgressResult<float, T> OpenAsync<T>(ViewModel viewModel = null) where T : View
        {
            ProgressResult<float, T> result = new ProgressResult<float, T>();
            InternalOpen(typeof(T), result, viewModel);
            return result;
        }

        public IProgressResult<float, View> OpenAsync(Type type, ViewModel viewModel = null)
        {
            ProgressResult<float, View> result = new ProgressResult<float, View>();
            InternalOpen(type, result, viewModel);
            return result;
        }

        private void InternalOpen<T>(Type type, IProgressPromise<float, T> promise, ViewModel viewModel) where T : View
        {
            if (_openedViews.TryGetValue(type.Name, out var view))
            {
                promise.UpdateProgress(1);
                promise.SetResult(view);
            }
            else
            {
                Executors.RunOnCoroutineNoReturn(CreateView(promise, type, viewModel));
            }
        }

        private IEnumerator CreateView<T>(IProgressPromise<float, T> promise, Type type, ViewModel viewModel)
            where T : View
        {
            if (_openedViews.TryGetValue(type.Name, out var view))
            {
                promise.UpdateProgress(1f);
                promise.SetResult(view);
                yield break;
            }
            view = ReflectionHelper.CreateInstance(type) as View;
            var path = view.Path;
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
            Sort(view);
            view.Show();
            promise.UpdateProgress(1f);
            promise.SetResult(view);
            if (view.IsSingle)
                _openedViews[type.Name] = view;
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
            var path = type.Name;
            if (_openedViews.TryGetValue(path, out var view))
                return view;
            return null;
        }

        public void Close(Type type)
        {
            var path = type.Name;
            if (!_openedViews.TryGetValue(path, out var view))
                return;
            _openedViews.Remove(path);
            view.Hide();
            _waitDestroyViews[view] = DateTime.Now.AddSeconds(ViewDestroyTime);
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
            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
                canvas = Object.Instantiate(Resources.Load<Canvas>("Canvas"));
            return canvas;
        }
    }
}