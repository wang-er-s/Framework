using System.Reflection;
using Framework.UIComponent;
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
        
        [MenuItem("GameObject/CustomUI/Text-TMP", false, -10)]
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
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("<b><color=white>Additional configs</color></b>", caption);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(customOutlineColor,new GUIContent("描边颜色"));
            EditorGUILayout.PropertyField(customOutlineWidth,new GUIContent("描边宽度"));
            EditorGUILayout.PropertyField(languageKey,new GUIContent("多语言Key"));
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("<b><color=white>For original ScrollRect</color></b>", caption);
            textMeshPro.outlineColor = customOutlineColor.colorValue;
            textMeshPro.outlineWidth = customOutlineWidth.floatValue;
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
        
    }
}