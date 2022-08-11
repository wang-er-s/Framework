using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ModelProcessor : AssetPostprocessor
{
    private void OnPostprocessModel(GameObject go)
    {
        if (!CommonAssetProcessor.FirstImport(assetImporter)) return;
        ModelImporter importer = assetImporter as ModelImporter;
        FormatModel(importer);
    }

    private static async void DelayDealGameObject(string assetPath, int delayTime)
    {
        await Task.Delay(delayTime);
        ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        var go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (go == null) return;
        // 去除无用骨骼节点，后缀为 Nub 的
        foreach (var child in go.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.name.EndsWith("Nub"))
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
        if (!CommonAssetProcessor.HasExtraUv(assetPath))
        {
            // 去除uv2 color信息
            List<Vector2> emptyUv = null;

            // 不带skin的网格
            MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in mfs)
            {
                meshFilter.sharedMesh.SetUVs(1, emptyUv);
                meshFilter.sharedMesh.SetUVs(2, emptyUv);
                meshFilter.sharedMesh.SetUVs(3, emptyUv);
            }

            // 带skin的网格
            SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var smr in smrs)
            {
                smr.sharedMesh.SetUVs(1, emptyUv);
                smr.sharedMesh.SetUVs(2, emptyUv);
                smr.sharedMesh.SetUVs(3, emptyUv);
            }
        }
        if (!CommonAssetProcessor.HasVertexColor(assetPath))
        {
            // 去除uv2 color信息
            List<Color> emptyColor = null;

            // 不带skin的网格
            MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in mfs)
            {
                meshFilter.sharedMesh.SetColors(emptyColor);
            }

            // 带skin的网格
            SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var smr in smrs)
            {
                smr.sharedMesh.SetColors(emptyColor);
            }
        }
        
        //关闭MotionVector
        SkinnedMeshRenderer[] smrs2 = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var smr in smrs2)
        {
            smr.skinnedMotionVectors = false;
        }
        
        List<AnimationClip> animationClips = CheckHasAnimation(assetPath);
        bool hasAnimation = animationClips.Count > 0;
        if (hasAnimation)
        {
            try
            {
                foreach (var clip in animationClips)
                {
                    if (!CommonAssetProcessor.AnimationHasScale(assetPath))
                    {
                        // 去除scale曲线
                        foreach (var curve in AnimationUtility.GetCurveBindings(clip))
                        {
                            string name = curve.propertyName.ToLower();
                            if (name.Contains("scale"))
                                AnimationUtility.SetEditorCurve(clip, curve, null);
                        }
                    }

                    // 浮点数精度压缩到f3
                    AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(clip);
                    foreach (var curveData in curves)
                    {
                        if (curveData.curve?.keys == null) continue;
                        var keyframes = curveData.curve.keys;
                        for (int j = 0; j < keyframes.Length; j++)
                        {
                            var keyframe = keyframes[j];
                            keyframe.value = float.Parse(keyframe.value.ToString("f3"));
                            keyframe.inTangent = float.Parse(keyframe.inTangent.ToString("f3"));
                            keyframe.outTangent = float.Parse(keyframe.outTangent.ToString("f3"));
                            keyframes[j] = keyframe;
                        }
                        curveData.curve.keys = keyframes;
                        clip.SetCurve(curveData.path, curveData.type, curveData.propertyName, curveData.curve);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"压缩动画失败，path:{assetPath} , error:{e}");
            }
        }

        // -------- Rig ---------
        importer.importAnimation = hasAnimation;
        if (hasAnimation)
        {
            importer.animationType = ModelImporterAnimationType.Generic;
            importer.optimizeGameObjects = true;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.resampleCurves = false;
        }

        // ------- Anim ------
        if (hasAnimation)
        {
            importer.animationCompression = ModelImporterAnimationCompression.Optimal;
            importer.animationRotationError = 0.1f;
            importer.animationPositionError = 0.5f;
            importer.animationScaleError = 1f;
        }
        
        importer.SaveAndReimport();
    }

    public static void FormatModel(ModelImporter importer, int delayTime = 0)
    {
        // --------model--------
        var assetPath = importer.assetPath;
        importer.isReadable = CommonAssetProcessor.ReadWrite(importer.assetPath);

        //OptimizeMesh:顶点优化选项,开启后顶点将被重新排序,GPU性能可以得到提升
        importer.optimizeMeshPolygons = true;
        importer.optimizeMeshVertices = true;
        importer.meshCompression = ModelImporterMeshCompression.Medium;
        importer.importNormals = ModelImporterNormals.None;
        importer.importTangents = ModelImporterTangents.None;
        importer.importCameras = false;
        importer.importLights = false;
        importer.importVisibility = false;
        importer.importBlendShapes = false;
        importer.importAnimation = true;

        // ------- Mat -----
        importer.materialImportMode = ModelImporterMaterialImportMode.None;

        DelayDealGameObject(assetPath, delayTime);
    }

    private static List<AnimationClip> CheckHasAnimation(string path)
    {
        List<AnimationClip> animationClips = new List<AnimationClip>();
        var child = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (var o in child)
        {
            if (o is AnimationClip clip)
                animationClips.Add(clip);
        }
        var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go != null && animationClips.Count <= 0)
        {
            animationClips.AddRange(AnimationUtility.GetAnimationClips(go));
            if (animationClips.Count <= 0)
            {
                var objs = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var o in objs)
                {
                    if (o is AnimationClip clip)
                    {
                        animationClips.Add(clip);
                    }
                }
            }
        }
        if (animationClips.Count <= 0)
        {
            animationClips.AddRange(Object.FindObjectsOfType<AnimationClip>());
        }
        return animationClips;
    }
}