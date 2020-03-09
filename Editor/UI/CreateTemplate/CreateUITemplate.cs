using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Framework;
using Framework.UI.Core;
using Plugins.XAsset.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CreateUITemplate
{
    [MenuItem("Assets/@UI Kit - Create UICode")]
	public static void CreateUICode()
	{
		var go = Selection.activeGameObject;
		if(go == null) return;
		CreateCode(go, AssetDatabase.GetAssetPath(go));
		AssetDatabase.Refresh();
	}

	[MenuItem("Assets/@UI Kit - Clear UIMark")]
	public static void ClearUIMark()
	{
		var go = Selection.activeGameObject;
		if(go == null) return;
		var ins = PrefabUtility.InstantiatePrefab(go) as GameObject;
		if(ins == null) return;
		string path = AssetDatabase.GetAssetPath(go);
		var marks = ins.GetComponentsInChildren<UIMark>();
		foreach (var mark in marks)
		{
			Object.DestroyImmediate(mark, true);
		}
		Log.Msg(">> clear uimark success");
		PrefabUtility.ApplyPrefabInstance(ins,InteractionMode.AutomatedAction);
		AssetDatabase.Refresh();
		Object.DestroyImmediate(ins);
	}

	private static void CreateCode(GameObject obj, string uiPrefabPath)
	{
#pragma warning disable 618
		var prefabType = PrefabUtility.GetPrefabType(obj);
		if (PrefabType.Prefab != prefabType)
#pragma warning restore 618
		{
			return;
		}

		var clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;
		if (null == clone)
		{
			return;
		}


		var panelCodeInfo = new PanelCodeInfo();

		FillPanelInfo(clone.transform, uiPrefabPath, panelCodeInfo);

		Generate(panelCodeInfo);
		
		StartAddComponent2PrefabAfterCompile(obj);

		//HotScriptBind(obj);

		Object.DestroyImmediate(clone);
	}

	private static void FillPanelInfo(Transform transform, string prefabPath, PanelCodeInfo panelCodeInfo)
	{
		panelCodeInfo.BehaviourName = transform.name.Replace("(clone)", string.Empty);
		panelCodeInfo.PanelPath = prefabPath;
		var marks = transform.GetComponentsInChildren<UIMark>();
		if(marks.Length <= 0) return;
		var elements = marks.Where(mark => mark._MarkType == UIMark.MarkType.Element);
		var elementsMark = new List<UIMark>();
		foreach (var uiMark in elements)
		{
			elementsMark.Add(uiMark);
			if(uiMark.transform == transform) continue;
			PanelCodeInfo elementPanel = new PanelCodeInfo();
			FillPanelInfo(uiMark.transform, prefabPath, elementPanel);
			elementPanel.IsElement = true;
			panelCodeInfo.Elements.Add(elementPanel);
			foreach (var elementMarks in elementPanel.FieldFullPathToUIMark.Values)
			{
				elementsMark.AddRange(elementMarks);
			}
		}
		foreach (var uiMark in marks)
		{
			if (uiMark.transform == transform && uiMark._MarkType == UIMark.MarkType.Element)
			{
				panelCodeInfo.BehaviourName = uiMark.FieldName;
			}
			if(elementsMark.Contains(uiMark)) continue;
			string fullPath = PathToParent(uiMark.transform, transform);
			if (!panelCodeInfo.FieldFullPathToUIMark.TryGetValue(fullPath, out var uiMarks))
			{
				uiMarks = new List<UIMark>();
				panelCodeInfo.FieldFullPathToUIMark.Add(fullPath, uiMarks);
			}
			uiMarks.Add(uiMark);
		}
	}

	private static void Generate(PanelCodeInfo panelCodeInfo)
	{
		foreach (var element in panelCodeInfo.Elements)
		{
			Generate(element);
		}
		GeneratorView(panelCodeInfo);
		GeneratorVM(panelCodeInfo);
	}

	private static void GeneratorView(PanelCodeInfo panelCodeInfo)
	{
		var dir = BuildScript.GetSettings().uiScriptPath;
		Directory.CreateDirectory(dir);
		var generateFilePath = $"{dir}{panelCodeInfo.BehaviourName}.cs";
		if (File.Exists(generateFilePath)) File.Delete(generateFilePath);
		var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
		var strBuilder = new StringBuilder();
		strBuilder.AppendLine("using UnityEngine;");
		strBuilder.AppendLine("using UnityEngine.UI;");
		strBuilder.AppendLine("using Framework;");
		strBuilder.AppendLine("using Framework.UI.Core;");
		strBuilder.AppendLine();
		strBuilder.AppendFormat("public class {0} : View", panelCodeInfo.BehaviourName);
		strBuilder.AppendLine();
		strBuilder.AppendLine("{");
		strBuilder.AppendLine();
		string vmName = $"{panelCodeInfo.BehaviourName}VM";
		strBuilder.AppendLine($"\tprivate UIBindFactory<{panelCodeInfo.BehaviourName}, {vmName}> binding;");
		strBuilder.AppendLine($"\tprivate {vmName} vm;");
		foreach (var uiMarks in panelCodeInfo.FieldFullPathToUIMark.Values)
		{
			foreach (var uiMark in uiMarks)
			{
				strBuilder.Append(
					$"\t[SerializeField] private {uiMark.CurComponent.GetType().Name} {uiMark.FieldName};\n");
			}
		}
		foreach (var element in panelCodeInfo.Elements)
		{
			strBuilder.Append(
				$"\t[SerializeField] private {element.BehaviourName} {element.BehaviourName};\n");
		}
		strBuilder.AppendLine();
		strBuilder.AppendLine(!panelCodeInfo.IsElement
			? $"\tpublic override UILevel UILevel {{ get; }} = UILevel.Common;"
			: $"\tpublic override UILevel UILevel {{ get; }} = UILevel.None;");
		strBuilder.AppendLine();

		strBuilder.AppendLine("\tprotected override void OnVmChange()");
		strBuilder.AppendLine("\t{");
		strBuilder.AppendLine($"\t\tvm = ViewModel as {vmName};");
		strBuilder.AppendLine("\t\tif (binding == null)");
		strBuilder.AppendLine($"\t\t\tbinding = new UIBindFactory<{panelCodeInfo.BehaviourName}, {vmName}>(this, vm);");
		strBuilder.AppendLine($"\t\tbinding.UpdateVm();");
		strBuilder.AppendLine("\t}");
		
		strBuilder.AppendLine("}");
		sw.Write(strBuilder);
		sw.Flush();
		sw.Close();
	}

	private static void GeneratorVM(PanelCodeInfo panelCodeInfo)
	{
		string className = $"{panelCodeInfo.BehaviourName}VM";
		var generateFilePath = $"{BuildScript.GetSettings().uiScriptPath}{className}.cs";
		if (File.Exists(generateFilePath)) File.Delete(generateFilePath);
		var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
		var strBuilder = new StringBuilder();
		strBuilder.AppendLine("using Framework.UI.Core;");
		strBuilder.AppendLine();
		strBuilder.AppendLine($"public class {className} : ViewModel");
		strBuilder.AppendLine("{");
		strBuilder.AppendLine($"\tpublic {className}()");
		strBuilder.AppendLine("\t{");
		strBuilder.AppendLine();
		strBuilder.AppendLine("\t}");
		strBuilder.AppendLine();
		strBuilder.AppendLine($"\tpublic override string ViewPath {{ get; }} = \"{GetPanelPath(panelCodeInfo)}\";");
		strBuilder.AppendLine();
		strBuilder.AppendLine($"\tpublic static {className} Create(VMCreator vmCreator)");
		strBuilder.AppendLine("\t{");
		strBuilder.AppendLine($"\t\t{className} vm = new {className}();");
		strBuilder.AppendLine("\t\vmCreator?.BindView(vm);");
		strBuilder.AppendLine("\t\treturn vm");
		strBuilder.AppendLine("\t}");
		strBuilder.AppendLine("}");
		sw.Write(strBuilder);
		sw.Flush();
		sw.Close();
	}

	private static void StartAddComponent2PrefabAfterCompile(GameObject uiPrefab)
	{
		var prefabPath = AssetDatabase.GetAssetPath(uiPrefab);
		if (string.IsNullOrEmpty(prefabPath))
			return;

		EditorPrefs.SetString("AutoGenUIPrefabPath", prefabPath);
	}

	[DidReloadScripts]
	private static void DoAddComponent2Prefab()
	{
		var pathStr = EditorPrefs.GetString("AutoGenUIPrefabPath");
		if (string.IsNullOrEmpty(pathStr))
			return;

		EditorPrefs.DeleteKey("AutoGenUIPrefabPath");
		Debug.Log(">>>>>>>SerializeUIPrefab: " + pathStr);

		var uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathStr);
		SetObjectRef2Property(uiPrefab, uiPrefab.name, ReflectionExtension.GetAssemblyCSharp());

		Debug.Log(">>>>>>>Success Serialize UIPrefab: " + uiPrefab.name);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private static void SetObjectRef2Property(GameObject obj, string behaviourName, Assembly assembly)
	{
		var uiMark = obj.GetComponent<UIMark>();
		var className = uiMark != null ? uiMark.FieldName : behaviourName;
		var t = assembly.GetType(className);
		var com = obj.GetComponent(t) ?? obj.AddComponent(t);
		var sObj = new SerializedObject(com);
		var marks = obj.GetComponentsInChildren<UIMark>(true);

		foreach (var mark in marks)
		{
			if(mark == uiMark) continue;
			if (mark._MarkType == UIMark.MarkType.Element)
			{
				SetObjectRef2Property(mark.gameObject, mark.FieldName, assembly);
			}
			
			var uiType = mark.CurComponent;
			var propertyName = mark.FieldName;

			if (sObj.FindProperty(propertyName) == null)
			{
				Log.Msg($"sObj is Null:{propertyName} {uiType} {sObj}");
				continue;
			}

			sObj.FindProperty(propertyName).objectReferenceValue = mark.transform.gameObject;
		}

		sObj.ApplyModifiedPropertiesWithoutUndo();
	}

	private static string GetPanelPath(PanelCodeInfo panelCodeInfo)
	{
		var path = panelCodeInfo.PanelPath;
		var rootPath = BuildScript.GetManifest().resRootPath;
		if (path.Contains(rootPath))
		{
			path = path.RemoveString(rootPath);
		}
		return path;
	}
	
	private static string PathToParent(Transform trans, Transform parent)
	{
		var retValue = new StringBuilder(trans.name);

		while (trans.parent != null)
		{
			if (trans.parent == parent)
			{
				break;
			}

			retValue = trans.parent.name.Append("/").Append(retValue);

			trans = trans.parent;
		}

		return retValue.ToString();
	}
	
	class PanelCodeInfo
	{
		public PanelCodeInfo()
		{
			Elements = new List<PanelCodeInfo>();
			FieldFullPathToUIMark = new Dictionary<string, List<UIMark>>();
			IsElement = false;
		}
		public string BehaviourName;
		public string PanelPath;
		public bool IsElement;
		public List<PanelCodeInfo> Elements;
		public Dictionary<string, List<UIMark>> FieldFullPathToUIMark;
	}
}


