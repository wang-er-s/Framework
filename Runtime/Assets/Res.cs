using System;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework.Assets
{
    
    public struct DownloadProgress
    {
        public string DownloadedSize;
        public string TotalSize;
        public string DownloadSpeed;
        public float Progress;
            
        public DownloadProgress(string downloadedSize, string totalSize, string downloadSpeed, float progress)
        {
            DownloadedSize = downloadedSize;
            TotalSize = totalSize;
            DownloadSpeed = downloadSpeed;
            Progress = progress;
        }
    }
    
    public abstract class Res : IRes
    {
        private static IRes @default;

        public static IRes Default => @default ?? (@default = Create());

        public static IRes Create()
        {
            IRes result = null;
            var config = ConfigBase.Load<FrameworkRuntimeConfig>();
            switch (config.LoadType)
            {
                case FrameworkRuntimeConfig.ResType.Resources:
                    result = new ResourcesRes();
                    break;
                case FrameworkRuntimeConfig.ResType.Addressable:
                    result = new AddressableRes();
                    break;
                case FrameworkRuntimeConfig.ResType.XAsset:
                    result = new XAssetRes();
                    break;
            }
            return result;
        }
        
        public IProgressResult<float,Scene> LoadScene(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            ProgressResult<float,Scene> progressResult = new ProgressResult<float, Scene>();
            LoadScene(progressResult, path, loadSceneMode);
            return progressResult;
        }

        protected abstract void LoadScene(IProgressPromise<float, Scene> promise, string path,
            LoadSceneMode loadSceneMode);
        
        public abstract Task<string> CheckDownloadSize(string key);
        public abstract Task<IProgressResult<DownloadProgress>> DownloadAssets(string key);
        
        public IProgressResult<float, T> LoadAssetAsync<T>(string key) where T : Object
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            loadAssetAsync(key, progressResult);
            return progressResult;
        }

        protected abstract void loadAssetAsync<T>(string key, IProgressPromise<float, T> promise) where T : Object;

        public IProgressResult<float, T> InstantiateAsync<T>(string key, Transform parent = null,
            bool instantiateInWorldSpace = false) where T : Component
        {
            var progress = InstantiateAsync(key, parent, instantiateInWorldSpace);
            ProgressResult<float, T> result = new ProgressResult<float, T>();
            progress.Callbackable().OnProgressCallback(result.UpdateProgress);
            progress.Callbackable().OnCallback(progressResult => result.SetResult(progressResult.Result.GetComponent<T>()));
            return result;
        }

        public IProgressResult<float, T> InstantiateAsync<T>(string key, Vector3 position, Quaternion rotation,
            Transform parent = null) where T : Component
        {
            var progress = InstantiateAsync(key, position, rotation, parent);
            ProgressResult<float, T> result = new ProgressResult<float, T>();
            progress.Callbackable().OnProgressCallback(result.UpdateProgress);
            progress.Callbackable().OnCallback(progressResult => result.SetResult(progressResult.Result.GetComponent<T>()));
            return result;
        }

        public abstract IProgressResult<float, GameObject> InstantiateAsync(string key, Transform parent = null,
            bool instantiateInWorldSpace = false);

        public abstract IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 position,
            Quaternion rotation,
            Transform parent = null);

        public abstract void Release();

        [Obsolete("仅做展示，暂时不使用同步加载")]
        public abstract GameObject Instantiate(string key, Transform parent = null,
            bool instantiateInWorldSpace = false);


        [Obsolete("仅做展示，暂时不使用同步加载")]
        public abstract T LoadAsset<T>(string key) where T : Object;
    }
}