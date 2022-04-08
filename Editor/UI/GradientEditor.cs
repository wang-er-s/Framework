using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [CustomEditor(typeof(Gradient), true)]
    [CanEditMultipleObjects]
    public class GradientEditor : UnityEditor.Editor
    {
        protected SerializedProperty m_StartColor;
        protected SerializedProperty m_EndColor;
        protected SerializedProperty m_Angle;

        private GUIContent m_StartColorContent;
        private GUIContent m_EndColorContent;
        private GUIContent m_FastAngleContent;

        private readonly GUIContent[] m_FastAngleTitle = new GUIContent[] { new GUIContent("Left"), new GUIContent("Up"), new GUIContent("Right"), new GUIContent("Down")};
        private readonly int[] m_FastAngleValue = new int[] { 0, 90, 180, 270 };

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnEnable()
        {
            m_StartColorContent = new GUIContent("Start Color");
            m_EndColorContent = new GUIContent("End Color");
            m_FastAngleContent = new GUIContent("Fast Angle");

            m_StartColor = serializedObject.FindProperty("m_StartColor");
            m_EndColor = serializedObject.FindProperty("m_EndColor");
            m_Angle = serializedObject.FindProperty("m_Angle");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_StartColor, m_StartColorContent);
            EditorGUILayout.PropertyField(m_EndColor, m_EndColorContent);

            int angleValue = (int)m_Angle.floatValue;

            EditorGUI.BeginChangeCheck();

            angleValue = EditorGUILayout.IntPopup(m_FastAngleContent, angleValue, m_FastAngleTitle, m_FastAngleValue);

            if (EditorGUI.EndChangeCheck())
            {
                m_Angle.floatValue = angleValue;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}