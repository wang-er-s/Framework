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
        IProgressResult<float, T> InstantiateAsync<T>(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) where T : MonoBehaviour;

        IProgressResult<float, T> InstantiateAsync<T>(string key, Vector3 position, Quaternion rotation,
            Transform parent = null, bool trackHandle = true) where T : MonoBehaviour;

        void Release();
        GameObject Instantiate(string key, Transform parent = null, bool instantiateInWorldSpace = false);
        T LoadAsset<T>(string key) where T : Object;
    }
}