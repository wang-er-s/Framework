using System;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework.Assets
{
    public interface IRes
    {
        string DownloadURL { get; set; }
        IProgressResult<float, T> LoadAssetAsync<T>(string key) where T : Object;

        IProgressResult<float, T> InstantiateAsync<T>(string key, Transform parent = null,
            bool instantiateInWorldSpace = false) where T : Component;

        IProgressResult<float, T> InstantiateAsync<T>(string key, Vector3 position, Quaternion rotation,
            Transform parent = null) where T : Component;
        
        IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 position,
            Quaternion rotation,
            Transform parent = null);
        
        IProgressResult<float, GameObject> InstantiateAsync(string key, Transform parent = null,
            bool instantiateInWorldSpace = false);

        IProgressResult<float, Scene> LoadScene(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
        
        Task<string> CheckDownloadSize(string key);
        Task<IProgressResult<DownloadProgress>> DownloadAssets(string key);

        void Release();
        
        GameObject Instantiate(string key, Transform parent = null, bool instantiateInWorldSpace = false);
        
        T LoadAsset<T>(string key) where T : Object;
    }
}