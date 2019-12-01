/*
* Create by Soso
* Time : 2018-12-26-02 下午
*/
using UnityEngine;

namespace AD
{
	public class MonoSingleton<T>  : MonoBehaviour,ISingleton where T : MonoSingleton<T>
	{
		private static T mInstance = null;
		
		private static readonly object locker = new object ();

		public static T Instance
		{
			get
			{
				lock ( locker )
				{
					if ( null == mInstance )
					{
						mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
					}
				}

				return mInstance;
			}
		}

		public static void Dispose()
		{
			Destroy ( mInstance.gameObject );
			mInstance = null;
		}

		public virtual void OnSingletonInit()
		{
			
		}
	}
}
