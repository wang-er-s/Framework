using UnityEngine;
using System.Collections;
using Framework.BaseUtil;
using System.IO;
using System;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class SceneLoader : MonoSingleton<SceneLoader>
	{
        public delegate void OnLevelLoad(float progress, bool isDone);

		public bool switchByEmptyScene = true;
		private string levelName = null;
		private bool isLoading = false;
		private SceneAsset sceneAsset = null;
        private OnLevelLoad onLoad;

        #region Public Method
		public void AsnycLoadLevel (string _levelName,OnLevelLoad onLoad = null)
		{
			if(isLoading)
				return;
			isLoading = true;
			if (null != sceneAsset)
			{
				sceneAsset.Release();
				sceneAsset = null;
			}
			levelName = _levelName;
			this.onLoad = onLoad;
			bool isAdded = string.IsNullOrEmpty(levelName) || AppEnv.IsSceneInBuild(levelName);
			#if UNITY_EDITOR
			if (!AppEnv.UseBundleInEditor && !isAdded)
			{
				//在编辑器里使用非bundle模式的场景，目前已知必须要在build setting里
				Debug.LogError("if use raw asset in editor,you must add scene to buildsetting");
				return;
			}
			#endif
			if(isAdded)
				StartCoroutine(LoadScene());
			else
			{
				sceneAsset = new SceneAsset();
				sceneAsset.Load(levelName, () =>
				{
					StartCoroutine(LoadScene(sceneAsset.asset));
				});
			}
		}
		#endregion
		#region Private Method
		private IEnumerator LoadScene(AssetBundle sceneBundle = null)
		{
			AsyncOperation async = null;
			if (switchByEmptyScene)
			{
				async = SceneManager.LoadSceneAsync("Empty");
				while (!async.isDone)
				{
					yield return null;
				}

				GameObjPool.Ins.Empty();
				BundleMgr.Instance.Gc(true);
				//yield return new WaitForSeconds(1f);
			}

			if (!string.IsNullOrEmpty(levelName))
			{
				async = SceneManager.LoadSceneAsync(levelName);
				while (!async.isDone)
				{
					yield return null;
				}
			}

			OnLevelLoad tmp = onLoad;
			onLoad = null;
			isLoading = false;
			OnSceneLoaded();
			if(null!=tmp)
				tmp(1f, true);
		}
		private void OnSceneLoaded()
		{
			if(null!=sceneAsset)
			{
				StartCoroutine(sceneAsset.DelayRelease());
			}
		}
		
		#endregion
	}
}