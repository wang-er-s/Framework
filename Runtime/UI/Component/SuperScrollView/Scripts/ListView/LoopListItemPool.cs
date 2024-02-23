﻿using System;
using System.Collections.Generic;
using UnityEngine;


namespace SuperScrollView
{
    public class ItemPool
    {
        private Func<LoopListViewItem2> createFunc;
        string mPrefabName;
        int mInitCreateCount = 1;
        float mPadding = 0;
        float mStartPosOffset = 0;
        List<LoopListViewItem2> mTmpPooledItemList = new List<LoopListViewItem2>();
        List<LoopListViewItem2> mPooledItemList = new List<LoopListViewItem2>();
        static int mCurItemIdCount = 0;
        RectTransform mItemParent = null;
        public ItemPool()
        {

        }
        public void Init(Func<LoopListViewItem2> createFunc,string prefabName, float padding, float startPosOffset, int createCount, RectTransform parent)
        {
            this.createFunc = createFunc;
            mPrefabName = prefabName;
            mInitCreateCount = createCount;
            mPadding = padding;
            mStartPosOffset = startPosOffset;
            mItemParent = parent;
            for (int i = 0; i < mInitCreateCount; ++i)
            {
                LoopListViewItem2 tViewItem = CreateItem();
                RecycleItemReal(tViewItem);
            }
        }
        public LoopListViewItem2 GetItem()
        {
            mCurItemIdCount++;
            LoopListViewItem2 tItem = null;
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
            tItem.Padding = mPadding;
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
        public LoopListViewItem2 CreateItem()
        {
            LoopListViewItem2 tViewItem = createFunc();
            GameObject go = tViewItem.GameObject;
            go.transform.SetParent(mItemParent);
            go.transform.localPosition = Vector3.zero;
            go.SetActive(true);
            RectTransform rf = go.GetComponent<RectTransform>();
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            tViewItem.ItemPrefabName = mPrefabName;
            tViewItem.StartPosOffset = mStartPosOffset;
            return tViewItem;
        }
        void RecycleItemReal(LoopListViewItem2 item)
        {
            item.GameObject.SetActive(false);
            mPooledItemList.Add(item);
        }
        public void RecycleItem(LoopListViewItem2 item)
        {
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
