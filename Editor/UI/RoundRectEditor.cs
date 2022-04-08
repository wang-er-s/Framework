using UnityEditor;
using UnityEditor.UI;

namespace Framework.Editor
{
    [CustomEditor(typeof(RoundRect), true)]
    [CanEditMultipleObjects]
    public class RoundRectEditor : ImageEditor
    {
        protected SerializedProperty m_FillCenter;

        protected SerializedProperty m_Radius;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_FillCenter = serializedObject.FindProperty("m_FillCenter");
            m_Radius = serializedObject.FindProperty("m_Radius");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SpriteGUI();
            AppearanceControlsGUI();
            RaycastControlsGUI();

            EditorGUILayout.PropertyField(m_FillCenter);
            EditorGUILayout.PropertyField(m_Radius);

            serializedObject.ApplyModifiedProperties();
        }
    }
}