using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class PrefabPool : Entity, IUpdateSystem, IAwakeSystem<ResComponent,string>,IAwakeSystem<ResComponent>, IDestroySystem
    {
        private RecyclableDic<int, OnePrefab> pathHash2Prefab;
        private RecyclableDic<int, int> goInstanceId2PathHash;
        private List<ProgressResult<float, GameObject>> needInsGo;
        private ResComponent res;
        private Transform  root;
        private static int NotRenderLayer = LayerMask.NameToLayer("NotRender");

        public void Awake(ResComponent res)
        {
            this.res = res;
            pathHash2Prefab = RecyclableDic<int, OnePrefab>.Create();
            goInstanceId2PathHash = RecyclableDic<int, int>.Create();
            root = new GameObject("PrefabPool").transform;
        }
        
        public void Awake(ResComponent res, string name)
        {
            Awake(res);
            root.name = name;
        }

        public IProgressResult<float, GameObject> Allocate(string path)
        {
            int pathHash = path.GetHashCode();
            var prefab = GetPrefab(pathHash);

            if (prefab.Caches.Count > 0)
            {
                ProgressResult<float, GameObject> result = ProgressResult<float, GameObject>.Create();
                var go = prefab.Caches.RemoveLast();
                goInstanceId2PathHash.Add(go.Go.GetInstanceID(), pathHash);
                result.SetResult(go.Go);
                return result;
            }
            
            var result2 = Instantiate(path);
            result2.Callbackable().OnCallback((r) =>
            {
                var go = r.Result;
                goInstanceId2PathHash.Add(go.GetInstanceID(), pathHash); 
            });
            
            return result2;
        }

        public GameObject AllocateSync(string path)
        {
            int pathHash = path.GetHashCode();
            var prefab = GetPrefab(pathHash);

            if (prefab.Caches.Count > 0)
            {
                var go = prefab.Caches.RemoveLast();
                goInstanceId2PathHash.Add(go.Go.GetInstanceID(), pathHash);
                return go.Go;
            }
            else
            {
                var go = InstantiateSync(path);
                goInstanceId2PathHash.Add(go.GetInstanceID(), pathHash);
                return go;
            }
        }

        public void Free(GameObject gameObject)
        {
            if(gameObject == null) return;
            var insId = gameObject.GetInstanceID();
            if (!goInstanceId2PathHash.TryGetValue(insId, out var pathHash))
            {
                Log.Warning($"对象不是通过对象池取出来的==={gameObject.name}");
                Object.Destroy(gameObject);
                return;
            }

            goInstanceId2PathHash.Remove(insId);
            if (pathHash2Prefab.TryGetValue(pathHash, out var prefab))
            {
                ResetGameObject(gameObject);
                prefab.AddCache(gameObject);
            }
            else
            {
                Log.Error("???取出来的怎么会么有呢");
            }
        }

        private IProgressResult<float, GameObject> Instantiate(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                ProgressResult<float, GameObject> result = ProgressResult<float, GameObject>.Create();
                result.SetResult(new GameObject(nameof(GameObject)));
                return result;
            }

            return res.Instantiate(path);
        }

        private GameObject InstantiateSync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new GameObject(nameof(GameObject));
            }

            return res.InstantiateSync(path);
        }
        
       
        public void SetCacheData(string path, float delayDestroyTime, int maxCount)
        {
            int pathHash = path.GetHashCode();
            var prefab = GetPrefab(pathHash);
            prefab.DelayDestroyTime = delayDestroyTime;
            prefab.MaxCount = maxCount;
        }

        private OnePrefab GetPrefab(int pathHash)
        {
            if (!pathHash2Prefab.TryGetValue(pathHash, out var prefab))
            {
                prefab = ReferencePool.Allocate<OnePrefab>();
                prefab.PathHash = pathHash;
                prefab.DelayDestroyTime = 100000;
                prefab.MaxCount = 30;
                prefab.Caches = RecyclableList<DelayDestroyGo>.Create();
                pathHash2Prefab.Add(pathHash, prefab);
            }

            return prefab;
        }

        private void ResetGameObject(GameObject gameObject)
        {
#if UNITY_EDITOR
            gameObject.transform.SetParent(root);
#endif
            gameObject.layer = NotRenderLayer;
        }

        public void Update(float deltaTime)
        {
            float curTime = Time.time;
            foreach (OnePrefab onePrefab in pathHash2Prefab.Values)
            {
                if (onePrefab.Caches.Count <= 0)
                {
                    continue;
                }

                for (int i = 0; i < onePrefab.Caches.Count; i++)
                {
                    DelayDestroyGo cache = onePrefab.Caches[i];
                    if (curTime > cache.DestroyTime)
                    {
                        Object.Destroy(cache.Go);
                        onePrefab.Caches.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private class OnePrefab : IReference
        {
            public int PathHash;
            public float DelayDestroyTime;
            public int MaxCount;
            public RecyclableList<DelayDestroyGo> Caches;

            public void AddCache(GameObject gameObject)
            {
                if (Caches.Count >= MaxCount)
                {
                    Object.Destroy(gameObject);
                }
                else
                {
                    Caches.Add(new DelayDestroyGo(Time.time + DelayDestroyTime, gameObject));
                }
            }

            public void Clear()
            {
                Caches.Dispose();
            }
        }

        private struct DelayDestroyGo
        {
            public float DestroyTime;
            public GameObject Go;

            public DelayDestroyGo(float destroyTime, GameObject go)
            {
                DestroyTime = destroyTime;
                Go = go;
            }
        }

        public void OnDestroy()
        {
            Object.Destroy(root.gameObject);
            foreach (OnePrefab onePrefab in pathHash2Prefab.Values)
            {
                ReferencePool.Free(onePrefab);
            }
            pathHash2Prefab.Dispose();
            goInstanceId2PathHash.Dispose();
        }

    }

}