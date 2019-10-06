/*
* Create by Soso
* Time : 2018-12-31-01 下午
*/
using UnityEngine;
using System;

namespace Nine
{
	public class DontDestoryOnLoad  : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}
