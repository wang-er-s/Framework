using System;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public static class HierarchyExtension
    {
        [MenuItem("GameObject/拷贝路径", false, -10)]
        public static void CopyPath()
        {
            var trans = Selection.activeTransform;
            if (trans == null) return;
            var path = trans.GetPathFromRoot();
            GUIUtility.systemCopyBuffer = path;
            try
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("已复制到剪切板"));
            }
            catch (Exception)
            {
            }
        }
        
        [MenuItem("Assets/拷贝路径", false,1)]
        public static void CreateUiCode()
        {
            var go = Selection.activeObject;
            if(go == null) return;
            GUIUtility.systemCopyBuffer = AssetDatabase.GetAssetPath(go);
            try
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("已复制到剪切板"));
            }
            catch (Exception)
            {
            }
        }
    }
}