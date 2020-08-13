using UnityEngine;

namespace Framework
{
	public class MonoSingleton<T>  : MonoBehaviour where T : MonoSingleton<T>
	{
		private static T instance = null;
		
		private static readonly object locker = new object ();

		public static T Ins
		{
			get
			{
				lock ( locker )
				{
					if ( null == instance )
					{
						instance = MonoSingletonCreator.CreateMonoSingleton<T>();
					}
				}

				return instance;
			}
		}

		public static void Dispose()
		{
			Destroy ( instance.gameObject );
			instance = null;
		}
	}
}
