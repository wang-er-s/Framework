using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nine
{
    public class AssetBundleLoader
    {

        private AssetBundle assetBundle;

        /// <summary>
        /// 依赖的包
        /// </summary>
        public AssetBundleRelation relation;

        /// <summary>
        /// WWW对象
        /// </summary>
        private WWW www;

        /// <summary>
        /// 包名
        /// </summary>
        public string bundleName;

        /// <summary>
        /// 包的路径
        /// </summary>
        private string bundlePath;

        /// <summary>
        /// 进度
        /// </summary>
        private float progress;

        /// <summary>
        /// 加载进度回调
        /// </summary>
        public LoadProgress lp;

        /// <summary>
        /// 加载完成回调
        /// </summary>
        public LoadAssetBundleCallback lc;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AssetBundleLoader(LoadProgress lp, string bundleName, AssetBundleRelation relation)
        {
            this.lp = lp;
            this.bundleName = bundleName;
            this.relation = relation;
            progress = 0f;
            bundlePath = PathUtil.GetWWWPath() + "/" + bundleName;
            www = null;
            assetBundle = null;
        }

        /// <summary>
        /// 加载资源包
        /// </summary>
        /// <returns></returns>
        public IEnumerator Load()
        {
            www = new WWW(bundlePath);

            while (!www.isDone)
            {
                this.progress = www.progress;

                //每一帧来调用一次 更新加载进度
                if (lp != null)
                    lp(bundleName, progress);

                yield return www;
            }

            progress = www.progress;

            if (progress >= 1f)
            {
                //加载完成了
                assetBundle = www.assetBundle;

                //每一帧来调用一次 更新加载进度
                lp?.Invoke(bundleName, progress);
                lc?.Invoke();
            }
        }


        /// <summary>
        /// 获取单个资源
        /// </summary>
        /// <param name="assetName">资源名字</param>
        /// <returns>Obj类型的资源</returns>
        public Object LoadAsset(string assetName)
        {
            if (assetBundle == null)
            {
                Debug.LogError("当前assetBundle为空，无法获取该 " + assetName + " 资源");
                return null;
            }

            return assetBundle.LoadAsset(assetName);
        }

        /// <summary>
        /// 获取包里所有资源
        /// </summary>
        /// <returns></returns>
        public Object[] LoadAllAssets()
        {
            if (assetBundle == null)
            {
                Debug.LogError("当前assetBundle为空，无法获取资源");
                return null;
            }
            else
                return assetBundle.LoadAllAssets();
        }

        /// <summary>
        /// 获取带有子物体的资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns>所有资源</returns>
        public Object[] LoadAssetWithSubAssets(string assetName)
        {
            if (assetBundle == null)
            {
                Debug.LogError("当前assetBundle为空，无法获取该 " + assetName + " 资源");
                return null;
            }

            return assetBundle.LoadAssetWithSubAssets(assetName);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset">资源</param>
        public void UnLoadAsset(Object asset)
        {
            Resources.UnloadAsset(asset);
        }

        /// <summary>
        /// 释放资源包
        /// </summary>
        public void Dispose()
        {
            if (this.assetBundle == null)
                return;
            //false:只卸载 包
            //true:卸载 包 和 Obj
            assetBundle.Unload(false);
            assetBundle = null;
        }
    }
}
