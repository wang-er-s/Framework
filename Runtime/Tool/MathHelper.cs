using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Framework
{
	public static class MathHelper
	{
		private struct MinAndMax
		{
			public int min;
			public int max;

			public MinAndMax(int min, int max)
			{
				this.min = min;
				this.max = max;
			}
		}

		/// <summary>
		/// 根据概率得到随机的索引 索引是从0开始
		/// 传入的是百分之的概率 50代表百分之50
		/// [20,30,50] 总和需要是100
		/// </summary>
		public static int GetRandomIndex(params int[] muls)
		{
			List<MinAndMax> tempList = new List<MinAndMax>(muls.Length);
			for (int i = 0; i < muls.Length; i++)
			{
				int min = 0, max = 0;
				for (int j = 0; j < i; j++)
				{
					min += muls[j];
				}

				max = min + muls[i];
				tempList.Add(new MinAndMax(min, max));
			}

			int rand = Random.Range(1, 101);
			foreach (MinAndMax item in tempList)
			{
				if (rand >= item.min && rand <= item.max)
					return tempList.IndexOf(item);
			}

			return -1;
		}
	}
}