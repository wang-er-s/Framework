using System.Collections;
using System.Collections.Generic;
using ScrollView;
using UnityEngine;

namespace ScrollView
{
    public abstract class LoopGridItemView : LoopItemView
    {
        // indicates the item’s index in the list the mItemIndex can only be from 0 to itemTotalCount -1.


        // the row index, the item is in. starting from 0.
        int mRow = -1;

        // the column index, the item is in. starting from 0.
        int mColumn = -1;

        //indicates the item’s id. 
        //This property is set when the item is created or fetched from pool, 
        //and will no longer change until the item is recycled back to pool.

        LoopGrid _parentGrid = null;
        bool mIsInitHandlerCalled = false;
        string mItemPrefabName;
        int mItemCreatedCheckFrameCount = 0;
        object mUserObjectData = null;
        int mUserIntData1 = 0;
        int mUserIntData2 = 0;
        string mUserStringData1 = null;
        string mUserStringData2 = null;

        LoopGridItemView _prevViewItem;
        LoopGridItemView _nextViewItem;

        public object UserObjectData
        {
            get { return mUserObjectData; }
            set { mUserObjectData = value; }
        }

        public int UserIntData1
        {
            get { return mUserIntData1; }
            set { mUserIntData1 = value; }
        }

        public int UserIntData2
        {
            get { return mUserIntData2; }
            set { mUserIntData2 = value; }
        }

        public string UserStringData1
        {
            get { return mUserStringData1; }
            set { mUserStringData1 = value; }
        }

        public string UserStringData2
        {
            get { return mUserStringData2; }
            set { mUserStringData2 = value; }
        }

        public int ItemCreatedCheckFrameCount
        {
            get { return mItemCreatedCheckFrameCount; }
            set { mItemCreatedCheckFrameCount = value; }
        }


        public string ItemPrefabName
        {
            get { return mItemPrefabName; }
            set { mItemPrefabName = value; }
        }

        public int Row
        {
            get { return mRow; }
            set { mRow = value; }
        }

        public int Column
        {
            get { return mColumn; }
            set { mColumn = value; }
        }


        public bool IsInitHandlerCalled
        {
            get { return mIsInitHandlerCalled; }
            set { mIsInitHandlerCalled = value; }
        }

        public LoopGrid ParentGrid
        {
            get { return _parentGrid; }
            set { _parentGrid = value; }
        }

        public LoopGridItemView PrevItem
        {
            get { return _prevViewItem; }
            set { _prevViewItem = value; }
        }

        public LoopGridItemView NextItem
        {
            get { return _nextViewItem; }
            set { _nextViewItem = value; }
        }
    }
}