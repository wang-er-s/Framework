using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Execution;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Assets
{
    public class AddressableRes : Res
    {
        #region static

        private const string DYNAMIC_TAG = "lab_dynamic";
        private static bool initialized = false;

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
            var list = await Addressables.LoadResourceLocationsAsync(DYNAMIC_TAG);
            if (list.Count <= 0) return string.Empty;
            //判断有没有需要动态加载的资源
            var size = await Addressables.GetDownloadSizeAsync(DYNAMIC_TAG);
            if (size <= 0) return String.Empty;
            var kb = size / 1024;
            if (kb < 1024)
                return $"{kb}kb";
            return $"{kb / 1024:.00}mb";
        }

        private static void CheckHaveDynamicAsset()
        {
            Addressables.LoadResourceLocationsAsync(DYNAMIC_TAG);
        }

        public static IProgressResult<float> DownloadNewAssets()
        {
            ProgressResult<float> progressResult = new ProgressResult<float>();
            var operation = Addressables.DownloadDependenciesAsync(DYNAMIC_TAG);
            Executors.RunOnCoroutineNoReturn(Download(progressResult, operation));            
            return progressResult;
        }

        static IEnumerator Download(IProgressPromise<float> promise, AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                promise.UpdateProgress(handle.PercentComplete);
                yield return null;
                if(handle.OperationException != null)
                    promise.SetException(handle.OperationException);
            }
            promise.SetException(handle.OperationException);
            promise.SetResult();
        }

        #endregion

        private static Type MonoType = typeof(MonoBehaviour);
        private List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();

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
            bool instantiateInWorldSpace = false, bool trackHandle = true)
        {
            ProgressResult<float, GameObject> progressResult = new ProgressResult<float, GameObject>();
            instantiateAsync(getOperation(key, parent, instantiateInWorldSpace, trackHandle), progressResult);
            return progressResult;
        }

        public override IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 position, Quaternion rotation,
            Transform parent = null, bool trackHandle = true)
        {
            ProgressResult<float, GameObject> progressResult = new ProgressResult<float, GameObject>();
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
            return Addressables.InstantiateAsync(key, position, rotation, parent, trackHandle);
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

//Test