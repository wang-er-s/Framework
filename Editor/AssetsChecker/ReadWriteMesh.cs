using UnityEditor;

namespace Framework.Editor.AssetsChecker
{
    [BelongToCollection(typeof(BasicAssetCheckerCollection))]
    public class ReadWriteMesh : IAssetRule
    {
        public override void Check(AssetImporter assetImporter)
        {
            var modelImporter = assetImporter as ModelImporter;
        }

        public override string Description => "开启Read/Write选项的网格";
        public override RulePriority Priority { get; }
        private RuleDataTable table;
        public override void Run(out bool hasTable, out RuleDataTable table)
        {
            hasTable = true;
            table = new RuleDataTable("");
            this.table = table;
        }
    }
}