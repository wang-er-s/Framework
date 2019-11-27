using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AD
{
    public class AssetCaching
    {

        /// <summary>
        /// 已经加载过的 资源名字 和 资源 的映射关系
        /// </summary>
        private Dictionary<string, Object> nameAssetDict;

        public AssetCaching()
        {
            nameAssetDict = new Dictionary<string, Object>();
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        public void AddAsset(string assetName, Object asset)
        {
            if (nameAssetDict.ContainsKey(assetName))
            {
                Debug.LogWarning("此 " + assetName + " 资源已经加载!");
                return;
            }
            //缓存起来
            nameAssetDict.Add(assetName, asset);
        }

        /// <summary>
        /// 获取缓存的资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Object GetAsset(string assetName)
        {
            if (nameAssetDict.ContainsKey(assetName))
            {
                return nameAssetDict[assetName];
            }
            else
            {
                Debug.LogError("此 " + assetName + " 资源尚未被加载!");
                return null;
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetName"></param>
        public void UnLoadAsset(string assetName)
        {
            //如果资源已经被加载 就直接释放
            if (nameAssetDict.ContainsKey(assetName))
            {
                Resources.UnloadAsset(nameAssetDict[assetName]);
            }
            else
            {
                Debug.LogError("此 " + assetName + " 资源尚未被加载!");
            }
        }

        /// <summary>
        /// 卸载所有的资源
        /// </summary>
        public void UnLoadAllAssets()
        {
            foreach (string assetName in nameAssetDict.Keys)
            {
                UnLoadAsset(assetName);
            }

            nameAssetDict.Clear();
        }
    }
}