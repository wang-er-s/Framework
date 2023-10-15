using Framework;
using UnityEngine;

namespace ScrollView
{
    public abstract class LoopItemView : View

    {
        int mItemIndex = -1;
        int mItemId = -1;
        RectTransform mCachedRectTransform;

        public RectTransform CachedRectTransform
        {
            get
            {
                if (GameObject == null)
                {
                    Log.Error("Go is null");
                }

                if (mCachedRectTransform == null)
                {
                    mCachedRectTransform = GameObject.GetComponent<RectTransform>();
                }

                return mCachedRectTransform;
            }
        }

        public int ItemIndex
        {
            get { return mItemIndex; }
            set { mItemIndex = value; }
        }

        public int ItemId
        {
            get { return mItemId; }
            set { mItemId = value; }
        }
    }
}