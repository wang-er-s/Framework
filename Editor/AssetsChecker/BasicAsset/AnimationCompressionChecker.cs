using System;
using System.IO;
using UnityEditor;

namespace Framework.Editor.AssetsChecker
{
    [BelongToCollectionAttribute(typeof(BasicAssetCheckerCollection))]
    public class AnimationCompressionChecker : AssetRule
    {
        public AnimationCompressionChecker()
        {
            Table = new RuleDataTable("名称", "路径", "压缩类型");
        }
        
        protected override string Description => "Compression != Optimal的动画资源";
        protected override RulePriority Priority { get; }
    
        public override void Run()
        {
            
        }

        public override void Check(AssetImporter assetImporter)
        {
            var modelImporter = assetImporter as ModelImporter;
            if(modelImporter == null) return;
            if (modelImporter.animationCompression != ModelImporterAnimationCompression.Optimal)
            {
                var path = assetImporter.assetPath;
                var name = Path.GetFileNameWithoutExtension(path);
                Table.AddRow(name, path, modelImporter.animationCompression.ToString());
            }
        }
    }
}