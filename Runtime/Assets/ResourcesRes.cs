using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
{
    public class ResourcesRes : Res
    {

        private List<Object> _handles = new List<Object>();
        private List<ResourceRequest> requests = new List<ResourceRequest>();

        public override IAsyncResult Init()
        {
            return AsyncResult.Void();   
        }

        public override string HostServerURL { get; set; }
        public override string FallbackHostServerURL { get; set; }

        protected override IEnumerator LoadScene(IProgressPromise<float, UnityEngine.SceneManagement.Scene> promise, string path,
            LoadSceneMode loadSceneMode, bool allowSceneActivation = true)
        {
            var operation = SceneManager.LoadSceneAsync(path, loadSceneMode);
            operation.allowSceneActivation = allowSceneActivation;
            while (!operation.isDone)
            {
                yield return null;
                promise.UpdateProgress(operation.progress);
            }
            promise.SetResult(Path.GetFileNameWithoutExtension(path));
        }

        public override IProgressResult<float,string> CheckDownloadSize()
        {
            return ProgressResult<float,string>.Void();
        }
        
        public override IProgressResult<DownloadProgress> DownloadAssets()
        {
            return ProgressResult<DownloadProgress>.Void();
        }

        protected override IEnumerator loadAssetAsync<T>(string key, IProgressPromise<float, T> promise)
        {
            var operation = Resources.LoadAsync<T>(key);
            requests.Add(operation);
            while (!operation.isDone)
            {
                promise.UpdateProgress(operation.progress);
                yield return null;
            }
            promise.UpdateProgress(1);
            promise.SetResult(operation.asset as T);
            _handles.Add(operation.asset);
        }

        public override T LoadAssetSync<T>(string key)
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