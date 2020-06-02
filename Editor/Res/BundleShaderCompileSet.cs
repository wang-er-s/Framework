/**
 * Created by wangliang on 2019/39/09 16:39:49
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// Shader编译配置
    /// 目前用于assetbundle打包和svc合并剔除
    /// </summary>
    public class BundleShaderCompileSet : ScriptableObject
    {
        public const string ASSET_NAME = "Assets/Setting/BundleShaderCompileSet";
        public List<string> excludeShaders;//不使用Shader，因为一部分内置Shader索引不到
        public List<string> excludeKeywords;

        private bool IsShaderNameExclude(string shaderName)
        {
            if (null != excludeShaders)
                return excludeShaders.Contains(shaderName);
            return false;
        }

        private bool IsKeywordsExclude(string[] keywordSet)
        {
            if (null == keywordSet)
                return false;
            foreach (var keyword in excludeKeywords)
            {
                if (Array.IndexOf(keywordSet, keyword) >= 0)
                    return true;
            }

            return false;
        }
    }
}