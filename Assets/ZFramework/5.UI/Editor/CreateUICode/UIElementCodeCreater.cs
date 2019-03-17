using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZFramework;
using Object = UnityEngine.Object;

public class UIElementData
{
	public string FilePath = Application.dataPath+"/Scripts/UI/Element/";
	public string BehaviourName;
	public List<string> btnNameLists = new List<string> ();
	public List<MarkedObjInfo> markNameLists = new List<MarkedObjInfo> ();
}

public class UIElementCodeCreater
{

	[MenuItem ( "Assets/@UI Kit - Create UIElementCode" )]
	public static void CreateElementCode ()
	{
		var objs =
			Selection.GetFiltered ( typeof ( GameObject ), SelectionMode.Assets | SelectionMode.TopLevel );
		var displayProgress = objs.Length > 1;
		if ( displayProgress ) EditorUtility.DisplayProgressBar ( "", "Create UIPrefab Code...", 0 );
		for ( var i = 0; i < objs.Length; i++ )
		{
			mInstance.CreateCode ( objs[ i ] as GameObject );
			if ( displayProgress )
				EditorUtility.DisplayProgressBar ( "", "Create UIPrefab Code...", (float) ( i + 1 ) / objs.Length );
		}

		AssetDatabase.Refresh ();
		if ( displayProgress ) EditorUtility.ClearProgressBar ();
	}


	private void CreateCode ( GameObject obj )
	{
		UIElementData elementData = new UIElementData();
		elementData.BehaviourName = obj.name;
		
		FindAllUIMark (elementData, obj.transform );
		elementData.markNameLists.Sort ( ( m1, m2 ) =>
			                                 string.Compare ( m1.MarkObj.ComponentName, m1.MarkObj.ComponentName, StringComparison.Ordinal ) );
		CreateComponentCode ( elementData );
		CreateElementCode(elementData);
		AssetDatabase.Refresh ();
		AddSerializeUIPrefab ( obj );
	}

	private void CreateComponentCode ( UIElementData elementData )
	{
		if ( !Directory.Exists ( elementData.FilePath ) ) Directory.CreateDirectory ( elementData.FilePath );
		UIElementCodeComponentTemplate.mGenerate ( elementData.FilePath + elementData.BehaviourName + "Component.cs",
		                                           elementData );
	}

	private void CreateElementCode ( UIElementData elementData )
	{
		if ( !File.Exists (elementData.FilePath + elementData.BehaviourName + ".cs") )
		{
			UIElementCodeTemplate.mGenerate ( elementData.FilePath + elementData.BehaviourName + ".cs", elementData );
		}
	}

	private void FindAllUIMark (UIElementData elementData, Transform tra )
	{
		foreach ( Transform transform in tra )
		{
			if ( transform.GetComponent<UIMark> () )
			{
				if ( transform.GetComponent<UIMark> ().ComponentName == "Button" )
				{
					elementData.btnNameLists.Add ( tra.name );
				}
				elementData.markNameLists.Add ( new MarkedObjInfo ()
					                                { MarkObj = transform.GetComponent<UIMark> (), Name = transform.name } );
			}
			if ( tra.childCount > 0 )
				FindAllUIMark (elementData, transform );
		}
	}


	private const string AutoGenUIElementPath = "AutoGenUIElementPath";
		
	private static void AddSerializeUIPrefab (GameObject obj)
	{
		string prefabPath = AssetDatabase.GetAssetPath ( obj );
		if(prefabPath == null) return;
		string pathStr = EditorPrefs.GetString ( AutoGenUIElementPath );
		if ( string.IsNullOrEmpty ( pathStr ) )
		{
			pathStr = prefabPath;
		}
		else
		{
			pathStr += ( ";" + prefabPath );
		}
		EditorPrefs.SetString(AutoGenUIElementPath,pathStr);
	}

	[UnityEditor.Callbacks.DidReloadScripts]
	private static void SerializeObj ()
	{
		var pathStr = EditorPrefs.GetString ( AutoGenUIElementPath);
		EditorPrefs.DeleteKey ( AutoGenUIElementPath);
		if ( string.IsNullOrEmpty ( pathStr ) )
			return;
		string[] paths = pathStr.Split ( new[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
		foreach ( string path in paths )
		{
			GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject> ( path );
			if(obj != null) mInstance.AttachSerializeObj(obj);
		}
		Debug.Log ( ">>>>>>>Serialize UI Element: " + pathStr );
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		
		foreach ( string path in paths )
		{
			GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject> ( path );
			if(obj != null) mInstance.AttachSerializeObj(obj);
		}
		Debug.Log ( ">>>>>>>Serialize UI Element: " + pathStr );
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}

	void AttachSerializeObj ( GameObject obj )
	{
		var  assembly = ReflectionExtension.GetAssemblyCSharp ();
		Type type     = assembly.GetType ( obj.name );
		var com     = obj.GetComponent ( type ) ?? obj.AddComponent ( type );
		var sObj    = new SerializedObject ( com );
		var uiMarks = obj.GetComponentsInChildren<UIMark>();
		foreach ( UIMark mark in uiMarks )
		{
			string uiType       = mark.ComponentName;
			string propertyName = mark.Transform.gameObject.name;
			if ( sObj.FindProperty ( propertyName ) == null )
			{
				Debug.Log ( $"sObj is Null:{propertyName} {uiType} {sObj}" );
				continue;
			}

			sObj.FindProperty ( propertyName ).objectReferenceValue = mark.Transform.gameObject;
		}
		sObj.ApplyModifiedPropertiesWithoutUndo ();
	}
	
	private static readonly UIElementCodeCreater mInstance = new UIElementCodeCreater ();

}
