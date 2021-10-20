#if XASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Execution;
using UnityEngine;
using UnityEngine.SceneManagement;
using VEngine;
using IAsyncResult = Framework.Asynchronous.IAsyncResult;
using Logger = VEngine.Logger;
using Scene = VEngine.Scene;

namespace Framework.Assets
{
    public class XAssetRes : Res
    {
        private List<DownloadInfo> needDownloadRes;
        private Dictionary<string, Loadable> handles = new Dictionary<string, Loadable>();
        private List<IProgressPromise<float>> loadProgress = new List<IProgressPromise<float>>();
        private InitializeVersions initializeVersions;

        public override IAsyncResult Init()
        {
            return Executors.RunOnCoroutine(init);
        }

        private IEnumerator init(IPromise promise)
        {
            Logger.Loggable = false;
            Versions.customLoadPath = Path.GetFileNameWithoutExtension;
            initializeVersions = Versions.InitializeAsync();
            yield return initializeVersions;
            var update = Versions.UpdateAsync(initializeVersions.manifests);
            yield return update;
            if (update.status == OperationStatus.Success)
            {
                update.Override();
                Log.Msg($"Success to update versions with version: {Versions.ManifestsVersion}");
            }
            if (!string.IsNullOrEmpty(update.error))
                promise.SetException(update.error);
            else
                promise.SetResult();
        }

        public override string DownloadURL
        {
            get => Versions.DownloadURL;
            set => Versions.DownloadURL = value;
        }

        public override IProgressResult<float,string> CheckDownloadSize()
        {
            return Executors.RunOnCoroutine<float,string>(checkDownloadSize);
        }

        private IEnumerator checkDownloadSize(IProgressPromise<float,string> promise)
        {
            List<string> assets = new List<string>();
            foreach (var manifest in Versions.Manifests)
            {
                foreach (var manifestBundle in manifest.bundles)
                {
                    assets.AddRange(manifestBundle.assets);
                }
            }
            var downloadInfos = Versions.GetDownloadSizeAsync(assets.ToArray());
            while (!downloadInfos.isDone)
            {
                promise.UpdateProgress(downloadInfos.progress);
                yield return null;
            }
            if (downloadInfos.status != OperationStatus.Success)
            {
                promise.SetException(downloadInfos.error);
                yield break;
            }
            // 判断是否有内容需要更新
            if (downloadInfos.result.Count <= 0)
            {
                promise.SetResult(String.Empty);
            }
            needDownloadRes = downloadInfos.result;
            promise.SetResult(Utility.FormatBytes(downloadInfos.totalSize));
        }

        public override IProgressResult<DownloadProgress> DownloadAssets()
        {
            return Executors.RunOnCoroutine<DownloadProgress>(Download);
        }

        private IEnumerator Download(IProgressPromise<DownloadProgress> promise)
        {
            var download = Versions.DownloadAsync(needDownloadRes);
            // 采样时间，推荐每秒采样一次
            float sampleTime = 0.5f;
            // 上次采样的进度，用来计算下载速度
            long lastDownloadedBytes = 0;
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
                    var speed = Utility.FormatBytes(amount * (long)(1 / sampleTime));
                    lastDownloadedBytes = download.downloadedBytes;
                    lastSampleTime = Time.realtimeSinceStartup;
                    promise.UpdateProgress(new DownloadProgress(now, max, speed, download.progress));
                }

                yield return null;
            }
            if (download.status != OperationStatus.Success)
            {
                promise.SetException(download.error);
            }
            else
            {
                promise.SetResult();
            }
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

            handles[path] = loader;
            promise.SetResult();
        }

        protected override void loadAssetAsync<T>(string key, IProgressPromise<float, T> promise)
        {
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
            handles[key] = asset;
        }

        public override void Release()
        {
            foreach (var loadable in handles.Values)
            {
                loadable?.Release();
            }
            foreach (var promise in loadProgress)
            {
                if(!promise.IsDone)
                    promise.SetCancelled();
            }
            loadProgress.Clear();
            handles.Clear();
        }

        public override T LoadAsset<T>(string key)
        {
            return Asset.Load(key, typeof(T)).asset as T;
        }
    }
}
#endif