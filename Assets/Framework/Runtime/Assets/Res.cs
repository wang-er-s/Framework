using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Assets
{
    public class Res
    {
        #region static

        private const string DYNAMIC_TAG = "dynamic";
        private static bool initialized = false;
        public static bool Ready => initialized;

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            Addressables.InitializeAsync().Completed += (operation) => initialized = true;
        }

        public static void UnloadUnusedAssets()
        {
            var per = typeof(Addressables).GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic);
            var obj = per.GetValue(null);
            Dictionary<object, AsyncOperationHandle> dic =
                obj.GetType().GetField("m_resultToHandle", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(obj) as
                    Dictionary<object, AsyncOperationHandle>;
            var operations = dic.Values.ToList();
            for (int i = 0; i < operations.Count; i++)
            {
                try
                {
                    Addressables.Release(operations[i]);
                }
                catch (Exception)
                {
                }
            }
        }

        public static async Task<string> CheckDownloadSize()
        {
            var size = await Addressables.GetDownloadSizeAsync(DYNAMIC_TAG);
            var kb = size / 1024;
            if (kb < 1024)
                return $"{kb}kb";
            return $"{kb / 1024:.00}mb";
        }

        public static async void DownloadNewAssets(Action<float> progressCb, Action endCb)
        {
            var operation = Addressables.DownloadDependenciesAsync(DYNAMIC_TAG);
            while (!operation.IsDone)
            {
                progressCb?.Invoke(operation.PercentComplete);
                await Task.Yield();
            }
            progressCb?.Invoke(1);
            endCb?.Invoke();
        }

        #endregion

        private List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();

        public IProgressResult<float, T> LoadAssetAsync<T>(object key)
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            loadAssetAsync(key, progressResult);
            return progressResult;
        }

        private async void loadAssetAsync<T>(object key, IProgressPromise<float, T> promise)
        {
            if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                var operation = Addressables.LoadAssetAsync<GameObject>(key);
                while (!operation.IsDone)
                {
                    promise.UpdateProgress(operation.PercentComplete);
                    await Task.Yield();
                }
                promise.UpdateProgress(1);
                promise.SetResult(operation.Result.GetComponent(typeof(T)));
            }
            else
            {
                var operation = Addressables.LoadAssetAsync<T>(key);
                while (!operation.IsDone)
                {
                    promise.UpdateProgress(operation.PercentComplete);
                    await Task.Yield();
                }
                promise.UpdateProgress(1);
                promise.SetResult(operation.Result);
            }
        }

        public IProgressResult<float, T> InstantiateAsync<T>(object key, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = true)
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            instantiateAsync(getOperation(key, parent, instantiateInWorldSpace, trackHandle), progressResult);
            return progressResult;
        }

        public IProgressResult<float, T> InstantiateAsync<T>(object key, Vector3 position, Quaternion rotation,
            Transform parent = null, bool trackHandle = true) where T : MonoBehaviour
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            instantiateAsync(getOperation(key, position, rotation, parent, trackHandle), progressResult);
            return progressResult;
        }

        private AsyncOperationHandle<GameObject> getOperation(object key, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = true)
        {
            return Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
        }

        private AsyncOperationHandle<GameObject> getOperation(object key, Vector3 position, Quaternion rotation,
            Transform parent = null, bool trackHandle = true)
        {
            return Addressables.InstantiateAsync(key, position, rotation, parent);
        }

        private async void instantiateAsync<T>(AsyncOperationHandle<GameObject> operation,
            IProgressPromise<float, T> promise)
        {
            while (!operation.IsDone)
            {
                promise.UpdateProgress(operation.PercentComplete);
                await Task.Yield();
            }
            promise.UpdateProgress(1);
            if (typeof(T) == typeof(GameObject))
                promise.SetResult(operation.Result);
            else
                promise.SetResult(operation.Result.GetComponent<T>());
        }

        public void Release()
        {
            for (int i = 0; i < _handles.Count; i++)
            {
                try
                {
                    Addressables.Release(_handles[i]);
                    _handles.RemoveAt(i);
                    i--;
                }
                catch (Exception)
                {
                }
            }
            _handles.Clear();
        }
        
        
        #region sync
        
        [Obsolete("仅做展示，暂时不实用同步加载", true)]
        public GameObject Instantiate(object key, Transform parent = null, bool instantiateInWorldSpace = false)
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

        [Obsolete("仅做展示，暂时不实用同步加载", true)]
        public TObject LoadAsset<TObject>(object key)
        {
            if (!initialized)
                throw new Exception("We haven't init'd yet!");
            var op = Addressables.LoadAssetAsync<TObject>(key);
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