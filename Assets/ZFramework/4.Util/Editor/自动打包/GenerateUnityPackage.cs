/*
* Create by Soso
* Time : 2018-12-08-05 下午
*/
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SF
{
	public class GenerateUnityPackage  
	{
        [MenuItem("Assets/SF/导出UnityPackage %-Ctrl+e")]
        private static void Create()
        {
            string packageName = "SF" + DateTime.Now.ToString("yyyyMMddhhtt");
            string[] assetPathNames = new string[Selection.objects.Length];
            for (int i = 0; i < assetPathNames.Length; i++)
            {
                assetPathNames[i] = AssetDatabase.GetAssetPath(Selection.objects[i]);
            }
            if (assetPathNames.Length <= 0) assetPathNames = new string[] { "Assets/SF" };
            assetPathNames = AssetDatabase.GetDependencies(assetPathNames);
            AssetDatabase.ExportPackage(assetPathNames, "C:\\Users\\zzzz\\Desktop\\"+ packageName+ ".unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
        }
	}
}
