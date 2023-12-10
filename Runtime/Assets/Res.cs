using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
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

        public override string ToString()
        {
            return $"{DownloadedSize}/{TotalSize}  {DownloadSpeed}";
        }
    }
    
    public abstract class Res : IRes
    {
        public abstract IAsyncResult Init();
        public static Type DefaultResType = typeof(YooRes);
        public abstract string HostServerURL { get; set; }
        public abstract string FallbackHostServerURL { get; set; }

        public static IRes Create()
        {
            return Activator.CreateInstance(DefaultResType) as IRes;
        }
        
        public abstract T LoadAssetSync<T>(string key) where T : Object;
        public abstract void Release();

        protected abstract IEnumerator LoadScene(IProgressPromise<float, UnityEngine.SceneManagement.Scene> promise, string path,
            LoadSceneMode loadSceneMode, bool allowSceneActivation = true);

        public abstract IProgressResult<float,string> CheckDownloadSize();
        public abstract IProgressResult<DownloadProgress> DownloadAssets();
        protected abstract IEnumerator loadAssetAsync<T>(string key, IProgressPromise<float, T> promise) where T : Object;
        
        public IProgressResult<float, UnityEngine.SceneManagement.Scene> LoadScene(string path,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single,
            bool allowSceneActivation = true)
        {
            ProgressResult<float,UnityEngine.SceneManagement.Scene> progressResult = ProgressResult<float, UnityEngine.SceneManagement.Scene>.Create();
            Executors.RunOnCoroutineReturn(LoadScene(progressResult, path, loadSceneMode, allowSceneActivation));
            return progressResult;
        }
        
        public IProgressResult<float, T> LoadAsset<T>(string key) where T : Object
        {
            ProgressResult<float, T> progressResult = ProgressResult<float, T>.Create();
            Executors.RunOnCoroutineReturn(loadAssetAsync(key, progressResult));
            return progressResult;
        }
        
        public IProgressResult<float, T> Instantiate<T>(string key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            var progress = Instantiate(key, parent, instantiateInWorldSpace);
            ProgressResult<float, T> result = ProgressResult<float, T>.Create(isFromPool: false);
            progress.Callbackable().OnProgressCallback(result.UpdateProgress);
            progress.Callbackable().OnCallback(progressResult =>
            {
                if (result.IsCancelled)
                {
                    Object.Destroy(progressResult.Result);
                }
                else
                {
                    result.SetResult(progressResult.Result.GetComponent<T>());
                }
            });
            return result;
        }

        public IProgressResult<float, T> Instantiate<T>(string key, Vector3 localPosition, Quaternion localRotation,
            Transform parent = null)
        {
            var progress = Instantiate(key, localPosition, localRotation, parent);
            ProgressResult<float, T> result = ProgressResult<float, T>.Create();
            result.Callbackable().OnCallback((progressResult =>
            {
                if (progressResult.IsCancelled)
                {
                    progress.Cancel();
                }
            }));
            progress.Callbackable().OnProgressCallback(result.UpdateProgress);
            progress.Callbackable().OnCallback(progressResult =>
            {
                GameObject pr = progressResult.Result;
                if (result.IsCancelled && pr != null)
                {
                    Object.Destroy(pr);
                }
                else
                {
                    result.SetResult(pr.GetComponent<T>());
                }
            });
            return result;
        }

        public IProgressResult<float, GameObject> Instantiate(string key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            ProgressResult<float, GameObject> loadProgress = ProgressResult<float, GameObject>.Create(isFromPool: true);
            ProgressResult<float, GameObject> resultProgress = ProgressResult<float, GameObject>.Create();
            loadProgress.Callbackable().OnCallback((result =>
            {
                if(resultProgress.IsCancelled) return;
                var go = Object.Instantiate(result.Result);
                go.transform.SetParent(parent, instantiateInWorldSpace);
                resultProgress.SetResult(go);
            }));
            loadProgress.Callbackable().OnProgressCallback(resultProgress.UpdateProgress);
            Executors.RunOnCoroutineNoReturn(loadAssetAsync(key, loadProgress));
            return resultProgress;
        }

        public IProgressResult<float, GameObject> Instantiate(string key, Vector3 localPosition,
            Quaternion localRotation,
            Transform parent = null)
        {
            ProgressResult<float, GameObject> loadProgress = ProgressResult<float, GameObject>.Create(isFromPool: true);
            ProgressResult<float, GameObject> resultProgress = ProgressResult<float, GameObject>.Create();
            loadProgress.Callbackable().OnCallback((result =>
            {
                if(resultProgress.IsCancelled) return;
                var trans = Object.Instantiate(result.Result).transform;
                trans.SetParent(parent);
                trans.localPosition = localPosition;
                trans.localRotation = localRotation;
                resultProgress.SetResult(trans.gameObject);
            }));
            loadProgress.Callbackable().OnProgressCallback(resultProgress.UpdateProgress);
            Executors.RunOnCoroutineNoReturn(loadAssetAsync(key, loadProgress));
            return resultProgress;
        }

        public GameObject InstantiateSync(string key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            var trans = Object.Instantiate(LoadAssetSync<GameObject>(key)).transform;
            trans.SetParent(parent, instantiateInWorldSpace);
            return trans.gameObject;
        }

        protected string DownloadUrl
        {
            get
            {
                string hostServerIP = HostServerURL;
                string gameVersion = ConfigBase.Load<FrameworkRuntimeConfig>().GameVersion;
                return $"{hostServerIP}/CDN/{ApplicationHelper.GetPlatformPath(Application.platform)}/{gameVersion}";
            }
        }
        
        protected string FallbackDownloadUrl
        {
            get
            {
                string hostServerIP = FallbackHostServerURL;
                string gameVersion = ConfigBase.Load<FrameworkRuntimeConfig>().GameVersion;
#if UNITY_EDITOR
                return $"{hostServerIP}/CDN/{ApplicationHelper.GetPlatformPath(UnityEditor.EditorUserBuildSettings.activeBuildTarget)}/{gameVersion}";
#else
                return $"{hostServerIP}/CDN/{ApplicationHelper.GetPlatformPath(Application.platform)}/{gameVersion}";
#endif
            }
        }

   
    }
}