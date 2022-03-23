using System.IO;
using UnityEditor;

namespace Framework.Editor.AssetsChecker
{
    public class AnimationChecker : IAssetRule<BasicAssetCheckerCollection, ModelImporter>
    {
        public string Description => "Compression != Optimal的动画资源";
        public RulePriority Priority { get; }
        private RuleDataTable table;
        public void Run(out bool hasTable, out RuleDataTable table)
        {
            hasTable = true;
            table = new RuleDataTable("名称", "路径", "压缩类型");
        }

        public void Check(ModelImporter assetImporter)
        {
            if (assetImporter.animationCompression == ModelImporterAnimationCompression.Off)
            {
                var path = assetImporter.assetPath;
                var name = Path.GetFileNameWithoutExtension(path);
                table.AddRow(name, path, assetImporter.animationCompression.ToString());
            }
        }
    }
}