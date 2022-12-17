using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    public abstract class ScanRule
    {
        public abstract string RuleId { get; }
        public virtual string Description { get; protected set; }
        public virtual string DisplayName { get; protected set; }
        public virtual string HelpUrl { get; protected set; }
        public string Value { get; protected set; }
        public abstract string Menu { get; }
        public bool IsEnable { get; private set; } = true;
        public abstract RulePriority Priority { get; }
        public bool HasFixMethod { get; }
        public List<object[]> ScanResult { get; } = new();

        public ScanRule()
        {
            var fixMethod = GetType().GetMethod(nameof(Fix), BindingFlags.Instance | BindingFlags.Public);
            HasFixMethod = fixMethod.IsOverride();
            if (ProjectScan.GlobalConfig.RuleNameConfig.TryGetValue(RuleId, out var rules))
            {
                DisplayName = rules.Name;
                Description = rules.Desc;
                HelpUrl = rules.HelpUrl;
            }

            if (ProjectScan.GlobalConfig.RuleConfig.TryGetValue(RuleId, out var ruleConfig))
            {
                IsEnable = ruleConfig.IsEnable;
                Value = ruleConfig.Value;
            }
        }

        public abstract void Scan();


        public virtual void Fix()
        {
        }

        protected void ShowProgress(string path, float progress)
        {
            EditorUtility.DisplayProgressBar(DisplayName, path, progress);
        }

        protected void LogResult()
        {
            if(ScanResult.Count <= 0) return;
            StringBuilder sb = new StringBuilder();
            foreach (var result in ScanResult)
            {
                sb.Append($"{DisplayName}\t");

                foreach (var o in result)
                {
                    sb.Append($"{o}\t");
                }

                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }

        protected GUIStyle maxFontSize;
        protected GUIContent disableContent;
        protected GUIContent enableContent;
        private bool showGui;


        [OnInspectorGUI]
        protected void CustomDrawer()
        {
            if (maxFontSize == null)
            {
                maxFontSize = new GUIStyle();
                maxFontSize.fontSize = 20;
                maxFontSize.normal.textColor = Color.white;

                disableContent = new GUIContent("已关闭", EditorGUIUtility.IconContent("lightMeter/redLight").image);
                enableContent = new GUIContent("已启用", EditorGUIUtility.IconContent("lightMeter/greenLight").image);
            }


            SirenixEditorGUI.BeginHorizontalToolbar(height: 30);
            if (SirenixEditorGUI.IconButton(
                    showGui
                        ? Sirenix.Utilities.Editor.EditorIcons.TriangleDown
                        : Sirenix.Utilities.Editor.EditorIcons.TriangleRight, height: 30))
            {
                showGui = !showGui;
            }

            EditorGUILayout.LabelField(DisplayName, maxFontSize);

            if (!string.IsNullOrEmpty(HelpUrl) && GUILayout.Button("帮助", GUILayout.Height(25),GUILayout.Width(50)))
            {
                Application.OpenURL(HelpUrl);
            }
            
            if (GUILayout.Button("Scan", GUILayout.Height(25), GUILayout.Width(50)))
            {
                Scan();
            }

            if (GUILayout.Button("Fix", GUILayout.Height(25), GUILayout.Width(50)))
            {
                Fix();
            }

            IsEnable = SirenixEditorGUI.ToolbarToggle(IsEnable, IsEnable ? enableContent : disableContent);
            SirenixEditorGUI.EndHorizontalToolbar();

            if (showGui)
            {
                EditorGUI.indentLevel += 2;
                EditorGUILayout.Space(10);
                SirenixEditorGUI.BeginVerticalList(drawBorder: false, drawDarkBg: false);
                
                EditorGUILayout.LabelField($"[规则ID]: {RuleId}", ProjectScanTools.DefaultStyle);
                EditorGUILayout.LabelField($"[说明]: {Description}", ProjectScanTools.DefaultStyle);
                EditorGUILayout.Space(10);
                ExtendDrawer();
                SirenixEditorGUI.EndVerticalList();
                EditorGUI.indentLevel -= 2;
            }
        }

        protected virtual void ExtendDrawer()
        {
        }
    }

    public abstract class ScanRuleWithDir : ScanRule
    {
        public bool UseSelfDirConfig { get; set; }

        public List<string> IncludeDir { get; private set; } = new();

        public List<string> IgnoreDir { get; private set; } = new();

        public ScanRuleWithDir() : base()
        {
            if (ProjectScan.GlobalConfig.RuleConfig.TryGetValue(RuleId, out var ruleConfig))
            {
                UseSelfDirConfig = ruleConfig.UseSelfDirConfig;
                IncludeDir.AddRange(ruleConfig.IncludeDir);
                IgnoreDir.AddRange(ruleConfig.IgnoreDir);
            }
        }

        protected override void ExtendDrawer()
        {
            using (new GUILayout.HorizontalScope())
            {
                // GUILayout.Space();
                EditorGUILayout.LabelField("是否启用单独的目标文件夹配置", ProjectScanTools.DefaultStyle);
                UseSelfDirConfig =
                    SirenixEditorGUI.ToolbarToggle(UseSelfDirConfig, UseSelfDirConfig ? enableContent : disableContent);
            }

            if (UseSelfDirConfig)
            {
                ProjectScanTools.DrawPathList(IncludeDir, "目标文件夹");
                ProjectScanTools.DrawPathList(IgnoreDir, "忽略文件夹");
            }
        }

        async void OpenFolderPanel(List<string> addList)
        {
            await Task.Delay(10);
            var selected = EditorUtility.OpenFolderPanel("添加目录", Application.dataPath, "");
            addList.Add(Path.GetRelativePath(Application.dataPath, selected));
        }

        protected List<T> GetRes<T>(string filter, out List<string> resultPath) where T : Object
        {
            var importers = GetAssetImporter<AssetImporter>(filter);
            List<T> result = new List<T>(importers.Count);
            resultPath = new List<string>(importers.Count);
            foreach (var importer in importers)
            {
                resultPath.Add(importer.assetPath);
                T res = AssetDatabase.LoadAssetAtPath<T>(importer.assetPath);
                result.Add(res);
            }

            return result;
        }

        protected List<T> GetAssetImporter<T>(string filter) where T : AssetImporter
        {
            List<string> targetFolders;
            List<string> ignoreFolders;
            if (!UseSelfDirConfig)
            {
                targetFolders = ProjectScan.GlobalConfig.IncludeDir;
                ignoreFolders = ProjectScan.GlobalConfig.IgnoreDir;
            }
            else
            {
                targetFolders = IncludeDir;
                ignoreFolders = IgnoreDir;
            }

            var guids = AssetDatabase.FindAssets(filter, targetFolders.ToArray()).ToList();
            var ignoreGuids = AssetDatabase.FindAssets(filter, ignoreFolders.ToArray());
            guids.RemoveRange(ignoreGuids);
            List<T> result = new List<T>(guids.Count);
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                result.Add(AssetImporter.GetAtPath(path) as T);
            }

            return result;
        }

        protected void InternalScanImporter<T>(string filter, Action<T> action) where T : AssetImporter
        {
            var assetImporter = GetAssetImporter<T>(filter);
            for (int i = 0; i < assetImporter.Count; i++)
            {
                ShowProgress(assetImporter[i].assetPath, i * 1.0f / assetImporter.Count);
                action(assetImporter[i]);
            }

            EditorUtility.ClearProgressBar();
            LogResult();
        }

        protected void InternalScanObject<T>(string filter, Action<T, string> action) where T : Object
        {
            var objs = GetRes<T>(filter, out var paths);
            for (int i = 0; i < objs.Count; i++)
            {
                ShowProgress(paths[i], i * 1.0f / objs.Count);
                action(objs[i], paths[i]);
            }

            EditorUtility.ClearProgressBar();
            LogResult();
        }

        protected void InternalScanImporterAndObject<TObject, TImporter>(string filter,
            Action<TObject, TImporter> action) where TImporter : AssetImporter where TObject : Object
        {
            var assetImporter = GetAssetImporter<TImporter>(filter);
            for (int i = 0; i < assetImporter.Count; i++)
            {
                ShowProgress(assetImporter[i].assetPath, i * 1.0f / assetImporter.Count);
                TObject obj = AssetDatabase.LoadAssetAtPath<TObject>(assetImporter[i].assetPath);
                action(obj, assetImporter[i]);
            }

            EditorUtility.ClearProgressBar();
            LogResult();
        }

        protected void InternalScanAllObj<T>(string filter, Action<T, string> action) where T : Object
        {
            var objs = GetRes<T>(filter, out var paths);
            foreach (var path in paths)
            {
                var tmpObjs = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var obj in tmpObjs)
                {
                    if (obj is T needObj)
                    {
                        action(needObj, path);
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            LogResult();
        }

        /// <summary>
        /// 第二个参数为object[]的toString值
        /// </summary>
        protected void InternalFixImporter<T>(Action<T, string[]> action) where T : AssetImporter
        {
            for (int i = 0; i < ScanResult.Count; i++)
            {
                var objects = ScanResult[i].Select((o)=> o.ToString()).ToArray();
                var path = objects[0];
                ShowProgress(path, i * 1.0f / ScanResult.Count);
                var obj = AssetImporter.GetAtPath(path) as T;
                action(obj, objects);
                obj.SaveAndReimport();
            }

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 第三个参数为object[]的toString值
        /// </summary>
        protected void InternalFixImporterAndObj<TObject, TImporter>(Action<TObject, TImporter, string[]> action)
            where TImporter : AssetImporter where TObject : Object
        {
            for (int i = 0; i < ScanResult.Count; i++)
            {
                var objects = ScanResult[i].Select((o) => o.ToString()).ToArray();
                var path = objects[0];
                ShowProgress(path, i * 1.0f / ScanResult.Count);
                var importer = AssetImporter.GetAtPath(path) as TImporter;
                var obj = AssetDatabase.LoadAssetAtPath<TObject>(path);
                action(obj, importer, objects);
                importer.SaveAndReimport();
            }

            EditorUtility.ClearProgressBar();
        }
    }

    public enum RulePriority
    {
        Low,
        Medium,
        High
    }
}