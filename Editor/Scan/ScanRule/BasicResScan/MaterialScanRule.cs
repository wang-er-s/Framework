using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class MaterialEmptySampleScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Material_EmptyTex";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Material";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanObject<Material>("t:material", (mat, path) =>
            {
                foreach (var texturePropertyName in mat.GetTexturePropertyNames())
                {
                    if (mat.GetTexture(texturePropertyName) == null)
                    {
                        ScanResult.Add(new object[] { path });
                        return;
                    }
                }
            });
        }
    }
    
    
    public class MaterialHasSameTexScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Material_SameTex";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Material";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanObject<Material>("t:material", (mat, path) =>
            {
                List<Texture> existTex = new();
                foreach (var texturePropertyName in mat.GetTexturePropertyNames())
                {
                    Texture tex;
                    if ((tex = mat.GetTexture(texturePropertyName)) != null)
                    {
                        if (existTex.Contains(tex))
                        {
                            ScanResult.Add(new object[] { path });
                            return;
                        }
                        else
                        {
                            existTex.Add(tex);
                        }
                    }
                }
            });
        }
    }
    
    
    public class MaterialHasPureTexScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Material_PureColorTex";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Material";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanObject<Material>("t:material", (mat, path) =>
            {
                foreach (var texturePropertyName in mat.GetTexturePropertyNames())
                {
                    Texture tex;
                    if ((tex = mat.GetTexture(texturePropertyName)) != null)
                    {
                        if (TexturePureColorScanRule.IsPureTexture(tex as Texture2D))
                        {
                            ScanResult.Add(new object[] { path });
                        }
                    }
                }
            });
        }
    }
    
    public class MaterialHasUselessTexScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Material_UselessTex";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Material";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            // TODO 检测材质无用属性
            InternalScanObject<Material>("t:material", (mat, path) =>
            {
            });
        }
    }

    public class MaterialDefaultMat : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Material_DefaultMat";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Material";
        public override RulePriority Priority { get; } = RulePriority.High;
        private const string DefaultMatStr = "Resources/unity_builtin_extra";

        public override void Scan()
        {
            InternalScanObject<GameObject>("t:prefab", (go, path) =>
            {
                var renderers = go.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    if (AssetDatabase.GetAssetPath(renderer.sharedMaterial) == DefaultMatStr)
                    {
                        ScanResult.Add(new object[] { path, renderer.GetRelativePath(go.transform) });
                    }
                }
            });
        }
    }
}