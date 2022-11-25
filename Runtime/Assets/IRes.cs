using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework
{
    public interface IRes : IDisposable
    {
        IAsyncResult Init();
        string HostServerURL { get; set; }
        string FallbackHostServerURL { get; set; }
        IProgressResult<float, T> LoadAssetAsync<T>(string key) where T : Object;

        IProgressResult<float, T> InstantiateAsync<T>(string key, Transform parent = null,
            bool instantiateInWorldSpace = false);

        IProgressResult<float, T> InstantiateAsync<T>(string key, Vector3 localPosition, Quaternion localRotation,
            Transform parent = null);
        
        IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 localPosition,
            Quaternion localRotation,
            Transform parent = null);
        
        IProgressResult<float, GameObject> InstantiateAsync(string key, Transform parent = null,
            bool instantiateInWorldSpace = false);

        IProgressResult<float, string> LoadScene(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool allowSceneActivation = true);

        IProgressResult<float, string> CheckDownloadSize();
        IProgressResult<DownloadProgress> DownloadAssets();

        GameObject Instantiate(string key, Transform parent = null, bool instantiateInWorldSpace = false);
        
        T LoadAsset<T>(string key) where T : Object;
    }
}