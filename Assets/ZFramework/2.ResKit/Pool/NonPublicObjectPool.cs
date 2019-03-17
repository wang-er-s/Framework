/*
* Create by Soso
* Time : 2018-12-27-11 下午
*/
using UnityEngine;
using System;

namespace ZFramework
{
    public class NonPublicObjectPool<T> : Pool<T>  where T : class, IPoolable
    {

        protected NonPublicObjectPool()
        {
            mFactory = new NonPublicFactory<T>();
        }

        public void Init(int maxCount, int initCount)
        {
            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
            }

            if (CurCount >= initCount) return;

            for (int i = CurCount; i < initCount; ++i)
            {
                DeSpawn(mFactory.Create());
            }
        }

        public int MaxCacheCount
        {
            get { return mMaxCount; }
            set
            {
                mMaxCount = value;

                if (mCacheStack == null) return;
                if (mMaxCount <= 0) return;
                if (mMaxCount >= mCacheStack.Count) return;
                int removeCount = mMaxCount - mCacheStack.Count;
                while (removeCount > 0)
                {
                    mCacheStack.Pop();
                    --removeCount;
                }
            }
        }

        public override T Spawn()
        {
            T result = base.Spawn();
            result.IsRecycled = false;
            return result;
        }

        public override bool DeSpawn(T t)
        {
            if (t == null || t.IsRecycled)
            {
                return false;
            }

            if (mMaxCount > 0)
            {
                if (mCacheStack.Count >= mMaxCount)
                {
                    t.OnRecycled();
                    return false;
                }
            }

            t.IsRecycled = true;
            t.OnRecycled();
            mCacheStack.Push(t);

            return true;
        }

    }
}
