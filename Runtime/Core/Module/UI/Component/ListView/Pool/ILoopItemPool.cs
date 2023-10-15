using System;
using UnityEngine;

namespace ScrollView
{
    interface ILoopItemPool<T> where T : LoopItemView
    {
        public event Action<T, GameObject> OnGetItem;

        public event Action<T, GameObject> OnRecycleItem;

        public void Init(GameObject prefabObj, float padding, float startPosOffset, int createCount,
            RectTransform parent);

        public T GetItem(out GameObject inGo);

        public void RecycleItem(T inView);

        public void ClearTmpRecycledItem();

        public void DestroyAllItem();
    }
}