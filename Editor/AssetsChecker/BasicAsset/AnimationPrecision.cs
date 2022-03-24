using UnityEditor;

namespace Framework.Editor.AssetsChecker
{
    [BelongToCollection(typeof(BasicAssetCheckerCollection))]
    public class AnimationPrecision : AssetRule
    {
        protected override string Description => "动画精度过高";
        protected override RulePriority Priority { get; }

        public AnimationPrecision()
        {
            Table = new RuleDataTable("名字","路径");
        }
        
        public override void Run()
        {

        }

        public override void Check(AssetImporter assetImporter)
        {
            var modelImporter = assetImporter as ModelImporter;
            if (modelImporter == null) return;
            // {
            //     var path = assetImporter.assetPath;
            //     var name = Path.GetFileNameWithoutExtension(path);
            //     var clips = AnimationUtility.GetAnimationClips(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            //     foreach (AnimationClip theAnimation in clips)
            //     {
            //         //浮点数精度压缩到f3
            //         AnimationClipCurveData[] curves = null;
            //         curves = AnimationUtility.GetAllCurves(theAnimation);
            //         Keyframe key;
            //         Keyframe[] keyFrames;
            //         for (int j = 0; j < curves.Length; ++j)
            //         {
            //             AnimationClipCurveData curveDate = curves[j];
            //             if (curveDate.curve == null || curveDate.curve.keys == null)
            //             {
            //                 //Debug.LogWarning(string.Format("AnimationClipCurveData {0} don't have curve; Animation name {1} ", curveDate, animationPath));
            //                 continue;
            //             }
            //             keyFrames = curveDate.curve.keys;
            //             for (int i = 0; i < keyFrames.Length; i++)
            //             {
            //                 key = keyFrames[i];
            //                 key.value = float.Parse(key.value.ToString("f3"));
            //                 key.inTangent = float.Parse(key.inTangent.ToString("f3"));
            //                 key.outTangent = float.Parse(key.outTangent.ToString("f3"));
            //                 keyFrames[i] = key;
            //             }
            //             curveDate.curve.keys = keyFrames;
            //             theAnimation.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
            //         }
            //
            //     }
            //     Table.AddRow(name, path, modelImporter.animationCompression.ToString());
            // }
        }
    }
}