using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework.UI.Core;
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

		Debug.Log(clone.name);
		panelCodeInfo.BehaviourName = clone.name.Replace("(clone)", string.Empty);
		FillPanelInfo(clone.transform, panelCodeInfo);
		CreateUIPanelCode(obj, uiPrefabPath, panelCodeInfo);

		UISerializer.StartAddComponent2PrefabAfterCompile(obj);

		HotScriptBind(obj);

		Object.DestroyImmediate(clone);
	}

	private static void FillPanelInfo(Transform transform, PanelCodeInfo panelCodeInfo)
	{
		var marks = transform.GetComponentsInChildren<UIMark>();
		foreach (var uiMark in marks)
		{
			
		}
	}

	private void CreateUIPanelCode(GameObject uiPrefab, string uiPrefabPath, PanelCodeInfo panelCodeInfo)
	{
		
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
	
	private static void Generate(string generateFilePath, string behaviourName,PanelCodeInfo panelCodeInfo)
	{
		var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
		var strBuilder = new StringBuilder();

		strBuilder.AppendLine("/****************************************************************************");
		strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month, SystemInfo.deviceName);
		strBuilder.AppendLine(" ***************************************************************************#1#");
		strBuilder.AppendLine();
		strBuilder.AppendLine("using UnityEngine;");
		strBuilder.AppendLine("using UnityEngine.UI;");
		strBuilder.AppendLine("using QFramework;");
		strBuilder.AppendLine();
		strBuilder.AppendFormat("\tpublic partial class {0}", behaviourName);
		strBuilder.AppendLine();
		strBuilder.AppendLine("\t{");

		foreach (var markInfo in elementCodeInfo.BindInfos)
		{
			var strUIType = markInfo.BindScript.ComponentName;
			strBuilder.AppendFormat("\t\t[SerializeField] public {0} {1};\r\n",
				strUIType, markInfo.Name);
		}

		strBuilder.AppendLine();

		strBuilder.Append("\t\t").AppendLine("public void Clear()");
		strBuilder.Append("\t\t").AppendLine("{");
		foreach (var markInfo in elementCodeInfo.BindInfos)
		{
			strBuilder.AppendFormat("\t\t\t{0} = null;\r\n", markInfo.Name);
		}

		strBuilder.Append("\t\t").AppendLine("}");
		strBuilder.AppendLine();

		strBuilder.Append("\t\t").AppendLine("public override string ComponentName");
		strBuilder.Append("\t\t").AppendLine("{");
		strBuilder.Append("\t\t\t");
		strBuilder.AppendLine("get { return \"" + elementCodeInfo.BindInfo.BindScript.ComponentName + "\";}");
		strBuilder.Append("\t\t").AppendLine("}");
		strBuilder.AppendLine("\t}");
		sw.Write(strBuilder);
		sw.Flush();
		sw.Close();

	}
	
	class PanelCodeInfo
	{
		public string BehaviourName;
		public Dictionary<string, UIMark> FieldFullNameToUIMark = new Dictionary<string, UIMark>();
	}
}


