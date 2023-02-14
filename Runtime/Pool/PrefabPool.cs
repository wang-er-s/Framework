using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class PrefabPool<TComponent> : Pool<TComponent> where TComponent : Component
    {
        private Transform root;
        private Transform parent;
        private bool autoActive;
        public PrefabPool(TComponent prefab, int initCount = 1, Action<TComponent> onAlloc = null,
            Action<TComponent> onFree = null, Action<TComponent> onDispose = null, bool autoActive = true, Transform parent = null) : base(
            () => Object.Instantiate(prefab, parent), 0, onAlloc, onFree, onDispose)
        {
            this.parent = parent;
            this.autoActive = autoActive;
            root = new GameObject($"POOL_{prefab.name}").transform;
            root.SetParent(PrefabPool.PrefabPoolRoot);
            for (int i = 0; i < initCount; i++)
            {
                Free(Allocate());
            } 
        }

        public override TComponent Allocate()
        {
            var result = base.Allocate();
            result.transform.SetParent(parent);
            if (autoActive)
                result.gameObject.SetActive(true);
            return result;
        }

        public override void Free(TComponent obj)
        {
            base.Free(obj);
            obj.transform.SetParent(root);
            if (autoActive)
                obj.gameObject.SetActive(false);
        }
        
        public override void Dispose()
        {
            Object.Destroy(root.gameObject);
            CacheStack.Clear();
        }
    }

    public class PrefabPool : Pool<GameObject>
    {
        private static Transform prefabPoolRoot;

        public static Transform PrefabPoolRoot
        {
            get
            {
                if (prefabPoolRoot == null)
                {
                    prefabPoolRoot = new GameObject("POOL").transform;
                }

                return prefabPoolRoot;
            }
        }
        private Transform root;
        private bool autoActive;
        private Transform parent;

        public PrefabPool(GameObject prefab, int initCount = 1, Action<GameObject> onAlloc = null,
            Action<GameObject> onFree = null, Action<GameObject> onDispose = null, bool autoActive = true, Transform parent = null) : base(
            () => Object.Instantiate(prefab, parent), 0, onAlloc, onFree, onDispose)
        {
            this.parent = parent;
            root = new GameObject($"POOL_{prefab.name}").transform;
            root.SetParent(PrefabPoolRoot);
            this.autoActive = autoActive;
            for (int i = 0; i < initCount; i++)
            {
                Free(Allocate());
            }
        }

        public override GameObject Allocate()
        {
            var result = base.Allocate();
            result.transform.SetParent(parent);
            if (autoActive)
                result.gameObject.SetActive(true);
            return result;
        }

        public override void Free(GameObject obj)
        {
            base.Free(obj);
            obj.transform.SetParent(root);
            if (autoActive)
                obj.gameObject.SetActive(false);
        }
        
        public override void Dispose()
        {
            Object.Destroy(root.gameObject);
            CacheStack.Clear();
        }
    }

    public class PrefabPoolWithKey<TKey, TComponent> : PoolWithKey<TKey, TComponent> where TComponent : Component
    {
        public PrefabPoolWithKey(Func<TKey, TComponent> factory, Action<TComponent> onAlloc = null,
            Action<TComponent> onFree = null, Action<TComponent> onDispose = null, Transform parent = null) : base(factory, onAlloc, onFree, onDispose)
        {
            Factory = key =>
            {
                var com = factory(key);
                return Object.Instantiate(com, parent);
            };
        }

        public override TComponent Allocate(TKey key)
        {
            var result = base.Allocate(key);
            result.gameObject.SetActive(true);
            return result;
        }

        public override void Free(TKey key, TComponent obj)
        {
            base.Free(key, obj);
            obj.gameObject.SetActive(false);
        }
    }
    
}