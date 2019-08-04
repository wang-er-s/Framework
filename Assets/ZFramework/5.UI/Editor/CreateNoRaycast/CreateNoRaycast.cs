using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class CreateNoRaycast 
{
	[MenuItem("GameObject/UI/NoRayTxt", priority = 100)]
	private static void CreateNoRaycastTxt ()
	{
		Text txt = new GameObject("Text").AddComponent<Text>();
		Transform activeTransform = Selection.activeTransform;
		GameObject canvasObj = SecurityCheck (); 
		if (!activeTransform) // 在根文件夹创建的， 自己主动移动到 Canvas下
		{
			txt.transform.SetParent(canvasObj.transform, false);
			txt.gameObject.layer = canvasObj.layer;
		}
		else
		{
			if (!activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
			{
				txt.transform.SetParent(canvasObj.transform, false);
				txt.gameObject.layer = canvasObj.layer;
			}
			else
			{
				txt.transform.SetParent(activeTransform, false);
				txt.gameObject.layer = activeTransform.gameObject.layer;
			}
		}
		txt.transform.localScale = Vector3.one;
		txt.raycastTarget = false;
		txt.supportRichText = false;
		Selection.activeObject = txt;
	}
	
	[MenuItem("GameObject/UI/NoRayImg", priority = 100)]
	private static void CreateNoRaycastImg ()
	{
		Image img = new GameObject("Image").AddComponent<Image>();
		Transform  activeTransform = Selection.activeTransform;
		GameObject canvasObj       = SecurityCheck (); 
		if (!activeTransform) // 在根文件夹创建的， 自己主动移动到 Canvas下
		{
			img.transform.SetParent(canvasObj.transform, false);
			img.gameObject.layer = canvasObj.layer;
		}
		else
		{
			if (!activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
			{
				img.transform.SetParent(canvasObj.transform, false);
				img.gameObject.layer = canvasObj.layer;
			}
			else
			{
				img.transform.SetParent(activeTransform, false);
				img.gameObject.layer = activeTransform.gameObject.layer;
			}
		}
		img.transform.localScale = Vector3.one;
		img.raycastTarget = false;
		Selection.activeObject = img;
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
	
}
