/*
* Create by Soso
* Time : 2018-12-31-01 下午
*/
using UnityEngine;
using System;

namespace Framework
{
	public class DontDestoryOnLoad  : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}
