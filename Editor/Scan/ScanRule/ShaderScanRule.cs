using System.Reflection;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace Framework.Editor
{
    public class ShaderTooManyGlobalKWScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Shader_ToolManyGlobalKWs";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Shader";
        public override RulePriority Priority { get; } = RulePriority.Medium;
        public override void Scan()
        {
            var getKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords", BindingFlags.Static | BindingFlags.NonPublic);
            int limit = Value.ToInt();
            InternalScanObject<Shader>("t:shader", (shader, path) =>
            {
                string[] keywords = getKeywordsMethod.Invoke(null, new object[] { shader }) as string[];
                if (keywords.Length > limit)
                    ScanResult.Add(new object[] { path, new KeyValue("keyword数量", keywords.Length )});
            });
        }
    }
    
    public class ShaderTooManyTextureScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Shader_ToolManyTexture";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Shader";
        public override RulePriority Priority { get; } = RulePriority.Medium;
        public override void Scan()
        {
            int limit = Value.ToInt();
            InternalScanObject<Shader>("t:shader", (shader, path) =>
            {
                var textureCount = new Material(shader).GetTexturePropertyNames().Length;
                if (textureCount > limit)
                    ScanResult.Add(new object[] { path, new KeyValue("贴图数量", textureCount) });
            });
        }
    }

    public class ShaderVariantScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Shader_TooManyVariants";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Shader";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            string tmpDir = "Assets/t2m5p921";
            string tmpPath = tmpDir + "/tmp.shadervariants";
            int limit = Value.ToInt();
            ShaderVariantCollector.Run(tmpPath, () =>
            {
                var files = tmpDir.DirGetSubFiles(pattern: "*.json", isRecursive: false);
                if (files.Count != 1)
                {
                    Debug.LogError("shader变体受体出错了");
                    return;
                }

                var svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(tmpPath);
                var manifest = ShaderVariantCollectionReadme.Extract(svc);

                foreach (var variantInfo in manifest.ShaderVariantInfos)
                {
                    if (variantInfo.ShaderVariantElements.Count > limit)
                    {
                        ScanResult.Add(new object[]
                            { variantInfo.AssetPath, new KeyValue("变体数量", variantInfo.ShaderVariantElements.Count) });
                    }
                }

                AssetDatabase.DeleteAsset(tmpDir);
            });
        }
    }
}