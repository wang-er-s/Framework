using System;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Assets
{
    public class ResourcesRes : Res
    {
        protected override async void loadAssetAsync<T>(string key, IProgressPromise<float, T> promise)
        {
            var operation = Resources.LoadAsync<T>(key);
            while (!operation.isDone)
            {
                promise.UpdateProgress(operation.progress);
                await Task.Yield();
            }
            promise.UpdateProgress(1);
            promise.SetResult(operation.asset as T);
        }

        public override IProgressResult<float, T> InstantiateAsync<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false,
            bool trackHandle = true)
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            ResourceRequest request = Resources.LoadAsync<T>(key);
            request.completed += operation =>
            {
                SetTransform(request.asset as T, parent, instantiateInWorldSpace);
            };
            instantiateAsync(request, progressResult);
            return progressResult;
        }

        public override IProgressResult<float, T> InstantiateAsync<T>(string key, Vector3 position, Quaternion rotation, Transform parent = null,
            bool trackHandle = true)
        {
            ProgressResult<float, T> progressResult = new ProgressResult<float, T>();
            ResourceRequest request = Resources.LoadAsync<T>(key);
            request.completed += operation =>
            {
                SetTransform(request.asset as T, position, rotation, parent, trackHandle);
            };
            instantiateAsync(request, progressResult);
            return progressResult;
        }
        
        private void SetTransform<T>(T obj, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = true) where T : MonoBehaviour
        {
            var trans = obj.transform;
            trans.SetParent(parent, instantiateInWorldSpace);
        }
        
        private void SetTransform<T>(T obj, Vector3 position, Quaternion rotation,
            Transform parent = null, bool trackHandle = true) where T : MonoBehaviour
        {
            var trans = obj.transform;
            trans.SetParent(parent);
            trans.localPosition = position;
            trans.localRotation = rotation;
        }

        private async void instantiateAsync<T>(ResourceRequest request, IProgressPromise<float, T> promise) where T : MonoBehaviour
        {
            while (!request.isDone)
            {
                promise.UpdateProgress(request.progress);
                await Task.Yield();
            }
            promise.UpdateProgress(1);
            T obj = Object.Instantiate(request.asset as T);
            if (typeof(T) == typeof(GameObject))
                promise.SetResult(obj);
            else
                promise.SetResult(obj.GetComponent<T>());
        }

        public override GameObject Instantiate(string key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            var obj = Object.Instantiate(Resources.Load<Object>(key), parent,instantiateInWorldSpace);
            return obj as GameObject;
        }

        public override T LoadAsset<T>(string key)
        {
            return Resources.Load<T>(key);
        }
    }
}