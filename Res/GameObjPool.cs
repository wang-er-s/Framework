#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UObj = UnityEngine.Object;
//GameObject回收站
namespace Framework
{
	public class GameObjPool : MonoSingleton<GameObjPool>
	{
		#region Def
		private class UnusedGo
		{
			private readonly string assetPath;
			private readonly List<GameObject> gos;
			private float updateTime;

			public bool IsEmpty => gos.Count == 0;

			public UnusedGo(string path)
			{
				assetPath = path;
				gos = new List<GameObject>();
				updateTime = 0f;
			}
			public GameObject Peek()
			{
				if(gos.Count > 0)
				{
					RefreshTime();
					GameObject go = gos[0];
					gos.RemoveAt(0);
					go.transform.SetParent(null, false);
					return go;
				}
				return null;
			}
			public void Push(GameObject go,Transform trans)
			{
				go.transform.SetParent(trans, false);
				gos.Add(go);
				RefreshTime();
			}
			public void Empty()
			{
				for(int i=0;i<gos.Count;++i)
				{
					GameObject go = gos[i];
					Destroy(go);
				}
				BundleMgr.Instance.ReleaseAsset(assetPath,false,gos.Count);
				gos.Clear();
			}
			public bool PeriodClean(float curTime)//周期清理
			{
				if (gos.Count == 0)
					return true;
				if(curTime - updateTime>=CLEAN_TIME)
				{
					Empty();
					return true;
				}
				return false;
			}

			public void RefreshTime()
			{
				updateTime = Time.realtimeSinceStartup;
			}
		}
		#endregion
		#region Variables
		private const float CLEAN_TIME = 30f;//清空超时
		public Transform poolRoot;
		
		private Dictionary<string,UnusedGo> unUsed = new Dictionary<string, UnusedGo>();
        private Dictionary<ulong, GoCallback> goTasks = new Dictionary<ulong, GoCallback>();
        private Dictionary<ulong, IEnumerator> instanters = new Dictionary<ulong, IEnumerator>();
        private List<ulong> onUpdateTasks = new List<ulong>();
        private Transform cachedTrans;
        public Transform CachedTrans { get { if (null == cachedTrans) cachedTrans = transform; return cachedTrans; } }
		#endregion
        
		#region Public Method

		#region Async
		public ulong GetGameObj(string assetPath,OnGameObjGot cb,bool doneOnUpdate = false)
		{
			if (string.IsNullOrEmpty(assetPath))
			{
				cb(null, 0);
				return 0;
			}
			
			ulong taskId = 0;
			if (GetFromPool(assetPath, cb, doneOnUpdate, out taskId))
			{
				return taskId;
			}
            
#if UNITY_EDITOR
			if (!AppEnv.UseBundleInEditor)
			{
				var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
				GameObject tempGo = null==obj?null: Instantiate(obj);
				//if(null!=tempGo)
				//	ApplyShader.CheckShader(tempGo);
				cb(tempGo, 0);
				return 0;
			}
#endif
			return GetByLoad(assetPath, cb, doneOnUpdate);
		}
        //取消获取，reserve标识对于取消加载的对象是缓存到自己的缓冲池
        //还是完全取消对象的加载
        public void CancelUngotGameObj(ulong cbIdx,bool reserve)
        {
            GoCallback gcb = GoCallback.Get(cbIdx);
            if (null == gcb)
                return;
            if(goTasks.ContainsKey(gcb.Id))
            {
                if(reserve)//需要缓存的情况
                {
                    gcb.Cb = (go, cbId) =>
                    {
                        UnuseGameObj(gcb.Path, go);
                    };
                }
                else
                {
                    goTasks.Remove(gcb.Id);
                    GoCallback.Remove(gcb.Id);
                    gcb.Cancel();
                    if (instanters.ContainsKey(gcb.Id))
                    {
                        IEnumerator instanter = instanters[gcb.Id];
                        StopCoroutine(instanter);
                        instanters.Remove(gcb.Id);
                    }
                }
            }
        }
        #endregion

        #region Sync

        public GameObject GetGameObj(string assetPath)
        {
	        GameObject ret = null;
	        if (string.IsNullOrEmpty(assetPath))
		        return ret;
	        ret = GetFromPool(assetPath);
	        if (null != ret)
		        return ret;
	        var uobj = BundleMgr.Instance.GetAssetSync(assetPath) as GameObject;
	        if(null!=uobj)
				ret = Instantiate(uobj);
	        return ret;
        }
        

        #endregion
		public void UnuseGameObj(string assetPath,GameObject go)
		{
			if(!unUsed.ContainsKey(assetPath))
				unUsed.Add(assetPath,new UnusedGo(assetPath));
			UnusedGo one = unUsed[assetPath];
			one.Push(go,poolRoot);
		}
		public void Empty()
		{
			foreach(KeyValuePair<string,UnusedGo> element in unUsed)
			{
				element.Value.Empty();
			}
			unUsed.Clear();
		}
        public bool IsIdle()
        {
            return goTasks.Count == 0;
        }
		public IEnumerator PeriodClean()
		{
			yield return null;
			float curTime = Time.realtimeSinceStartup;
            Dictionary<string, UnusedGo>.Enumerator it = unUsed.GetEnumerator();
            List<string> dels = null;
            while(it.MoveNext())
            {
                if (it.Current.Value.PeriodClean(curTime))
                {
                    if(null == dels)
                        dels = new List<string>();
                    dels.Add(it.Current.Key);
                }
            }
            if (null != dels)
            {
                for (int i = 0; i < dels.Count; ++i)
                    unUsed.Remove(dels[i]);
            }
		}
		public void CheckDoneOnUpdate()
		{
			if(onUpdateTasks.Count == 0)
				return;
			foreach (var taskId in onUpdateTasks)
			{
				if (goTasks.ContainsKey(taskId))
				{
					GoCallback task = goTasks[taskId];
					if (!task.Canceled)
					{
						if (task.Asset!=null)
						{
							DoInstant(taskId,task.Asset);
						}
						else
						{
							GameObject go = DoGetFromUnused(task.Path);
							task.Do(go);
						}
					}
				}        
			}
			onUpdateTasks.Clear();
		}
		#endregion
        #region Private Method

        protected void Awake()
        {
	        if (null == poolRoot)
	        {
		        GameObject go = new GameObject("_unused_");
		        poolRoot = go.transform;
		        poolRoot.SetParentIdentically(transform);
	        }
	        poolRoot.gameObject.SetActive(false);
        }

        private bool GetFromPool(string assetPath,OnGameObjGot cb,bool doneOnUpdate,out ulong taskId)
        {
	        taskId = 0;
	        if (!unUsed.ContainsKey(assetPath))
		        return false;
	        UnusedGo one = unUsed[assetPath];
	        if (one.IsEmpty)
		        return false;
	        if (doneOnUpdate)
	        {
		        GoCallback gcb = GenTask(assetPath, cb);
		        taskId = gcb.Id;
		        onUpdateTasks.Add(taskId);
	        }
	        else
	        {
		        GameObject go = one.Peek();
			    cb(go, 0);
	        }
	        return true;
        }

        private GameObject GetFromPool(string assetPath)
        {
	        if (!unUsed.ContainsKey(assetPath))
		        return null;
	        var one = unUsed[assetPath];
	        return one.IsEmpty ? null : one.Peek();
        }

        private ulong GetByLoad(string assetPath, OnGameObjGot cb, bool doneOnUpdate)
        {
	        GoCallback gcb = GenTask(assetPath, cb);
	        gcb.AssetCbId = BundleMgr.Instance.GetAsset(assetPath, (asset, cbId) =>
	        {
		        gcb.AssetCbId = 0;
		        if (doneOnUpdate)
		        {
			        gcb.Asset = (UObj)asset;
			        onUpdateTasks.Add(gcb.Id);
		        }
		        else
		        {
			        IEnumerator instant = Instant(gcb.Id, (UObj)asset);
			        instanters.Add(gcb.Id, instant);
			        StartCoroutine(instant);    
		        }
	        });
	        return gcb.Id;
        }

        private GoCallback GenTask(string resPath, OnGameObjGot cb)
        {
            GoCallback gcb = GoCallback.Gen(cb,resPath);
            goTasks.Add(gcb.Id, gcb);
            return gcb;
        }
        private IEnumerator Instant(ulong gcbId,UObj asset)
        {
            yield return null;
	        DoInstant(gcbId,asset);
        }

        private GameObject DoGetFromUnused(string assetPath)
        {
	        if (!unUsed.ContainsKey(assetPath))
	        {
		        return null;
	        }
	        UnusedGo one = unUsed[assetPath];
	        if (one.IsEmpty)
	        {
		        return null;
	        }
	        return one.Peek();
        }

        private void DoInstant(ulong gcbId, UObj asset)
        {
	        GameObject go = null;
	        if(null!=asset)
		        go = Instantiate(asset) as GameObject;
//#if UNITY_EDITOR
//	        ApplyShader.CheckShader(go);
//#endif
	        FinishInstant(gcbId, go);
        }
        private void FinishInstant(ulong gcbId, GameObject go)
        {
            if (goTasks.ContainsKey(gcbId))
            {
                GoCallback task = goTasks[gcbId];
                task.Do(go);
                GoCallback.Remove(gcbId);
                goTasks.Remove(gcbId);
            }
            if (instanters.ContainsKey(gcbId))
                instanters.Remove(gcbId);
        }
        #endregion
    }
}