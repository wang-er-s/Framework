using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Execution;
using Framework.Helper;
using Framework.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using IAsyncResult = Framework.Asynchronous.IAsyncResult;

namespace Framework.Assets
{
    public class YooRes : Res
    {
        private Dictionary<string, AssetOperationHandle> handles = new Dictionary<string, AssetOperationHandle>();
        private List<IProgressPromise<float>> loadProgress = new List<IProgressPromise<float>>();

        public override IAsyncResult Init()
        {
            return Executors.RunOnCoroutine(init);
        }

        private IEnumerator init(IPromise promise)
        {
            switch (YooAssetSettingsData.Setting.PlayMode)
            {
                case YooAssets.EPlayMode.EditorSimulateMode:
                    var createParameters = new YooAssets.EditorSimulateModeParameters();
                    createParameters.LocationServices = new DefaultLocationServices(string.Empty);
                    yield return YooAssets.InitializeAsync(createParameters);
                    break;
                case YooAssets.EPlayMode.OfflinePlayMode:
                    var createParameters2 = new YooAssets.OfflinePlayModeParameters();
                    createParameters2.LocationServices = new DefaultLocationServices(string.Empty);
                    yield return YooAssets.InitializeAsync(createParameters2);
                    break;
                case YooAssets.EPlayMode.HostPlayMode:
                    var createParameters3 = new YooAssets.HostPlayModeParameters();
                    createParameters3.LocationServices = new DefaultLocationServices(string.Empty);
                    createParameters3.DecryptionServices = null;
                    createParameters3.ClearCacheWhenDirty = false;
                    createParameters3.DefaultHostServer = DownloadUrl;
                    createParameters3.FallbackHostServer = FallbackDownloadUrl;
                    createParameters3.VerifyLevel = EVerifyLevel.High;
                    yield return YooAssets.InitializeAsync(createParameters3);
                    break;
            }
            promise.SetResult();
        }

        public override string HostServerURL { get; set; }

        public override string FallbackHostServerURL { get; set; }

        public override IProgressResult<float,string> CheckDownloadSize()
        {
            return Executors.RunOnCoroutine<float,string>(checkDownloadSize);
        }

        private PatchDownloaderOperation downloader; 
       
        private IEnumerator checkDownloadSize(IProgressPromise<float,string> promise)
        {
            if (YooAssetSettingsData.Setting.PlayMode != YooAssets.EPlayMode.HostPlayMode)
            {
                promise.SetResult(string.Empty);
                yield break;
            }
            
            // 更新资源版本号
            var versionOperation = YooAssets.UpdateStaticVersionAsync(30);
            yield return versionOperation;
            int resourceVersion = 0;
            if (versionOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"Found static version : {versionOperation.ResourceVersion}");
                resourceVersion = versionOperation.ResourceVersion;
            }
            else
            {
                promise.SetException(versionOperation.Error);
            }
            
            // 更新补丁清单
            var patchOperation = YooAssets.UpdateManifestAsync(resourceVersion, 30);
            yield return patchOperation;
            if(patchOperation.Status == EOperationStatus.Failed)
            {
                promise.SetException(patchOperation.Error);
            }
            
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            downloader = YooAssets.CreatePatchDownloader(downloadingMaxNum, failedTryAgain);
            if (downloader.TotalDownloadCount == 0)
            {
                promise.SetResult(String.Empty);
            }
            else
            {
                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                promise.SetResult(CommonHelper.FormatBytes(totalDownloadBytes));
            }
        }

        public override IProgressResult<DownloadProgress> DownloadAssets()
        {
            return Executors.RunOnCoroutine<DownloadProgress>(Download);
        }

        private IEnumerator Download(IProgressPromise<DownloadProgress> promise)
        {
            if (YooAssetSettingsData.Setting.PlayMode != YooAssets.EPlayMode.HostPlayMode)
            {
                promise.SetResult();
                yield break;
            }
            
            if (downloader == null)
            {
                throw new Exception("需要先调用 CheckDownloadSize()");
            }
            downloader.BeginDownload();
            // 采样时间，推荐每秒采样一次
            float sampleTime = 0.1f;
            long lastSampleDownloadBytes = 0;
            var lastSampleTime = 0f;

            downloader.OnDownloadProgressCallback += (_, _, totalBytes, downloadedBytes) =>
            {
                if (Time.realtimeSinceStartup - lastSampleTime > sampleTime)
                {
                    // 获取已经下载的内容大小
                    var now = CommonHelper.FormatBytes(downloadedBytes);
                    // 获取总大小
                    var max = CommonHelper.FormatBytes(totalBytes);
                    // 计算速度
                    var amount = downloadedBytes - lastSampleDownloadBytes;
                    var speed = CommonHelper.FormatBytes(amount * (long)(1 / sampleTime));
                    lastSampleTime = Time.realtimeSinceStartup;
                    promise.UpdateProgress(new DownloadProgress(now, max, speed, downloader.Progress));
                    lastSampleDownloadBytes = downloadedBytes;
                }
            };
            
            while (!downloader.IsDone)
            {
                yield return null;
            }

            if (downloader.Status != EOperationStatus.Succeed)
            {
                promise.SetException(downloader.Error);
            }
            else
            {
                promise.SetResult();
            }
        }

        protected override IEnumerator LoadScene(IProgressPromise<float, string> promise, string path,
            LoadSceneMode loadSceneMode, bool allowSceneActivation = true)
        {
            loadProgress.Add(promise);
            var loader = YooAssets.LoadSceneAsync(path, loadSceneMode, allowSceneActivation);
            while (!loader.IsDone)
            {
                promise.UpdateProgress(loader.Progress);
                yield return null;
            }
            if (loader.Status != EOperationStatus.Succeed)
            {
                promise.SetException(loader.LastError);
            }
            else
            {
                promise.SetResult();
            }
        }

        protected override IEnumerator loadAssetAsync<T>(string key, IProgressPromise<float, T> promise)
        {
            loadProgress.Add(promise);
            var loader = YooAssets.LoadAssetAsync<T>(key);
            handles[key] = loader;
            while (!loader.IsDone)
            {
                promise.UpdateProgress(loader.Progress);
                yield return null;
            }
            
            if (loader.Status != EOperationStatus.Succeed)
            {
                promise.SetException(loader.LastError);
            }
            else
            {
                promise.SetResult(loader.AssetObject);
            }
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
            return YooAssets.LoadAssetSync<T>(key).AssetObject as T;
        }
    }
}