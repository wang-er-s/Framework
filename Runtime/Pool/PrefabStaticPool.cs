using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class PrefabStaticPool
    {
         private static Dictionary<string, PrefabPool> gameObjectPool = new();
         public static GameObject Allocate(string path)
         {
             return GetGoPool(path).Allocate();
         }
         
         public static GameObject Allocate(string path, Transform parent)
         {
             var go = GetGoPool(path).Allocate();
             go.transform.SetParent(parent);
             return go;
         }
        
         private static PrefabPool GetGoPool(string path)
         {
             if (!gameObjectPool.TryGetValue(path, out var pool))
             {
                 pool = new PrefabPool(Res.Default.LoadAsset<GameObject>(path));
                 gameObjectPool[path] = pool;
             }

             return pool;
         }

         public static void Free(string path, GameObject go)
         {
             GetGoPool(path).Free(go);
         }

         public static void Dispose(string path)
         {
             if (!gameObjectPool.TryGetValue(path, out var pool)) return;
             pool.Dispose();
             gameObjectPool.Remove(path);
         }
    }
}