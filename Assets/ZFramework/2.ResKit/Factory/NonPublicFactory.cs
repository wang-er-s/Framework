/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/
using UnityEngine;
using System;
using System.Reflection;

namespace ZFramework
{
    public class NonPublicFactory<T> : IFactory<T> where T : class
    {
        public T Create()
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
            return ctor.Invoke(null) as T;
        }
    }
}
