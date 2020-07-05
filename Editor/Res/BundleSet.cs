using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.BaseUtil;
using Framework.Util;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class BundleSet : ScriptableObject
    {
        public const string ASSET_NAME = "Assets/Setting/BundleSet";
        public string outputDir;

        public List<ResDir> resDirs = new List<ResDir>();
        public List<PrefabDir> prefabs = new List<PrefabDir>();
        public List<SceneDir> scenes = new List<SceneDir>();
        public List<string> excludeRes = new List<string>();
        
        public string ignoreResPattern;

        public bool usePathId = true;
        #region Var

        private string exportPath;
        private Dictionary<string,ResInfo> resList = null;
        private Dictionary<string,PrefabInfo> prefabList = null;
        private Dictionary<string,SceneInfo> sceneList = null;
        private Dictionary<string, OneInfo> totals;
        private string[] ignorePatterns = null;
        
        private BundleConfig config;
        private BundleBuild bundleBuild;
        #endregion

        #region Res Scan
        private void ScanRes()
        {
            GetResList();
            GetPrefabList();
            GetSceneList();

            totals = new Dictionary<string, OneInfo>();
            if(!string.IsNullOrEmpty(ignoreResPattern))
                ignorePatterns = ignoreResPattern.Split(new char[]{'|'});
            CheckSceneDepend();
            CheckPrefabDepend();
            CheckResDepend();
        }

        private void GetResList()
        {
            resList = new Dictionary<string, ResInfo>();
            string _progressTitle = "获取基础资源";
            EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
            for(int i=0;i<resDirs.Count;++i)
            {
                ResDir dir = resDirs[i];
                GetResFromDir(dir);
                EditorUtility.DisplayProgressBar(_progressTitle, dir.path, (float)(i+1) / resDirs.Count);
            }
            EditorUtility.ClearProgressBar();
        }
        
        private void GetResFromDir(ResDir dir)
        {
            if(dir.ignore)
                return;
            List<string> files = FileUtils.GetFiles(dir.path, dir.filePattern);
            string shaderBundleSingleName = null;
            
            foreach (var file in files)
            {
                if(resList.ContainsKey(file))
                    continue;
                ResInfo resInfo = new ResInfo()
                {
                    res = file,
                    dir = dir,
                    refBy = null,
                    refCount = 0,
                    depends = null,
                };
                resList.Add(file,resInfo);
            }
        }
        
        private void GetPrefabList()
        {
            prefabList = new Dictionary<string, PrefabInfo>();
            string _progressTitle = "获取预制体";
            EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
            for(int i=0;i<prefabs.Count;++i)
            {
                PrefabDir dir = prefabs[i];
                GetPrefabFromPath(dir);
                EditorUtility.DisplayProgressBar(_progressTitle, dir.path, (float)(i+1) / prefabs.Count);
            }
            EditorUtility.ClearProgressBar();
        }
        
        private void GetPrefabFromPath(PrefabDir dir)
        {
            if(dir.ignore)
                return;
            List<string> files = FileUtils.GetFiles(dir.path, ".prefab");
            foreach (var file in files)
            {
                if(prefabList.ContainsKey(file))
                    continue;
                prefabList.Add(file,new PrefabInfo()
                {
                    res = file,
                    dir = dir,
                    refBy = null,
                    refCount = 0,
                    depends = null,
                });
            }
        }
        
        private void GetSceneList()
        {
            sceneList = new Dictionary<string, SceneInfo>();
            string _progressTitle = "获取场景";
            EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
            for(int i=0;i<scenes.Count;++i)
            {
                SceneDir dir = scenes[i];
                GetSceneFromPath(dir);
                EditorUtility.DisplayProgressBar(_progressTitle, dir.path, (float)(i+1) / scenes.Count);
            }
            EditorUtility.ClearProgressBar();
        }
        
        private void GetSceneFromPath(SceneDir dir)
        {
            if(dir.ignore)
                return;
            List<string> files = FileUtils.GetFiles(dir.path, ".unity");
            foreach (var file in files)
            {
                if(sceneList.ContainsKey(file))
                    continue;
                sceneList.Add(file,new SceneInfo()
                {
                    res = file,
                    dir = dir,
                    refBy = null,
                    refCount = 0,
                    depends = null,
                });
            }
        }
        private void CheckPrefabDepend()
        {
            string _progressTitle = "检测预制体依赖数据";
            EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
            int index = 0;
            int count = prefabList.Count;
            foreach (var kvp in prefabList)
            {
                PrefabInfo prefab = kvp.Value;
                ProcessOneInfo(prefab,ignorePatterns);
                index++;
                EditorUtility.DisplayProgressBar(_progressTitle, prefab.res, (float)(index) / count);
            }
            EditorUtility.ClearProgressBar();
        }
        
        private void CheckSceneDepend()
        {
            string _progressTitle = "检测场景依赖数据";
            EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
            int index = 0;
            int count = sceneList.Count;
            foreach (var kvp in sceneList)
            {
                SceneInfo scene = kvp.Value;
                ProcessOneInfo(scene,ignorePatterns);
                index++;
                EditorUtility.DisplayProgressBar(_progressTitle, scene.res, (float)(index) / count);
            }
            EditorUtility.ClearProgressBar();
        }
        private void CheckResDepend()
        {
            string _progressTitle = "检测基础资源依赖数据";
            EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
            int index = 0;
            int count = resList.Count;
            foreach (var kvp in resList)
            {
                ResInfo resInfo = kvp.Value;
                ProcessOneInfo(resInfo,ignorePatterns);
                index++;
                EditorUtility.DisplayProgressBar(_progressTitle, resInfo.res, (float)(index) / count);
            }
            EditorUtility.ClearProgressBar();
        }

        private PrefabInfo GetOrCreatePrefabInfo(string res)
        {
            if (prefabList.ContainsKey(res))
                return prefabList[res];
            return new PrefabInfo()
            {
                res = res,
                dir =  null,
                refCount = 0,
                refBy = null,
                depends = null,
            };
        }

        private ResInfo GetOrCreateResInfo(string res)
        {
            if (resList.ContainsKey(res))
                return resList[res];
            return new ResInfo()
            {
                res = res,
                dir = null,
                refCount = 0,
                refBy = null,
                depends = null,
            };
        }
        
        private void ProcessOneInfo(OneInfo oneInfo,string[] ignorePatterns)
        {
            if(totals.ContainsKey(oneInfo.res))
                return;
            totals.Add(oneInfo.res,oneInfo);
            string[] deps = AssetDatabase.GetDependencies(oneInfo.res,false);
            if(null == deps)
                return;
            foreach (var dep in deps)
            {
                if(dep.Equals(oneInfo.res))
                    continue;
                OneInfo info = null;
                if (totals.ContainsKey(dep))
                {
                    info = totals[dep];
                    info.RefBy(oneInfo.res);
                    oneInfo.AddDepend(info);
                }
                else if(!IsResIgnore(dep,ignorePatterns))
                {
                    bool isPrefab = dep.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase);
                    if (isPrefab)
                    {
                        info = GetOrCreatePrefabInfo(dep);
                    }
                    else
                    {
                        info = GetOrCreateResInfo(dep);
                    }
                    info.RefBy(oneInfo.res);
                    oneInfo.AddDepend(info);
                    ProcessOneInfo(info,ignorePatterns);
                }
            }
        }
        private bool IsResIgnore(string res,string[] partterns)
        {
            if (null == partterns)
                return false;
            foreach (var pattern in partterns)
            {
                if (res.EndsWith(pattern, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private void ProcessExcludeRes()
        {
            string _progressTitle = "处理bundle排除资源";
            EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
            int count = excludeRes.Count;
            for (int i = 0; i < count; ++i)
            {
                string exclude = excludeRes[i];
                if (totals.ContainsKey(exclude))
                {
                    OneInfo oneInfo = totals[exclude];
                    if (!oneInfo.bundleExclude)
                    {
                        ProcessOneExclude(oneInfo as ResInfo);
                    }
                }
                EditorUtility.DisplayProgressBar(_progressTitle, exclude, (float)(i+1) / count);
            }
            EditorUtility.ClearProgressBar();
        }

        private void ProcessOneExclude(OneInfo info)
        {
            if(null == info||info.bundleExclude)
                return;
            Debug.Log("exclude " + info.res);
            info.bundleExclude = true;
            if (null != info.refBy)
            {
                foreach (var refResPath in info.refBy)
                {
                    if (totals.ContainsKey(refResPath))
                    {
                        OneInfo refRes = totals[refResPath];
                        if (!refRes.bundleExclude && refRes.Type == InfoType.RES)//对于依赖它的值只处理基础资源
                        {
                            ResInfo aRes = refRes as ResInfo;
                            ProcessOneExclude(aRes);
                        }
                    }
                }
            }
        }

        private void ProcessAllSpriteAtlas()
        {
            string _progressTitle = "处理SpriteAtlas";
            EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
            foreach (var kvp in totals)
            {
                OneInfo info = kvp.Value;
                if(info.Type != InfoType.RES||info.bundleExclude)
                    continue;
                ResInfo resInfo = info as ResInfo;
                if (resInfo.IsSpriteAtlas())
                {
                    ProcessOneSpriteAtlas(resInfo);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private void ProcessOneSpriteAtlas(ResInfo resInfo)
        {
            List<OneInfo> deps = resInfo.depends;
            foreach (var dep in deps)
            {
                if(dep.Type != InfoType.RES)
                    continue;
                ResInfo oneRes = dep as ResInfo;
                if (oneRes.IsTexture())
                {
                    oneRes.bundleExclude = true;
                    List<string> refBy = oneRes.refBy;
                    if (null != refBy)
                    {
                        foreach (var oneRefStr in refBy)
                        {
                            if (totals.ContainsKey(oneRefStr))
                            {
                                OneInfo refInfo = totals[oneRefStr];
                                if(refInfo.Type == InfoType.RES && ((ResInfo)refInfo).IsTexture())
                                    continue;
                                refInfo.RemoveDepend(oneRes);
                                refInfo.AddDepend(resInfo);//依赖于spriteAtlas
                                resInfo.RefBy(refInfo.res);
                            }
                        }
                    }
                }
            }
        }
        private void OutputInfo()
        {
            string output = exportPath +"/output.txt" ;
            Debug.Log($"start output info to {output}");
            
            EditorUtility.DisplayProgressBar("Output Info", "writing info", 0);
            var totalList = totals.ToList();
            totalList.Sort((pair1, pair2) =>
            {
                OneInfo info1 = pair1.Value;
                OneInfo info2 = pair2.Value;
                if (info1.Type != info2.Type)
                {
                    int comp = ((int)info1.Type).CompareTo((int)info2.Type);
                    return -comp;
                }
                return info1.refCount.CompareTo(info2.refCount);
            });
            StreamWriter sw = File.CreateText(output);
            sw.WriteLine("res include count : {0}" ,resList.Count);
            sw.WriteLine("prefab include count ： {0}", prefabList.Count);
            sw.WriteLine("scene include count ： {0}", sceneList.Count);
            sw.WriteLine();
            sw.WriteLine("全部资源===============>");
            sw.WriteLine("count:{0}", totalList.Count);
            
            List<OneInfo> shaderInfos = new List<OneInfo>();
            sw.WriteLine("===非Shader==============>");
            foreach (var dpair in totalList)
            {
                sw.WriteLine(dpair.Value.ToString());
            }

            sw.WriteLine("===所有Shader==============>");
            sw.WriteLine("shader count:{0}",shaderInfos.Count);
            foreach (var oneInfo in shaderInfos)
            {
                sw.WriteLine(oneInfo.ToString());
            }
            sw.Close();
            
            AssetDatabase.Refresh();
            Debug.Log("output info done");
            EditorUtility.ClearProgressBar();
        }
        #endregion

        #region Gen

        public void Gen(bool bClear,bool genInfo = false)
        {
            BuildByPlatform(EditorUserBuildSettings.activeBuildTarget,bClear,genInfo);
        }

        public void GenByPlatform(BuildTarget target, bool bClear)
        {
            BuildByPlatform(target,bClear,true);
        }

        private void BuildByPlatform(BuildTarget target, bool bClear = true,bool genInfo = false)
        {
            try
            {
                InitPath();
                InitPathIdProfile(target,bClear);

                config = new BundleConfig();
                ScanRes();
                ProcessExcludeRes();

                //ProcessAllSpriteAtlas();

                bundleBuild = new BundleBuild(config);
                GenBundle();
                bundleBuild.Exec(exportPath, target, bClear);

                if (genInfo)
                    OutputInfo();
                
                ClearForBundleGen();
                Debug.Log("BuildByPlatform finish");
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw;
            }
        }
        
        private void InitPath()
        {
            if (string.IsNullOrEmpty(outputDir))
                exportPath = Application.streamingAssetsPath + "/assetbundles";
            else if(outputDir.StartsWith("./")||outputDir.StartsWith("../"))
            {
                exportPath = Application.dataPath + "/" + outputDir;
            }
            else
            {
                exportPath = outputDir;
            }
            exportPath = Path.GetFullPath(exportPath);
            FileUtils.CreateDir(exportPath,false);
        }

        private void InitPathIdProfile(BuildTarget target,bool clear)
        {
            PathIdProfile.Ins.Clear();
            if(clear)
                return;
            string path = exportPath + "/" + BundleConfig.GetPlatformRootFolder(target) + "/" + PathIdProfile.fileName;
            PathIdProfile.Ins.Import(path);
        }
        
        private void GenBundle()
        {
            PathIdProfile.Ins.MarkUpdating(usePathId);
            string _progressTitle = "设置bundle";
            EditorUtility.DisplayProgressBar(_progressTitle, "", 0);
            int index = 0;
            int count = totals.Count;
            foreach (var kvp in totals)
            {
                OneInfo resInfo = kvp.Value;
                if (!resInfo.bundleExclude)
                {
                    resInfo.SetAssetBundleName(bundleBuild);
                    resInfo.GenConfig(config);
                }
                index++;
                EditorUtility.DisplayProgressBar(_progressTitle, resInfo.res, (float)(index) / count);
            }
            EditorUtility.ClearProgressBar();
        }

        private void ClearForBundleGen()
        {
            PathIdProfile.Ins.FinishUpdating();
            resList = null;
            prefabList = null;
            sceneList = null;
            bundleBuild = null;
            config = null;
            totals = null;
        }
        #endregion
    }
}