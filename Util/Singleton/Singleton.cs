/*
* Create by Soso
* Time : 2018-12-11-03 下午
*/
using UnityEngine;
using System;
using System.Reflection;

namespace Framework
{
	public class Singleton<T> where T : class
	{
		private static readonly object _locker = new object ();

		public static readonly Type[] EmptyTypes = new Type[ 0 ];

		private static T _instance;

		public static T Ins
		{
			get
			{
				lock ( _locker )
				{
					if ( _instance == null )
					{
						lock ( _locker )
						{
							_instance = SingletonCreator.CreateSingleton<T>();
						}
					}
				}

				return _instance;
			}
		}
	}
}
