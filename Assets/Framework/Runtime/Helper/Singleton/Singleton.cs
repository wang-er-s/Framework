using System;

namespace Framework
{
	public class Singleton<T> where T : class
	{
		private static readonly object locker = new object ();

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
							instance = Activator.CreateInstance<T>();
						}
					}
				}

				return instance;
			}
		}
	}
}
