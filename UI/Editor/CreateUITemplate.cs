using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework.UI.Core;
using Plugins.XAsset.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class CreateUITemplate
{
    [MenuItem("Assets/@UI Kit - Create UICode (alt+c) &c")]
	public static void CreateUICode()
	{
		var go = Selection.activeGameObject;
		if(go == null) return;
		CreateCode(go, AssetDatabase.GetAssetPath(go));
		AssetDatabase.Refresh();
	}

	private static void CreateCode(GameObject obj, string uiPrefabPath)
	{
		var prefabType = PrefabUtility.GetPrefabType(obj);
		if (PrefabType.Prefab != prefabType)
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
		
		/*UISerializer.StartAddComponent2PrefabAfterCompile(obj);

		HotScriptBind(obj);*/

		Object.DestroyImmediate(clone);
	}

	private static void FillPanelInfo(Transform transform, string prefabPath, PanelCodeInfo panelCodeInfo)
	{
		panelCodeInfo.BehaviourName = transform.name.Replace("(clone)", string.Empty);
		panelCodeInfo.PanelPath = prefabPath;
		var marks = transform.GetComponentsInChildren<UIMark>();
		foreach (var uiMark in marks)
		{
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
		var generateFilePath = $"{BuildScript.GetSettings().uiScriptPath}{panelCodeInfo.BehaviourName}.cs";
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
					$"\t[SerializeField] public {uiMark.CurComponent.GetType().Name} {uiMark.FieldName};");
			}
		}
		strBuilder.AppendLine();
		strBuilder.AppendLine();

		strBuilder.AppendLine("\tprotected override void OnVmChange()");
		strBuilder.AppendLine("\t{");
		strBuilder.AppendLine($"\t\tvm = ViewModel as {vmName};");
		strBuilder.AppendLine("\t\tif (binding == null)");
		strBuilder.AppendLine($"\t\t\tbinding = new UIBindFactory<{panelCodeInfo.BehaviourName}, {vmName}>(this, vm);");
		strBuilder.AppendLine("\t}");
		strBuilder.AppendLine();

		strBuilder.AppendLine($"\tpublic static string Path = \"{GetPanelPath(panelCodeInfo)}\";");
		strBuilder.AppendLine("}");
		sw.Write(strBuilder);
		sw.Flush();
		sw.Close();

	}

	private static string GetPanelPath(PanelCodeInfo panelCodeInfo)
	{
		var path = panelCodeInfo.PanelPath;
		var rootPath = BuildScript.GetSettings().assetRootPath;
		if (path.Contains(rootPath))
		{
			path = path.RemoveString(rootPath);
		}
		return path;
	}
	
	public static string PathToParent(Transform trans, Transform parent)
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
		public string BehaviourName;
		public string PanelPath;
		public Dictionary<string, List<UIMark>> FieldFullPathToUIMark = new Dictionary<string, List<UIMark>>();
	}
}


