#if XASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Execution;
using UnityEngine;
using UnityEngine.SceneManagement;
using VEngine;
using Object = UnityEngine.Object;
using Scene = VEngine.Scene;

namespace Framework.Assets
{
    public class XAssetRes : Res
    {
        private string _preDownloadKey;
        private List<DownloadInfo> _needDownloadRes;
        private Dictionary<string, Loadable> _handles = new Dictionary<string, Loadable>();

        public override async Task<string> CheckDownloadSize(string key)
        {
            _preDownloadKey = key;
            var update = Versions.UpdateAsync(key);
            await update;
            var check = Versions.GetDownloadSizeAsync();
            await check;
            // 判断是否有内容需要更新
            if (check.result.Count <= 0)
            {
                return String.Empty;
            }
            _needDownloadRes = check.result;
            return Utility.FormatBytes(check.totalSize);
        }
        
        public override async Task<IProgressResult<DownloadProgress>> DownloadAssets(string key)
        {
            if (_preDownloadKey != key)
                await CheckDownloadSize(key);
            ProgressResult<DownloadProgress> progressResult = new ProgressResult<DownloadProgress>();
            Executors.RunOnCoroutineNoReturn(Download(progressResult));
            return progressResult;
        }

        private IEnumerator Download(IProgressPromise<DownloadProgress> promise)
        {
            // 这里省略了请求下载的逻辑，直接启动更新
            var download = Versions.DownloadAsync(_needDownloadRes.ToArray());
// 采样时间，推荐每秒采样一次
            const float sampleTime = 1f;
// 上次采样的进度，用来计算下载速度
            var lastDownloadedBytes = 0UL;
            var lastSampleTime = 0f;
            while (!download.isDone)
            {
                if (Time.realtimeSinceStartup - lastSampleTime > sampleTime)
                {
                    // 获取已经下载的内容大小
                    var now = Utility.FormatBytes(download.downloadedBytes);
                    // 获取总大小
                    var max = Utility.FormatBytes(download.totalSize);
                    // 计算速度
                    var amount = lastDownloadedBytes - download.downloadedBytes;
                    var speed = Utility.FormatBytes((ulong) (amount / sampleTime));
                    lastDownloadedBytes = download.downloadedBytes;
                    lastSampleTime = Time.realtimeSinceStartup;
                    promise.UpdateProgress(new DownloadProgress(now, max, speed, download.progress));
                }

                yield return null;
            }
            promise.SetResult();
        }
        
        protected override async void LoadScene(IProgressPromise<float, UnityEngine.SceneManagement.Scene> promise, string path, LoadSceneMode loadSceneMode)
        {
            var loader = Scene.LoadAsync(path, additive: loadSceneMode == LoadSceneMode.Additive);
            while (!loader.isDone)
            {
                await Task.Yield();
                promise.UpdateProgress(loader.progress);
            }

            _handles[path] = loader;
            promise.SetResult();
        }

        protected override void loadAssetAsync<T>(string key, IProgressPromise<float, T> promise)
        {
            var asset = Asset.LoadAsync(key, typeof(T), result =>
            {
                if (result.status == LoadableStatus.SuccessToLoad)
                {
                    promise.SetResult(result.asset);
                }
                else
                {
                    Log.Error("load", key, "error", result.error);
                    promise.SetException(result.error);   
                }
                //TODO  考虑一下怎么release asset。。 
                //result.Release();
            });
            _handles[key] = asset;
        }

        public override IProgressResult<float, GameObject> InstantiateAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false) 
        {
            return instantiateAsync(key, go => go.transform.SetParent(parent, instantiateInWorldSpace));
        }

        private IProgressResult<float, GameObject> instantiateAsync(string key, Action<GameObject> dealGo)
        {
            ProgressResult<float, GameObject> progressResult = new ProgressResult<float, GameObject>();
            var asset = Asset.LoadAsync(key, typeof(GameObject), result =>
            {
                if (result.status == LoadableStatus.SuccessToLoad)
                {
                    var go = Object.Instantiate(result.asset as GameObject);
                    dealGo(go);
                    progressResult.SetResult(go);
                }
                else
                {
                    Log.Error("load", key, "error", result.error);
                    progressResult.SetException(result.error);   
                }
                result.Release();
            });
            _handles[key] = asset;
            return progressResult;
        }
        

        public override void Release()
        {
            foreach (var loadable in _handles.Values)
            {
                loadable?.Release();
            }
            _handles.Clear();
        }

        [Obsolete("仅做展示，暂时不使用同步加载")]
        public override GameObject Instantiate(string key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return Object.Instantiate(Asset.Load(key, typeof(GameObject)).asset as GameObject);
        }

        [Obsolete("仅做展示，暂时不使用同步加载")]
        public override T LoadAsset<T>(string key)
        {
            return Asset.Load(key, typeof(T)).asset as T;
        }

        public override IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return instantiateAsync(key, go =>
            {
                go.transform.SetParent(parent);
                go.transform.position = position;
                go.transform.rotation = rotation;
            });
        }
    }
}
#endif