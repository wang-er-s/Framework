#if XASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Execution;
using Framework.Runtime.UI.Component;
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
        private List<IProgressPromise<float>> loadProgress = new List<IProgressPromise<float>>();

        public override string DownloadURL
        {
            get => Versions.DownloadURL;
            set => Versions.DownloadURL = value;
        }

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

        // public IAsyncResult<string> CheckDownloadSize2(string key)
        // {
        //     AsyncResult<string> result = new AsyncResult<string>();
        //     CheckDownloadSize2(key,result);
        //     return result;
        // }
        //
        // async void CheckDownloadSize2(string key, IPromise<string> promise)
        // {
        //     var update = Versions.UpdateAsync(key);
        //     await update;
        //     if (update.status == OperationStatus.Failed)
        //     {
        //        promise.SetException(update.error);
        //        return;
        //     }
        //     var check = Versions.GetDownloadSizeAsync(update);
        //     await check;
        // }
        
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
            loadProgress.Add(promise);
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
            Debug.Log($"load {key}");
            loadProgress.Add(promise);
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

        public override void Release()
        {
            foreach (var loadable in _handles.Values)
            {
                loadable?.Release();
            }
            foreach (var promise in loadProgress)
            {
                if(!promise.IsDone)
                    promise.SetCancelled();
            }
            loadProgress.Clear();
            _handles.Clear();
        }

        public override T LoadAsset<T>(string key)
        {
            return Asset.Load(key, typeof(T)).asset as T;
        }
    }
}
#endif