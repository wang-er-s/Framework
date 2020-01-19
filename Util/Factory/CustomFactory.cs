/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/
using UnityEngine;
using System;

namespace Framework
{
    public class CustomFactory<T> : IFactory<T>
    {

        public CustomFactory(Func<T> func)
        {
            mAllocMethod = func;
        }

        protected Func<T> mAllocMethod;

        public T Create()
        {
            return mAllocMethod();
        }
    }
}
