using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	[AddComponentMenu("FrameWork/BundleLoader")]
	public class BundleLoader : MonoSingleton<BundleLoader>
	{
		#region Sub Class
		private class ProgressLoadTask
		{
			private List<BundleHolder> loadingHolders = null;
			public ProgressLoadTask(List<BundleHolder> _loadings)
			{
				loadingHolders = _loadings;
			}
			public float GetProgress(ref bool isAllLoaded)
			{
				isAllLoaded = true;
				if(null == loadingHolders||loadingHolders.Count == 0)
					return 1f;
				float progress = 0f;
				for(int i=0;i<loadingHolders.Count;++i)
				{
					progress += loadingHolders[i].SelfLoadingProgress;
					if(!loadingHolders[i].IsLoaded())
						isAllLoaded = false;
				}
				return progress/loadingHolders.Count;
			}
		}
		#endregion
		#region Variables
		private ProgressLoadTask progressTask = null;//唯一的进度任务
		private float clearDeltaTime = 0;
		private const float CLEAR_PERIOD = 5f;//无用资源的存在时间超过这个时间的才删除，
		private const int BUNDLE_TASK_MAX = 10;
		private int runningCount = 0;
		private List<BundleLoadTask> runningTasks = new List<BundleLoadTask>();
		private Queue<BundleLoadTask> waitingTasks = new Queue<BundleLoadTask>();
		#endregion
		#region Public Method
		public void StartProgressTask(List<BundleHolder> loadingHoladers)
		{
			if(null == progressTask)
				progressTask = new ProgressLoadTask(loadingHoladers);
		}
		public void StartLoadTask(BundleLoadTask task)
		{
			waitingTasks.Enqueue(task);
		}
		#endregion
		#region Private Method

		private void RunTask(BundleLoadTask task)
		{
			if(null!=task)
			{
				task.Start();
				++runningCount;
				runningTasks.Add(task);
			}
		}

		private void CheckRunning()
		{
			for (int i = 0; i < runningCount;)
			{
				BundleLoadTask task = runningTasks[i];
				if (task.IsFinished)
				{
					runningTasks.RemoveAt(i);
					--runningCount;
				}
				else
				{
					++i;
				}
			}

			while (waitingTasks.Count>0&&runningCount<BUNDLE_TASK_MAX)
			{
				RunTask(waitingTasks.Dequeue());
			}
		}
		#endregion
		void Update ()
		{
			CheckRunning();
			GameObjPool.Ins.CheckDoneOnUpdate();
			if(null!=progressTask)
			{
				bool isAllLoaded = false;
				float progress = progressTask.GetProgress(ref isAllLoaded);
				//LuaEntrance.Instance.OnProgressLoad(progress);
				if(isAllLoaded)
					progressTask = null;
			}
			//关闭尝试只是在切换场景的时候delete
			float deltaTime = Time.deltaTime;
			clearDeltaTime += deltaTime;
			if(clearDeltaTime>CLEAR_PERIOD)
			{
				clearDeltaTime = 0f;
				StartCoroutine(GameObjPool.Ins.PeriodClean());
				StartCoroutine(BundleMgr.Instance.TryDelete());
			}
		}
	}
}
