using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class ResFixWindow : OdinMenuEditorWindow
    {
        public static void Open(List<ScanRule> rules)
        {
            Init(rules);
            GetWindow<ResFixWindow>().Show();
        }

        private static List<MenuItem> menuItems = new();
        private static Dictionary<string, List<string>> notFixListDic = new();

        private static void Init(List<ScanRule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.ScanResult.Count > 0)
                    menuItems.Add(new MenuItem(rule));
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false, OdinMenuStyle.TreeViewStyle);
            tree.Add("[首页]", new TitleMenu(menuItems));
            foreach (var menuItem in menuItems)
            {
                tree.Add(menuItem.Name, menuItem);
            }
            return tree;
        }

        private class TitleMenu
        {
            private List<MenuItem> menuItems;
            public TitleMenu(List<MenuItem> menuItems)
            {
                this.menuItems = menuItems;
            }
            [Button("全部修复", ButtonSizes.Large)]
            private void FixAll()
            {
                foreach (var menuItem in menuItems)
                {
                    menuItem.Fix();
                }
            }
        }

        private class MenuItem
        {
            public string Name { get; }
            private ScanRule rule;
            private List<string> notFixList;
            [ShowInInspector]
            private string ruleId => rule.RuleId;
            [ShowInInspector]
            [PropertyOrder(1)]
            [TableList(IsReadOnly = true, AlwaysExpanded = true, HideToolbar = true)]
            private List<DrawRule> drawRules;

            public MenuItem(ScanRule rule)
            {
                string ruleType = rule.Menu.Split('/').Last();
                Name = ruleType + "/" + rule.DisplayName;
                this.rule = rule;
                notFixList = new List<string>();
                notFixListDic[this.rule.RuleId] = notFixList;

                drawRules = new List<DrawRule>();
                foreach (var objs in rule.ScanResult)
                {
                    string path = objs[0] as string;
                    drawRules.Add(new DrawRule(notFixList, path, rule.RuleId));
                }
            }

            [Button("@(rule.HasFixMethod ? \"一键修复\" : \"无法一键修复，请自行手动修改\")",ButtonSizes.Large)]
            public void Fix()
            {
                if(!rule.HasFixMethod) return;
                rule.Fix((path) => notFixList.Contains(path));
            }

            private class DrawRule
            {
                private List<string> notFixList;
                private string ruleId;
                private List<string> whiteList;
                public DrawRule(List<string> notFixList, string path, string ruleId)
                {
                    this.notFixList = notFixList;
                    this.ruleId = ruleId;
                    this.path = path;
                    ProjectScan.GlobalConfig.WhiteListDic.TryGetValue(this.ruleId, out whiteList);
                }
                
                private string path;
                
                [PropertyOrder(0)]
                [Button("@path")]
                private void Path()
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                }

                [PropertyOrder(1)]
                [TableColumnWidth(80, resizable:false)]
                [ShowInInspector]
                private bool 是否修复
                {
                    get
                    {
                        if (notFixList.Contains(path)) return false;
                        if (whiteList != null && whiteList.Contains(path))
                        {
                            return false;
                        }

                        return true;
                    }
                    set
                    {
                        if (value)
                        {
                            notFixList.TryRemove(path);
                        }
                        else
                        {
                            notFixList.TryAddSingle(path);
                        }
                    }
                }

                private bool hasInWhiteList => whiteList != null && whiteList.Contains(path);
                [PropertyOrder(2)]
                [TableColumnWidth(130, resizable: false)]
                [Button(
                    "@(hasInWhiteList ? \"[-]从白名单删除\" : \"[+]添加到白名单\")")]
                private void 白名单()
                {
                    if (hasInWhiteList)
                    {
                        whiteList.TryRemove(path);
                    }
                    else
                    {
                        if (whiteList == null)
                        {
                            whiteList = new List<string>();
                            ProjectScan.GlobalConfig.WhiteListDic[ruleId] = whiteList;
                        }

                        whiteList.TryAddSingle(path);
                    }
                }
            }
        }
    }

}