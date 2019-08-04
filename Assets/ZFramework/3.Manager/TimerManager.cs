/*
* Create by Soso
* Time : 2018-12-11-03 下午
*/
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;

namespace SF
{
	/// <summary>
	/// 当定时器达到时间后的触发
	/// </summary>
	public delegate void TimerDelegate();

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

		private TimerDelegate timeEndHandler;

		public TimerModel(int id, double time, TimerDelegate td)
		{
			Id             = id;
			Time           = time;
			timeEndHandler = td;
		}

		public void Run()
		{
			timeEndHandler.InvokeGracefully();
		}
	}
	
	public class TimerManager  : Singleton<TimerManager>
	{
		private ConcurrentInt id = new ConcurrentInt(-1);

		/// <summary>
		/// 实现定时器的主要功能就是这个Timer类
		/// </summary>
		private Timer timer;

		/// <summary>
		/// 多线程安全的字典 ： 任务id和任务模型的映射
		/// </summary>
		private Dictionary<int, TimerModel> idModelDic = new Dictionary<int, TimerModel>();

		/// <summary>
		/// 要移除的任务id列表
		/// </summary>
		private List<int> removeList = new List<int>();

		private DateTime baseTime = new DateTime(2000, 1, 1);

		private TimerManager()
		{
			timer         =  new Timer(100);
			timer.Elapsed += Timer_Elapsed;
			timer.Enabled =  true;
		}

		/// <summary>
		/// 达到时间间隔时触发
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
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
				if (item.Time <= (DateTime.Now-baseTime).TotalSeconds)
				{
					item.Run();
					removeList.Add(item.Id);
				}
			}
		}

		/// <summary>
		/// 添加定时任务，执行触发的时间
		/// </summary>
		public void AddTimerEvent(DateTime dateTime, TimerDelegate timerDelegate)
		{
			double delayTime =((dateTime - DateTime.Now).TotalSeconds);
			if (delayTime <= 0)
				return;
			AddTimerEvent(delayTime, timerDelegate);
		}

		/// <summary>
		/// 添加定时任务，指定延迟的时间
		/// </summary>
		/// <param name="delayTime">秒</param>
		/// <param name="timerDelegate"></param>
		public void AddTimerEvent(double delayTime, TimerDelegate timerDelegate)
		{
			TimerModel model = new TimerModel(id.Add_Get(), (DateTime.Now-baseTime).TotalSeconds + delayTime, timerDelegate);
			idModelDic.Add(model.Id, model);
		}
	}
}
