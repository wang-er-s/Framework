/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/
using UnityEngine;
using System;

namespace AD
{
    public class SimpleObjectPool<T> : Pool<T>
    {
        private readonly Action<T> mResetMethod;

        public SimpleObjectPool(Func<T> factoryMethod,Action<T> resetMethod = null,int initCount = 0)
        {
            mFactory = new CustomFactory<T>(factoryMethod);
            mResetMethod = resetMethod;
            for (int i = 0; i < initCount; i++)
            {
                mCacheStack.Push(mFactory.Create());
            }
        }

        public override bool DeSpawn(T obj)
        {
            mResetMethod.InvokeGracefully(obj);
            mCacheStack.Push(obj);
            return true;
        }
    }
}
