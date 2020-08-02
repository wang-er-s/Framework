using System;

namespace Framework
{
	public static class SingletonProperty<T> where T : class
	{
		private static T _instance;
		private static readonly object _lock = new object();

		public static T Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = Activator.CreateInstance<T>();
					}
				}

				return _instance;
			}
		}

		public static void Dispose()
		{
			_instance = null;
		}
	}
}