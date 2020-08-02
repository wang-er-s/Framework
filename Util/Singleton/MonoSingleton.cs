/*
* Create by Soso
* Time : 2018-12-26-02 下午
*/
using UnityEngine;

namespace Framework
{
	public class MonoSingleton<T>  : MonoBehaviour where T : MonoSingleton<T>
	{
		private static T _instance = null;
		
		private static readonly object _locker = new object ();

		public static T Ins
		{
			get
			{
				lock ( _locker )
				{
					if ( null == _instance )
					{
						_instance = MonoSingletonCreator.CreateMonoSingleton<T>();
					}
				}

				return _instance;
			}
		}

		public static void Dispose()
		{
			Destroy ( _instance.gameObject );
			_instance = null;
		}

		public virtual void OnSingletonInit()
		{
			
		}
	}
}
