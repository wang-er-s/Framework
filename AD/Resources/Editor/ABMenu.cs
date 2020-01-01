using System.Collections;
using System.Collections.Generic;
using System.IO;
using AD;
using UnityEditor;
using UnityEngine;

public class ABMenu
{
    [MenuItem("Tools/打包")]
    private static void PackBundle()
    {
        string path = $"{ResPath.EditorProductFullPath}/{ResPath.BundlesDirName}/{ResPath.GetBuildPlatformName()}";
        
        Debug.Log(path);
        Directory.CreateDirectory(path);
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}
