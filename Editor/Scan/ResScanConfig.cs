using System.Collections.Generic;

namespace Framework.Editor
{
    public class ResScanConfig
    {
        public List<string> IncludeDir = new(){"Assets"};
        public List<string> IgnoreDir = new(){"Assets/Ignore"};
        public Dictionary<string, ScanRuleConfig> RuleConfig = new();
        public Dictionary<string, bool> MenuEnable = new();
    }

    public class ScanRuleConfig
    {
        public string Id;
        public bool IsEnable;
        public bool UseSelfDirConfig;
        public List<string> IncludeDir;
        public List<string> IgnoreDir;
        public string Value = string.Empty;

        public ScanRuleConfig()
        {
        }
        
        public ScanRuleConfig(ScanRule scanRule)
        {
            Id = scanRule.RuleId;
            IsEnable = scanRule.IsEnable;
            Value = scanRule.Value;
            if (scanRule is ScanRuleWithDir scanRuleWithDir)
            {
                UseSelfDirConfig = scanRuleWithDir.UseSelfDirConfig;
                IncludeDir = scanRuleWithDir.IncludeDir;
                IgnoreDir = scanRuleWithDir.IgnoreDir;
            }
            else
            {
                UseSelfDirConfig = false;
            }
        }
    }
}