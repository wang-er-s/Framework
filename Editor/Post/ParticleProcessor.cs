using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ParticleProcessor
{
	// https://github.com/sunbrando/ParticleEffectProfiler  特效profiler工具
    static string[] shaderList = new string[] 
    {
        "Mobile/Particles/Additive",
        "Mobile/Particles/Alpha Blended",
        "Mobile/Particles/Multiply",
    };
    
    public static void AutoValidate()
    {
		//写入csv日志
        StreamWriter sw = new StreamWriter("ValidateS03.csv", false, System.Text.Encoding.UTF8);
		sw.WriteLine("Validate -- Session03");

        string[] allAssets = AssetDatabase.GetAllAssetPaths();
		foreach (string s in allAssets)
        {
        	if (s.StartsWith("Assets/Particles/Prefabs"))
        	{
				GameObject go = AssetDatabase.LoadAssetAtPath(s, typeof(GameObject)) as GameObject;
				if (go)
				{
					ParticleSystem[] particles = go.GetComponentsInChildren<ParticleSystem>();
					foreach (ParticleSystem ps in particles)
					{
						//命名是否使用了默认名称“Particle System”
						if (ps.name.Contains("Particle System"))
						{
							sw.WriteLine(string.Format("illegal particle name,{0}", s));
						}

						//粒子系统的材质球，在选择shader的时候，只能在有限的列表中选择
						ParticleSystemRenderer psr = ps.gameObject.GetComponent<ParticleSystemRenderer>();	
						Material[] mats = psr.sharedMaterials;
						foreach (Material mat in mats)
						{
							if (mat && (Array.IndexOf(shaderList, mat.shader.name) < 0))
							{
								sw.WriteLine(string.Format("illegal shader,{0},{1}", s, mat.shader.name));
							}
						}	

						//粒子渲染器序列化信息里有网格
						if (psr.mesh)
						{
							if (psr.renderMode == ParticleSystemRenderMode.Mesh)
							{
								//网格不要超过500面
								if (psr.mesh.vertexCount > 500)
								{
									sw.WriteLine(string.Format("illegal vertex count,{0},{1},{2}", s, AssetDatabase.GetAssetPath(psr.mesh) ,psr.mesh.vertexCount));
								}

								//发射粒子最大数量不能超过5
								if (ps.main.maxParticles > 5)
								{
									sw.WriteLine(string.Format("illegal max particles,{0},{1},{2}", s, ps.name ,ps.main.maxParticles));
								}

								//网格需要打开Read Write Enabled开关
								if (!psr.mesh.isReadable)
								{
									sw.WriteLine(string.Format("illegal mesh ReadWrite,{0},{1}", s, AssetDatabase.GetAssetPath(psr.mesh)));
								}

							}
							else
							{
								//渲染器模式不是Mesh，去掉冗余的网格引用
								psr.mesh = null;
								EditorUtility.SetDirty(go);
							}
						}
					}
				}
        	}

			//粒子使用贴图尺寸不能超过256x256
			if (s.StartsWith("Assets/Particles/Textures"))
        	{
				Texture tex = AssetDatabase.LoadAssetAtPath(s, typeof(Texture)) as Texture;

	            if (tex)
	            {
					if ((tex.width > 256) || (tex.height > 256))
					{
						sw.WriteLine(string.Format("illegal texture size,{0},{1},{2}", s, tex.width, tex.height));
					}
	            }
        	}
		}

		sw.Flush();
		sw.Close();
    }
    
    [MenuItem("Framework/Tool/清理选定材质球里面无用的属性")]
    static void ClearMatProperties()
    {
	    UnityEngine.Object[] objs = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
	    for (int i = 0; i < objs.Length; ++i)
	    {
		    EditorUtility.DisplayProgressBar("处理中", objs[i].name, (float)i / objs.Length);
		    Material mat = objs[i] as Material;

		    if (mat)
		    {
			    SerializedObject psSource = new SerializedObject(mat);
			    SerializedProperty emissionProperty = psSource.FindProperty("m_SavedProperties");
			    SerializedProperty texEnvs = emissionProperty.FindPropertyRelative("m_TexEnvs");

			    if(CleanMaterialSerializedProperty (texEnvs, mat))
			    {
				    Debug.LogError("Find and clean useless texture propreties in " + mat.name);
			    }
			    psSource.ApplyModifiedProperties();
			    EditorUtility.SetDirty(mat);
		    }
	    }

	    AssetDatabase.SaveAssets();
	    EditorUtility.ClearProgressBar();
    }	

    //true: has useless propeties
    private static bool CleanMaterialSerializedProperty(SerializedProperty property, Material mat)
    {
	    bool res = false;

	    for (int j = property.arraySize - 1; j >= 0; j--)
	    {
		    string propertyName = property.GetArrayElementAtIndex(j).FindPropertyRelative("first").stringValue;

		    if (!mat.HasProperty(propertyName))
		    {
			    if(propertyName == "_MainTex")		//_MainTex是自带属性，最好不要删除，否则UITexture等控件在获取mat.maintexture的时候会报错
			    {
				    if (property.GetArrayElementAtIndex (j).FindPropertyRelative ("second").FindPropertyRelative ("m_Texture").objectReferenceValue != null) 
				    {
					    property.GetArrayElementAtIndex (j).FindPropertyRelative ("second").FindPropertyRelative ("m_Texture").objectReferenceValue = null;
					    Debug.Log("Set _MainTex is null");
					    res = true;
				    }
			    }
			    else
			    {
				    property.DeleteArrayElementAtIndex(j);
				    Debug.Log("Delete legacy property in serialized object : " + propertyName);
				    res = true;
			    }
	            
		    }
	    }

	    return res;
    }
}