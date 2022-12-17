using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    public class AnimationResampleCurveScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Animation_ResampleCurve";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Animation";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanImporter<ModelImporter>("t:animation", (importer) =>
            {
                if (importer.resampleCurves)
                    ScanResult.Add(new object[] { importer.assetPath });
            });
        }

        public override void Fix(Func<string,bool> filter = null)
        {
            InternalFixImporter<ModelImporter>((importer, _) =>
            {
                importer.resampleCurves = false;
            }, filter);
        }
    }
    
    public class AnimationCompressionScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Animation_Compression";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Animation";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanImporter<ModelImporter>("t:animation", (importer) =>
            {
                if (importer.animationCompression != ModelImporterAnimationCompression.Optimal)
                    ScanResult.Add(new object[] { importer.assetPath });
            });
        }

        public override void Fix(Func<string,bool> filter = null)
        {
            InternalFixImporter<ModelImporter>((importer, _) =>
            {
                importer.animationCompression = ModelImporterAnimationCompression.Optimal;
            }, filter);
        }
    }
    
    public class AnimationHasScaleScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Animation_HasScale";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Animation";
        public override RulePriority Priority { get; } = RulePriority.Medium;
        public override void Scan()
        {
            InternalScanAllObj<AnimationClip>("t:animation", (clip, path) =>
            {
                foreach (var curve in AnimationUtility.GetCurveBindings(clip))
                {
                    string name = curve.propertyName.ToLower();
                    if (name.Contains("scale"))
                    {
                        ScanResult.Add(new object[] { path, new KeyValue("clip名字",clip.name) });
                        return;
                    }
                }
            });
        }
    }
    
    public class AnimationStateLimitScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Animation_StateLimit";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Animation";
        public override RulePriority Priority { get; } = RulePriority.Medium;
        public override void Scan()
        {
            var animator = new GameObject("tmp").AddComponent<Animator>();
            int limit = Value.ToInt();
            InternalScanAllObj<AnimatorController>("t:animatorcontroller", (controller, path) =>
            {
                animator.runtimeAnimatorController = controller;
                if (animator.runtimeAnimatorController.animationClips.Length > limit)
                {
                    ScanResult.Add(new object[]{path, new KeyValue("AnimationState数量", animator.runtimeAnimatorController.animationClips.Length)});
                }
            });
            Object.DestroyImmediate(animator.gameObject);
        }
    }
    
    public class AnimationOptimizeTransformScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Animation_OptimizeTransform";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Animation";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanImporter<ModelImporter>("t:animation", (importer) =>
            {
                if (importer.animationType == ModelImporterAnimationType.Human ||
                    importer.animationType == ModelImporterAnimationType.Generic)
                {
                    if (importer.avatarSetup != ModelImporterAvatarSetup.NoAvatar && !importer.optimizeGameObjects)
                    {
                        ScanResult.Add(new object[] { importer.assetPath });
                    }
                }
            });
        }

        public override void Fix(Func<string,bool> filter = null)
        {
            InternalFixImporter<ModelImporter>((importer, _) =>
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.optimizeGameObjects = true;
            }, filter);
        }
    }
}