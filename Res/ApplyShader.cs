using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.Reflection;

#endif

namespace Framework
{
    public class ApplyShader : MonoBehaviour
    {
        public static bool checkLocalFirst = false;
        // Use this for initialization
        public void Start()
        {
            ResetShader(gameObject);
        }

        public static ApplyShader CheckShader(Object o)
        {
            /*
            GameObject go = o as GameObject;
            if (null != go)
            {
                ApplyShader comp = go.GetComponent<ApplyShader>();
                if (comp == null)
                    comp = go.AddComponent<ApplyShader>();
                return comp;
            }
            */
            return null;
        }

        public static void CheckShader(Material m)
        {
            if (null != m && null != m.shader)
            {
                m.shader = FindShader(m.shader.name);
            }
        }

        private static Shader FindShader(string shaderName)
        {
            Shader shader = FindShader(shaderName,checkLocalFirst);
            if (null == shader)
                shader = FindShader(shaderName, !checkLocalFirst);
            return shader;
        }

        private static Shader FindShader(string shaderName, bool checkLocal)
        {
            if (checkLocal)
                return Shader.Find(shaderName);
            return ShaderLib.Ins == null ? null : ShaderLib.Ins.GetShader(shaderName);
        }

        private void ResetShader(GameObject go)
        {
            List<Renderer> renders = go.GetComponentsInChildren<Renderer>().ToList();
            Dictionary<Material, string> materialShaders = new Dictionary<Material, string>();
            for (int i = 0; i < renders.Count; ++i)
            {
                Material[] materials = renders[i].sharedMaterials;
                if (materials == null)
                    continue;
                for (int j = 0; j < materials.Length; ++j)
                {
                    Material m = materials[j];
                    if (null != m && !materialShaders.ContainsKey(m))
                        materialShaders.Add(m, m.shader.name);
                }
            }

            Terrain terrain = go.GetComponentInChildren<Terrain>();
            if (null != terrain)
            {
                Material m = terrain.materialTemplate;
                if (null != m && !materialShaders.ContainsKey(m))
                    materialShaders.Add(m, m.shader.name);
            }
            /*
            created by wangliang on 2019/14/15 11:14:56
            对于textmesh pro的submesh情况不适用
            List<Graphic> graphics = null;
            Utils.GetComponent(go, ref graphics);
            for (int i = 0; i < graphics.Count; ++i)
            {
                Material m = graphics[i]?.material;
                if (null != m && !materialShaders.ContainsKey(m))
                    materialShaders.Add(m, m.shader.name);
            }
            */

            foreach (KeyValuePair<Material, string> element in materialShaders)
            {
#if UNITY_EDITOR
                int rawRQ = GetMaterialRawRQFromShader(element.Key);
#endif
                Shader shader = FindShader(element.Value);
                if (null != shader)
                    element.Key.shader = shader;
#if UNITY_EDITOR
                if (rawRQ != -1)
                    element.Key.renderQueue = rawRQ;
#endif
            }
        }
        [ContextMenu("ResetShader")]
        private void ResetShader()
        {
            ResetShader(gameObject);
        }

#if UNITY_EDITOR
        private static MethodInfo getMaterialRawRQMethod;

        private static MethodInfo getGetMaterailRawRQMethod()
        {
            if (null == getMaterialRawRQMethod)
            {
                Assembly asm = Assembly.GetAssembly(typeof(UnityEditor.Editor));
                Type type = asm.GetType("UnityEditor.ShaderUtil");
                getMaterialRawRQMethod = type.GetMethod("GetMaterialRawRenderQueue",
                    BindingFlags.Static | BindingFlags.NonPublic);
            }

            return getMaterialRawRQMethod;
        }

        private static int GetMaterialRawRQFromShader(Material m)
        {
            MethodInfo methodInfo = getGetMaterailRawRQMethod();
            if (null != methodInfo)
            {
                System.Object result = methodInfo.Invoke(null, new[] {m});
                return (int) result;
            }

            return -1;
        }
#endif
    }
}