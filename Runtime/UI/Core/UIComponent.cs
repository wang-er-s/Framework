using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    [Event(SceneType.Root)]
    public class ChangeSceneCleanEvent : AEvent<SceneChangedData>
    {
        protected override async ETTask Run(Scene scene, SceneChangedData a)
        {
            Root.Instance.Scene.GetComponent<UIComponent>().GetComponent<ResComponent>().Release();
            await ETTask.CompletedTask;
        }
    }

    public class UIComponent : Entity, IAwakeSystem,IDestroySystem
    {
        private readonly Dictionary<Type, UIAttribute> viewType2Attribute = new();
        private readonly Dictionary<Type, Window> createdSingleViews = new();
        private readonly Dictionary<Type, IAsyncResult> loadingView = new();
        private readonly Dictionary<UILevel, List<Window>> uiLevel2ShowedView = new();
        public Canvas Canvas { get; private set; }
        private PrefabPool prefabPool;
        private ResComponent resComponent;
 
        public void Awake()
        {
            this.Canvas = this.RootScene().GetComponent<GlobalReferenceComponent>().UICanvas;
            resComponent = AddComponent<ResComponent>();
            prefabPool = AddComponent<PrefabPool, ResComponent, string>(resComponent, $"{this.DomainScene().Name}UI_PrefabPool");
            var canvas = this.RootScene().GetComponent<GlobalReferenceComponent>().UICanvas;
            Object.DontDestroyOnLoad(canvas.transform.parent.gameObject);
            foreach (var tuple in EventSystem.Instance.GetTypesAndAttribute(typeof(UIAttribute)))
            {
                viewType2Attribute[tuple.type] = tuple.attribute as UIAttribute;
            }
        }

        public IProgressResult<float, T> CreateWindow<T>(ViewModel viewModel) where T : Window
        {
            var type = typeof(T);
            if (viewType2Attribute[type].IsSingle && loadingView.TryGetValue(type, out var result))
                return result as IProgressResult<float, T>;
            ProgressResult<float, T> result1 = ProgressResult<float, T>.Create(isFromPool: false); 
            DoCreateWindow(result1, viewModel);
            return result1;
        }

        public T CreateViewWithGo<T>(ViewModel viewModel, GameObject gameObject) where T : View
        {
            var view = AddChild<T>() as View;
            view.SetGameObject(gameObject);
            gameObject.name = typeof(T).Name;

            view.SetVm(viewModel);
            return view as T;
        }

        private void DoCreateWindow<T>(ProgressResult<float, T> promise, ViewModel viewModel)
            where T : Window
        {
            var type = typeof(T);
            var attribute = viewType2Attribute[type];
            loadingView[type] = promise;
            promise.Callbackable().OnCallback(progressResult =>
            {
                // 如果加载过程中就关闭了，直接销毁
                if (progressResult.IsCancelled)
                {
                    progressResult.Result.Dispose();
                    return;
                }

                Window result = progressResult.Result;
                AddOpenWindow(result);
                loadingView.Remove(type);
            });
            if (createdSingleViews.TryGetValue(type, out var view))
            {
                promise.UpdateProgress(1);
                promise.SetResult(view);
            }
            else
            {
                view = AddChild(type) as Window;
                SetViewGmeObjectAndVM(promise, view, attribute.Path, viewModel);
            }
        }

        public IProgressResult<float, T> CreateSubViewAsync<T>(ViewModel vm) where T : View
        {
            var type = typeof(T);
            ProgressResult<float, T> progressResult = ProgressResult<float, T>.Create(isFromPool: true);
            var view = AddChild(type) as View;
            SetViewGmeObjectAndVM(progressResult, view, viewType2Attribute[type].Path, vm);
            return progressResult;
        }
        
        private async void SetViewGmeObjectAndVM<T>(IProgressPromise<float, T> promise, View view, string path,
            ViewModel viewModel)
            where T : View
        {
            var type = view.GetType();

            var go = await prefabPool.Allocate(viewType2Attribute[type].Path);
            if (go == null)
            {
                promise.UpdateProgress(1f);
                Log.Error($"Not found the window path = \"{path}\".");
                promise.SetException(new FileNotFoundException(path));
                return;
            }

            view.SetGameObject(go);
            go.name = type.Name;
            if (view is Window window)
            {
                go.transform.SetParent(UIRootHelper.GetTargetRoot(this.RootScene(), window.UILevel));
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
            }
            view.SetVm(viewModel);
            promise.SetResult(view);
        }

        public T CreateWindowSync<T>(ViewModel viewModel) where T : Window
        {
            var type = typeof(T);
            var go = CreateViewGameObject(type);
            Window window = AddChild<T>();
            window.SetGameObject(go);
            window.SetVm(viewModel);
            return (T)window;
        }

        private void AddOpenWindow(Window window)
        {
            var type = window.GetType();
            if (viewType2Attribute[type].IsSingle)
            {
                createdSingleViews[type] = window;
            }
        }

        /// <summary>
        /// close isSingle=true的窗口
        /// </summary>
        public void Close<T>() where T : Window
        {
            Close(typeof(T));
        }
        
        public void Close(Type type)
        {
            if (!createdSingleViews.TryGetValue(type, out var view))
                return;
            Close(view);
        }

        public void Close(Window window)
        {
            var type = window.GetType();
            createdSingleViews.Remove(type);
            uiLevel2ShowedView[window.UILevel].Remove(window);
            window.Dispose();
            window.ViewModel?.Dispose();
            if (window.GameObject != null)
            {
                if (viewType2Attribute[type].IsPool)
                    prefabPool.Free(window.GameObject);
                else
                    Object.Destroy(window.GameObject);
            }

            MaskViews(window, false);
        }

        public T Get<T>() where T : Window
        {
            if (createdSingleViews.TryGetValue(typeof(T), out var view))
            {
                return view as T;
            }

            return null;
        }

        public void CloseAll()
        {
            using RecyclableList<Window> views = RecyclableList<Window>.Create();
            uiLevel2ShowedView.Values.ForEach(v => views.AddRange(v));
            foreach (var view in views)
            {
                Close(view);
            }
        }

        public GameObject CreateViewGameObject(Type type)
        {
            var path = viewType2Attribute[type].Path;
            GameObject go = null;
            if (viewType2Attribute[type].IsPool)
                go = prefabPool.AllocateSync(path);
            else
                go = resComponent.InstantiateSync(path); 
            return go;
        }

        public void ShowSort(Window window)
        {
            var viewTransform = window.GameObject.transform;
            // 把当前界面移动到当前层的最高位置
            if (uiLevel2ShowedView.TryGetValue(window.UILevel, out var views))
            {
                views.Remove(window);
                views.Add(window);
            }
            else
            {
                views = new List<Window>();
                uiLevel2ShowedView.Add(window.UILevel, views);
                views.Add(window);
            }
            (viewTransform as  RectTransform).offsetMax = Vector2.zero;
            (viewTransform as  RectTransform).offsetMin = Vector2.zero;
            viewTransform.SetAsLastSibling();
            MaskViews(window, true);
        }

        public void HideSort(Window window)
        {
            window.GameObject.transform.SetAsFirstSibling();
            MaskViews(window, false);
        }

        public void PassivateWindow(Window window, bool ignoreAnim)
        {
            window.Passivate(ignoreAnim);
            if (uiLevel2ShowedView.TryGetValue(window.UILevel, out var views) && views.Count > 0)
            {
                views.Last().Activate(ignoreAnim);
            }
        }

        public void ActiveWindow(Window window, bool ignoreAnim)
        {
            window.Activate(ignoreAnim);
            if(window.UILevel == UILevel.Pop)return;
            if (uiLevel2ShowedView.TryGetValue(window.UILevel, out var views) && views.Count > 0)
            {
                foreach (Window value in views)
                {
                    if (value != window)
                    {
                        value.Passivate(ignoreAnim);
                    }
                }
            }
        }

        private void MaskViews(Window window, bool open)
        {
            bool isMaskBottomView = viewType2Attribute[window.GetType()].IsMaskBottomView;
            // 如果不会挡住下面的界面，则直接返回
            if (!isMaskBottomView) return;
            if (open)
            {
                // 打开则隐藏下面的所有ui
                for (int i = (int)window.UILevel; i > (int)UILevel.None; i--)
                {
                    UILevel level = (UILevel)i;
                    if (!uiLevel2ShowedView.TryGetValue(level, out var views) || views.Count <= 0) continue;
                    foreach (var openedView in views)
                    {
                        if (openedView != window)
                            openedView.Visibility = false;
                    }
                }
            }
            else
            {
                // 打开下面的ui，直到碰到一个遮挡下面ui的ui
                for (int i = (int)window.UILevel; i > (int)UILevel.None; i--)
                {
                    UILevel level = (UILevel)i;
                    if (!uiLevel2ShowedView.TryGetValue(level, out var views) || views.Count <= 0) continue;
                    foreach (var openedView in views)
                    {
                        if (openedView == window) continue;
                        openedView.Visibility = true;
                        var type = openedView.GetType();
                        if (viewType2Attribute[type].IsMaskBottomView)
                        {
                            return;
                        }
                    }
                }
            }
        }

        public void OnDestroy()
        {
            CloseAll();
        }
    }
}