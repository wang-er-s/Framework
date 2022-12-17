using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class GeneralSettingMenu : ScanMenu
    {
        public override string Name => "通用设置";
        public override int Priority { get; protected set; } = 0;

        private string loadedResult;
        
        [HideInInspector]
        public List<string> IncludeDir;
        [HideInInspector]
        public List<string> IgnoreDir;

        public GeneralSettingMenu()
        {
            IncludeDir = ProjectScan.GlobalConfig.IncludeDir;
            IgnoreDir = ProjectScan.GlobalConfig.IgnoreDir;
        }

        [OnInspectorGUI]
        protected void CustomDrawer()
        {
            EditorGUILayout.LabelField(Name, ProjectScanTools.DefaultStyle);
            EditorGUILayout.Space(10);
            ProjectScanTools.DrawPathList(IncludeDir, "目标文件夹");
            ProjectScanTools.DrawPathList(IgnoreDir, "忽略文件夹");
            if (!string.IsNullOrEmpty(loadedResult))
            {
                EditorGUILayout.LabelField($"当前已读取的数据文件：  {loadedResult}");
            }

            if (GUILayout.Button("选择读取的数据文件", GUILayout.Width(140), GUILayout.Height(35)))
            {
                LoadScanResult();
            }
        }

        private async void LoadScanResult()
        {
            await Task.Delay(10);
            string path = EditorUtility.OpenFilePanel("选择要加载的文件", ProjectScanPath.ProjectConfigRootPath,
                Path.GetExtension(ProjectScanPath.ScanResultJsonPath).Remove(0, 1));
            if(string.IsNullOrEmpty(path)) return;
            ProjectScan.LoadScanResult(path);
            loadedResult = path;
        }
    }
}