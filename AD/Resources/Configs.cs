using System.Collections;
using System.Collections.Generic;
using AD;
using UnityEngine;

public static class Configs
{
    public const string AssetBundleExt = ".ab";
    public const string EditorResourcesDir = "ResPackage";
    public const string BundlesDirName = "Bundles"; //---
    public const string EditorAssetBundlePath = "";
    public const bool LoadAssetBundle = true;
    public const bool ApplicationQuited = false;
    public const bool IsEditor = false;
    /// <summary>
    /// 是否優先找下載的資源?還是app本身資源優先. 优先下载的资源，即采用热更新的资源
    /// </summary>
    public static KResourcePathPriorityType ResourcePathPriorityType =
        KResourcePathPriorityType.PersistentDataPathPriority;
}
