using System;
using System.Reflection;
using Framework.UIComponent;
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
            void FindAndReplace<T, TNew>(GameObject go, Action<T,TNew> replace = null)
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

            var txt = gameObject.GetComponent<CustomText>();
            if (txt)
            {
                txt.color = Color.black;
                txt.text = "说点什么..";
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
        
        [MenuItem("GameObject/CustomUI/Image", false, -10)]
        private static void CreateImage(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddImage", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/RawImage", false, -10)]
        private static void CreateRawImage(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddRawImage", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/Text", false, -10)]
        private static void CreateText(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddText", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/Text-TMP", false, -10)]
        private static void CreateTextTMP(MenuCommand menuCommand)
        {
            var method = typeof(TMPro_CreateObjectMenu).GetMethod("CreateTextMeshProGuiObjectPerform",
                BindingFlags.Static | BindingFlags.NonPublic);
            method.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/Toggle", false, -10)]
        private static void CreateToggle(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddToggle", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/Slider", false, -10)]
        private static void CreateSlider(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddSlider", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/InputField", false, -10)]
        private static void CreateDropdown(MenuCommand menuCommand)
        {
            var menuOptions = typeof(MaskEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            var createBtn = menuOptions.GetMethod("AddInputField", BindingFlags.Static | BindingFlags.Public);
            createBtn.Invoke(null, new object[] {menuCommand});
            var obj = Selection.activeGameObject;
            ReplaceComponent(obj);
        }
        
        [MenuItem("GameObject/CustomUI/InputField-TMP", false, -10)]
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