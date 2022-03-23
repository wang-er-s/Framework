using UnityEditor;

namespace Framework.Editor.AssetsChecker
{
    [BelongToCollection(typeof(BasicAssetCheckerCollection))]
    public class AnimationHasScale : AssetRule
    {
        public AnimationHasScale()
        {
            Table = new RuleDataTable();
        }
        
        public override void Check(AssetImporter assetImporter)
        {
        }

        protected override string Description { get; }
        protected override RulePriority Priority { get; }
        public override void Run()
        {
        }
    }
}