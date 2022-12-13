using System;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public static class HierarchyExtension
    {
        [MenuItem("Assets/拷贝路径", false,1)]
        public static void CopyPath()
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