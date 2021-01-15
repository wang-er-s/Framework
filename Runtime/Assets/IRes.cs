using System;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Assets
{
    public interface IRes
    {
        IProgressResult<float, T> LoadAssetAsync<T>(string key) where T : Object;

        IProgressResult<float, T> InstantiateAsync<T>(string key, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = true) where T : Component;

        IProgressResult<float, T> InstantiateAsync<T>(string key, Vector3 position, Quaternion rotation,
            Transform parent = null, bool trackHandle = true) where T : Component;
        
        IProgressResult<float, GameObject> InstantiateAsync(string key, Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            bool trackHandle = true);
        
        IProgressResult<float, GameObject> InstantiateAsync(string key, Transform parent = null,
            bool instantiateInWorldSpace = false,
            bool trackHandle = true);

        void Release();
        [Obsolete("仅做展示，暂时不使用同步加载")]
        GameObject Instantiate(string key, Transform parent = null, bool instantiateInWorldSpace = false);
        [Obsolete("仅做展示，暂时不使用同步加载")]
        T LoadAsset<T>(string key) where T : Object;
    }
}