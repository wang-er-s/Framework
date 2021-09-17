using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Editor
{
    [CustomEditor(typeof(CustomButton))]
    public class CustomButtonEditor : ButtonEditor
    {
        private SerializedProperty singleClickIntervalTime;
        private SerializedProperty doubleClickIntervalTime;
        private SerializedProperty longClickTime;
        private SerializedProperty longPressIntervalTime;
        
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

        [MenuItem("GameObject/UI/CustomButton", false, -10)]
        private static void Create(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddButton", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] { menuCommand });
            var obj = Selection.activeGameObject;
            DestroyImmediate(obj.GetComponent<Button>());
            obj.AddComponent<CustomButton>();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            singleClickIntervalTime = serializedObject.FindProperty("singleClickIntervalTime");
            doubleClickIntervalTime = serializedObject.FindProperty("doubleClickIntervalTime");
            longPressIntervalTime = serializedObject.FindProperty("longPressIntervalTime");
            longClickTime = serializedObject.FindProperty("longClickTime");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("<b><color=white>Additional configs</color></b>", caption);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(singleClickIntervalTime,new GUIContent("单击间隔时间(s)"));
            EditorGUILayout.PropertyField(doubleClickIntervalTime,new GUIContent("多少秒内算双击(s)"));
            EditorGUILayout.PropertyField(longClickTime,new GUIContent("超过多久算长按(s)"));
            EditorGUILayout.PropertyField(longPressIntervalTime,new GUIContent("按下状态,多久触发一次OnLongPress(s)"));
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("<b><color=white>For original ScrollRect</color></b>", caption);
            base.OnInspectorGUI();
        }
    }
}