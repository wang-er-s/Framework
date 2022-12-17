using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public static class ProjectScanPath
    {
        // 当前脚本的根目录
        public static string LocalFolderRootPath { get; private set; }
        public static string ProjectConfigRootPath { get; private set; }
        public static string LocalScanRuleTxtPath => Path.Combine(LocalFolderRootPath, "Config/ScanRule.txt");
        public static string LocalScanConfigPath => Path.Combine(LocalFolderRootPath, "Config/GameScanGlobalConfig.txt");
        public static string ProjectScanConfigPath => Path.Combine(ProjectConfigRootPath, "GameScanGlobalConfig.txt");
        public static string ScanResultJsonPath => Path.Combine(ProjectConfigRootPath, "JsonResult{0}.json");
        public static string ScanResultCSVPath => Path.Combine(ProjectConfigRootPath, "CSVResult{0}.csv");
        public static string FixWhiteListPath => Path.Combine(ProjectConfigRootPath, "WhiteList.txt");

        public static void Init()
        {
            var guids = AssetDatabase.FindAssets($"{nameof(ProjectScan)}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileName(path) == $"{nameof(ProjectScan)}.cs")
                {
                    LocalFolderRootPath = Path.GetDirectoryName(path);
                    break;
                }
            }

            ProjectConfigRootPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", nameof(ProjectScan)));
            Directory.CreateDirectory(ProjectConfigRootPath);
        }
    }
}