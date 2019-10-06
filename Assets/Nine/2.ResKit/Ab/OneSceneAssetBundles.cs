using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nine
{
    public class OneSceneAssetBundles
    {

        /// <summary>
        /// 包名 和 对应的包 的映射
        /// </summary>
        private Dictionary<string, AssetBundleLoader> nameBundleDict;

        /// <summary>
        /// 包名 和 对应包的缓存 的映射
        /// </summary>
        private Dictionary<string, AssetCaching> nameCacheDict;

        /// <summary>
        /// 当前场景的名字
        /// </summary>
        //private string sceneName;

        /// <summary>
        /// 当前场景拥有的所以ab包的名字
        /// </summary>
        private List<string> abNameList;

        /// <summary>
        /// 构造函数
        /// </summary>
        public OneSceneAssetBundles(string sceneName, List<string> nameList = null)
        {
           //this.sceneName = sceneName;
            abNameList = nameList;
            nameBundleDict = new Dictionary<string, AssetBundleLoader>();
            nameCacheDict = new Dictionary<string, AssetCaching>();
        }

        #region 加载包

        public IEnumerator LoadAllAssetBundle(LoadProgress lp, LoadAssetBundleCallback loadCompleteCallBack)
        {
            int maxIndex = abNameList.Count;
            int nowIndex = 0;
            LoadProgress tempLp = (abName, process) => { lp?.Invoke(abName, (nowIndex + process) / maxIndex); };
            foreach (string abName in abNameList)
            {
                yield return Load(abName, tempLp, loadCompleteCallBack);
                nowIndex++;
            }
            loadCompleteCallBack.InvokeGracefully();
        }

        /// <summary>
        /// 加载包
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <returns></returns>
        public IEnumerator Load(string bundleName, LoadProgress lp, LoadAssetBundleCallback loadCompleteCallBack)
        {
            //如果这个包已经被加载了 就提示
            if (nameBundleDict.ContainsKey(bundleName))
            {
                Debug.LogWarning("此包已经加载了 : " + bundleName);
                yield break;
            }

            while (!AssetBundleManifestLoader.Instance.Finish)
            {
                yield return null;
            }

            //StartCoroutine("Load", bundleName);
            Debug.Log("开始加载");
            AssetBundleRelation assetBundleRelation = new AssetBundleRelation();
            //没有被加载
            AssetBundleLoader assetBundleLoader =
                new AssetBundleLoader(lp, bundleName, assetBundleRelation);
            //保存到字典里面
            nameBundleDict.Add(bundleName, assetBundleLoader);
            //先获取这个包的所有依赖关系
            string[] dependenceBundles = AssetBundleManifestLoader.Instance.GetDependencies(bundleName);
            //添加他的依赖关系
            foreach (var dependencebundleName in dependenceBundles)
            {
                assetBundleRelation.AddDependence(dependencebundleName);
                //加载这个包的所有依赖关系
                yield return LoadDependence(dependencebundleName, bundleName, assetBundleLoader.lp);
            }

            //开始加载这个包
            yield return assetBundleLoader.Load();
            loadCompleteCallBack?.Invoke();
        }

        /// <summary>
        /// 加载依赖的包
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <param name="referenceBundleName">被依赖的包名</param>
        /// <param name="lp">进度回调</param>
        /// <returns></returns>
        private IEnumerator LoadDependence(string bundleName, string referenceBundleName, LoadProgress lp)
        {

            if (nameBundleDict.ContainsKey(bundleName))
            {
                //已经加载过 就直接添加他的被依赖关系
                AssetBundleLoader assetBundleLoader = nameBundleDict[bundleName];
                //添加这个包的被依赖关系
                assetBundleLoader.relation.AddReference(referenceBundleName);
            }
            else
            {
                Debug.Log("开始加载依赖");
                //开始加载这个依赖的包
                yield return Load(bundleName, lp, null);
            }
        }

        #endregion

        #region 加载资源

        /// <summary>
        /// 获取单个资源
        /// </summary>
        /// <param name="assetName">资源名字</param>
        /// <returns>Obj类型的资源</returns>
        public Object LoadAsset(string bundleName, string assetName)
        {
            //先判断缓存没缓存
            if (nameCacheDict.ContainsKey(bundleName))
            {
                Object assets = nameCacheDict[bundleName].GetAsset(assetName);
                //安全校验
                if (assets != null)
                    return assets;
            }

            //当前包有没有被加载
            if (!nameBundleDict.ContainsKey(bundleName))
            {
                Debug.LogError("当前 " + bundleName + " 包没有加载，无法获取资源");
                return null;
            }

            //当前的包已经被加载了 
            Object asset = nameBundleDict[bundleName].LoadAsset(assetName);

            //有这个缓存层 里面也有资源 但是 这次获取的资源名字 是以前没缓存过的
            if (nameCacheDict.ContainsKey(bundleName))
            {
                //直接加进去
                nameCacheDict[bundleName].AddAsset(assetName, asset);
            }
            else
            {
                // 但是 第一次获取这个包里面的资源

                //创建一个新的缓存层
                AssetCaching caching = new AssetCaching();
                caching.AddAsset(assetName, asset);

                //保存到字典里面 方便下次使用
                nameCacheDict.Add(bundleName, caching);

                Debug.Log(111111111);
            }

            return asset;
        }

        /// <summary>
        /// 获取包里所有资源
        /// </summary>
        /// <returns></returns>
        public Object[] LoadAllAssets(string bundleName)
        {
            if (!nameBundleDict.ContainsKey(bundleName))
            {
                Debug.LogError("当前 " + bundleName + " 包没有加载，无法获取资源");
                return null;
            }
            else
                return nameBundleDict[bundleName].LoadAllAssets();
        }
        #endregion

        #region 卸载

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset">资源</param>
        public void UnLoadAsset(string bundleName, string assetName)
        {
            if (!nameCacheDict.ContainsKey(bundleName))
            {
                Debug.LogError("当前 " + bundleName + " 包没有缓存资源，无法卸载资源");
            }
            else
            {
                //已经缓存资源了 可以卸载
                nameCacheDict[bundleName].UnLoadAsset(assetName);

                Resources.UnloadUnusedAssets();
            }
        }

        /// <summary>
        /// 卸载一个包里面的所有资源
        /// </summary>
        /// <param name="bundleName"></param>
        public void UnLoadAllAsset(string bundleName)
        {
            if (!nameCacheDict.ContainsKey(bundleName))
            {
                Debug.LogError("当前 " + bundleName + " 包没有缓存资源，无法卸载资源");
            }
            else
            {
                //已经缓存资源了 可以卸载
                nameCacheDict[bundleName].UnLoadAllAssets();
                nameCacheDict.Remove(bundleName);

                Resources.UnloadUnusedAssets();
            }
        }

        /// <summary>
        /// 卸载所有的资源
        /// </summary>
        public void UnLoadAll()
        {
            foreach (var item in nameCacheDict.Keys)
            {
                UnLoadAllAsset(item);
            }

            nameCacheDict.Clear();
        }

        /// <summary>
        /// 卸载包
        /// </summary>
        public void Dispose(string bundleName)
        {
            if (!nameBundleDict.ContainsKey(bundleName))
            {
                Debug.LogError("当前 " + bundleName + " 包没有加载，无法获取资源");
                return;
            }

            //先获取到当前的包
            AssetBundleLoader assetBundleLoader = nameBundleDict[bundleName];

            //获取当前包的所有依赖关系
            string[] allDependences = assetBundleLoader.relation.GetAllDependences();
            foreach (string dependenceBundleName in allDependences)
            {
                AssetBundleLoader tmpAssetBundle = nameBundleDict[dependenceBundleName];
                //首先 移除 依赖的包里面的被依赖关系
                if (tmpAssetBundle.relation.RemoveReference(bundleName))
                {
                    //递归
                    Dispose(tmpAssetBundle.bundleName);
                }
            }

            //才开始卸载当前包
            if (assetBundleLoader.relation.GetAllReferences().Length <= 0)
            {
                nameBundleDict[bundleName].Dispose();
                nameBundleDict.Remove(bundleName);
            }

        }

        /// <summary>
        /// 卸载所有包
        /// </summary>
        public void DisposeAll()
        {
            foreach (var item in nameBundleDict.Keys)
            {
                Dispose(item);
            }

            nameBundleDict.Clear();
        }

        /// <summary>
        ///  卸载所有包和资源
        /// </summary>
        public void DisposeAndUnLoadAll()
        {
            UnLoadAll();

            DisposeAll();
        }

        #endregion

        public bool IsLoadedAssetBundle(string folderName)
        {
            return nameBundleDict.ContainsKey(folderName);
        }
    }
}