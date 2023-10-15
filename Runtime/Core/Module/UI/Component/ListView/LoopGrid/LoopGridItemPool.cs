using System;
using System.Collections.Generic;
using Framework;
using UnityEngine;


namespace ScrollView
{
    public abstract class LoopGridItemPoolBase : ILoopItemPool<LoopGridItemView>
    {
        public abstract event Action<LoopGridItemView, GameObject> OnRecycleItem;
        public abstract event Action<LoopGridItemView, GameObject> OnGetItem;

        public abstract void Init(GameObject prefabObj, float padding, float startPosOffset, int createCount,
            RectTransform parent);

        public abstract LoopGridItemView GetItem(out GameObject inGo);

        public abstract void RecycleItem(LoopGridItemView @in);

        public abstract void DestroyAllItem();

        public abstract void ClearTmpRecycledItem();
    }


    public class LoopGridItemPool<T> : LoopGridItemPoolBase where T : LoopGridItemView
    {
        private ItemGameObjectPool _gameObjectPool;

        private LoopGridItemViewPool<T> _viewPool;

        public override event Action<LoopGridItemView, GameObject> OnRecycleItem;
        public override event Action<LoopGridItemView, GameObject> OnGetItem;

        public override void Init(GameObject prefabObj, float padding, float startPosOffset, int createCount,
            RectTransform parent)
        {
            _gameObjectPool = new ItemGameObjectPool();
            _viewPool = new LoopGridItemViewPool<T>();
            _gameObjectPool.Init(prefabObj, createCount, parent);
            _viewPool.Init(createCount);
        }


        public override LoopGridItemView GetItem(out GameObject inGo)
        {
            var result = _viewPool.GetItem();
            var go = _gameObjectPool.GetItem();
            inGo = go;
            OnGetItem?.Invoke(result, go);
            return result;
        }

        public override void RecycleItem(LoopGridItemView @in)
        {
            var go = @in.GameObject;
            OnRecycleItem?.Invoke(@in, go);
            _gameObjectPool.RecycleItem(go);
            _viewPool.RecycleItem(@in as T);
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