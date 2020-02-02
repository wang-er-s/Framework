using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(UIMark))]
public class UIMark_Editor : Editor
{
    private SerializedProperty markType;
    private SerializedProperty fieldName;
    private SerializedProperty curComponent;

    private void OnEnable()
    {
        markType = serializedObject.FindProperty("_MarkType");
        fieldName = serializedObject.FindProperty("FieldName");
        curComponent = serializedObject.FindProperty("CurComponent");
    }

    public override void OnInspectorGUI()
    {
        //markType.enumValueIndex = EditorGUILayout.EnumPopup()
    }
}
