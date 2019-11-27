using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AD
{
    public class AssetBundleManifestLoader
    {

        private static AssetBundleManifestLoader instance;
        /// <summary>
        /// 单例对象
        /// </summary>
        public static AssetBundleManifestLoader Instance
        {
            get { return instance ?? (instance = new AssetBundleManifestLoader()); }
        }


        /// <summary>
        /// Manifest文件
        /// </summary>
        private AssetBundleManifest manifest;

        /// <summary>
        /// 路径
        /// </summary>
        private string manifestPath;

        private bool finish;

        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool Finish
        {
            get { return finish; }
        }

        /// <summary>
        /// 全局存在的assetbundle
        /// </summary>
        private AssetBundle assetBundle;

        /// <summary>
        /// 构造函数
        /// </summary>
        private AssetBundleManifestLoader()
        {
            this.manifestPath = PathUtil.GetWWWPath() + "/" + PathUtil.GetPlatformName();

            this.manifest = null;
            this.assetBundle = null;
            this.finish = false;
        }

        /// <summary>
        /// 开始加载
        /// </summary>
        /// <returns></returns>
        public IEnumerator Load()
        {
            WWW www = new WWW(manifestPath);
            yield return www;

            if (www.error != null)
            {
                Debug.LogError("加载Manifest文件出错 : " + www.error);
            }
            else
            {
                if (www.progress >= 1f)
                {
                    this.assetBundle = www.assetBundle;
                    this.manifest = this.assetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                    this.finish = true;
                }
            }
        }

        /// <summary>
        /// 获取所有的依赖关系
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <returns></returns>
        public string[] GetDependencies(string bundleName)
        {
            return manifest.GetAllDependencies(bundleName);
        }

        /// <summary>
        /// 卸载 manifest
        /// </summary>
        public void UnLoad()
        {
            assetBundle.Unload(true);
        }

    }
}