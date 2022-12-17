using UnityEngine;
using UnityEngine.Video;

namespace Framework.Editor
{
    public class VideoSizeLimitScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Video_SizeLimit";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}";
        public override RulePriority Priority { get; } = RulePriority.Medium;
        public override void Scan()
        {
            int limit = Value.ToInt();
            InternalScanObject<VideoClip>("t:VideoClip", (clip, path) =>
            {
                if (clip.width > limit && clip.height > limit)
                {
                    ScanResult.Add(new object[] { path, new KeyValue("视频的宽高", new Vector2(clip.width, clip.height)) });
                }
            });
        }
    }
}