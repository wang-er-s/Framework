using System;
using System.Collections.Generic;
using Framework.UI.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIMark : MonoBehaviour
{
    public string FieldName;

    [ValueDropdown("Components")]
    public Component CurComponent;

    private List<Component> Components;

    public bool IgnoreSelf;

    public bool IgnoreChild;

    private void Awake()
    {
        Components = new List<Component>();
        gameObject.GetComponents(typeof(Component), Components);
        if (!string.IsNullOrEmpty(FieldName)) return;
        FieldName = gameObject.name;
        foreach (var component in Components)
        {
            if (component == DefaultComponent)
            {
                CurComponent = component;
                break;
            }
        }
    }

    private void Update()
    {
        gameObject.GetComponents(typeof(Component), Components);
    }
    
    private Component DefaultComponent
    {
        get
        {
            if (GetComponent<View>()) return GetComponent<View>();
            if (GetComponent<ScrollRect>()) return GetComponent<ScrollRect>();
            if (GetComponent<InputField>()) return GetComponent<InputField>();

            // text mesh pro supported
            if (GetComponent("TMP.TextMeshProUGUI")) return GetComponent("TMP.TextMeshProUGUI");
            if (GetComponent("TMPro.TextMeshProUGUI")) return GetComponent("TMPro.TextMeshProUGUI");
            if (GetComponent("TMPro.TextMeshPro")) return GetComponent("TMPro.TextMeshPro");
            if (GetComponent("TMPro.TMP_InputField")) return GetComponent("TMPro.TMP_InputField");

            // ugui bind
            if (GetComponent<Dropdown>()) return GetComponent<Dropdown>();
            if (GetComponent<Button>()) return GetComponent<Button>();
            if (GetComponent<Text>()) return GetComponent<Text>();
            if (GetComponent<RawImage>()) return GetComponent<RawImage>();
            if (GetComponent<Toggle>()) return GetComponent<Toggle>();
            if (GetComponent<Slider>()) return GetComponent<Slider>();
            if (GetComponent<Scrollbar>()) return GetComponent<Scrollbar>();
            if (GetComponent<Image>()) return GetComponent<Image>();
            if (GetComponent<ToggleGroup>()) return GetComponent<ToggleGroup>();

            // other
            if (GetComponent<Rigidbody2D>()) return GetComponent<Rigidbody2D>();
            if (GetComponent<BoxCollider2D>()) return GetComponent<BoxCollider2D>();
            if (GetComponent<CircleCollider2D>()) return GetComponent<CircleCollider2D>();
            if (GetComponent<Collider2D>()) return GetComponent<Collider2D>();
            if (GetComponent<Animator>()) return GetComponent<Animator>();
            if (GetComponent<Canvas>()) return GetComponent<Canvas>();
            if (GetComponent<Camera>()) return GetComponent<Camera>();
            if (GetComponent<RectTransform>()) return GetComponent<RectTransform>();
            if (GetComponent<Transform>()) return GetComponent<Transform>();
            if (GetComponent<MeshRenderer>()) return GetComponent<MeshRenderer>();
            if (GetComponent<SpriteRenderer>()) return GetComponent<SpriteRenderer>();
            return null;
        }
    }
}