/*
* Create by Soso
* Time : 2018-12-11-03 下午
*/
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;

namespace AD
{

	/// <summary>
	/// 定时器的数据模型
	/// </summary>
	public class TimerModel
	{

		public int Id;

		/// <summary>
		/// 任务执行的时间
		/// </summary>
		public double Time;

		private readonly Action timeEndCall;
		private readonly Action<double> timeCdCall;
		private readonly double startMs; 

		public TimerModel(int id, double time, Action endCall, Action<double> cdCall)
		{
			Id = id;
			Time = time;
			timeEndCall = endCall;
			timeCdCall = cdCall;
			startMs = TimerManager.CurMilliseconds;
		}

		public void End()
		{
			timeEndCall?.Invoke();
		}

		public void CD(double curMs)
		{
			timeCdCall?.Invoke(curMs - curMs);
		}
	}
	
	public class TimerManager
	{
		private static ConcurrentInt id = new ConcurrentInt(-1);

		/// <summary>
		/// 实现定时器的主要功能就是这个Timer类
		/// </summary>
		private static Timer timer;

		/// <summary>
		/// 多线程安全的字典 ： 任务id和任务模型的映射
		/// </summary>
		private static Dictionary<int, TimerModel> idModelDic = new Dictionary<int, TimerModel>();

		/// <summary>
		/// 要移除的任务id列表
		/// </summary>
		private static List<int> removeList = new List<int>();

		private static DateTime baseTime = new DateTime(2000, 1, 1);

		public static double CurMilliseconds
		{
			get { return (DateTime.Now - baseTime).TotalMilliseconds; }
		}

		static TimerManager()
		{
			timer = new Timer(100);
			timer.Elapsed += Timer_Elapsed;
			timer.Enabled = true;
		}

		/// <summary>
		/// 达到时间间隔时触发
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			lock (removeList)
			{
				foreach (var item in removeList)
				{
					idModelDic.Remove(item);
				}
				removeList.Clear();
			}

			foreach (var item in idModelDic.Values)
			{
				double cur = CurMilliseconds;
				if (item.Time <= cur)
				{
					item.End();
					removeList.Add(item.Id);
				}
				else
				{
					item.CD(cur);
				}
			}
		}

		/// <summary>
		/// 添加定时任务，执行触发的时间
		/// </summary>
		public static void AddTimerEvent(DateTime dateTime, Action endCall = null, Action<double> cdCall = null)
		{
			double delayTime = (dateTime - DateTime.Now).TotalMilliseconds;
			if (delayTime <= 0)
				return;
			AddTimerEvent(delayTime, endCall, cdCall);
		}

		/// <summary>
		/// 添加定时任务，指定延迟的时间
		/// </summary>
		public static void AddTimerEvent(double delayTime, Action endCall = null, Action<double> cdCall = null)
		{
			TimerModel model = new TimerModel(id.Add_Get(), CurMilliseconds + delayTime, endCall, cdCall);
			idModelDic.Add(model.Id, model);
		}
	}
}
