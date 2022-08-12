using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework.Assets;
using Framework.Asynchronous;
using Framework.Execution;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Core
{
    public class UIManager : GameModuleWithAttribute<UIManager, UIAttribute, Type>
    {
        private IRes _res;

        [SerializeField]
        private Canvas canvas;
        public Canvas Canvas => canvas;

        public override void Init()
        {
            DontDestroyOnLoad(Canvas);
            _res = Res.Create();
        }

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
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
            View view = null;
            var path = (GetClassData(type).Attribute as UIAttribute).Path;
            promise.Callbackable().OnCallback(progressResult =>
            {
                Sort(view);
                view.Show();
            });
            if (openedSingleViews.TryGetValue(type, out var view1))
            {
                promise.UpdateProgress(1);
                promise.SetResult(view1);
            }
            else
            {
                view = ReflectionHelper.CreateInstance(type) as View;
                Executors.RunOnCoroutineNoReturn(CreateViewGo(promise, view, path, viewModel));
            }
            openedViews.Add(view);
            if (view.IsSingle)
                openedSingleViews[type] = view;
        }

        public IProgressResult<float, T> CreateView<T>(ViewModel vm) where T : View
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            var type = typeof(T);
            var view = ReflectionHelper.CreateInstance(type) as View;
            var path = (GetClassData(type).Attribute as UIAttribute).Path;
            Executors.RunOnCoroutineNoReturn(CreateViewGo(progressResult, view, path, vm));
            return progressResult;
        }
        
        public IProgressResult<float, View> CreateView(Type type, ViewModel vm)
        {
            ProgressResult<float, View> progressResult = new ProgressResult<float, View>();
            var view = ReflectionHelper.CreateInstance(type) as View;
            var path = (GetClassData(type).Attribute as UIAttribute).Path;
            Executors.RunOnCoroutineNoReturn(CreateViewGo(progressResult, view, path, vm));
            return progressResult;
        }

        private IEnumerator CreateViewGo<T>(IProgressPromise<float, T> promise,View view,string path, ViewModel viewModel)
            where T : View
        {
            
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
            view.SetGameObject(go);
            go.name = viewTemplateGo.name;
            promise.UpdateProgress(1f);
            promise.SetResult(view);
            view.SetVm(viewModel);
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
                Close(view.GetCLRType());
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
        
        public T Get<T>() where T : View
        {
            var view = Get(typeof(T));
            return view as T;
        }
        
        public View Get(Type type)
        {
            if (openedSingleViews.TryGetValue(type, out var view))
            {
                return view;
            }
            return null;
        }
        
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
        
        public void CloseAll()
        {
            foreach (var openedView in openedViews)
            {
                openedView.Dispose();
            }
            openedViews.Clear();
            openedSingleViews.Clear();
        }

        private void Sort(View view)
        {
            var viewTransform = view.Go.transform;
            Transform lastTrans = null;
            int index = Int32.MaxValue;
            foreach (View openedView in openedViews)
            {
				if(openedView.Go == null) continue;
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
    }
}