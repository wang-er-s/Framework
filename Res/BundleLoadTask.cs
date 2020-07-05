using UnityEngine;
using System.Collections;
using UObj = UnityEngine.Object;
using System.Collections.Generic;
using System.IO;
using System;
using Framework.BaseUtil;
using UnityEngine.Networking;

namespace Framework
{
	public class BundleLoadTask
	{
		#region Variables
		private BundleHolder holder;
        private bool assetSync = true;
		private UnityWebRequest wwwRequest;//自www的文件请求
		private AssetBundleCreateRequest diskRequest;//自文件请求
		private AssetBundleRequest assetRequest;//资源load请求
		private bool isFinished;
		private IEnumerator loadCouroutine;
		#endregion
		#region Properties
		public bool IsFinished 
		{get {return isFinished;}}
		public BundleHolder Holder
		{get {return holder;}}
		#endregion
		#region Public Method
		public BundleLoadTask(BundleHolder _holder,bool _assetSync = true)
		{
			holder = _holder;
            assetSync = _assetSync;
		}

		public void Start()
		{
			loadCouroutine = Run();
			BundleLoader.Ins.StartCoroutine(loadCouroutine);
		}
		public void Cancel()
		{
			if (null != loadCouroutine)
			{
				BundleLoader.Ins.StopCoroutine(loadCouroutine);
				loadCouroutine = null;
			}
			holder = null;
			isFinished = true;
		}
		
		public float GetProgress()
		{
			if(null!=wwwRequest)
				return wwwRequest.downloadProgress*0.5f;
			if(null!=diskRequest)
				return diskRequest.progress*0.5f;
			if(null!=assetRequest)
				return 0.5f + assetRequest.progress*0.5f;
			return 0f;
		}
		#endregion
		#region Private Method

		private IEnumerator Run()
		{
			if (null == holder)
			{
				isFinished = true;
				yield break;
			}

			string bundlePath = BundleConfig.GetBundlePath(holder.Info.path);
			if (bundlePath == null)
			{
				isFinished = true;
				OnBundleLoaded(null);
				yield break;
			}
			if(AppEnv.ResLog)
				this.Msg($"start load file {PathIdProfile.Ins.GetPath(holder.Info.path)}");
			if (bundlePath.Contains("://"))
			{
				using(wwwRequest = UnityWebRequestAssetBundle.GetAssetBundle(bundlePath))
				{
					yield return wwwRequest.SendWebRequest();
					loadCouroutine = null;
					if(null == wwwRequest.error)
					{
						AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(wwwRequest);
						OnBundleLoaded(assetBundle);
					}
					else
					{
						this.Warning($"fail to load file {holder.Info.path}");
						OnBundleLoaded(null);
					}
					wwwRequest.Dispose();
				}
				wwwRequest = null;
			}
			else
			{
				diskRequest = AssetBundle.LoadFromFileAsync(bundlePath);
				yield return diskRequest;
				loadCouroutine = null;
				OnBundleLoaded(diskRequest.assetBundle);
				diskRequest = null;
			}
		}
		
		private void OnBundleLoaded(AssetBundle bundle)
		{
			if(null!=holder)
			{
				holder.MyBundle = bundle;
				if (null == bundle)//读取资源文件失败的
				{
					isFinished = true;
					this.Warning($"err: load file {holder.Info.path},bundle is null");
					holder.FailLoad();
				}
				else if(assetSync)
				{
                    holder.LoadAssetSync();
					isFinished = true;
				}
				else
                {
	                loadCouroutine = DoAsyncAssetLoad(bundle);
	                BundleLoader.Ins.StartCoroutine(loadCouroutine);
                }
			}
			else if(null!=bundle)//没有holder直接删除
			{
				isFinished = true;
				bundle.Unload(true);
				UObj.DestroyImmediate(bundle,true);
			}
		}

        private IEnumerator DoAsyncAssetLoad(AssetBundle bundle)
        {
	        assetRequest = holder.LoadAssetAsync();
            
            yield return assetRequest;
            if (holder != null)
            {
	            holder.OnAsyncAssetSet(assetRequest);
            }
            else
            {
	            if (null != assetRequest.asset)
	            {
		            this.Warning($"del before {assetRequest.asset.name} create finished");
		            UObj.DestroyImmediate(assetRequest.asset, true);
	            }
	            if (null != assetRequest.allAssets)
	            {
		            for (int i = 0; i < assetRequest.allAssets.Length; ++i)
		            {
			            UObj asset = assetRequest.allAssets[i];
			            this.Warning($"del before {asset.name} create finished");
			            UObj.DestroyImmediate(asset, true);
		            }
	            }

	            bundle.Unload(true);
                UObj.DestroyImmediate(bundle, true);
                bundle = null;
            }
            assetRequest = null;
            loadCouroutine = null;
	        isFinished = true;
        }
		#endregion
	}
}
