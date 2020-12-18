using System;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Framework.Assets
{
    public abstract class Res : IRes
    {
        public IProgressResult<float, T> LoadAssetAsync<T>(string key) where T : Object
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            loadAssetAsync(key, progressResult);
            return progressResult;
        }

        protected abstract void loadAssetAsync<T>(string key, IProgressPromise<float, T> promise) where T : Object;

        public IProgressResult<float, T> InstantiateAsync<T>(string key, Transform parent = null,
            bool instantiateInWorldSpace = false,
            bool trackHandle = true) where T : Component
        {
            var progress = InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
            ProgressResult<float, T> result = new ProgressResult<float, T>();
            progress.Callbackable().OnProgressCallback(result.UpdateProgress);
            progress.Callbackable().OnCallback(progressResult => result.SetResult(progressResult.Result.GetComponent<T>()));
            return result;
        }

        public IProgressResult<float, T> InstantiateAsync<T>(string key, Vector3 position, Quaternion rotation,
            Transform parent = null,
            bool trackHandle = true) where T : Component
        {
            var progress = InstantiateAsync(key, position, rotation, parent, trackHandle);
            ProgressResult<float, T> result = new ProgressResult<float, T>();
            progress.Callbackable().OnProgressCallback(result.UpdateProgress);
            progress.Callbackable().OnCallback(progressResult => result.SetResult(progressResult.Result.GetComponent<T>()));
            return result;
        }

        public abstract IProgressResult<float, GameObject> InstantiateAsync(string key, Transform parent = null,
            bool instantiateInWorldSpace = false,
            bool trackHandle = true);

        public abstract IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            bool trackHandle = true);

        public abstract void Release();

        public abstract GameObject Instantiate(string key, Transform parent = null,
            bool instantiateInWorldSpace = false);


        public abstract T LoadAsset<T>(string key) where T : Object;
    }
}