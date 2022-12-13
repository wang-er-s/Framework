using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class MeshHasUv2ScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Mesh_Uv2";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Mesh";
        public override RulePriority Priority { get; } = RulePriority.High;
        public override void Scan()
        {
            InternalScanAllObj<Mesh>("t:mesh",(mesh, path) =>
            {
                if (mesh.uv2.Length > 0)
                    ScanResult.Add(new object[] { path, new KeyValue("mesh名字", mesh.name) });
            });
        }
    }

    public class MeshHasUv34ScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Mesh_Uv34";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Mesh";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanAllObj<Mesh>("t:mesh",(mesh, path) =>
            {
                if (mesh.uv3.Length > 0 || mesh.uv4.Length > 0)
                    ScanResult.Add(new object[] { path,new KeyValue("mesh名字", mesh.name), new KeyValue("额外uv数量",$"uv3:{mesh.uv3.Length}|uv4:{mesh.uv4.Length}" )});
            });
        }
    }
    
    public class MeshHasNormalScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Mesh_Normal";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Mesh";
        public override RulePriority Priority { get; } = RulePriority.Medium;

        public override void Scan()
        {
            InternalScanAllObj<Mesh>("t:mesh",(mesh, path) =>
            {
                if (mesh.normals.Length > 0)
                    ScanResult.Add(new object[] { path,new KeyValue("mesh名字", mesh.name) });
            });
        }
    }
    
    public class MeshHasTangentScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Mesh_Tangent";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Mesh";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanAllObj<Mesh>("t:mesh",(mesh, path) =>
            {
                if (mesh.tangents.Length > 0)
                    ScanResult.Add(new object[] { path,new KeyValue("mesh名字", mesh.name )});
            });
        }
    }

    public class MeshHasColorScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Mesh_Color";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Mesh";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanAllObj<Mesh>("t:mesh",(mesh, path) =>
            {
                if (mesh.colors.Length > 0)
                    ScanResult.Add(new object[] { path, new KeyValue("mesh名字", mesh.name ) });
            });
        }
    }

    public class MeshTriangleLimitScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Mesh_TriangleLimit";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Mesh";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            var limit = Value.ToInt();
            InternalScanAllObj<Mesh>("t:mesh",(mesh, path) =>
            {
                if (mesh.triangles.Length / 3 > limit)
                    ScanResult.Add(new object[] { path, new KeyValue("mesh名字", mesh.name ), new KeyValue("三角面数量", mesh.triangles.Length / 3 )});
            });
        }
    }
    
    
    public class MeshHasSameScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Mesh_SameMesh";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Mesh";
        public override RulePriority Priority { get; } = RulePriority.High;

        private class MeshCache
        {
            public Vector3[] VertexPos = new Vector3[10];
            public int[] Triangles = new int[10];
            public int VertexCount;
            public int TrianglesCount;

            public List<string> MeshPath = new();

            public bool TryAdd(Mesh mesh, string path)
            {
                var vertices = mesh.vertices;
                var triangles = mesh.triangles;
                if (mesh.vertices.Length != VertexCount || mesh.triangles.Length != TrianglesCount) return false;
                for (int i = 0; i < 10; i++)
                {
                    if (!vertices[i].NearEqual(VertexPos[i]))
                    {
                        return false;
                    }

                    if (triangles[i] != Triangles[i])
                    {
                        return false;
                    }
                }

                MeshPath.Add(path);
                return true;
            }

            public static MeshCache Create(Mesh mesh, string path)
            {
                MeshCache result = new MeshCache();
                var vertices = mesh.vertices;
                var triangles = mesh.triangles;
                for (int i = 0; i < 10; i++)
                {
                    result.VertexPos[i] = vertices[i];
                    result.Triangles[i] = triangles[i];
                }

                result.VertexCount = vertices.Length;
                result.TrianglesCount = triangles.Length;
                result.MeshPath.Add(path);
                return result;
            }
        }
        
        public override void Scan()
        {
            List<MeshCache> meshCaches = new();
            InternalScanAllObj<Mesh>("t:mesh",(mesh, path) =>
            {
                string meshPath = $"{path}/{mesh.name}";
                if (mesh.vertices.Length > 10 && mesh.triangles.Length > 10)
                {
                    foreach (var meshCache in meshCaches)
                    {
                        if(meshCache.TryAdd(mesh, meshPath)) return;
                    }
                    meshCaches.Add(MeshCache.Create(mesh, meshPath));
                }
            });

            foreach (var meshCache in meshCaches)
            {
                if (meshCache.MeshPath.Count > 1)
                {
                    ScanResult.Add(new object[] { string.Join("|", meshCache.MeshPath) });
                }
            }
            
            LogResult();
        }
    }
    
    public class MeshRWScanRule : ScanRuleWithDir
    {
        public override string RuleId { get; } = "Mesh_RW";
        public override string Menu { get; } = $"{nameof(BasicResCheckMenu)}/Mesh";
        public override RulePriority Priority { get; } = RulePriority.High;

        public override void Scan()
        {
            InternalScanImporter<ModelImporter>("t:model", (importer) =>
            {
                if (importer.isReadable)
                    ScanResult.Add(new object[] { importer.assetPath });
            });
        }

        public override void Fix()
        {
            InternalFixImporter<ModelImporter>((importer, _) =>
            {
                importer.isReadable = false;
            });
        }
    }
}