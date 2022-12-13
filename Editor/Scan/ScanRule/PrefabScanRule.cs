using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Animations;
using UnityEngine;

namespace Framework.Editor
{
    public class SkinMotionVectorScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Skin_MotionVector";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Prefab";
        public override RulePriority Priority { get; } = RulePriority.Medium;
        public override void Scan()
        {
            InternalScanObject<GameObject>("t:prefab", (go, path) =>
            {
                var skins = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                if(skins.Length <= 0) return;
                foreach (var skin in skins)
                {
                    if (skin.skinnedMotionVectors)
                    {
                        ScanResult.Add(new object[] { path });
                    }
                }
            });
        }
    }

    public class SkinTooManyBonesScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Skin_TooManyBones";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Prefab";
        public override RulePriority Priority { get; } = RulePriority.Medium;

        public override void Scan()
        {
            int limit = Value.ToInt();
            InternalScanObject<GameObject>("t:prefab", (go, path) =>
            {
                var skins = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (skins.Length <= 0) return;
                foreach (var skin in skins)
                {
                    if (skin.bones.Length > limit)
                    {
                        ScanResult.Add(new object[] { path, new KeyValue("骨骼数量", skin.bones.Length )});
                    }
                }
            });
        }
    }
}