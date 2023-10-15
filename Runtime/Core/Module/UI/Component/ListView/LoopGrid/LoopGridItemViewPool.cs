using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScrollView
{
    public class LoopGridItemViewPool<T> where T : LoopGridItemView
    {
        int mInitCreateCount = 1;
        List<T> mTmpPooledItemList = new List<T>();
        List<T> mPooledItemList = new List<T>();
        static int mCurItemIdCount = 0;

        public void Init(int createCount)
        {
            mInitCreateCount = createCount;
            PreloadPool();
        }

        private void PreloadPool()
        {
            for (int i = 0; i < mInitCreateCount; ++i)
            {
                var tViewItem = CreateItem();
                RecycleItemReal(tViewItem);
            }
        }


        public T GetItem()
        {
            mCurItemIdCount++;
            T tItem = null;
            if (mTmpPooledItemList.Count > 0)
            {
                int count = mTmpPooledItemList.Count;
                tItem = mTmpPooledItemList[count - 1];
                mTmpPooledItemList.RemoveAt(count - 1);
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
                }
            }

            tItem.ItemId = mCurItemIdCount;
            return tItem;
        }

        public void DestroyAllItem()
        {
            ClearTmpRecycledItem();
            mPooledItemList.Clear();
        }

        public T CreateItem()
        {
            return Activator.CreateInstance<T>();
        }

        void RecycleItemReal(T item)
        {
            mPooledItemList.Add(item);
        }

        public void RecycleItem(T item)
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