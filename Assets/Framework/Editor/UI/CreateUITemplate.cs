using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Framework.UI.Core;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Framework.UI.Editor
{
	public static class CreateUITemplate
	{
		private static string _templatePath;
		// Editor/UI
		private static string templatePath
		{
			get
			{
				if (string.IsNullOrEmpty(_templatePath))
				{
					string[] res = Directory.GetFiles(Application.dataPath, "ViewTemplate.txt", SearchOption.AllDirectories);
					Log.Assert(res.Length == 1,"没有找到ViewTemplate.txt或存在多个");
					_templatePath = Path.GetDirectoryName(res[0]);
				}
				return _templatePath;
			}
		}
		
		[MenuItem("Assets/@UI Kit - Create UICode")]
		public static void CreateUiCode()
		{
			var go = Selection.activeGameObject;
			if(go == null) return;
			CreateCode(go, AssetDatabase.GetAssetPath(go));
			AssetDatabase.Refresh();
		}

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
			var prefabType = PrefabUtility.GetPrefabAssetType(obj);
			if (prefabType == PrefabAssetType.NotAPrefab)
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

			Object.DestroyImmediate(clone);
		}

		private static void FillPanelInfo(Transform transform, string prefabPath, PanelCodeInfo panelCodeInfo)
		{
			panelCodeInfo.BehaviourName = transform.name.Replace("(clone)", string.Empty);
			panelCodeInfo.PanelPath = prefabPath;
			List<_uiMark> marks = new List<_uiMark>();
			CollectMark(transform, ref marks);
			if (marks.Count <= 0) return;
			foreach (var uiMark in marks)
			{
				string fullPath = PathToParent(uiMark.transform, transform);
				if (!panelCodeInfo.FieldFullPathToUIMark.TryGetValue(fullPath, out var uiMarks))
				{
					uiMarks = new List<_uiMark>();
					panelCodeInfo.FieldFullPathToUIMark.Add(fullPath, uiMarks);
				}
				uiMarks.Add(uiMark);
			}
		}

		private static void CollectMark(Transform trans, ref List<_uiMark> uiMarks)
		{
			UIMark uiMark = trans.GetComponent<UIMark>();
			if (uiMark != null && uiMark.IgnoreChild && uiMark.IgnoreSelf)
			{
				return;
			}

			if (uiMark == null || (uiMark != null && !uiMark.IgnoreChild))
			{
				for (int i = 0; i < trans.childCount; i++)
				{
					CollectMark(trans.GetChild(i), ref uiMarks);
				}
			}

			if (uiMark != null && uiMark.IgnoreSelf)
			{
				return;
			}

			string fieldName = trans.name;
			if (uiMark != null)
			{
				var _mark = new _uiMark(uiMark);
				uiMarks.Add(_mark);
			}
			else
			{
				foreach (var component in autoAddComponents)
				{
					var com = trans.GetComponent(component);
					if (com != null)
					{
						var _mark = new _uiMark(fieldName, com, trans);
						uiMarks.Add(_mark);
						break;
					}
				}
			}
		}

		private static void Generate(PanelCodeInfo panelCodeInfo)
		{
			GeneratorView(panelCodeInfo);
			GeneratorVM(panelCodeInfo);
		}

		private static void GeneratorView(PanelCodeInfo panelCodeInfo)
		{
			Directory.CreateDirectory(UIEnv.UIPanelScriptFolder);
			var generateFilePath = Path.Combine(UIEnv.UIPanelScriptFolder, $"{panelCodeInfo.BehaviourName}.cs");
			var strBuilder = new StringBuilder();
			var template = File.ReadAllText(File.Exists(generateFilePath)
				? generateFilePath
				: $"{templatePath}/ViewTemplate.txt");
			string vmName = $"{panelCodeInfo.BehaviourName}VM";
			template = template.Replace("#ClassName", panelCodeInfo.BehaviourName);
			template = template.Replace("#VMName", vmName);
			template = template.Replace("#PrefabPath", GetPanelPath(panelCodeInfo));
			foreach (var uiMarks in panelCodeInfo.FieldFullPathToUIMark.Values)
			{
				foreach (var uiMark in uiMarks)
				{
					strBuilder.AppendLine(
						$"\t[SerializeField] private {uiMark.component.GetType().Name} {uiMark.fieldName};");
				}
			}
			var markStr = strBuilder.ToString();
			if (markStr.EndsWith("\n"))
			{
				markStr = markStr.Substring(0, markStr.Length - 1);
			}
			template = Regex.Replace(template, @"(#region Components\r*\n*)([\s\S]*?)(\s*?#endregion)",
				$"$1{markStr}$3");
			File.WriteAllText(generateFilePath, template);
		}

		private static void GeneratorVM(PanelCodeInfo panelCodeInfo)
		{
			string className = $"{panelCodeInfo.BehaviourName}VM";
			//var generateFilePath = $"{BuildScript.GetSettings().uiScriptPath}{className}.cs";
			var generateFilePath = Path.Combine(UIEnv.UIPanelScriptFolder, $"{className}.cs");
			if (File.Exists(generateFilePath)) return;
			var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
			var template = File.ReadAllText($"{templatePath}/VMTemplate.txt");
			template = template.Replace("#ClassName", className);
			sw.Write(template);
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
			var t = assembly.GetType(behaviourName);
			var com = obj.GetComponent(t) ?? obj.AddComponent(t);
			var sObj = new SerializedObject(com);
			var marks = new List<_uiMark>();
			CollectMark(obj.transform, ref marks);

			foreach (var mark in marks)
			{
				var uiType = mark.component;
				var propertyName = mark.fieldName;

				if (sObj.FindProperty(propertyName) == null)
				{
					Log.Error($"sObj is Null:{propertyName} {uiType} {sObj}");
					continue;
				}

				sObj.FindProperty(propertyName).objectReferenceValue = mark.transform.gameObject;
			}

			sObj.ApplyModifiedPropertiesWithoutUndo();
		}

		private static string GetPanelPath(PanelCodeInfo panelCodeInfo)
		{
			var path = panelCodeInfo.PanelPath;
			//var rootPath = BuildScript.GetManifest().resRootPath;
			if (path.Contains(UIEnv.UIPrefabRootPath))
			{
				path = path.RemoveString(UIEnv.UIPrefabRootPath);
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
				FieldFullPathToUIMark = new Dictionary<string, List<_uiMark>>();
			}
			public string BehaviourName;
			public string PanelPath;
			public Dictionary<string, List<_uiMark>> FieldFullPathToUIMark;
		}
		
		private static List<Type> autoAddComponents = new List<Type>()
		{
			typeof(Button),
			typeof(InputField),
			typeof(Toggle),
			typeof(Dropdown),
			typeof(Slider),
		};

		class _uiMark
		{
			public string fieldName;
			public Component component;
			public Transform transform;
			public _uiMark(string fieldName, Component component, Transform transform)
			{
				this.fieldName = fieldName;
				this.component = component;
				this.transform = transform;
			}

			public _uiMark(UIMark uiMark)
			{
				fieldName = uiMark.FieldName;
				component = uiMark.CurComponent;
				transform = uiMark.transform;
			}
		}
	}
}
