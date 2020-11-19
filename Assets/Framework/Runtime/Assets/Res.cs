using System.Collections.Generic;
using System.Reflection;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Assets
{
    public static class Res
    {
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
    }
}