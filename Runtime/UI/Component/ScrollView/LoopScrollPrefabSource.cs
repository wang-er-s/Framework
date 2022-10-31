using UnityEngine;

namespace Framework
{
    [System.Serializable]
    public class LoopScrollPrefabSource
    {
        public GameObject Prefab;
        public int PoolSize = 5;
        private PrefabPool pool;
        private Transform root;

        public void Init(Transform scrollRect)
        {
            root = new GameObject("Pool").transform;
            root.SetParent(scrollRect, false);
            pool = new PrefabPool(Prefab, PoolSize,
                onFree: o => o.transform.SetParent(root, false));
        }
        
        public virtual GameObject GetObject()
        {
            return pool.Allocate();
        }

        public virtual void ReturnObject(Transform go)
        {
            pool.Free(go.gameObject);
        }
    }
}
