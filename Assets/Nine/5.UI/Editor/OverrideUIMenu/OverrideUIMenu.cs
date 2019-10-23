using System;
using Nine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

public class OverrideUIMenu 
{
	[MenuItem("GameObject/UI/Text")]
	private static DText CreateText ()
	{
		DText txt = new GameObject("Text").AddComponent<DText>();
		SetTransform(txt.transform);
        return txt;
    }
	
	[MenuItem("GameObject/UI/Image")]
	private static DImage CreateImage ()
	{
		DImage img = new GameObject("Image").AddComponent<DImage>();
		SetTransform(img.transform);
        return img;
    }

    [MenuItem("GameObject/UI/Raw Image")]
    private static DRawImage CreateRawImage()
    {
        DRawImage img = new GameObject("RawImage").AddComponent<DRawImage>();
        SetTransform(img.transform);
        return img;
    }

    [MenuItem("GameObject/UI/Button")]
    private static DButton CreateButton()
    {
        DButton btn = new GameObject("Button").AddComponent<DButton>();
        SetTransform(btn.transform);
        Transform txt = CreateText().transform;
        txt.transform.SetParent(btn.transform, false);
        btn.gameObject.AddComponent<DImage>();
        return btn;
    }

    [MenuItem("GameObject/UI/Toggle2")]
    private static DToggle CreateToggle ()
    {
	    DToggle toggle = new GameObject("Toggle").AddComponent<DToggle>();
	    toggle.isOn = true;
	    SetTransform(toggle.transform);
	    toggle.GetComponent<RectTransform> ().SetSizeWidth ( 160 ).SetSizeHeight ( 20 );
	    DImage bg = CreateImage ();
	    bg.sprite = CreateBuildInSprite ( UISPRITE );
	    bg.GetType ().GetField ( "m_Type" ).SetValue ( bg, Image.Type.Sliced );
	    bg.fillCenter = true;
	    RectTransform bgRect = bg.GetComponent<RectTransform> ();
	    bg.name = "Background";
	    bgRect.SetParent ( toggle.transform, false );
	    bgRect.localPosition = new Vector2 ( 10, -10 );
	    SetAnchors ( bg, new Vector2 ( 0, 1 ), new Vector2 ( 0, 1 ) );
	    bgRect.SetSizeWidth ( 20 ).SetSizeHeight ( 20 );
	    DText lable = CreateText ();
	    lable.name = "Lable";
	    lable.text = "Toggle";
	    lable.color = Color.black;
	    var labRect = lable.rectTransform;
	    labRect.SetParent(toggle.transform, false);
	    SetAnchors ( labRect, Vector2.zero, Vector2.one );
	    labRect.SetLeft ( 23 ).SetTop ( 2 ).SetRight ( 5 ).SetBottom ( 1 );
	    DImage checkmark = CreateImage ();
	    checkmark.name = CHECKMARK;
	    checkmark.sprite = CreateBuildInSprite ( CHECKMARK );
	    checkmark.GetComponent<RectTransform> ().SetSizeWidth ( 20 ).SetSizeHeight ( 20 );
	    checkmark.transform.SetParent(bgRect, false);
	    toggle.targetGraphic = bg;
	    toggle.graphic = checkmark;
	    
	    return toggle;
    }

    private static void SetTransform(Transform transform)
    {
        Transform activeTransform = Selection.activeTransform;
        GameObject canvasObj = SecurityCheck();
        if (!activeTransform) // 在根文件夹创建的， 自己主动移动到 Canvas下
        {
            transform.SetParent(canvasObj.transform, false);
            transform.gameObject.layer = canvasObj.layer;
        }
        else
        {
            if (!activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
            {
                transform.SetParent(canvasObj.transform, false);
                transform.gameObject.layer = canvasObj.layer;
            }
            else
            {
                transform.SetParent(activeTransform, false);
                transform.gameObject.layer = activeTransform.gameObject.layer;
            }
        }
        Selection.activeObject = transform;
    }

    private static void SetAnchors ( Component component, Vector2 min, Vector2 max )
    {
	    SetAnchors ( component.GetComponent<RectTransform>(), min, max );
    }

    private static void SetAnchors ( RectTransform rectTrans, Vector2 min, Vector2 max )
    {
	    rectTrans.anchorMin = min;
	    rectTrans.anchorMax = max;
    }

    private static Transform CreateEmpty (Transform parent = null)
    {
	    GameObject go = new GameObject("GameObject");
	    SetTransform(go.transform);
	    if ( parent != null )
	    {
		    go.transform.SetParent(parent);
	    }
	    return go.transform;
    }

    private static Sprite CreateBuildInSprite (string name)
    {
	    return AssetDatabase.GetBuiltinExtraResource<Sprite> ( $"UI/Skin/{name}.psd" );
    }
	
	// 假设第一次创建UI元素 可能没有 Canvas、EventSystem对象！
	private static GameObject SecurityCheck()
	{
		Canvas     cv = Object.FindObjectOfType<Canvas>();
		GameObject canvas;
		if (!cv)
		{
			canvas = new GameObject("Canvas", typeof(Canvas));
			Undo.RegisterCreatedObjectUndo(canvas, "Canvas");
		}
		else
		{
			canvas = cv.gameObject;
		}

		if (!Object.FindObjectOfType<EventSystem>())
		{
			GameObject go = new GameObject("EventSystem", typeof(EventSystem));
			Undo.RegisterCreatedObjectUndo(go, "EventSystem");
		}

		canvas.layer = LayerMask.NameToLayer("UI");
		return canvas;
	}

	private const string CHECKMARK = "Checkmark";
	private const string UISPRITE = "UISprite";

}
