/*
* Create by Soso
* Time : 2018-12-27-11 下午
*/
using UnityEngine;
using System;

namespace Nine
{

    /// <summary>
    /// I pool able.
    /// </summary>
    public interface IPoolable
    {
        void OnRecycled();
        bool IsRecycled { get; set; }
    }

    public class SafeObjectPool<T> : Pool<T> where T : IPoolable, new()
    {

        protected SafeObjectPool()
        {
            mFactory = new DefaultFactory<T>();
        }

        public void Init(int maxCount, int initCount)
        {
            MaxCacheCount = maxCount;

            if (maxCount > 0)
            {
                initCount = Math.Min(maxCount, initCount);
            }

            if (CurCount < initCount)
            {
                for (int i = CurCount; i < initCount; ++i)
                {
                    DeSpawn(new T());
                }
            }
        }

        public int MaxCacheCount
        {
            get { return mMaxCount; }
            set
            {
                mMaxCount = value;

                if (mCacheStack != null)
                {
                    if (mMaxCount > 0)
                    {
                        if (mMaxCount < mCacheStack.Count)
                        {
                            int removeCount = mMaxCount - mCacheStack.Count;
                            while (removeCount > 0)
                            {
                                mCacheStack.Pop();
                                --removeCount;
                            }
                        }
                    }
                }
            }
        }

        public override T Spawn()
        {
            T result = base.Spawn();
            result.IsRecycled = false;
            return result;
        }

        public override bool DeSpawn(T obj)
        {
            if (obj == null || obj.IsRecycled)
            {
                return false;
            }

            if (mMaxCount > 0)
            {
                if (mCacheStack.Count >= mMaxCount)
                {
                    obj.OnRecycled();
                    return false;
                }
            }

            obj.IsRecycled = true;
            obj.OnRecycled();
            mCacheStack.Push(obj);

            return true;
        }
    }
}
