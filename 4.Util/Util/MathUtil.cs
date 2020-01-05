/*
* Create by Soso
* Time : 2018-12-11-11 上午
*/
using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AD
{
	public static class MathUtil  
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
			//System.Random rand = new System.Random(System.DateTime.Now.)
			int rand = UnityEngine.Random.Range(1, 101);
			foreach (MinAndMax item in tempList)
			{
				if (rand >= item.min && rand <= item.max)
					return tempList.IndexOf(item);
			}
        
			return -1;
		}
		
		/// <summary>
		/// 传入数组，得到一个随机的数据
		/// </summary>
		public static T GetRandomValue<T> ( T[] values )
		{
			return values[ Random.Range ( 0, values.Length ) ];
		}
		
		/// <summary>
		/// 传入List，得到一个随机的数据
		/// </summary>
		public static T GetRandomValue<T> ( List<T> values )
		{
			return GetRandomValue ( values.ToArray () );
		}

	}
}
