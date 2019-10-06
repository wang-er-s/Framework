/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/
using UnityEngine;
using System;

namespace Nine
{
	public interface IPool<T>
	{
        T Spawn();

        bool DeSpawn(T obj);

	}
}
