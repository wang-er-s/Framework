using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AD
{
    /// <summary>
    /// 根據不同模式，從AssetBundle中獲取Asset或從Resources中獲取,两种加载方式同时实现的桥接类
    /// 读取一个文件的对象，不做拷贝和引用
    /// </summary>
    public class AssetFileLoader : AbstractResourceLoader
    {
        public delegate void AssetFileBridgeDelegate(bool isOk, Object resultObj);

        public Object Asset
        {
            get { return ResultObject as Object; }
        }

        private bool IsLoadAssetBundle;
		public static bool EditorLoadAsset
		{
			get
			{
	            return Configs.IsEditor;
			}
		}

        public override float Progress
        {
            get
            {
                if (_bundleLoader != null)
                    return _bundleLoader.Progress;
                return 0;
            }
        }

        private AssetBundleLoader _bundleLoader;

        public static AssetFileLoader Load(string path, AssetFileBridgeDelegate assetFileLoadedCallback = null, LoaderMode loaderMode = LoaderMode.Async)
        {
            // 添加扩展名
			if (!EditorLoadAsset)
	            path = path + Configs.AssetBundleExt;

            LoaderDelgate realcallback = null;
            if (assetFileLoadedCallback != null)
            {
                realcallback = (isOk, obj) => assetFileLoadedCallback(isOk, obj as Object);
            }

            return AutoNew<AssetFileLoader>(path, realcallback, false, loaderMode);
        }

        protected override void Init(string url, params object[] args)
        {
            var loaderMode = (LoaderMode)args[0];

            base.Init(url, args);
            ResourceModule.Ins.StartCoroutine(_Init(Url, loaderMode));
        }

        private IEnumerator _Init(string path, LoaderMode loaderMode)
        {
            IsLoadAssetBundle = Configs.LoadAssetBundle;

            Object getAsset = null;

			if (EditorLoadAsset) 
			{
#if UNITY_EDITOR
				if (path.EndsWith(".unity"))
				{
					// scene
					getAsset = ResourceModule.Ins;
					Log.LogWarning("Load scene from Build Settings: {0}", path);
				}
				else 
				{
					getAsset = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + Configs.EditorResourcesDir + "/" + path, typeof(UnityEngine.Object));
					if (getAsset == null)
					{
						Log.Error("Asset is NULL(from {0} Folder): {1}", Configs.EditorResourcesDir, path);
					}
				}
#else
				Log.Error("`IsEditorLoadAsset` is Unity Editor only");

#endif
				OnFinish(getAsset);

			}
            else if (!IsLoadAssetBundle)
            {
                string extension = Path.GetExtension(path);
                path = path.Substring(0, path.Length - extension.Length); // remove extensions

                getAsset = Resources.Load<Object>(path);
                if (getAsset == null)
                {
                    Log.Error("Asset is NULL(from Resources Folder): {0}", path);
                }
                OnFinish(getAsset);
            }
            else
            {
                _bundleLoader = AssetBundleLoader.Load(path, null, loaderMode);

                while (!_bundleLoader.IsCompleted)
                {
                    if (IsReadyDisposed) // 中途释放
                    {
                        _bundleLoader.Release();
                        OnFinish(null);
                        yield break;
                    }
                    yield return null;
                }

                if (!_bundleLoader.IsSuccess)
                {
                    Log.Error("[AssetFileLoader]Load BundleLoader Failed(Error) when Finished: {0}", path);
                    _bundleLoader.Release();
                    OnFinish(null);
                    yield break;
                }

                var assetBundle = _bundleLoader.Bundle;

                DateTime beginTime = DateTime.Now;
#if UNITY_5 || UNITY_2017_1_OR_NEWER
                // Unity 5 下，不能用mainAsset, 要取对象名
                var abAssetName = Path.GetFileNameWithoutExtension(Url).ToLower();
                if (!assetBundle.isStreamedSceneAssetBundle)
                {
                    if (loaderMode == LoaderMode.Sync)
                    {
                        getAsset = assetBundle.LoadAsset(abAssetName);
                        Debugger.Assert(getAsset);
                        _bundleLoader.PushLoadedAsset(getAsset);
                    }
                    else
                    {
                        var request = assetBundle.LoadAssetAsync(abAssetName);
                        while (!request.isDone)
                        {
                            yield return null;
                        }
                        Debugger.Assert(getAsset = request.asset);
                        _bundleLoader.PushLoadedAsset(getAsset);
                    }
                }
                else
                {
                    // if it's a scene in asset bundle, did nothing
                    // but set a fault Object the result
                    getAsset = ResourceModule.Ins;
                }
#else
                // 经过AddWatch调试，.mainAsset这个getter第一次执行时特别久，要做序列化
                //AssetBundleRequest request = assetBundle.LoadAsync("", typeof(Object));// mainAsset
                //while (!request.isDone)
                //{
                //    yield return null;
                //}
                try
                {
                    Debugger.Assert(getAsset = assetBundle.mainAsset);
                }
                catch
                {
                    Log.Error("[OnAssetBundleLoaded:mainAsset]{0}", path);
                }
#endif

                ResourceModule.LogLoadTime("AssetFileBridge", path, beginTime);

                if (getAsset == null)
                {
                    Log.Error("Asset is NULL: {0}", path);
                }

            }

            if (Application.isEditor)
            {
                if (getAsset != null)
                    ResourceLoadedAssetDebugger.Create(getAsset.GetType().Name, Url, getAsset);

                // 编辑器环境下，如果遇到GameObject，对Shader进行Fix
                if (getAsset is GameObject)
                {
                    var go = getAsset as GameObject;
                    foreach (var r in go.GetComponentsInChildren<Renderer>(true))
                    {
                        RefreshMaterialsShaders(r);
                    }
                }
            }

            if (getAsset != null && IsLoadAssetBundle)
            {
                // 更名~ 注明来源asset bundle 带有类型
                getAsset.name = String.Format("{0}~{1}", getAsset, Url);
            }
            OnFinish(getAsset);
        }

        /// <summary>
        /// 编辑器模式下，对指定GameObject刷新一下Material
        /// </summary>
        public static void RefreshMaterialsShaders(Renderer renderer)
        {
            if (renderer.sharedMaterials != null)
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat != null && mat.shader != null)
                    {
                        mat.shader = Shader.Find(mat.shader.name);
                    }
                }
            }
        }

        protected override void DoDispose()
        {
            base.DoDispose();
            _bundleLoader.Release(); // 释放Bundle(WebStream)
            //if (IsFinished)
            {
                if (!IsLoadAssetBundle)
                {
                    Resources.UnloadAsset(ResultObject as Object);
                }
                else
                {
                    //Object.DestroyObject(ResultObject as UnityEngine.Object);

                    // Destroying GameObjects immediately is not permitted during physics trigger/contact, animation event callbacks or OnValidate. You must use Destroy instead.
//                    Object.DestroyImmediate(ResultObject as Object, true);
                }

                //var bRemove = Caches.Remove(Url);
                //if (!bRemove)
                //{
                //    Log.Warning("[DisposeTheCache]Remove Fail(可能有两个未完成的，同时来到这) : {0}", Url);
                //}
            }
            //else
            //{
            //    // 交给加载后，进行检查并卸载资源
            //    // 可能情况TIPS：两个未完成的！会触发上面两次！
            //}
        }
    }

}
