/*
* Create by Soso
* Time : 2018-12-11-03 下午
*/
using UnityEngine;
using System;
using System.Reflection;

namespace AD
{
	public class Singleton<T> where T : class
	{
		private static readonly object locker = new object ();

		public static readonly Type[] EmptyTypes = new Type[ 0 ];

		private static T instance;

		public static T Ins
		{
			get
			{
				lock ( locker )
				{
					if ( instance == null )
					{
						lock ( locker )
						{
							instance = SingletonCreator.CreateSingleton<T>();
						}
					}
				}

				return instance;
			}
		}
	}
}
