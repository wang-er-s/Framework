using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    public class AssetBundleRelation
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="lp"></param>
        public AssetBundleRelation()
        {
            dependenceBundleList = new List<string>();
            referenceBundleList = new List<string>();
        }


        #region 依赖关系

        /// <summary>
        /// 所有依赖的包名
        /// </summary>
        private List<string> dependenceBundleList;

        /// <summary>
        /// 添加依赖关系
        /// </summary>
        /// <param name="bundleName"></param>
        public void AddDependence(string ab)
        {
            if (dependenceBundleList.Contains(ab))
                return;
            else
                dependenceBundleList.Add(ab);
        }

        /// <summary>
        /// 移除依赖关系
        /// </summary>
        /// <param name="bundleName"></param>
        public void RemoveDependence(string ab)
        {
            if (dependenceBundleList.Contains(ab))
                dependenceBundleList.Remove(ab);
        }

        /// <summary>
        /// 获取所有的依赖关系
        /// </summary>
        /// <returns></returns>
        public string[] GetAllDependences()
        {
            return dependenceBundleList.ToArray();
        }

        #endregion


        #region 被依赖关系

        /// <summary>
        /// 所有被依赖的包名
        /// </summary>
        private List<string> referenceBundleList;

        /// <summary>
        /// 添加被依赖关系
        /// </summary>
        /// <param name="bundleName"></param>
        public void AddReference(string ab)
        {
            if (referenceBundleList.Contains(ab))
                return;
            else
                referenceBundleList.Add(ab);
        }

        /// <summary>
        /// 移除被依赖关系
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns>true代表包被释放掉了 false代表包没被释放掉</returns>
        public bool RemoveReference(string ab)
        {
            if (referenceBundleList.Contains(ab))
            {
                referenceBundleList.Remove(ab);
                //移除一个包的时候 我们要做一个判断
                //还有没有被别的包所依赖
                //有  无所谓
                //没有  就需要释放掉这个AssetBundle
                return referenceBundleList.Count <= 0;
            }

            return false;
        }

        /// <summary>
        /// 获取所有的被依赖关系
        /// </summary>
        /// <returns></returns>
        public string[] GetAllReferences()
        {
            return referenceBundleList.ToArray();
        }

        #endregion
    }
}