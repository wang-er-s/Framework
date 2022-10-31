using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using TextEditor = UnityEditor.UI.TextEditor;

namespace Framework.Editor
{
    [CustomEditor(typeof(CustomText)), CanEditMultipleObjects]
    public class CustomTextEditor : TextEditor
    {
        private SerializedProperty languageKey;
        GUIStyle m_caption;

        GUIStyle caption
        {
            get
            {
                if (m_caption == null)
                {
                    m_caption = new GUIStyle {richText = true, alignment = TextAnchor.MiddleCenter};
                }

                return m_caption;
            }
        }

        [MenuItem("GameObject/CustomUI/Text", false, -10)]
        private static void CreateText(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddText", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            CustomComponentEditor.ReplaceComponent(obj);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            languageKey = serializedObject.FindProperty("languageKey");
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("<b><color=white>Additional configs</color></b>", caption);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(languageKey,new GUIContent("多语言Key"));
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("<b><color=white>For original Text</color></b>", caption);
            base.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}