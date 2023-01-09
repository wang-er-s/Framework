using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;

namespace Framework.Editor
{
    public abstract class ScanMenu 
    {
        [PropertyOrder(1)]
        [ShowInInspector]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, IsReadOnly = true, KeyLabel = "类型", ValueLabel = "")]
        [HideIf("@Rules.Count <= 0")]
        protected Dictionary<string, ShowRules> Rules = new();
        public bool IsEnable { get; set; } = true;
        public abstract string Name { get; }
        public virtual int Priority { get; protected set; } = 1;
        private bool hasFixRule = false;

        public ScanMenu()
        {
            if (ProjectScan.GlobalConfig.MenuEnable.TryGetValue(GetType().Name, out var enable))
            {
                IsEnable = enable;
            }
        }

        public virtual void CheckRule(ScanRule rule)
        {
            if (rule.Menu.StartsWith(GetType().Name))
            {
                string type = string.Empty;
                var match = Regex.Match(rule.Menu, @"(\w+)/(\w+)");
                if (match.Success)
                {
                    type = match.Groups[2].Value;
                }
                
                if (!Rules.TryGetValue(type, out var list))
                {
                    list = new ShowRules();
                    Rules[type] = list;
                }

                list.Rules.Add(rule);
            }
        }

        public void FillRuleFinish()
        {
            foreach (var rules in Rules.Values)
            {
                foreach (var rule in rules.Rules)
                {
                    if (rule.HasFixMethod)
                    {
                        hasFixRule = true;
                        break;
                    }
                }
                if(hasFixRule) break;
            }
        }

        public void Scan()
        {
            foreach (var ruleList in Rules.Values)
            {
                foreach (var scanRule in ruleList.Rules)
                {
                    if (scanRule.IsEnable)
                        scanRule.Scan();
                }
            }     
        }

        
        [PropertyOrder(0)]
        [ShowIf("@hasFixRule")]
        [Button("打开资源修复面板", ButtonSizes.Large)]
        private void ShowFixWindow()
        {
            List<ScanRule> rules = new();
            foreach (var showRules in Rules.Values)
            {
                rules.AddRange(showRules.Rules);
            }

            ResFixWindow.Open(rules);
        }

        [HideReferenceObjectPicker]
        [HideLabel]
        protected class ShowRules
        {
            [ListDrawerSettings(Expanded = true, IsReadOnly = true, ShowPaging = true, ShowIndexLabels = false,
                DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ShowItemCount = false)]
            [HideReferenceObjectPicker]
            public List<ScanRule> Rules = new();
        }
    }
}