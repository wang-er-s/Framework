using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.AssetsChecker
{
    public class BasicAssetCheckerCollection : CheckerCollection
    {
        public override string Name => "基本资源检查";
        private List<AssetRule> assetRules = new List<AssetRule>();

        public override void AddRule(Rule rule)
        {
            base.AddRule(rule);
            if(rule is AssetRule assetRule)
                assetRules.Add(assetRule);
        }

        protected override void Run()
        {
            base.Run();
            var assets = AssetDatabase.FindAssets("t:model");
            foreach (var asset in assets)
            {
                var importer = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(asset));
                foreach (var rule in assetRules)
                {
                    rule.Check(importer);
                }
            }

            foreach (var rule in assetRules)
            {
                Debug.Log(rule);
                Debug.Log(rule.Table.ToString());
            }
        }
    }
}