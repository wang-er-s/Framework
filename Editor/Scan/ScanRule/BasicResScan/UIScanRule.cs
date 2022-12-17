using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Editor
{
    public class UIOutlineTextScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "UI_TextWithOutline";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/UI";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanObject<GameObject>("t:prefab", (go, path) =>
            {
                if(go.GetComponent<RectTransform>() == null) return;
                var texts = go.GetComponentsInChildren<Text>();
                foreach (var text in texts)
                {
                    if (text.GetComponent<Outline>() != null)
                    {
                        ScanResult.Add(new object[]
                        {
                            path,
                            new KeyValue("组件路径", text.GetRelativePath(go.transform))
                        });
                    }
                }
            });
        }
    }
    
    public class UIMipmapSpriteScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "UI_MipmapSprite";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/UI";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanImporter<TextureImporter>("t:sprite", (importer) =>
            {
                if (importer.mipmapEnabled)
                {
                    ScanResult.Add(new object[] { importer.assetPath });
                }
            });
        }

        public override void Fix(Func<string,bool> filter = null)
        {
            InternalFixImporter<TextureImporter>((importer, _) =>
            {
                importer.mipmapEnabled = false;
            }, filter);
        }
    }
    
    
    public class UIRawImageAlphaAllZeroScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "UI_RawImageAlphaAllZero";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/UI";
        public override RulePriority Priority { get; } = RulePriority.Medium;
        public override void Scan()
        {
            InternalScanObject<GameObject>("t:prefab", (go, path) =>
            {
                if (go.GetComponent<RectTransform>() == null) return;
                var objs = go.GetComponentsInChildren<RawImage>();
                foreach (var image in objs)
                {
                    if(image.canvasRenderer.cullTransparentMesh) return;
                    if (image.color.a <= 0)
                    {
                        ScanResult.Add(new object[] { path, new KeyValue("组件路径", image.GetRelativePath(go.transform)) });
                    }
                }
            });
        }
    }
    
    public class UIRawImageNullTexScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "UI_RawImageNullTex";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/UI";
        public override RulePriority Priority { get; } = RulePriority.Medium;
        public override void Scan()
        {
            InternalScanObject<GameObject>("t:prefab", (go, path) =>
            {
                if (go.GetComponent<RectTransform>() == null) return;
                var objs = go.GetComponentsInChildren<RawImage>();
                foreach (var image in objs)
                {
                    if (image.texture == null)
                    {
                        ScanResult.Add(new object[] { path, new KeyValue("组件路径", image.GetRelativePath(go.transform)) });
                    }
                }
            });
        }
    }
    
    public class UITiledImageScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "UI_TiledImage";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/UI";
        public override RulePriority Priority { get; } = RulePriority.Medium;
        public override void Scan()
        {
            InternalScanObject<GameObject>("t:prefab", (go, path) =>
            {
                if (go.GetComponent<RectTransform>() == null) return;
                var objs = go.GetComponentsInChildren<Image>();
                foreach (var image in objs)
                {
                    if (image.type == Image.Type.Tiled)
                    {
                        var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(image.sprite)) as TextureImporter;
                        if (importer.wrapMode != TextureWrapMode.Repeat)
                            ScanResult.Add(new object[] { path, new KeyValue("组件路径", image.GetRelativePath(go.transform)) });
                    }
                }
            });
        }
    }
}