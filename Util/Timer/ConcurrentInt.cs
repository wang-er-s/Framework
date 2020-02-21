/*
* Create by Soso
* Time : 2018-12-11-03 下午
*/

namespace Framework
{
	/// <summary>
	/// 线程安全的int类型
	/// </summary>
	public class ConcurrentInt
	{
		private int value;

		public ConcurrentInt ( int va )
		{
			value = va;
		}

		/// <summary>
		/// 添加并获取
		/// </summary>
		/// <returns></returns>
		public int Add_Get ()
		{
			lock ( this )
			{
				value++;
				return value;
			}
		}

		/// <summary>
		/// 减少并获取
		/// </summary>
		/// <returns></returns>
		public int Reduce_Get ()
		{
			lock ( this )
			{
				value--;
				return value;
			}
		}

		public int Get ()
		{
			return value;
		}

	}
}
