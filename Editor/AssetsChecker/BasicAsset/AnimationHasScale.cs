using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.AssetsChecker
{
    [BelongToCollection(typeof(BasicAssetCheckerCollection))]
    public class AnimationHasScale : AssetRule
    {
        public AnimationHasScale()
        {
            Table = new RuleDataTable("名字","路径");
        }
        
        public override void Check(AssetImporter assetImporter)
        {
            var modelImporter = assetImporter as ModelImporter;
            if(modelImporter == null) return;
            {
                var path = assetImporter.assetPath;
                var name = Path.GetFileNameWithoutExtension(path);
                var objs = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var o in objs)
                {
                    if (!(o is AnimationClip clip)) continue;
                    foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(clip))
                    {
                        string propertyName = theCurveBinding.propertyName.ToLower();
                        if (propertyName.Contains("scale"))
                        {
                            Table.AddRow($"{name}/{clip.name}", path);
                        }
                    }
                }
            }
        }

        protected override string Description => "动画有scale曲线";
        protected override RulePriority Priority { get; }
        public override void Run()
        {
        }

        public override void TryFix()
        {
            // for (int i = 0; i < Table.DataColumnCount; i++)
            // {
            //     using (var datas = Table.GetDataByRow(i))
            //     {
            //         var go = AssetDatabase.LoadAssetAtPath<GameObject>(datas[1]);
            //         var clips = AnimationUtility.GetAnimationClips(go);
            //         foreach (AnimationClip theAnimation in clips)
            //         {
            //             //去除scale曲线
            //             foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(theAnimation))
            //             {
            //                 string propertyName = theCurveBinding.propertyName.ToLower();
            //                 if (propertyName.Contains("scale"))
            //                 {
            //                     AnimationUtility.SetEditorCurve(theAnimation, theCurveBinding, null);
            //                 }
            //             }
            //         }
            //     }
            // }
        }
    }
}