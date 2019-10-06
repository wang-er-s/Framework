/*
* Create by Soso
* Time : 2018-12-11-03 下午
*/
using UnityEngine;
using System;
using System.Reflection;

namespace Nine
{
	public class Singleton<T> where T : class
	{
		private static readonly object locker = new object ();

		public static readonly Type[] EmptyTypes = new Type[ 0 ];

		private static T instance;

		public static T Instance
		{
			get
			{
				lock ( locker )
				{
					if ( instance == null )
					{
						lock ( locker )
						{
							ConstructorInfo ci =
								typeof ( T ).GetConstructor ( BindingFlags.NonPublic | BindingFlags.Instance, null,
								                              EmptyTypes, null );
							if ( ci == null )
							{
								throw new InvalidOperationException ( "class must contain a private constructor" );
							}

							instance = (T) ci.Invoke ( null );
						}
					}
				}

				return instance;
			}
		}
	}
}
