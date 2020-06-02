using UnityEditor;
using UnityEngine;

namespace SoUtil.Editor.Show
{
    public static class EditorGUIUtil
    {
        public static float space = 5;
        public static float indexWidth = 20;
        public static float reorderableListHeaderIndent = 15;
        public static float singleLineHeight = EditorGUIUtility.singleLineHeight;

        
        public static Rect DrawReorderableListHeaderIndex(Rect rect)
        {
            float width = reorderableListHeaderIndent + indexWidth;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, width, singleLineHeight), "NO.");
            rect.x += width; rect.width -= width;
            return rect;
        }
        public static Rect DrawReorderableListIndex(Rect rect, SerializedProperty property, int index)
        {
            if (GUI.Button(new Rect(rect.x, rect.y, indexWidth, singleLineHeight), index.ToString("00"), EditorStyles.label))
            {
                DrawReorderMenu(property, index).ShowAsContext();
            }
            rect.x += indexWidth; rect.width -= indexWidth;
            return rect;
        }
        public static GenericMenu DrawReorderMenu(SerializedProperty property, int index)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Insert"), false, delegate
            {
                property.InsertArrayElementAtIndex(index);
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.AddItem(new GUIContent("Delete"), false, delegate
            {
                property.DeleteArrayElementAtIndex(index);
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Move to Top"), false, delegate
            {
                property.MoveArrayElement(index, 0);
                property.serializedObject.ApplyModifiedProperties();
            });
            menu.AddItem(new GUIContent("Move to Bottom"), false, delegate
            {
                property.MoveArrayElement(index, property.arraySize - 1);
                property.serializedObject.ApplyModifiedProperties();
            });
            return menu;
        }
    }
}