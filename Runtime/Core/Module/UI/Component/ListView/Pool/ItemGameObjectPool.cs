using System.Collections.Generic;
using UnityEngine;

namespace ScrollView
{
    /// <summary>
    /// GameObjectPool
    /// </summary>
    public class ItemGameObjectPool
    {
        GameObject mPrefabObj;
        string mPrefabName;
        int mInitCreateCount = 1;
        float mPadding = 0;
        float mStartPosOffset = 0;
        List<GameObject> mTmpPooledItemList = new List<GameObject>();
        List<GameObject> mPooledItemList = new List<GameObject>();
        static int mCurItemIdCount = 0;
        RectTransform mItemParent = null;

        public void Init(GameObject prefabObj, int createCount,
            RectTransform parent)
        {
            mPrefabObj = prefabObj;
            mPrefabName = mPrefabObj.name;
            mInitCreateCount = createCount;
            mItemParent = parent;
            mPrefabObj.SetActive(false);
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


        public GameObject GetItem()
        {
            mCurItemIdCount++;
            GameObject tItem = null;
            if (mTmpPooledItemList.Count > 0)
            {
                int count = mTmpPooledItemList.Count;
                tItem = mTmpPooledItemList[count - 1];
                mTmpPooledItemList.RemoveAt(count - 1);
                tItem.SetActive(true);
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
                    tItem.SetActive(true);
                }
            }

            return tItem;
        }

        public void DestroyAllItem()
        {
            ClearTmpRecycledItem();
            int count = mPooledItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                GameObject.DestroyImmediate(mPooledItemList[i]);
            }

            mPooledItemList.Clear();
        }

        public GameObject CreateItem()
        {
            GameObject go =
                GameObject.Instantiate<GameObject>(mPrefabObj, Vector3.zero, Quaternion.identity, mItemParent);
            go.SetActive(true);
            RectTransform rf = go.GetComponent<RectTransform>();
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            return go;
        }

        void RecycleItemReal(GameObject item)
        {
            item.SetActive(false);
            mPooledItemList.Add(item);
        }

        public void RecycleItem(GameObject item)
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