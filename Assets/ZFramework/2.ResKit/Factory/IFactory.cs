/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/
using UnityEngine;
using System;

namespace SF
{
    public interface IFactory<T>
    {
        T Create();
    }
}
