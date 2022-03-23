using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.AssetsChecker
{
    [BelongToCollection(typeof(BasicAssetCheckerCollection))]
    public class ReadWriteMesh : AssetRule
    {
        public ReadWriteMesh()
        {
            Table = new RuleDataTable("名字","地址","顶点数","面片数");
        }
        
        public override void Check(AssetImporter assetImporter)
        {
            var modelImporter = assetImporter as ModelImporter;
            if(modelImporter == null) return;
            // if (modelImporter.isReadable)
            {
                var path = modelImporter.assetPath;
                var name = Path.GetFileNameWithoutExtension(path);
                var objs = AssetDatabase.LoadAllAssetsAtPath(path);
                int meshVerticesCount = 0; 
                int meshTrisCount = 0;
                foreach (Object o in objs)
                {
                    GameObject go = o as GameObject;
                    if(go == null) continue;
                    var meshFilter = go.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        var mesh = meshFilter.mesh;
                        meshVerticesCount = mesh.vertexCount;
                        meshTrisCount = mesh.triangles.Length;
                        break;
                    }

                    var skinMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();
                    if (skinMeshRenderer != null)
                    {
                        var mesh = skinMeshRenderer.sharedMesh;
                        meshVerticesCount = mesh.vertexCount;
                        meshTrisCount = mesh.triangles.Length;
                        break;
                    }
                }
                //MeshUtility.
                Table.AddRow(name, path, meshVerticesCount, meshTrisCount);
            }
        }

        protected override string Description => "开启Read/Write选项的网格";
        protected override RulePriority Priority => RulePriority.High;
        public override void Run()
        {
        }
    }
}