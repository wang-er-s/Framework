using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace Framework
{
    public class YooRes : Res
    {
        private Dictionary<string, AssetOperationHandle> handles = new Dictionary<string, AssetOperationHandle>();
        private List<IProgressPromise<float>> loadProgress = new List<IProgressPromise<float>>();
        private AssetsPackage defaultPackage;

        public override IProgressResult<float> Init()
        {
            return Executors.RunOnCoroutine<float>(init);
        }

        public YooRes()
        {
            if (YooAssets.IsInitialize)
                defaultPackage = YooAssets.GetAssetsPackage("DefaultPackage");
        }

        private IEnumerator init(IProgressPromise<float> promise)
        {
            YooAssets.Initialize();
            defaultPackage = YooAssets.CreateAssetsPackage("DefaultPackage");
            YooAssets.SetDefaultAssetsPackage(defaultPackage);
            InitializationOperation initializationOperation = null;
            switch (YooAssetSettingsData.Setting.PlayMode)
            {
                case YooAsset.EPlayMode.EditorSimulateMode:
                    var createParameters = new EditorSimulateModeParameters();
                    createParameters.SimulatePatchManifestPath =
                        EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
                    initializationOperation = defaultPackage.InitializeAsync(createParameters);
                    break;
                case YooAsset.EPlayMode.OfflinePlayMode:
                    var createParameters2 = new OfflinePlayModeParameters();
                    createParameters2.DecryptionServices = new Decryption();
                    initializationOperation = defaultPackage.InitializeAsync(createParameters2);
                    break;
                case YooAsset.EPlayMode.HostPlayMode:
                    var createParameters3 = new HostPlayModeParameters();
                    createParameters3.DecryptionServices = null;
                    createParameters3.DefaultHostServer = DownloadUrl;
                    createParameters3.FallbackHostServer = FallbackDownloadUrl;
                    initializationOperation = defaultPackage.InitializeAsync(createParameters3);
                    break;
            }
            
            while (!initializationOperation.IsDone)
            {
                promise.UpdateProgress(initializationOperation.Progress);
                yield return null;
            }

            promise.SetResult();
        }

        public override string HostServerURL { get; set; }

        public override string FallbackHostServerURL { get; set; }

        public override IProgressResult<float, string> CheckDownloadSize()
        {
            return Executors.RunOnCoroutine<float, string>(checkDownloadSize);
        }

        private PatchDownloaderOperation downloader;

        private IEnumerator checkDownloadSize(IProgressPromise<float, string> promise)
        {
            if (YooAssetSettingsData.Setting.PlayMode != YooAsset.EPlayMode.HostPlayMode)
            {
                promise.SetResult(string.Empty);
                yield break;
            }

            // 更新资源版本号
            var versionOperation = defaultPackage.UpdatePackageVersionAsync();
            yield return versionOperation;
            string resourceVersion = String.Empty;
            if (versionOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"Found static version : {versionOperation.PackageVersion}");
                resourceVersion = versionOperation.PackageVersion;
            }
            else
            {
                promise.SetException(versionOperation.Error);
            }

            // 更新补丁清单
            var patchOperation = defaultPackage.UpdatePackageManifestAsync(resourceVersion);
            yield return patchOperation;
            if (patchOperation.Status == EOperationStatus.Failed)
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
                promise.SetResult(StringHelper.FormatSizeBytes(totalDownloadBytes));
            }
        }

        public override IProgressResult<DownloadProgress> DownloadAssets()
        {
            return Executors.RunOnCoroutine<DownloadProgress>(Download);
        }

        private IEnumerator Download(IProgressPromise<DownloadProgress> promise)
        {
            if (YooAssetSettingsData.Setting.PlayMode != YooAsset.EPlayMode.HostPlayMode)
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
                    var now = StringHelper.FormatSizeBytes(downloadedBytes);
                    // 获取总大小
                    var max = StringHelper.FormatSizeBytes(totalBytes);
                    // 计算速度
                    var amount = downloadedBytes - lastSampleDownloadBytes;
                    var speed = StringHelper.FormatSizeBytes(amount * (long)(1 / sampleTime));
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

        protected override IEnumerator LoadScene(IProgressPromise<float, UnityEngine.SceneManagement.Scene> promise, string path,
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
                promise.SetResult(loader.SceneObject);
            }
        }

        protected override IEnumerator loadAssetAsync<T>(string key, IProgressPromise<float, T> promise)
        {
            loadProgress.Add(promise);
            var loader = YooAssets.LoadAssetAsync<T>(key);
            handles[key] = loader;
            while (loader.IsValid && !loader.IsDone)
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
            if (handles.Count <= 0) return;
            foreach (var loadable in handles.Values)
            {
                loadable?.Release();
            }

            foreach (var promise in loadProgress)
            {
                if (!promise.IsDone)
                    promise.SetCancelled();
            }

            loadProgress.Clear();
            handles.Clear();
            defaultPackage.UnloadUnusedAssets();
        }

        public override T LoadAssetSync<T>(string key)
        {
            return YooAssets.LoadAssetSync<T>(key).AssetObject as T;
        }
        private class Decryption : IDecryptionServices
        {
            public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
            {
                return (ulong)fileInfo.BundleName.Length;
            }

            public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
            {
                throw new NotImplementedException();
            }

            public FileStream LoadFromStream(DecryptFileInfo fileInfo)
            {
                var bundleStream = new YooBundleStream(fileInfo.FilePath, fileInfo.BundleName, FileMode.Open, FileAccess.Read, FileShare.Read);
                return bundleStream;
            }

            public uint GetManagedReadBufferSize()
            {
                return 0U;
            }
        }
        
        public class FileStreamEncryption : IEncryptionServices
        {
            public EncryptResult Encrypt(EncryptFileInfo fileInfo)
            {
                int encryptOffset = YooBundleStream.CalcEncryptOffset(fileInfo.BundleName.Length);
                byte[] fileData = File.ReadAllBytes(fileInfo.FilePath);
                if (fileData.Length > encryptOffset)
                {
                    for (int i = 0; i < encryptOffset; i++)
                    {
                        fileData[i] ^= (byte)(fileInfo.BundleName[i % fileInfo.BundleName.Length] + i);
                    }
                }
                EncryptResult result = new EncryptResult();
                result.LoadMethod = EBundleLoadMethod.LoadFromStream;
                result.EncryptedData = fileData;
                return result;
            }
        } 
    }
}