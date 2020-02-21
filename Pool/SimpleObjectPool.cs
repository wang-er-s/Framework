/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/
using UnityEngine;
using System;

namespace Framework
{
    public class SimpleObjectPool<T> : Pool<T>
    {
        private readonly Action<T> _resetMethod;

        public SimpleObjectPool(Func<T> factoryMethod,Action<T> resetMethod = null,int initCount = 0)
        {
            _factory = new CustomFactory<T>(factoryMethod);
            _resetMethod = resetMethod;
            for (int i = 0; i < initCount; i++)
            {
                _cacheStack.Push(_factory.Create());
            }
        }

        public override bool DeSpawn(T obj)
        {
            _resetMethod.InvokeGracefully(obj);
            _cacheStack.Push(obj);
            return true;
        }
    }
}
