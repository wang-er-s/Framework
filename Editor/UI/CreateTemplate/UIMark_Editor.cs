using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(UIMark))]
public class UIMark_Editor : Editor
{
    private SerializedProperty _markType;
    private SerializedProperty _fieldName;
    private SerializedProperty _curComponent;

    private void OnEnable()
    {
        _markType = serializedObject.FindProperty("_MarkType");
        _fieldName = serializedObject.FindProperty("FieldName");
        _curComponent = serializedObject.FindProperty("CurComponent");
    }

    public override void OnInspectorGUI()
    {
        //markType.enumValueIndex = EditorGUILayout.EnumPopup()
    }
}
