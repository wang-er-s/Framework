using System;

namespace Framework
{
	public static class SingletonProperty<T> where T : class
	{
		private static T instance;
		private static readonly object @lock = new object();

		public static T Instance
		{
			get
			{
				lock (@lock)
				{
					if (instance == null)
					{
						instance = Activator.CreateInstance<T>();
					}
				}

				return instance;
			}
		}

		public static void Dispose()
		{
			instance = null;
		}
	}
}