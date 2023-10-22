using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class ResComponent : Entity , IAwakeSystem , IDestroySystem
    {
        private IRes res;
        public void Awake()
        {
            res = Res.Create();
        }

        public IAsyncResult Init()
        {
            return res.Init();
        }

        public string HostServerURL
        {
            get => res.HostServerURL;
            set => res.HostServerURL = value;
        }

        public string FallbackHostServerURL
        {
            get => res.FallbackHostServerURL;
            set => res.FallbackHostServerURL = value;
        }

        public IProgressResult<float, T> LoadAsset<T>(string key) where T : Object
        {
            return res.LoadAsset<T>(key);
        }

        internal IProgressResult<float, T> Instantiate<T>(string key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            return res.Instantiate<T>(key, parent, instantiateInWorldSpace);
        }

        internal IProgressResult<float, T> Instantiate<T>(string key, Vector3 localPosition, Quaternion localRotation,
            Transform parent = null)
        {
            return res.Instantiate<T>(key, localPosition, localRotation, parent);
        }

        internal IProgressResult<float, GameObject> Instantiate(string key, Vector3 localPosition,
            Quaternion localRotation,
            Transform parent = null)
        {
            return res.Instantiate(key, localPosition, localRotation, parent);
        }

        internal IProgressResult<float, GameObject> Instantiate(string key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            return res.Instantiate(key, parent, instantiateInWorldSpace);
        }

        public IProgressResult<float, string> LoadScene(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Single,
            bool allowSceneActivation = true)
        {
            return res.LoadScene(path, loadSceneMode, allowSceneActivation);
        }

        public IProgressResult<float, string> CheckDownloadSize()
        {
            return res.CheckDownloadSize();
        }

        public IProgressResult<DownloadProgress> DownloadAssets()
        {
            return res.DownloadAssets();
        }

        internal GameObject InstantiateSync(string key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return res.InstantiateSync(key, parent, instantiateInWorldSpace);
        }

        public T LoadAssetSync<T>(string key) where T : Object
        {
            return res.LoadAssetSync<T>(key);
        }

        public void Release()
        {
            res.Release();
        }

        public void OnDestroy()
        {
            res.Release();
        }
    }
}