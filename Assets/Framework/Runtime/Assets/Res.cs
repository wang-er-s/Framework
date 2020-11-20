using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Assets
{
    public static class Res
    {
        private const string DYNAMIC_TAG = "dynamic";
        
        public static void UnloadUnusedAssets()
        {
            var per = typeof(Addressables).GetProperty("Instance", BindingFlags.Static| BindingFlags.NonPublic);
            var obj = per.GetValue(null);
            Dictionary<object, AsyncOperationHandle> dic = obj.GetType().GetField("m_resultToHandle", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj) as
                Dictionary<object, AsyncOperationHandle>;
            foreach (var key in dic.Keys)
            {
                if (key != null)
                {
                    Addressables.Release(key);
                }
            }
        }

        public static async Task<string> CheckDownloadSize()
        {
            var size = await Addressables.GetDownloadSizeAsync(DYNAMIC_TAG);
            var kb = size / 1024;
            if (kb < 1024)
                return $"{kb}kb";
            return $"{kb / 1024:.00}mb";
        }

        public static async void DownloadNewAssets(Action<float> progressCb, Action endCb)
        {
            var operation = Addressables.DownloadDependenciesAsync(DYNAMIC_TAG);
            while (!operation.IsDone)
            {
                progressCb?.Invoke(operation.PercentComplete);
                await Task.Yield();
            }
            progressCb?.Invoke(1);
            endCb?.Invoke();
        }
    }
}