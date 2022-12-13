using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class AudioDoubleChannelsScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Audio_DoubleChannels";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Audio";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanObject<AudioClip>("t:audioclip", (clip, path) =>
            {
                if (clip.channels > 1)
                {
                    ScanResult.Add(new object[] { path });
                }
            });
        }

        public override void Fix()
        {
            InternalFixImporter<AudioImporter>((importer, _) =>
            {
                importer.forceToMono = true;
            });
        }
    }
    
    public class AudioStreamingScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Audio_Streaming";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Audio";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanImporterAndObject<AudioClip, AudioImporter>("t:audioclip", (clip, importer) =>
            {
                var settingsAndroid = importer.GetOverrideSampleSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetOverrideSampleSettings(BuildTargetGroup.iOS.ToString());
                if (ResScanTools.IsLongAudio(clip) && (settingsAndroid.loadType != AudioClipLoadType.Streaming ||
                                         settingsIos.loadType != AudioClipLoadType.Streaming))
                {
                    ScanResult.Add(new object[] { importer.assetPath });
                }
            });
        }

        public override void Fix()
        {
            InternalFixImporter<AudioImporter>((importer, _) =>
            {
                var settingsAndroid = importer.GetOverrideSampleSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetOverrideSampleSettings(BuildTargetGroup.iOS.ToString());
                settingsAndroid.loadType = AudioClipLoadType.Streaming;
                settingsIos.loadType = AudioClipLoadType.Streaming;
                importer.SetOverrideSampleSettings(BuildTargetGroup.iOS.ToString(), settingsAndroid);
                importer.SetOverrideSampleSettings(BuildTargetGroup.Android.ToString(), settingsIos);
            });
        }
    }
    
    public class AudioOptimizeSampleRateScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Audio_OptimizeSampleRate";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Audio";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
           InternalScanImporter<AudioImporter>("t:audioclip", (importer) =>
           {
                var settingsAndroid = importer.GetOverrideSampleSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetOverrideSampleSettings(BuildTargetGroup.iOS.ToString());
                if (settingsAndroid.sampleRateSetting != AudioSampleRateSetting.OptimizeSampleRate ||
                    settingsIos.sampleRateSetting != AudioSampleRateSetting.OptimizeSampleRate)
                {
                    ScanResult.Add(new object[] { importer.assetPath });
                }
           }); 
        }

        public override void Fix()
        {
            InternalFixImporter<AudioImporter>((importer, _) =>
            {
                importer.forceToMono = true;
            });
        }
    }


    public class AudioQualityScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Audio_Quality";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Audio";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanImporterAndObject<AudioClip, AudioImporter>("t:audioclip", (clip, importer) =>
            {
                var settingsAndroid = importer.GetOverrideSampleSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetOverrideSampleSettings(BuildTargetGroup.iOS.ToString());
                if (ResScanTools.IsLongAudio(clip) && (settingsAndroid.quality > 51 || settingsIos.quality > 51))
                {
                    ScanResult.Add(new object[] { importer.assetPath });
                }
                else if (ResScanTools.IsMediumAudio(clip) &&
                         (settingsAndroid.quality > 71 || settingsIos.quality > 71))
                {
                    ScanResult.Add(new object[] { importer.assetPath });
                }
                else if (ResScanTools.IsShortAudio(clip) && (settingsAndroid.quality < 99 || settingsIos.quality < 99))
                {
                    ScanResult.Add(new object[] { importer.assetPath });
                }
            });
        }

        public override void Fix()
        {
            InternalFixImporterAndObj<AudioClip, AudioImporter>((clip, importer, _) =>
            {
                var settingsAndroid = importer.GetOverrideSampleSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetOverrideSampleSettings(BuildTargetGroup.iOS.ToString());
                if (ResScanTools.IsLongAudio(clip))
                {
                    settingsAndroid.quality = 50;
                    settingsIos.quality = 50;
                }
                else if (ResScanTools.IsMediumAudio(clip))
                {
                    settingsAndroid.quality = 70;
                    settingsIos.quality = 70;
                }
                else if (ResScanTools.IsShortAudio(clip))
                {
                    settingsAndroid.quality = 100;
                    settingsIos.quality = 100;
                }
                importer.SetOverrideSampleSettings(BuildTargetGroup.iOS.ToString(), settingsAndroid);
                importer.SetOverrideSampleSettings(BuildTargetGroup.Android.ToString(), settingsIos);
            });
        }
    }
    
    public class AudioCompressFormatScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Audio_Compress";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Audio";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanImporterAndObject<AudioClip, AudioImporter>("t:audioclip", (clip, importer) =>
            {
                var settingsAndroid = importer.GetOverrideSampleSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetOverrideSampleSettings(BuildTargetGroup.iOS.ToString());
                if (settingsAndroid.compressionFormat != AudioCompressionFormat.Vorbis ||
                    settingsIos.compressionFormat != AudioCompressionFormat.Vorbis)
                {
                    ScanResult.Add(new object[] { importer.assetPath });
                }
            });
        }

        public override void Fix()
        {
            InternalFixImporterAndObj<AudioClip, AudioImporter>((clip, importer, _) =>
            {
                var settingsAndroid = importer.GetOverrideSampleSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetOverrideSampleSettings(BuildTargetGroup.iOS.ToString());
                settingsAndroid.compressionFormat = AudioCompressionFormat.Vorbis;
                settingsIos.compressionFormat = AudioCompressionFormat.Vorbis;
                importer.SetOverrideSampleSettings(BuildTargetGroup.iOS.ToString(), settingsAndroid);
                importer.SetOverrideSampleSettings(BuildTargetGroup.Android.ToString(), settingsIos);
            });
        }
    }


    public class AudioDecompressOnLoadScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Audio_DecompressOnLoad";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Audio";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanImporterAndObject<AudioClip, AudioImporter>("t:audioclip", (clip, importer) =>
            {
                var settingsAndroid = importer.GetOverrideSampleSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetOverrideSampleSettings(BuildTargetGroup.iOS.ToString());
                if (ResScanTools.IsShortAudio(clip) &&
                    (settingsAndroid.loadType != AudioClipLoadType.DecompressOnLoad ||
                     settingsIos.loadType != AudioClipLoadType.DecompressOnLoad))
                {
                    ScanResult.Add(new object[] { importer.assetPath });
                }
            });
        }

        public override void Fix()
        {
            InternalFixImporter<AudioImporter>((importer, _) =>
            {
                var settingsAndroid = importer.GetOverrideSampleSettings(BuildTargetGroup.Android.ToString());
                var settingsIos = importer.GetOverrideSampleSettings(BuildTargetGroup.iOS.ToString());
                settingsAndroid.loadType = AudioClipLoadType.DecompressOnLoad;
                settingsIos.loadType = AudioClipLoadType.DecompressOnLoad;
                importer.SetOverrideSampleSettings(BuildTargetGroup.iOS.ToString(), settingsAndroid);
                importer.SetOverrideSampleSettings(BuildTargetGroup.Android.ToString(), settingsIos);
            });
        }
    }
}