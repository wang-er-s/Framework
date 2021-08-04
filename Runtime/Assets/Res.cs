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
        public abstract string DownloadURL { get; set; }

        public static IRes Default => @default ?? (@default = Create());

        public static IRes Create(FrameworkRuntimeConfig.ResType? loadType = null)
        {
            var config = ConfigBase.Load<FrameworkRuntimeConfig>();
            if (loadType == null)
                loadType = config.LoadType;
            IRes result = null;
            switch (loadType)
            {
                case FrameworkRuntimeConfig.ResType.Resources:
                    result = new ResourcesRes();
                    break;
#if ADDRESSABLE
                case FrameworkRuntimeConfig.ResType.Addressable:
                    result = new AddressableRes();
                    break;
#endif
#if XASSET
                case FrameworkRuntimeConfig.ResType.XAsset:
                    result = new XAssetRes();
                    break;
#endif
#if UNITY_EDITOR
                case FrameworkRuntimeConfig.ResType.Editor:
                    result = new EditorRes();
                    break;
#endif
            }
            return result;
        }
        
        public abstract T LoadAsset<T>(string key) where T : Object;
        protected abstract void LoadScene(IProgressPromise<float, Scene> promise, string path,
            LoadSceneMode loadSceneMode);
        public abstract Task<string> CheckDownloadSize(string key);
        public abstract Task<IProgressResult<DownloadProgress>> DownloadAssets(string key);
        protected abstract void loadAssetAsync<T>(string key, IProgressPromise<float, T> promise) where T : Object;
        public abstract void Release();
        
        public IProgressResult<float,Scene> LoadScene(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            ProgressResult<float,Scene> progressResult = new ProgressResult<float, Scene>();
            LoadScene(progressResult, path, loadSceneMode);
            return progressResult;
        }
        
        public IProgressResult<float, T> LoadAssetAsync<T>(string key) where T : Object
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            loadAssetAsync(key, progressResult);
            return progressResult;
        }
        
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

        public IProgressResult<float, GameObject> InstantiateAsync(string key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            ProgressResult<float, GameObject> loadProgress = new ProgressResult<float, GameObject>();
            ProgressResult<float, GameObject> resultProgress = new ProgressResult<float, GameObject>();
            loadProgress.Callbackable().OnCallback((result =>
            {
                var go = Object.Instantiate(result.Result);
                go.transform.SetParent(parent, instantiateInWorldSpace);
                resultProgress.SetResult(go);
            }));
            loadAssetAsync(key, loadProgress);
            return resultProgress;
        }

        public IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 position,
            Quaternion rotation,
            Transform parent = null)
        {
            ProgressResult<float, GameObject> loadProgress = new ProgressResult<float, GameObject>();
            ProgressResult<float, GameObject> resultProgress = new ProgressResult<float, GameObject>();
            loadProgress.Callbackable().OnCallback((result =>
            {
                var trans = Object.Instantiate(result.Result).transform;
                trans.SetParent(parent);
                trans.localPosition = position;
                trans.localRotation = rotation;
                resultProgress.SetResult(trans.gameObject);
            }));
            loadAssetAsync(key, loadProgress);
            return resultProgress;
        }

        public GameObject Instantiate(string key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            var trans = Object.Instantiate(LoadAsset<GameObject>(key)).transform;
            trans.SetParent(parent, instantiateInWorldSpace);
            return trans.gameObject;
        }
    }
}