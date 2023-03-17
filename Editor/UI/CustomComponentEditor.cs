using System;
using System.Reflection;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Framework.Editor
{
    public static class CustomComponentEditor
    {
        public static void ReplaceComponent(GameObject gameObject)
        {
            void FindAndReplace<T, TNew>(GameObject go)
            {
                var components = go.GetComponents(typeof(T));
                for (int i = 0; i < components.Length; i++)
                {
                    Object.DestroyImmediate(components[i]);
                    go.AddComponent(typeof(TNew));
                }

                foreach (Transform child in go.transform)
                {
                    ReplaceComponent(child.gameObject);
                }
            }

            FindAndReplace<Button, CustomButton>(gameObject);
            FindAndReplace<Image, CustomImage>(gameObject);
            FindAndReplace<Text, CustomText>(gameObject);
            FindAndReplace<TextMeshProUGUI, CustomTextMeshPro>(gameObject);
            FindAndReplace<Toggle, CustomToggle>(gameObject);
            FindAndReplace<Slider, CustomSlider>(gameObject);
            FindAndReplace<InputField, CustomInputField>(gameObject);
            FindAndReplace<TMP_InputField, CustomInputFieldTMP>(gameObject);
            FindAndReplace<TextMeshPro, CustomTextMesh3D>(gameObject);

            var img = gameObject.GetComponent<Image>();
            if (img)
            {
                if (img.GetComponent<Button>() == null)
                {
                    if (img.transform.parent.GetComponent<Button>() == null)
                    {
                        img.raycastTarget = false;
                    }
                    else
                    {
                        if (img.transform.parent.GetComponent<Button>().targetGraphic != img)
                        {
                            img.raycastTarget = false;
                        }
                    }
                }
            }
            
            var txt = gameObject.GetComponent<CustomText>();
            if (txt)
            {
                txt.color = Color.black;
                txt.text = "说点什么..";
                txt.supportRichText = false;
                txt.raycastTarget = false;
                txt.alignment = TextAnchor.MiddleCenter;
            }
            
            var tmp = gameObject.GetComponent<CustomTextMeshPro>();
            if (tmp)
            {
                tmp.color = Color.black;
                tmp.text = "说点什么..";
                tmp.richText = false;
                tmp.raycastTarget = false;
                tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
                tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
            }

            var btn = gameObject.GetComponent<CustomButton>();
            if (btn)
            {
                btn.targetGraphic = gameObject.GetComponent<Graphic>();
            }

            var toggle = gameObject.GetComponent<CustomToggle>();
            if (toggle)
            {
                toggle.targetGraphic = gameObject.transform.Find("Background").GetComponent<Graphic>();
                toggle.graphic = gameObject.transform.Find("Background/Checkmark").GetComponent<Graphic>();
            }

            var slider = gameObject.GetComponent<CustomSlider>();
            if (slider)
            {
                slider.fillRect = gameObject.transform.Find("Fill Area/Fill").GetComponent<RectTransform>();
                slider.handleRect = gameObject.transform.Find("Handle Slide Area/Handle").GetComponent<RectTransform>();
                slider.targetGraphic = gameObject.transform.Find("Handle Slide Area/Handle").GetComponent<Graphic>();
            }

            var input = gameObject.GetComponent<CustomInputField>();
            if (input)
            {
                input.targetGraphic = gameObject.GetComponent<Graphic>();
                input.textComponent = gameObject.transform.Find("Text").GetComponent<Text>();
                input.placeholder = gameObject.transform.Find("Placeholder").GetComponent<Text>();
            }
            
            var input2 = gameObject.GetComponent<CustomInputFieldTMP>();
            if (input2)
            {
                input2.targetGraphic = gameObject.GetComponent<Graphic>();
                input2.textViewport = gameObject.transform.Find("Text Area").GetComponent<RectTransform>();
                input2.textComponent = gameObject.transform.Find("Text Area/Text").GetComponent<TextMeshProUGUI>();
                input2.placeholder = gameObject.transform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>();
                input2.fontAsset = TMP_Settings.defaultFontAsset;
            }
        }
        
        [MenuItem("GameObject/CustomUI/Image", false, -6)]
        private static void CreateImage(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddImage", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/RawImage", false, -5)]
        private static void CreateRawImage(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddRawImage", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }

        [MenuItem("GameObject/CustomUI/Toggle", false, -4)]
        private static void CreateToggle(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddToggle", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/Slider", false, -3)]
        private static void CreateSlider(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddSlider", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/InputField", false, -2)]
        private static void CreateDropdown(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddInputField", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/InputField-TMP", false, -1)]
        private static void CreateInputFieldTMP(MenuCommand menuCommand)
        {
            var method = typeof(TMPro_CreateObjectMenu).GetMethod("AddTextMeshProInputField",
                BindingFlags.Static | BindingFlags.NonPublic);
            method.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
    }
}