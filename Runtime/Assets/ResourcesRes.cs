using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Assets
{
    public class ResourcesRes : Res
    {

        private List<Object> _handles = new List<Object>();
        
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
            _handles.Add(operation.asset);
        }

        public override IProgressResult<float, GameObject> InstantiateAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false,
            bool trackHandle = true)
        {
            ProgressResult<float, GameObject> progressResult = new ProgressResult<float, GameObject>();
            progressResult.Callbackable().OnCallback((result =>
            {
                var ins = Object.Instantiate(result.Result);
                SetTransform(ins, parent, instantiateInWorldSpace);
                progressResult.SetResult(ins);
            }));
            loadAssetAsync(key, progressResult);
            return progressResult;
        }

        public override IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 position, Quaternion rotation, Transform parent = null,
            bool trackHandle = true)
        {
            ProgressResult<float, GameObject> progressResult = new ProgressResult<float, GameObject>();
            progressResult.Callbackable().OnCallback((result =>
            {
                var ins = Object.Instantiate(result.Result);
                SetTransform(ins, position, rotation, parent, trackHandle);
                progressResult.SetResult(ins);
            }));
            loadAssetAsync(key, progressResult);
            return progressResult;
        }

        private void SetTransform(GameObject obj, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = true)
        {
            var trans = obj.transform;
            trans.SetParent(parent, instantiateInWorldSpace);
        }
        
        private void SetTransform(GameObject obj, Vector3 position, Quaternion rotation,
            Transform parent = null, bool trackHandle = true)
        {
            var trans = obj.transform;
            trans.SetParent(parent);
            trans.localPosition = position;
            trans.localRotation = rotation;
        }
        
        [Obsolete("仅做展示，暂时不使用同步加载")]
        public override GameObject Instantiate(string key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            var obj = Object.Instantiate(Resources.Load<Object>(key), parent,instantiateInWorldSpace);
            _handles.Add(obj);
            return obj as GameObject;
        }

        [Obsolete("仅做展示，暂时不使用同步加载")]
        public override T LoadAsset<T>(string key)
        {
            var obj = Resources.Load<T>(key);
            _handles.Add(obj);
            return obj;
        }
        
        public override void Release()
        {
            foreach (var handle in _handles)
            {
                Resources.UnloadAsset(handle);
            }
            _handles.Clear();
        }
    }
}