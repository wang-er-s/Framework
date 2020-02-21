/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public abstract class Pool<T> : IPool<T>
    {
        public int CurCount
        {
            get { return _cacheStack.Count; }
        }

        protected IFactory<T> _factory;

        protected readonly Stack<T> _cacheStack = new Stack<T>();

        protected int _maxCount = 12;

        public abstract bool DeSpawn(T obj);

        public virtual T Spawn()
        {
            return _cacheStack.Count == 0 ?
                _factory.Create() : _cacheStack.Pop();
        }
    }
}
