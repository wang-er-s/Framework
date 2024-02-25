using System;
using System.Collections.Generic;
using UnityEngine;


namespace SuperScrollView
{
    public class GridItemPool
    {
        private Func<LoopGridViewItem> createFunc;
        string mPrefabName;
        int mInitCreateCount = 1;
        List<LoopGridViewItem> mTmpPooledItemList = new List<LoopGridViewItem>();
        List<LoopGridViewItem> mPooledItemList = new List<LoopGridViewItem>();
        static int mCurItemIdCount = 0;
        RectTransform mItemParent = null;
        public GridItemPool()
        {

        }
        public void Init(Func<LoopGridViewItem> createFunc,string prefabName, int createCount, RectTransform parent)
        {
            this.createFunc = createFunc;
            mPrefabName = prefabName;
            mInitCreateCount = createCount;
            mItemParent = parent;
            for (int i = 0; i < mInitCreateCount; ++i)
            {
                LoopGridViewItem tViewItem = CreateItem();
                RecycleItemReal(tViewItem);
            }
        }
        public LoopGridViewItem GetItem()
        {
            mCurItemIdCount++;
            LoopGridViewItem tItem = null;
            if (mTmpPooledItemList.Count > 0)
            {
                int count = mTmpPooledItemList.Count;
                tItem = mTmpPooledItemList[count - 1];
                mTmpPooledItemList.RemoveAt(count - 1);
                tItem.GameObject.SetActive(true);
            }
            else
            {
                int count = mPooledItemList.Count;
                if (count == 0)
                {
                    tItem = CreateItem();
                }
                else
                {
                    tItem = mPooledItemList[count - 1];
                    mPooledItemList.RemoveAt(count - 1);
                    tItem.GameObject.SetActive(true);
                }
            }
            tItem.ItemId = mCurItemIdCount;
            return tItem;

        }

        public void DestroyAllItem()
        {
            ClearTmpRecycledItem();
            int count = mPooledItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                GameObject.DestroyImmediate(mPooledItemList[i].GameObject);
            }
            mPooledItemList.Clear();
        }
        public LoopGridViewItem CreateItem()
        {
            LoopGridViewItem tViewItem = createFunc();
            GameObject go = tViewItem.GameObject;
            go.transform.SetParent(mItemParent);
            go.transform.localPosition = Vector3.zero;
            go.SetActive(true);
            RectTransform rf = go.GetComponent<RectTransform>();
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            tViewItem.ItemPrefabName = mPrefabName;
            return tViewItem;
        }
        void RecycleItemReal(LoopGridViewItem item)
        {
            item.GameObject.SetActive(false);
            mPooledItemList.Add(item);
        }
        public void RecycleItem(LoopGridViewItem item)
        {
            item.PrevItem = null;
            item.NextItem = null;
            mTmpPooledItemList.Add(item);
        }
        public void ClearTmpRecycledItem()
        {
            int count = mTmpPooledItemList.Count;
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; ++i)
            {
                RecycleItemReal(mTmpPooledItemList[i]);
            }
            mTmpPooledItemList.Clear();
        }
    }
}
