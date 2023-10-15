using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;


namespace ScrollView
{
    public abstract class LoopListItemPoolBase : ILoopItemPool<LoopListItemView>
    {
        public abstract event Action<LoopListItemView, GameObject> OnRecycleItem;
        public abstract event Action<LoopListItemView, GameObject> OnGetItem;

        public abstract void Init(GameObject prefabObj, float padding, float startPosOffset, int createCount,
            RectTransform parent);

        public abstract LoopListItemView GetItem(out GameObject outGo);

        public abstract void RecycleItem(LoopListItemView inView);

        public abstract void DestroyAllItem();

        public abstract void ClearTmpRecycledItem();
    }

    public class LoopListItemPool<T> : LoopListItemPoolBase where T : LoopListItemView
    {
        private ItemGameObjectPool _gameObjectPool;

        private LoopListItemViewPool<T> _viewPool;

        public override event Action<LoopListItemView, GameObject> OnRecycleItem;
        public override event Action<LoopListItemView, GameObject> OnGetItem;

        public override void Init(GameObject prefabObj, float padding, float startPosOffset, int createCount,
            RectTransform parent)
        {
            _gameObjectPool = new ItemGameObjectPool();
            _viewPool = new LoopListItemViewPool<T>();
            _gameObjectPool.Init(prefabObj, createCount, parent);
            _viewPool.Init(createCount, padding, startPosOffset);
        }


        public override LoopListItemView GetItem(out GameObject outGo)
        {
            var result = _viewPool.GetItem();
            var go = _gameObjectPool.GetItem();
            outGo = go;
            OnGetItem?.Invoke(result, go);
            return result;
        }

        public override void RecycleItem(LoopListItemView inView)
        {
            var go = inView.GameObject;
            OnRecycleItem?.Invoke(inView, go);
            _gameObjectPool.RecycleItem(go);
            _viewPool.RecycleItem(inView as T);
        }

        public override void DestroyAllItem()
        {
            _gameObjectPool.DestroyAllItem();
            _viewPool.DestroyAllItem();
        }

        public override void ClearTmpRecycledItem()
        {
            _gameObjectPool.ClearTmpRecycledItem();
            _viewPool.ClearTmpRecycledItem();
        }
    }
}