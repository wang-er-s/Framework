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
    public class UIManager : ManagerBase<UIManager, UIAttribute, Type>
    {
        private IRes _res;
       
        public Canvas Canvas { get; private set; }

        public override void Init()
        {
            Canvas = GameObject.Find("UIRoot").GetComponent<Canvas>();
            Object.DontDestroyOnLoad(Canvas);
            _res = Res.Create();
            GameLoop.Ins.OnUpdate += Update;
        }

        public override void CheckType(Type type)
        {
            
            var attrs = type.GetCustomAttributes(typeof(UIAttribute), false);
            if (attrs.Length <= 0) return;
            var attr = (UIAttribute)attrs[0];
            if (attr != null)
            {
                ClassDataMap[type] = new ClassData {Attribute = attr, Type = type};
            }
        }

        private const int ViewDestroyTime = 5;
        private Dictionary<View,DateTime> waitDestroyViews = new Dictionary<View, DateTime>();
        private List<View> openedViews = new List<View>();
        private Dictionary<Type, View> openedSingleViews = new Dictionary<Type, View>();

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
                openedViews.Add(_view);
                if (_view.IsSingle)
                    openedSingleViews[type] = _view;
            });
            if (openedSingleViews.TryGetValue(type, out var view))
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
            var view = ReflectionHelper.CreateInstance(type) as View;
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
        
#if UNITY_EDITOR
        public async void EditorCreateView(View view, ViewModel viewModel,string path)
        {
            var goPrefab = await Res.Default.LoadAssetAsync<GameObject>(path);
            Canvas = GameObject.Find("UIRoot").GetComponent<Canvas>();
            GameObject go = Object.Instantiate(goPrefab, Canvas.transform);
            view.SetGameObject(go);
            view.SetVm(viewModel);
        }
#endif

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

        /// <summary>
        /// close isSingle=true的窗口
        /// </summary>
        public void Close<T>() where T : View
        {
            Close(typeof(T));
        }

        public void Close(View view)
        {
            if (view.IsSingle)
            {
                Close(view.GetType());
                return;
            }
            for (int i = 0; i < openedViews.Count; i++)
            {
                if (view == openedViews[i])
                {
                    openedViews.RemoveAt(i);
                    view.Dispose();
                    break;
                }
            }
        }

        /// <summary>
        /// get isSingle=true的窗口
        /// </summary>
        public T Get<T>() where T : View
        {
            var view = Get(typeof(T));
            return view as T;
        }

        /// <summary>
        /// get isSingle=true的窗口
        /// </summary>
        public View Get(Type type)
        {
            if (openedSingleViews.TryGetValue(type, out var view))
            {
                return view;
            }
            return null;
        }

        /// <summary>
        /// close isSingle=true的窗口
        /// </summary>
        public void Close(Type type)
        {
            if (!openedSingleViews.TryGetValue(type, out var view))
                return;
            openedSingleViews.Remove(type);
            for (int i = 0; i < openedViews.Count; i++)
            {
                if (view == openedViews[i])
                {
                    openedViews.RemoveAt(i);
                    break;
                }
            }
            view.Dispose();
            //_waitDestroyViews[view] = DateTime.Now.AddSeconds(ViewDestroyTime);
        }

        private void Sort(View view)
        {
            var viewTransform = view.Go.transform;
            Transform lastTrans = null;
            int index = Int32.MaxValue;
            foreach (View openedView in openedViews)
            {
                if(openedView.UILevel <= view.UILevel)
                    continue;
                try
                {
                    if (openedView.Go.transform.GetSiblingIndex() < index)
                    {
                        lastTrans = openedView.Go.transform;
                        index = lastTrans.GetSiblingIndex();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
               
            }
            
            viewTransform.SetParent(Canvas.transform, false);
            if (lastTrans == null)
                viewTransform.SetAsLastSibling();
            else
                viewTransform.SetSiblingIndex(index);
        }

        private List<View> _needDestroyViews = new List<View>();
        private void Update()
        {
            if(waitDestroyViews.Count <= 0) return;
            var nowTime = DateTime.Now;
            foreach (var waitDestroyView in waitDestroyViews)
            {
                if (waitDestroyView.Value <= nowTime)
                {
                    waitDestroyView.Key.Dispose();
                    _needDestroyViews.Add(waitDestroyView.Key);
                }
            }
            foreach (var view in _needDestroyViews)
            {
                waitDestroyViews.Remove(view);
            }
            _needDestroyViews.Clear();
        }
    }
}