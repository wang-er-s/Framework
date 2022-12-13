using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CatJson;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class ResScan : OdinMenuEditorWindow
    {
        [MenuItem("Framework/Res Scan/Setting")]
        private static void CreateWindow()
        {
            Init();
            GetWindow<ResScan>().Show();
        }

        [MenuItem("Framework/Res Scan/Scan All")]
        private static void Scan()
        {
            Init();
            foreach (var scanMenu in scanMenus)
            {
                if (scanMenu.IsEnable)
                    scanMenu.Scan();
            }
            SaveScanResultJson();
            SaveScanResultToCsv();
            EditorUtility.OpenWithDefaultApp(ProjectConfigRootPath);
        }

        private static void SaveScanResultToCsv()
        {
            StringBuilder result = new StringBuilder();
            foreach (var scanRule in scanRules)
            {
                foreach (var objs in scanRule.ScanResult)
                {
                    result.Append(ResScanTools.ParseCSVItem(scanRule.RuleId));
                    result.Append(ResScanTools.ParseCSVItem(scanRule.DisplayName));
                    foreach (var obj in objs)
                    {
                        result.Append(ResScanTools.ParseCSVItem(obj));
                    }
                    result.AppendLine();
                }
            }

            File.WriteAllText(string.Format(ScanResultCSVPath,$"{DateTime.Now:yy-MM-dd HH.mm}"), result.ToString());
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
            File.WriteAllText(string.Format(ScanResultJsonPath,$"{DateTime.Now:yy-MM-dd HH.mm}"), JsonParser.Default.ToJson(resultSaves));
        }

        public static void LoadScanResult(string path)
        {
            var resultSaves = JsonParser.Default.ParseJson<Dictionary<string,ScanResultSave>>(File.ReadAllText(path));
            foreach (var scanRule in scanRules)
            {
                if (resultSaves.TryGetValue(scanRule.RuleId, out var save))
                {
                    scanRule.ScanResult.AddRange(save.ObjectsList);
                }
            }
        }

        private static void Init()
        {
            var guids = AssetDatabase.FindAssets($"{nameof(ResScan)}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileName(path) == $"{nameof(ResScan)}.cs")
                {
                    LocalFolderRootPath = Path.GetDirectoryName(path);
                    break;
                }
            }

            ProjectConfigRootPath = Path.Combine(Application.dataPath, "..", "ResScan");

            var content = File.Exists(ProjectScanConfigPath) ? File.ReadAllText(ProjectScanConfigPath) : string.Empty;
            if (string.IsNullOrEmpty(content))
            {
                content = File.ReadAllText(LocalScanConfigPath);
            }
            Config = JsonParser.Default.ParseJson<ResScanConfig>(content);

            var ruleContent = AssetDatabase.LoadAssetAtPath<TextAsset>(ScanRuleTxtPath);
            if (ruleContent == null)
            {
                throw new Exception(ScanRuleTxtPath);
            }

            var result = JsonParser.Default.ParseJson<List<ScanRuleConfig>>(ruleContent.text);
            foreach (var rule in result)
            {
                RuleConfig[rule.Id] = rule;
            }

            CreateMenuRules();
            
            
        }

        private void Update()
        {
            if (Config == null)
            {
                Init();
            }
        }
        
        public static ResScanConfig Config { get; private set; }
        // 当前脚本的根目录
        public static string LocalFolderRootPath { get; private set; }
        public static string ProjectConfigRootPath { get; private set; }
        private static string ScanRuleTxtPath => Path.Combine(LocalFolderRootPath, "Config/ScanRule.txt");
        private static string LocalScanConfigPath => Path.Combine(LocalFolderRootPath, "Config/ResScanConfig.txt");
        private static string ProjectScanConfigPath => Path.Combine(ProjectConfigRootPath, "ResScanConfig.txt");
        private static string ScanResultJsonPath => Path.Combine(ProjectConfigRootPath, "JsonResult{0}.txt");
        private static string ScanResultCSVPath => Path.Combine(ProjectConfigRootPath, "CSVResult{0}.csv");
        public static Dictionary<string, ScanRuleConfig> RuleConfig = new();
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
            Type menuInterface = typeof(ScanMenu);
            Type scanRuleInterface = typeof(ScanRule);
            List<Type> scanMenuType = new();
            List<Type> scanRuleType = new();
            foreach (var type in typeof(ResScan).Assembly.GetTypes())
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
                Config.RuleConfig[scanRule.RuleId] = new Editor.ScanRuleConfig(scanRule);
            }

            foreach (var scanMenu in scanMenus)
            {
                Config.MenuEnable[scanMenu.GetType().Name] = scanMenu.IsEnable;
            }

            Directory.CreateDirectory(ProjectConfigRootPath);
            File.WriteAllText(ProjectScanConfigPath, JsonParser.Default.ToJson(Config));
        }

        // 用来保存扫描后的结果
        private struct ScanResultSave
        {
            public string RuleId;
            public List<Type> Types;
            public List<object[]> ObjectsList;
        }

        // 用来加载scan rule的配置
        [Serializable]
        public struct ScanRuleConfig : IEquatable<ScanRuleConfig>
        {
            public static ScanRuleConfig Empty = new ScanRuleConfig() { Id = "-1" };
            public string Id;
            public string Name;
            public string Desc;
            public string HelpUrl;

            public bool Equals(ScanRuleConfig other)
            {
                return Id == other.Id && Name == other.Name && Desc == other.Desc && HelpUrl == other.HelpUrl;
            }

            public override bool Equals(object obj)
            {
                return obj is ScanRuleConfig other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id, Name, Desc, HelpUrl);
            }

            public static bool operator ==(ScanRuleConfig s1, ScanRuleConfig s2)
            {
                return s1.Equals(s2);
            }

            public static bool operator !=(ScanRuleConfig s1, ScanRuleConfig s2)
            {
                return !(s1 == s2);
            }
        }
    }
}