/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SF
{
    public abstract class Pool<T> : IPool<T>
    {
        public int CurCount
        {
            get { return mCacheStack.Count; }
        }

        protected IFactory<T> mFactory;

        protected readonly Stack<T> mCacheStack = new Stack<T>();

        protected int mMaxCount = 12;

        public abstract bool DeSpawn(T obj);

        public virtual T Spawn()
        {
            return mCacheStack.Count == 0 ?
                mFactory.Create() : mCacheStack.Pop();
        }
    }
}
