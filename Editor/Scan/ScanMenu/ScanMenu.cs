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

        private string typeName;

        public ScanMenu()
        {
            typeName = GetType().Name;
            if (ResScan.Config.MenuEnable.TryGetValue(GetType().Name, out var enable))
            {
                IsEnable = enable;
            }
        }

        public virtual void CheckRule(ScanRule rule)
        {
            if (rule.Menu.StartsWith(typeName))
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
        }

        public void Scan()
        {
            foreach (var ruleList in Rules.Values)
            {
                foreach (var scanRule in ruleList.Rules)
                {
                    scanRule.Scan();
                }
            }     
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