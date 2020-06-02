using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Framework.BaseUtil;
using Framework.Util;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class BundleHelp
    {
        //统一处理Shader的
        public static string GetShaderBundleName(string shaderName)
        {
            return shaderName.Replace('/', '_');
        }
    }
    public class BundleBuild
    {
        private List<AssetBundleBuild> bundleList = new List<AssetBundleBuild>();
        private string outputPath;
        private BundleConfig config;

        public BundleBuild(BundleConfig config)
        {
            this.config = config;
        }
        
        private void InitPath(string outputDir, BuildTarget target, bool bClear)
        {
            outputPath = outputDir + "/" + BundleConfig.GetPlatformRootFolder(target);
            FileUtils.CreateDir(outputPath,bClear);
        }

        public void SetBundle(string res, string bundleName)
        {
            AssetBundleBuild abBuild = new AssetBundleBuild()
            {
                assetBundleName = bundleName.ToLower(),
                assetNames = new[] {res},
            };
            bundleList.Add(abBuild);
        }

        public void Exec(string outputDir, BuildTarget target, bool bClear = true)
        {
            this.InitPath(outputDir,target,bClear);
            Debug.Log(string.Format("build assetbundle for platform : {0} path : {1}",target,outputPath));
            BuildAssetBundleOptions options =
                //BuildAssetBundleOptions.DisableWriteTypeTree | 
                BuildAssetBundleOptions.ChunkBasedCompression |
                BuildAssetBundleOptions.DeterministicAssetBundle;
            if (bClear)
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            AssetBundleManifest mani = BuildPipeline.BuildAssetBundles(outputPath, bundleList.ToArray(), options, target);
            if (null == mani)
            {
                Debug.LogError("Error: build assetbundle failed");
            }
            else
            {
                GenBundleInfo(mani);
                GenBundleHash(mani);  
                UpdatePathIdFile();
            }
        }

        private void GenBundleInfo(AssetBundleManifest mani)
        {
            string[] bundles = mani.GetAllAssetBundles();
            foreach (string bundle in bundles)
            {
                GenOneBundleInfo(mani, bundle);
            }
            config.Export(outputPath + "/" + BundleConfig.allDepFile);
        }
        
        private void GenOneBundleInfo(AssetBundleManifest mani, string bundle)
        {
            BundleInfo info = config.GetInfo(bundle,false);
            if (null == info)
            {
                Debug.LogWarning(string.Format("uninfo bundle {0}",bundle));
                return;
            }

            string[] dependBundles = mani.GetAllDependencies(bundle);
            //todo:变体嵌套Prefab目前包含对于其他prefab的依赖，但是本版本测试未发现对于加载有相关性，去除相关性减少加载量？
            if (null != dependBundles && dependBundles.Length > 0)
                info.depends = dependBundles;
            else
                info.depends = null;
        }
        
        private void GenBundleHash(AssetBundleManifest mani)
        {
            string[] bundles = mani.GetAllAssetBundles();
            StreamWriter sw = File.CreateText(outputPath + "/hash.info");
            foreach (string bundle in bundles)
            {
                var path = Path.Combine(outputPath, bundle);
                var sha1 = CalculateSHA1(path);
                sw.WriteLine($"{bundle}:{sha1}");
            }

            var bundleInfoPath = Path.Combine(outputPath, BundleConfig.allDepFile);
            sw.WriteLine($"{BundleConfig.allDepFile}:{CalculateSHA1(bundleInfoPath)}");

            sw.Close();
        }

        private void UpdatePathIdFile()
        {
            if(PathIdProfile.Ins.IsDirty())
                PathIdProfile.Ins.Export(outputPath + "/" + PathIdProfile.fileName);
        }
        
        static string CalculateSHA1(string filename)
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = sha1.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}