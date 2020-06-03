using UnityEngine;
using System.Collections;
using UObj = UnityEngine.Object;
using System.Collections.Generic;
using System.IO;
using System;
using Framework.BaseUtil;
using Framework.Util;

namespace Framework
{
	public abstract class BundleHolder
	{
		#region Def
		public enum AssetHolderKind
		{
			ZERO,ONE,MULTI,
		}
		#endregion
		#region Variables
		private static readonly int MAX_RETRY_TIMES = 5;//用于限制失败重试加载的次数
		protected BundleInfo info = null;
		protected AssetBundle bundle = null;
		
		private BundleMgr mgr = null;

		private int count = 0;
		private int retryTimes = 0;
		private float noRefTime = 0f;//没有引用的时间

        private List<AssetCallback> cbs = new List<AssetCallback>();
		private bool isLoading = false;
		private BundleLoadTask loadTask = null;
        private int waitDependNum;
        private Dictionary<ulong,string> waitDependCbIds = null;
		#endregion
		#region Properties
        public virtual object MyAsset
        {
            get { return null; }
        }

		public abstract AssetHolderKind holderKind { get; }
		public BundleInfo Info {get {return info;}}
		public AssetBundle MyBundle 
		{
			set 
			{	
				bundle = value;
				if(null == bundle)
					this.Warning($"warning: {info.path} bundle is null");
			}
			get {return bundle;}
		}
		public float SelfLoadingProgress
		{
			get
			{
				if(this.IsLoaded())
					return 1f;
				if(null != this.loadTask)
					return loadTask.GetProgress();
				return 0f;
			}
		}
		public float NoRefTime
		{
			get {return noRefTime;}
		}
		public int RefCount {get {return count;}}
		#endregion
		#region Private Method
		private void StartLoad()
		{
			if (AppEnv.ResVerbose)
				this.Msg($"start loading {PathIdProfile.Ins.GetPath(info.path)}");
			retryTimes = 0;
			isLoading = true;
            waitDependNum = null == info.depends ? 0 : info.depends.Length;
            if(waitDependNum>0)
            {
                waitDependCbIds = new Dictionary<ulong, string>();
                for(int i=0,iCount = waitDependNum;i<iCount;++i)
                {
                    ulong cbId = mgr.GetAsset(info.depends[i], OnDependAssetGot);
                    if (cbId > 0)
                        waitDependCbIds.Add(cbId,info.depends[i]);
                }
            }
			else
			    TryLoadSelf();
		}
        private void OnDependAssetGot(System.Object asset,ulong cbId)
        {
            if(cbId>0&&null!=waitDependCbIds)
                waitDependCbIds.Remove(cbId);
            --waitDependNum;
            if (waitDependNum == 0)
            {
                waitDependCbIds = null;
                TryLoadSelf();
            }
        }
		private void TryLoadSelf()
		{
			if(this.IsLoaded()||null!=loadTask||null == BundleLoader.Ins)//有可能出现，因为通过depend的回调有可能先执行到这，然后在StartLoaded执行到这
				return;
			loadTask = new BundleLoadTask(this);
			BundleLoader.Ins.StartLoadTask(loadTask);
		}
		#endregion
		#region Public Method
        public static BundleHolder Gen(BundleInfo bi,BundleMgr _mgr)
        {
            if (bi.type == AssetType.MULTI_ASSETS || bi.type == AssetType.SHADER_COMPOSE)
                return new BundleHolderMulti(bi, _mgr);
	        else if(bi.type == AssetType.SCENE)
		        return new BundleHolderZero(bi,_mgr);
            return new BundleHolderOne(bi, _mgr);
        }

		protected BundleHolder(BundleInfo bi,BundleMgr _mgr)
		{
			info = bi;
			mgr = _mgr;
		}
		public abstract bool IsLoaded();

		public bool IsAsyncLoading()
		{
			return isLoading;
		}
		public void Ref()
		{
			if(count == 0)
				mgr.NoDelete(this);
			++count;
			if(this.IsLoaded()||this.isLoading)
			{
				if(null!=info.depends)
				{
					for(int i=0;i<info.depends.Length;++i)
						mgr.RefAsset(info.depends[i]);
				}
			}
			if(this.IsLoaded())
				this.OnLoaded();
			else if(!this.isLoading)
				this.StartLoad();
			if (AppEnv.ResVerbose)
				this.Msg($"ref bundle = {PathIdProfile.Ins.GetPath(info.path)},count = {count}");
		}
		public ulong RefBy(OnAssetGot cb)
		{
            if (this.IsLoaded())
            {
	            Ref();
	            DoCallback(cb);
                return 0;
            }
            else
            {
                AssetCallback acb = AssetCallback.Gen(cb,info.path);
                cbs.Add(acb);
                Ref();
                return acb.Id;
            }
		}
		public void UnRefBy(ulong cbId)
		{
            int ret = -1;
            for (int i = 0; i < cbs.Count;++i )
            {
                if(cbs[i].Id == cbId)
                {
                    ret = i;
                    break;
                }
            }
            if(ret>=0)
            {
                AssetCallback.Remove(cbs[ret].Id);
                cbs.RemoveAt(ret);
                UnRef(true);
            }
		}
		public void UnRef(bool delNow)
		{
			if(count <= 0)
				return;
			--count;
			if(AppEnv.ResVerbose)
				this.Msg($"unref bundle = {PathIdProfile.Ins.GetPath(info.path)},count = {count}");
			if(null!=info.depends)
			{
                List<string> unDone = null;
                if(null!=waitDependCbIds)
                {
                    unDone = new List<string>();
                    Dictionary<ulong,string>.Enumerator it = waitDependCbIds.GetEnumerator();
                    while(it.MoveNext())
                    {
                        unDone.Add(it.Current.Value);
                        mgr.CancelUngotAsset(it.Current.Key);
                    }
                }
				for(int i=0;i<info.depends.Length;++i)
                {
                    if(null==unDone||!unDone.Contains(info.depends[i]))
                        mgr.ReleaseAsset(info.depends[i], delNow);
                }
			}
			if(0==count)
			{
				noRefTime = Time.realtimeSinceStartup;
				if(delNow)
					Unload();
				else
				{
					//Debugger.Log("unref to 0 bundle {0}",info.path);
					mgr.AddDelete(this);
				}
			}
		}
		public void Unload()
		{
			if (AppEnv.ResVerbose)
				this.Msg($"unload bundle {PathIdProfile.Ins.GetPath(info.path)}");
			if(this.isLoading)
			{
				if(null!=loadTask)
				{
					loadTask.Cancel();
					loadTask = null;
				}
				this.isLoading = false;
			}
			else
			{
                ReleaseAsset();
				if(null!=bundle)
				{
					if(AppEnv.ResVerbose)
						this.Msg($"bundle unloaded:{PathIdProfile.Ins.GetPath(info.path)}");
					bundle.Unload(true);
					bundle = null;
				}
			}
			mgr.RemoveHolder(info.path);
		}
		private void OnLoaded()
		{
			int num = cbs.Count;
			while(num>0)
			{
                AssetCallback acb = cbs[num - 1];
                AssetCallback.Remove(acb.Id);
				cbs.RemoveAt(num-1);
				DoAssetCallback(acb);
				num = cbs.Count;
			}
		}
		/// <summary>
		/// 获取所有相关的holder，注意并不ref
		/// </summary>
		public void GetAllRelativeHolders(Dictionary<string,BundleHolder> holders)
		{
			if(!holders.ContainsKey(info.path))
				holders[info.path] = this;
			if(null!=info.depends)
			{
				for(int i=0;i<info.depends.Length;++i)
				{
					BundleHolder holder = mgr.GetHolder(info.depends[i]);
					if(null!=holder)
						holder.GetAllRelativeHolders(holders);
				}
			}
		}
		protected void OnAssetSet(bool retry = false)
		{
			loadTask = null;//清除任务
			bool needRetry = false;
			if(IsLoaded())
			{
				//UObj.DontDestroyOnLoad(asset);//它的删除自己管理
				if(retryTimes>0)
					this.Warning($"{retryTimes} times load {info.path} success");
			}
			else
			{
				this.Warning($"warning: {info.path} asset load null");
				if(retry&&retryTimes<=MAX_RETRY_TIMES)
					needRetry = true;
			}
			if(needRetry)
			{
				if(null!=bundle)
				{
					this.Warning($"warning: {info.path} bundle release cause retry fail");
					bundle.Unload(true);
					bundle = null;
				}
				++retryTimes;
				TryLoadSelf();
			}
			else
			{
				if(null!=bundle)
				{
                    if (!IsLoaded())
                    {
	                    this.Warning($"warning: {info.path} bundle unload cause asset no use");
                        bundle.Unload(true);
                        bundle = null;
                    }
				}
				isLoading = false;
				OnLoaded();
			}
		}
		protected virtual void DoAssetCallback(AssetCallback acb)
		{
			acb.Do(MyAsset);
		}
		protected virtual void DoCallback(OnAssetGot cb)
		{
			cb(MyAsset, 0);
		}

		#region Sync Load
        public virtual void RefSync()
        {
	        if(count == 0)
		        mgr.NoDelete(this);
	        ++count;
	        if(null!=info.depends)
	        {
		        foreach (var depend in info.depends)
			        mgr.GetAssetSync(depend);
	        }

	        if (!this.IsLoaded())
	        {
		        string bundlePath = BundleConfig.GetBundlePath(info.path);
		        bundle = AssetBundle.LoadFromFile(bundlePath);
		        LoadAssetSync();
	        }
	        if(AppEnv.ResVerbose)
		        this.Msg($"ref bundle = {PathIdProfile.Ins.GetPath(info.path)},count = {count}");
        }
        #endregion
		public virtual void FailLoad(){}
		public abstract void LoadAssetSync();
		public virtual AssetBundleRequest LoadAssetAsync()
		{
			return null;
		}
		public virtual void OnAsyncAssetSet(AssetBundleRequest request){}
		protected virtual void ReleaseAsset(){}
		#endregion
	}

	public class BundleHolderZero : BundleHolder
	{
		public override AssetHolderKind holderKind
		{
			get { return AssetHolderKind.ZERO; }
		}
		
		public BundleHolderZero(BundleInfo bi, BundleMgr _mgr) : base(bi, _mgr){}

		public override bool IsLoaded()
		{
			return bundle!=null;
		}

		public override void LoadAssetSync()
		{
			OnAssetSet();
		}
		protected override void DoAssetCallback(AssetCallback acb)
		{
			acb.Do(bundle);
		}

		protected override void DoCallback(OnAssetGot cb)
		{
			cb(bundle, 0);
		}
	}
    public class BundleHolderOne : BundleHolder
    {
	    public override AssetHolderKind holderKind
	    {
		    get { return AssetHolderKind.ONE; }
	    }

	    public override object MyAsset
        {
            get
            {
                return asset;
            }
        }
        private UObj asset = null;
	    public BundleHolderOne(BundleInfo bi, BundleMgr _mgr) : base(bi, _mgr) { }
        
        public override bool IsLoaded()
        {
            return asset != null;
        }
        protected override void ReleaseAsset()
        {
	        CheckShaderLoadRelease(false);
			asset = null;
        }
        private void SetAsset(UObj _asset,bool retry = false)
        {
	        asset = _asset;
	        CheckShaderLoadRelease(true);
	        this.OnAssetSet(retry);
        }

        private void CheckShaderLoadRelease(bool loadRelease)
        {
	        if(null == asset)
		        return;
	        if (info.type != AssetType.SHADER)
				return;
	        Shader shader = asset as Shader;
	        if(null == shader)
		        return;
	        if (loadRelease)
	        {
		        ShaderLib.Ins.AddShader(shader);
	        }
	        else
	        {
		        ShaderLib.Ins.RemoveShader(shader);
	        }
        }

	    public override void FailLoad()
	    {
		    SetAsset(null);
	    }

	    public override void LoadAssetSync()
	    {
		    UObj uo = bundle.LoadAsset(info.mainName, info.GetAssetType());
		    SetAsset(uo, true);
	    }

	    public override AssetBundleRequest LoadAssetAsync()
	    {
		    return bundle.LoadAssetAsync(info.mainName, info.GetAssetType());
	    }

	    public override void OnAsyncAssetSet(AssetBundleRequest request)
	    {
		    SetAsset(request.asset,true);
	    }
    }

    public class BundleHolderMulti:BundleHolder
    {
        private UObj[] assets = null;

        public override object MyAsset
        {
	        get { return assets; }
        }

        public override AssetHolderKind holderKind
	    {
		    get { return AssetHolderKind.MULTI; }
	    }

	    public BundleHolderMulti(BundleInfo bi, BundleMgr _mgr) : base(bi, _mgr) { }
        
        public override bool IsLoaded()
        {
            return assets != null;
        }
        protected override void ReleaseAsset()
        {
	        CheckShaderLoadRelease(false);
			assets = null;
        }
        private void SetAssets(UObj[] _assets, bool retry = false)
        {
            assets = _assets;
            CheckShaderLoadRelease(true);
            this.OnAssetSet(retry);
        }

	    protected override void DoAssetCallback(AssetCallback acb)
	    {
		    acb.Do(assets);
	    }

	    protected override void DoCallback(OnAssetGot cb)
	    {
		    cb(assets, 0);
	    }

	    public override void FailLoad()
	    {
		    SetAssets(null);
	    }

	    public override void LoadAssetSync()
	    {
		    UObj[] uos = bundle.LoadAllAssets();
		    SetAssets(uos, true);
	    }
	    
	    public override AssetBundleRequest LoadAssetAsync()
	    {
		    return bundle.LoadAllAssetsAsync();
	    }

	    public override void OnAsyncAssetSet(AssetBundleRequest request)
	    {
		    SetAssets(request.allAssets,true);
	    }
	    
	    private void CheckShaderLoadRelease(bool loadRelease)
	    {
		    if(null == assets)
			    return;
		    if (info.type != AssetType.SHADER_COMPOSE)
			    return;
		    foreach (var v in assets)
		    {
			    var tempShader = v as Shader;
			    if (tempShader != null)
			    {
				    ShaderLib.Ins.AddRemoveShader(tempShader,loadRelease);
			    }
			    else
			    {
				    if (v is ShaderVariantCollection)
				    {
					    ShaderLib.Ins.AddRemoveSvc(v as ShaderVariantCollection,loadRelease);
				    }
			    }
		    }
	    }
    }
}
