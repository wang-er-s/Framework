using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    public class TextureSizeScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Texture_Size";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Texture";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            var maxSize = Value.ToInt();
            InternalScanObject<Texture>("t:texture", (texture, path) =>
            {
                if (texture.width > maxSize && texture.height > maxSize)
                {
                    ScanResult.Add(new object[]
                        { path, new KeyValue("贴图大小", new Vector2(texture.width, texture.height)) });
                }
            });
        }
    }

    public class TextureReadWriteScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Texture_RW";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Texture";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanImporter<TextureImporter>("t:texture", (textureImporter) =>
            {
                if (textureImporter.isReadable)
                {
                    ScanResult.Add(new object[] { textureImporter.assetPath });
                }
            });
        }

        public override void Fix(Func<string,bool> filter = null)
        {
            InternalFixImporter<TextureImporter>((importer, _) => { importer.isReadable = false; }, filter);
        }
    }

    public class TextureRepeatScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Texture_WrapMode";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Texture";
        public override RulePriority Priority { get; } = RulePriority.Medium;

        public override void Scan()
        {
            InternalScanImporter<TextureImporter>("t:texture", (textureImporter) =>
            {
                if (textureImporter.wrapMode == TextureWrapMode.Repeat)
                {
                    ScanResult.Add(new object[] { textureImporter.assetPath });
                }
            });
        }

        public override void Fix(Func<string,bool> filter = null)
        {
            InternalFixImporter<TextureImporter>((importer, _) => { importer.wrapMode = TextureWrapMode.Clamp; },
                filter);
        }
    }

    public class TextureMoreAlphaScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Texture_TooManyTransparentPixels";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Texture";
        public override RulePriority Priority { get; } = RulePriority.High;

        private float maxRate;

        public TextureMoreAlphaScanRule()
        {
            maxRate = Value.ToFloat();
        }

        public override void Scan()
        {
            InternalScanObject<Texture2D>("t:texture", (texture, path) =>
            {
                if (HasMoreTransparentPixels(texture, out var rate))
                    ScanResult.Add(new object[] { path, new KeyValue("透明占比", rate) });
            });
        }

        private bool HasMoreTransparentPixels(Texture2D texture, out float rate)
        {
            rate = 0;
            if (texture.width == 0 || texture.height == 0) return false;
            texture = ProjectScanTools.CreateRwTexture2D(texture);
            var pixels = texture.GetPixels();
            int count = 0;
            foreach (var color in pixels)
            {
                if (color.a <= 0.01f)
                    count++;
            }

            rate = count * 1.0f / pixels.Length;
            Object.DestroyImmediate(texture);
            if (rate > maxRate) return true;
            return false;
        }
    }

    public class TextureTrilinearScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Texture_FilterMode";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Texture";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanImporter<TextureImporter>("t:texture", (textureImporter) =>
            {
                if (textureImporter.filterMode == FilterMode.Trilinear)
                {
                    ScanResult.Add(new object[] { textureImporter.assetPath });
                }
            });
        }

        public override void Fix(Func<string,bool> filter = null)
        {
            InternalFixImporter<TextureImporter>((importer, _) => { importer.filterMode = FilterMode.Bilinear; }, filter);
        }
    }


    public class TextureCompressionScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Texture_Compression";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Texture";
        public override RulePriority Priority { get; } = RulePriority.High;

        private List<TextureImporterFormat> canUseFormat = new()
        {
            TextureImporterFormat.ASTC_4x4,
            TextureImporterFormat.ASTC_5x5,
            TextureImporterFormat.ASTC_6x6,
            TextureImporterFormat.ASTC_8x8,
            TextureImporterFormat.ASTC_10x10,
            TextureImporterFormat.ASTC_12x12
        };

        public override void Scan()
        {
            InternalScanImporter<TextureImporter>("t:texture", (importer) =>
            {
                var settingsAndroid = importer.GetPlatformTextureSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetPlatformTextureSettings(BuildTargetGroup.iOS.ToString());
                if (!canUseFormat.Contains(settingsAndroid.format) || !canUseFormat.Contains(settingsIos.format))
                {
                    ScanResult.Add(new object[] { importer.assetPath });
                }
            });
        }

        public override void Fix(Func<string,bool> filter = null)
        {
            InternalFixImporter<TextureImporter>((importer, _) =>
            {
                var settingsAndroid = importer.GetPlatformTextureSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetPlatformTextureSettings(BuildTargetGroup.iOS.ToString());
                settingsAndroid.overridden = true;
                settingsIos.overridden = true;
                if (importer.DoesSourceTextureHaveAlpha())
                {
                    settingsAndroid.format = TextureImporterFormat.ASTC_4x4;
                    settingsIos.format = TextureImporterFormat.ASTC_4x4;
                }
                else
                {
                    settingsAndroid.format = TextureImporterFormat.ASTC_6x6;
                    settingsIos.format = TextureImporterFormat.ASTC_6x6;
                }

                importer.SetPlatformTextureSettings(settingsAndroid);
                importer.SetPlatformTextureSettings(settingsIos);
            }, filter);
        }
    }

    public class TexturePureColorScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Texture_PureColor";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Texture";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanObject<Texture2D>("t:texture", (texture, path) =>
            {
                if (IsPureTexture(texture))
                    ScanResult.Add(new object[]
                        { path, new KeyValue("透明贴图大小", new Vector2(texture.width, texture.height)) });
            });
        }

        public static bool IsPureTexture(Texture2D texture)
        {
            if (texture.width == 0 || texture.height == 0) return false;
            texture = ProjectScanTools.CreateRwTexture2D(texture);
            var pixels = texture.GetPixels();
            Color defaultColor = pixels.First();
            foreach (var color in pixels)
            {
                if (!color.NearlySame(defaultColor))
                    return false;
            }

            return true;
        }
    }


    public class TextureAlphaAllOneScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Texture_AlphaAllOne";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Texture";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanObject<Texture2D>("t:texture", (texture, path) =>
            {
                if (IsTextureAlphaAllOne(texture))
                    ScanResult.Add(new object[] { path });
            });
        }

        private static bool IsTextureAlphaAllOne(Texture2D texture)
        {
            if (texture.width == 0 || texture.height == 0) return false;
            texture = ProjectScanTools.CreateRwTexture2D(texture);
            var pixels = texture.GetPixels();
            foreach (var color in pixels)
            {
                if (color.a.NearlyEqual(1))
                    return false;
            }

            return true;
        }
    }
}