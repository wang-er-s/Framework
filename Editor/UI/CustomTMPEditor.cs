using System.Reflection;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [CustomEditor(typeof(CustomTextMeshPro), true), CanEditMultipleObjects]
    public class CustomTMPEditor : TMP_EditorPanelUI
    {
        private SerializedProperty customOutlineColor;
        private SerializedProperty customOutlineWidth;
        private SerializedProperty languageKey;
        
        private SerializedProperty shadow;
        private SerializedProperty shadowColor;
        private SerializedProperty offsetX;
        private SerializedProperty offsetY;
        private SerializedProperty dilate;
        private SerializedProperty softness;
        
        private CustomTextMeshPro textMeshPro;
        
        GUIStyle m_caption;
        GUIStyle caption 
        {
            get
            {
                if(m_caption == null)
                {
                    m_caption = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };
                }
                return m_caption;
            }
        }
        
        [MenuItem("GameObject/CustomUI/Text-TMP", false, -9)]
        private static void CreateTextTMP(MenuCommand menuCommand)
        {
            var method = typeof(TMPro_CreateObjectMenu).GetMethod("CreateTextMeshProGuiObjectPerform",
                BindingFlags.Static | BindingFlags.NonPublic);
            method.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            CustomComponentEditor.ReplaceComponent(obj);
        }
        
        protected override void OnEnable()
        {
            textMeshPro = target as CustomTextMeshPro;
            base.OnEnable();
            customOutlineColor = serializedObject.FindProperty("customOutLineColor");
            customOutlineWidth = serializedObject.FindProperty("customOutLineWidth");
            shadow = serializedObject.FindProperty("shadow");
            shadowColor = serializedObject.FindProperty("shadowColor");
            offsetX = serializedObject.FindProperty("offsetX");
            offsetY = serializedObject.FindProperty("offsetY");
            dilate = serializedObject.FindProperty("dilate");
            softness = serializedObject.FindProperty("softness");
            languageKey = serializedObject.FindProperty("languageKey");
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("<b><color=white>Additional configs</color></b>", caption);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(customOutlineColor,new GUIContent("描边颜色"));
            EditorGUILayout.PropertyField(customOutlineWidth,new GUIContent("描边宽度"));
            EditorGUILayout.PropertyField(languageKey,new GUIContent("多语言Key"));
            EditorGUILayout.PropertyField(shadow,new GUIContent("开启阴影"));
            if (shadow.boolValue)
            {
                EditorGUILayout.PropertyField(shadowColor,new GUIContent("  阴影颜色"));
                EditorGUILayout.PropertyField(offsetX,new GUIContent("  横向偏移"));
                EditorGUILayout.PropertyField(offsetY,new GUIContent("  纵向偏移"));
                EditorGUILayout.PropertyField(dilate,new GUIContent("  放大倍数"));
                EditorGUILayout.PropertyField(softness,new GUIContent("  模糊"));
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("<b><color=white>For original ScrollRect</color></b>", caption);
            textMeshPro.ShowOutline();
            textMeshPro.ShowShadow();
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
        
    }
}