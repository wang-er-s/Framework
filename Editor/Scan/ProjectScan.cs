using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CatJson;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Framework.Editor
{
    public class ProjectScan : OdinMenuEditorWindow
    {
        [UnityEditor.MenuItem("Framework/Game Scan/Setting")]
        private static void CreateWindow()
        {
            Init();
            GetWindow<ProjectScan>().Show();
        }

        [UnityEditor.MenuItem("Framework/Game Scan/Scan All")]
        private static void Scan()
        {
            Init();
            foreach (var scanMenu in scanMenus)
            {
                if (scanMenu.IsEnable)
                    scanMenu.Scan();
            }
            SaveResult();
        }


        private static void Init()
        {
            ProjectScanPath.Init();
            var content = File.Exists(ProjectScanPath.ProjectScanConfigPath) ? File.ReadAllText(ProjectScanPath.ProjectScanConfigPath) : string.Empty;
            if (string.IsNullOrEmpty(content))
            {
                content = File.ReadAllText(ProjectScanPath.LocalScanConfigPath);
            }
            GlobalConfig = JsonParser.Default.ParseJson<ProjectScanGlobalConfig>(content);
            CreateMenuRules();
        }

        private void Update()
        {
            if (GlobalConfig == null)
            {
                Init();
            }
        }
        
        public static ProjectScanGlobalConfig GlobalConfig { get; private set; }
        private static List<ScanMenu> scanMenus = new();
        private static List<ScanRule> scanRules = new();

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false, OdinMenuStyle.TreeViewStyle);
            foreach (var scanMenu in scanMenus)
            {
                tree.Add(scanMenu.Name, scanMenu);
            }
            
            return tree;
        }

        private static void CreateMenuRules()
        {
            scanMenus.Clear();
            scanRules.Clear();
            Type menuInterface = typeof(ScanMenu);
            Type scanRuleInterface = typeof(ScanRule);
            List<Type> scanMenuType = new();
            List<Type> scanRuleType = new();
            foreach (var type in typeof(ProjectScan).Assembly.GetTypes())
            {
                if (!type.IsInterface && !type.IsAbstract)
                {
                    if (menuInterface.IsAssignableFrom(type))
                    {
                        scanMenuType.Add(type);
                    }
                    else if(scanRuleInterface.IsAssignableFrom(type))
                    {
                        scanRuleType.Add(type);
                    }
                }
            }

            foreach (var type in scanMenuType)
            {
                ScanMenu menu = Activator.CreateInstance(type) as ScanMenu;
                scanMenus.Add(menu);
            }

            scanMenus.Sort((s1, s2) => s1.Priority.CompareTo(s2.Priority));

            foreach (var type in scanRuleType)
            {
                ScanRule rule = Activator.CreateInstance(type) as ScanRule;
                scanRules.Add(rule);
                foreach (var scanMenu in scanMenus)
                {
                    scanMenu.CheckRule(rule);
                }
            }

            foreach (var scanMenu in scanMenus)
            {
                scanMenu.FillRuleFinish();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Save();
        }

        private static void Save()
        {
            foreach (var scanRule in scanRules)
            {
                GlobalConfig.RuleConfig[scanRule.RuleId] = new ScanRuleConfig(scanRule);
            }

            foreach (var scanMenu in scanMenus)
            {
                GlobalConfig.MenuEnable[scanMenu.GetType().Name] = scanMenu.IsEnable;
            }

            GlobalConfig.Save();
        }

        public static void SaveResult()
        {
            SaveScanResultJson();
            SaveScanResultToCsv();
            EditorUtility.OpenWithDefaultApp(ProjectScanPath.ProjectConfigRootPath);
        }
        
        private static void SaveScanResultToCsv()
        {
            StringBuilder result = new StringBuilder();
            foreach (var scanRule in scanRules)
            {
                foreach (var objs in scanRule.ScanResult)
                {
                    result.Append(ProjectScanTools.ParseCSVItem(scanRule.RuleId));
                    result.Append(ProjectScanTools.ParseCSVItem(scanRule.DisplayName));
                    foreach (var obj in objs)
                    {
                        result.Append(ProjectScanTools.ParseCSVItem(obj));
                    }
                    result.AppendLine();
                }
            }

            File.WriteAllText(string.Format(ProjectScanPath.ScanResultCSVPath,$"{DateTime.Now:yy-MM-dd HH.mm}"), result.ToString());
        }

        private static void SaveScanResultJson()
        {
            Dictionary<string,ScanResultSave> resultSaves = new();
            foreach (var scanRule in scanRules)
            {
                List<object[]> scanResult = scanRule.ScanResult;
                if (scanResult.Count > 0)
                {
                    ScanResultSave scanResultSave = new ScanResultSave();
                    List<Type> types = new List<Type>(scanResult.Count);
                    foreach (var o in scanResult[0])
                    {
                        types.Add(o.GetType());
                    }

                    scanResultSave.Types = types;
                    scanResultSave.ObjectsList = scanResult;
                    scanResultSave.RuleId = scanRule.RuleId;
                    resultSaves.Add(scanRule.RuleId,scanResultSave);
                }
            }
            File.WriteAllText(string.Format(ProjectScanPath.ScanResultJsonPath,$"{DateTime.Now:yy-MM-dd HH.mm}"), JsonParser.Default.ToJson(resultSaves));
        }

        public static void LoadScanResult(string path)
        {
            var resultSaves = JsonParser.Default.ParseJson<Dictionary<string,ScanResultSave>>(File.ReadAllText(path));
            foreach (var scanRule in scanRules)
            {
                if (resultSaves.TryGetValue(scanRule.RuleId, out var save))
                {
                    scanRule.ScanResult.Clear();
                    scanRule.ScanResult.AddRange(save.ObjectsList);
                }
            }
        }

        // 用来保存扫描后的结果
        private struct ScanResultSave
        {
            public string RuleId;
            public List<Type> Types;
            public List<object[]> ObjectsList;
        }

    }
}