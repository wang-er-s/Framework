using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Framework.Assets;
using Framework.Asynchronous;
using Framework.Execution;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Framework.UI.Core
{
    public enum UILevel
    {
        None = -1, //留给内部Element的空值
        Bg = -1, //背景层UI
        Common = 0, //普通层UI
        Pop = 1, //弹出层UI
        Toast = 2, //对话框层UI
        Guide = 3 //新手引导层
    }

    public class DefaultViewLocator : Singleton<DefaultViewLocator>, IViewLocator
    {
        private Dictionary<UILevel, List<View>> _sortViews = new Dictionary<UILevel, List<View>>();
        
        private Dictionary<string, WeakReference> templates = new Dictionary<string, WeakReference>();

        private Res _res;
        
        public Canvas Canvas { get; private set; }
        
        public DefaultViewLocator(Canvas canvas = null)
        {
            if (Canvas == null) Canvas = CreateCanvas();
            Object.DontDestroyOnLoad(Canvas);
            foreach (UILevel level in (UILevel[]) Enum.GetValues(typeof(UILevel)))
            {
                _sortViews[level] = new List<View>();
            }
            _res = new Res();
        }

        public DefaultViewLocator() : this(null)
        {
        }

        [Obsolete("use LoadViewAsync replace", true)]
        public View LoadView(string path, ViewModel viewModel = null, bool autoShow = true)
        {
            var view = CreateUI(path);
            view.SetVm(viewModel);
            if(autoShow)
                view.Show();
            return view;
        }

        [Obsolete("use LoadViewAsync replace", true)]
        public T LoadView<T>(string path, ViewModel viewModel = null, bool autoShow = true) where T : View
        {
            return LoadView(path, viewModel, autoShow) as T;
        }

        public IProgressResult<float, View> LoadViewAsync(string path, ViewModel viewModel = null, bool autoShow = true)
        {
            ProgressResult<float, View> result = new ProgressResult<float, View>();
            Executors.RunOnCoroutineNoReturn(LoadView(result, path, viewModel, autoShow));
            return result;
        }

        public IProgressResult<float, T> LoadViewAsync<T>(string path, ViewModel viewModel = null, bool autoShow = true) where T : View
        {
            ProgressResult<float, T> result = new ProgressResult<float, T>();
            Executors.RunOnCoroutineNoReturn(LoadView(result, path, viewModel, autoShow));
            return result;
        }

        public T GetView<T>(string path = null) where T : View
        {
            foreach (var views in _sortViews.Values)
            {
                foreach (var view in views)
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        if (view.GetType() == typeof(T))
                            return view as T;
                    }
                    else
                    {
                        if (view.ViewPath == path)
                            return view as T;
                    }
                }
            }
            return null;
        }

        private IEnumerator LoadView<T>(IProgressPromise<float, T> promise, string path, ViewModel viewModel = null,
            bool autoShow = true) where T : View
        {
            GameObject viewTemplateGo = null;
            try
            {
                if (this.templates.TryGetValue(path, out var weakRef) && weakRef.IsAlive)
                {
                    viewTemplateGo = (GameObject)weakRef.Target;
                }
            }
            catch (Exception)
            {
                viewTemplateGo = null;
            }
            
            if (viewTemplateGo == null)
            {
                var request = _res.LoadAssetAsync<GameObject>(path);
                while (!request.IsDone)
                {
                    promise.UpdateProgress(request.Progress);
                    yield return null;
                }

                viewTemplateGo = request.Result;
                if (viewTemplateGo != null)
                {
                    this.templates[path] = new WeakReference(viewTemplateGo);
                }
            }
            
            if (viewTemplateGo == null || viewTemplateGo.GetComponent<T>() == null)
            {
                promise.UpdateProgress(1f);
                promise.SetException(new FileNotFoundException(path));
                yield break;
            }

            GameObject go = Object.Instantiate(viewTemplateGo);
            go.name = viewTemplateGo.name;
            T view = go.GetComponent<T>();
            if (view == null)
            {
                Object.Destroy(go);
                promise.SetException(new FileNotFoundException($"{path} not have View component"));
            }
            else
            {
                SortView(view);
                promise.UpdateProgress(1f);
                promise.SetResult(view);
            }
            if(viewModel != null)
                view.SetVm(viewModel);
            if(autoShow)
                view.Show();
        }

        [Obsolete("仅做展示，暂时不实用同步加载", true)]
        private View CreateUI(string panelName)
        {
            var loadGo = _res.LoadAsset<GameObject>(panelName);
            var view = loadGo.GetComponent<View>();
            SortView(view);
            return view;
        }
        
        private void SortView(View view)
        {
            int index = 0;
            foreach (UILevel level in (UILevel[]) Enum.GetValues(typeof(UILevel)))
            {
                var views = _sortViews[level];
                for (int i = 0; i < views.Count; i++)
                {
                    if (views[i] != null) continue;
                    views.RemoveAt(i);
                    i--;
                }
                if (level <= view.UILevel) continue;
                if (views.Count > 0)
                {
                    index = views[0].transform.GetSiblingIndex();
                    break;
                }
            }
            view.transform.SetParent(Canvas.transform, false);
            view.transform.SetSiblingIndex(index);
            _sortViews[view.UILevel].Add(view);
        }

        public static Canvas CreateCanvas()
        {
            var canvas = Object.Instantiate(Resources.Load<Canvas>("Canvas"));
            return canvas;
        }
    }
}