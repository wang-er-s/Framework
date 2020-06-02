using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoUtil.Editor.Show
{
    public class ShowUtil
    {
        public static Type GetGameViewType()
        {
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            return assembly.GetType("UnityEditor.GameView");
        }

        public static Type GetInspectorViewType()
        {
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            return assembly.GetType("UnityEditor.InspectorWindow");
        }

        public static Scene NewScene()
        {
            return EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        }

        public static void ScriptTitle(UnityEngine.Object target)
        {
            GUI.enabled = false;
            if (target is MonoBehaviour)
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as MonoBehaviour),
                    typeof(MonoScript), false);
            if (target is ScriptableObject)
                EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(target as ScriptableObject),
                    typeof(MonoScript), false);
            GUI.enabled = true;
        }

        public static void Separator()
        {
            GUIStyle separator = new GUIStyle();

            Rect r = GUILayoutUtility.GetRect(new GUIContent(), separator);

            if (Event.current.type == EventType.Repaint)
            {
                separator.Draw(r, false, false, false, false);
            }
        }

        public static void DrawLine(float lineHight = 2f)
        {
            GUILayout.Box("", GUILayout.Height(lineHight), GUILayout.ExpandWidth(true));
        }

        public static void DrawTitle(string title, Color color)
        {
            Color tmp = GUI.color;
            GUI.color = color;
            GUILayout.Box(title, GUILayout.ExpandWidth(true));
            GUI.color = tmp;
        }

        public static void ShowList<T>(List<T> list, String title, Func<int, T> newOne, Action<int> display,
            Action<int> onRemove = null)
        {
            Rect rect = EditorGUILayout.BeginVertical();
            GUI.Box(rect, "");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title + " : " + list.Count);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+"))
            {
                list.Add(newOne(list.Count - 1));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                display(i);
                if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                {
                    list.Insert(i + 1, newOne(i));
                    break;
                }

                if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                {
                    list.RemoveAt(i);
                    if (null != onRemove)
                        onRemove(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        public static UnityEngine.Object ObjectDraw(UnityEngine.Object obj, string head,
            Action<UnityEngine.Object> menuSel)
        {
            Type t = typeof(GameObject);
            GameObject go = null;
            if (null != obj)
            {
                if (obj is GameObject)
                    go = (GameObject) obj;
                else
                {
                    t = obj.GetType();
                    go = ((Component) obj).gameObject;
                }
            }

            EditorGUILayout.BeginHorizontal();
            /*
            List<Type> types;
            string[] typeNames = GetGameObjectTypeList(go,out types);
            string typename = t.FullName;
            int typeindex = Array.IndexOf(typeNames, typename);
            EditorGUILayout.TextArea(head);
            typeindex = EditorGUILayout.Popup(typeindex, typeNames);
            t = types[typeindex];
            if (null != go && obj.GetType() != t)
                obj = go.GetComponent(t);
            obj = EditorGUILayout.ObjectField(GUIContent.none, obj, t, true);
            */
            EditorGUILayout.TextArea(head);
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(t.Name))
                {
                    DrawGameObjCompTypesMenu(go, (type) =>
                    {
                        t = (Type) type;
                        if (null == go)
                            menuSel(null);
                        else
                        {
                            if (t == typeof(GameObject))
                                menuSel(go);
                            else
                                menuSel(go.GetComponent(t));
                        }
                    });
                }
            }

            obj = EditorGUILayout.ObjectField(GUIContent.none, obj, t, true);
            EditorGUILayout.EndHorizontal();
            return obj;
        }

        private static void DrawGameObjCompTypesMenu(GameObject go, GenericMenu.MenuFunction2 callback)
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(GameObject));
            if (null != go)
            {
                Behaviour[] behaviours = go.GetComponents<Behaviour>();
                if (null != behaviours)
                {
                    foreach (var behav in behaviours)
                    {
                        Type t = behav.GetType();
                        if (!types.Contains(t))
                        {
                            types.Add(t);
                        }
                    }
                }
            }

            GenericMenu menu = new GenericMenu();
            foreach (var type in types)
            {
                menu.AddItem(new GUIContent(type.FullName), false, callback, type);
            }

            menu.ShowAsContext();
        }

        private static string[] GetGameObjectTypeList(GameObject go, out List<Type> types)
        {
            List<string> typeNames = new List<string>();
            types = new List<Type>();
            types.Add(typeof(GameObject));
            typeNames.Add(typeof(GameObject).FullName);
            if (null != go)
            {
                Behaviour[] behaviours = go.GetComponents<Behaviour>();
                if (null != behaviours)
                {
                    foreach (var behav in behaviours)
                    {
                        Type t = behav.GetType();
                        string typeName = t.FullName;
                        if (!typeNames.Contains(typeName))
                        {
                            types.Add(t);
                            typeNames.Add(typeName);
                        }
                    }
                }
            }

            return typeNames.ToArray();
        }
    }
}