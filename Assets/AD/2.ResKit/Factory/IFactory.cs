/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/
using UnityEngine;
using System;

namespace AD
{
    public interface IFactory<T>
    {
        T Create();
    }
}
