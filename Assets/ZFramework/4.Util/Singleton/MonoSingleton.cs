/*
* Create by Soso
* Time : 2018-12-26-02 下午
*/
using UnityEngine;

namespace SF
{
	public class MonoSingleton<T>  : MonoBehaviour where T : MonoBehaviour
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
						mInstance = FindObjectOfType<T>();
                        if (mInstance == null)
                        {
                            mInstance = new GameObject(typeof(T).Name).AddComponent<T>();
                            Object.DontDestroyOnLoad(mInstance.gameObject);
                        }
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
    }
}
