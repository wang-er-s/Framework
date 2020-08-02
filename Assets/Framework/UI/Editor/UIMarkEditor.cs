using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

// Script created by Custom Inspector Generator
[CustomEditor(typeof(UIMark))]
public class UIMarkEditor : Editor 
{
	// The target script in serialized and non-serialized form
	UIMark targetScript;
	SerializedObject serializedTargetScript;

	private string[] popComponents;
		
	void OnEnable() {
		// Get a reference to the target script and serialize it
		targetScript = (UIMark)target;
		popComponents = new string[targetScript.Components.Count];
		for (int i = 0; i < targetScript.Components.Count; i++)
		{
			popComponents[i] = targetScript.Components[i].GetType().Name;
		}
		serializedTargetScript = new SerializedObject(targetScript);
	}
	
    public override void OnInspectorGUI() {
		serializedTargetScript.Update();
		targetScript._MarkType = (UIMark.MarkType)EditorGUILayout.EnumPopup("Enum", targetScript._MarkType);
		targetScript.FieldName = EditorGUILayout.TextField("FieldName",targetScript.FieldName);
		if (targetScript._MarkType == UIMark.MarkType.Component)
		{
			targetScript.SelectedComponent = EditorGUILayout.Popup("Component",targetScript.SelectedComponent, popComponents);
		}
		
		serializedTargetScript.ApplyModifiedProperties();
	}
}



