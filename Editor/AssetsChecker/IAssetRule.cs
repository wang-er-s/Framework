using System;
using Sirenix.OdinInspector;

namespace Framework.Editor.AssetsChecker
{
    using UnityEditor;

    [Serializable]
    [HideLabel]
    [HideInTables]
    public abstract class IAssetRule : IRule
    {
        public abstract void Check(AssetImporter assetImporter);
        [ShowInInspector]
        [HideLabel]
        public abstract string Description { get; }
        [ShowInInspector]
        [HideLabel]
        public abstract RulePriority Priority { get; }
        public abstract void Run(out bool hasTable, out RuleDataTable table);
    }
}