using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ABMenu
{
    [MenuItem("Tools/打包")]
    private static void PackBundle()
    {
        BuildPipeline.BuildAssetBundles()
    }
}
