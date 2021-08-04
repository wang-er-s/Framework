#if ADDRESSABLE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Execution;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Framework.Assets
{
    public class AddressableRes : Res
    {
        private static bool initialized = false;

        public override string DownloadURL { get; set; }

        static IEnumerator Download(IProgressPromise<DownloadProgress> promise, AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                promise.UpdateProgress(new DownloadProgress("", "", "", handle.PercentComplete));
                yield return null;
                if(handle.OperationException != null)
                    promise.SetException(handle.OperationException);
            }
            promise.SetException(handle.OperationException);
            promise.SetResult();
        }

        private static Type MonoType = typeof(MonoBehaviour);
        private List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();

        public override async Task<string> CheckDownloadSize(string key)
        {
            var list = await Addressables.LoadResourceLocationsAsync(key);
            if (list.Count <= 0) return string.Empty;
            //判断有没有需要动态加载的资源
            var size = await Addressables.GetDownloadSizeAsync(key);
            if (size <= 0) return String.Empty;
            var kb = size / 1024;
            if (kb < 1024)
                return $"{kb}kb";
            return $"{kb / 1024:.00}mb";
        }
        

        protected override async void LoadScene(IProgressPromise<float, Scene> promise,string path, LoadSceneMode loadSceneMode)
        {
            var loader = Addressables.LoadSceneAsync(path, loadSceneMode);
            while (!loader.IsDone)
            {
                await Task.Yield();
                promise.UpdateProgress(loader.PercentComplete);
            }
            
            promise.UpdateProgress(1);
            promise.SetResult(loader.Result.Scene);
        }


#pragma warning disable 1998
        public override async Task<IProgressResult<DownloadProgress>> DownloadAssets(string key)
#pragma warning restore 1998
        {
            ProgressResult<DownloadProgress> progressResult = new ProgressResult<DownloadProgress>();
            var operation = Addressables.DownloadDependenciesAsync(key);
            Executors.RunOnCoroutineNoReturn(Download(progressResult, operation));            
            return progressResult;
        }

        protected override async void loadAssetAsync<T>(string key, IProgressPromise<float, T> promise)
        {
            var operation = Addressables.LoadAssetAsync<T>(key);
            while (!operation.IsDone)
            {
                promise.UpdateProgress(operation.PercentComplete);
                await Task.Yield();
            }
            if (typeof(T).IsSubclassOf(MonoType))
            {
                promise.SetResult((operation.Result as MonoBehaviour)?.GetComponent(typeof(T)));
            }
            else
            {
                promise.SetResult(operation.Result);
            }
            _handles.Add(operation);
        }

        public override IProgressResult<float, GameObject> InstantiateAsync(string key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            ProgressResult<float, GameObject> progressResult = new ProgressResult<float, GameObject>();
            instantiateAsync(getOperation(key, parent, instantiateInWorldSpace), progressResult);
            return progressResult;
        }

        public override IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 position, Quaternion rotation,
            Transform parent = null)
        {
            ProgressResult<float, GameObject> progressResult = new ProgressResult<float, GameObject>();
            instantiateAsync(getOperation(key, position, rotation, parent), progressResult);
            return progressResult;
        }

        private AsyncOperationHandle<GameObject> getOperation(object key, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = true)
        {
            return Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
        }

        private AsyncOperationHandle<GameObject> getOperation(object key, Vector3 position, Quaternion rotation,
            Transform parent = null)
        {
            return Addressables.InstantiateAsync(key, position, rotation, parent);
        }

        private async void instantiateAsync(AsyncOperationHandle<GameObject> operation,
            IProgressPromise<float, GameObject> promise)
        {
            _handles.Add(operation);
            while (!operation.IsDone)
            {
                promise.UpdateProgress(operation.PercentComplete);
                await Task.Yield();
            }
            promise.UpdateProgress(1);
            promise.SetResult(operation.Result);
        }

        public override void Release()
        {
            for (int i = 0; i < _handles.Count; i++)
            {
                try
                {
                    Addressables.Release(_handles[i]);
                }
                catch (Exception)
                {
                }
            }
            _handles.Clear();
        }


        #region sync

        [Obsolete("仅做展示，暂时不使用同步加载", true)]
        public override GameObject Instantiate(string key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            if (!initialized)
                throw new Exception("We haven't init'd yet!");
            var op = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace);
            _handles.Add(op);
            if (!op.IsDone)
                throw new Exception("Sync Instantiate failed to finish! " + key);
            if (op.Result == null)
            {
                var message = "Sync Instantiate has null result " + key;
                if (op.OperationException != null)
                    message += " Exception: " + op.OperationException;
                throw new Exception(message);
            }
            return op.Result;
        }

        [Obsolete("仅做展示，暂时不使用同步加载", true)]
        public override T LoadAsset<T>(string key)
        {
            if (!initialized)
                throw new Exception("We haven't init'd yet!");
            var op = Addressables.LoadAssetAsync<T>(key);
            _handles.Add(op);
            if (!op.IsDone)
                throw new Exception("Sync LoadAsset failed to load in a sync way! " + key);
            if (op.Result == null)
            {
                var message = "Sync LoadAsset has null result " + key;
                if (op.OperationException != null)
                    message += " Exception: " + op.OperationException;
                throw new Exception(message);
            }
            return op.Result;
        }

        #endregion
    }
}

#endif