using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityIO;

namespace Framework.Editor
{
    public static class GenAddressablePath
    {
        private static string GenPath => Path.Combine(IO.Root.path, "_Scripts/Base/ResPath.cs");
        private const string LabPre = "lab";

        [MenuItem("Tools/生成资源地址")]
        public static void Gen()
        {
            var groups = AddressableAssetSettingsDefaultObject.Settings.groups;
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public static class ResPath");
            sb.AppendLine("{");
            foreach (var assetGroup in groups)
            {
                if(assetGroup.ReadOnly) continue;
                foreach (var assetEntry in assetGroup.entries)
                {
                    try
                    {
                        var path = $"{Path.GetFileNameWithoutExtension(assetEntry.MainAsset.name)}";
                        assetEntry.address = path.Trim();
                        sb.AppendLine($"\tpublic const string {assetEntry.address} = \"{assetEntry.address}\";");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(assetEntry.address + " get error ===>" + e);
                    }
                    
                }
            }
            
            foreach (var label in AddressableAssetSettingsDefaultObject.Settings.GetLabels())
            {
                sb.AppendLine($"\tpublic const string {label} = \"{label}\";");
            }
            sb.AppendLine("}");
            var dirPath = Path.GetDirectoryName(GenPath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            File.WriteAllText(GenPath, sb.ToString());
            AssetDatabase.Refresh();
            Debug.Log($"生成成功-->{GenPath}");
        }
    }
}