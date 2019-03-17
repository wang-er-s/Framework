/*
* Create by Soso
* Time : 2018-12-31-01 下午
*/
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using UnityEngine.UI;

namespace ZFramework
{
	public class CreateUIRoot : EditorWindow
	{
		private UIManager mUIManager;

		[MenuItem ( "ZFramework/CreateUIRoot", priority = 600 )]
		public static void ShowWindow ()
		{
			GetWindow ( typeof ( CreateUIRoot ) );
		}

		private void OnGUI ()
		{
			titleContent.text = "CreateUIRoot";
			EditorGUILayout.BeginVertical ();
			UIManagerGUI();
			EditorGUILayout.EndVertical();
		}

		private bool isFoldUImanager = false;
		public Vector2 m_referenceResolution = new Vector2(720, 1280);
		public CanvasScaler.ScreenMatchMode m_MatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		public bool m_isOnlyUICamera = false;
		public bool m_isVertical = false;
		
		private void UIManagerGUI ()
		{
			EditorGUI.indentLevel = 0;
			isFoldUImanager       = EditorGUILayout.Foldout(isFoldUImanager, "UIRoot:");
			if (isFoldUImanager)
			{
				EditorGUI.indentLevel = 1;
				m_referenceResolution = EditorGUILayout.Vector2Field("参考分辨率", m_referenceResolution);
				m_isOnlyUICamera      = EditorGUILayout.Toggle("只有一个UI摄像机", m_isOnlyUICamera);
				m_isVertical          = EditorGUILayout.Toggle("是否竖屏", m_isVertical);

				if (GUILayout.Button("创建UIRoot"))
				{
					CreatUIManager(m_referenceResolution, m_MatchMode, m_isOnlyUICamera, m_isVertical);
				}

			}
		}
		
		public static void CreatUIManager(Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode, bool isOnlyUICamera, bool isVertical)
        {

            //UIManager
			GameObject fixCanvas = new GameObject("FixedRoot"){ layer = LayerMask.NameToLayer ( "UI" ) };
	        UIManager UIManager = fixCanvas.AddComponent<UIManager> ();
	        CreateUICamera ( UIManager, fixCanvas, 99, referenceResolution, MatchMode, isOnlyUICamera, isVertical );

            ProjectWindowUtil.ShowCreatedAsset(fixCanvas);

            //保存UIManager
            //ReSaveUIManager(UIManagerGo);
        }

        public static void CreateUICamera(UIManager UIManager, GameObject fixCanvas, float cameraDepth, Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode, bool isOnlyUICamera, bool isVertical)
        {

            GameObject UIManagerGo = UIManager.gameObject;
	        Transform fCanvas = fixCanvas.AddComponent<RectTransform>();

            var sObj = new SerializedObject(UIManager);
	        
	        GameObject fgoTmp = null;
            RectTransform frtTmp = null;

            fgoTmp = new GameObject("Bg");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp = fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("fBgTrans").objectReferenceValue = frtTmp.gameObject;


            fgoTmp = new GameObject("AnimationUnder");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp = fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("fAnimationUnderTrans").objectReferenceValue = frtTmp.gameObject;


            fgoTmp = new GameObject("Common");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp = fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("mCommonTrans").objectReferenceValue = frtTmp.gameObject;


            fgoTmp = new GameObject("AnimationOn");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp = fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("fAnimationOnTrans").objectReferenceValue = frtTmp.gameObject;


            fgoTmp = new GameObject("PopUI");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp = fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("mPopUITrans").objectReferenceValue = frtTmp.gameObject;


            fgoTmp = new GameObject("Const");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp = fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;
            sObj.FindProperty("fConstTrans").objectReferenceValue = frtTmp.gameObject;


            fgoTmp = new GameObject("Toast");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp = fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;

            sObj.FindProperty("fToastTrans").objectReferenceValue = frtTmp.gameObject;


            fgoTmp = new GameObject("Forward");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp = fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;
            sObj.FindProperty("fForwardTrans").objectReferenceValue = frtTmp.gameObject;


            fgoTmp = new GameObject("Design");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp =fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;
            fgoTmp.AddComponent<Hide>();

            fgoTmp = new GameObject("EventSystem");
            fgoTmp.layer = LayerMask.NameToLayer("UI");
            fgoTmp.transform.SetParent(fCanvas);
            fgoTmp.transform.localScale = Vector3.one;
            frtTmp = fgoTmp.AddComponent<RectTransform>();
            frtTmp.anchorMax = new Vector2(1, 1);
            frtTmp.anchorMin = new Vector2(0, 0);
            frtTmp.anchoredPosition3D = Vector3.zero;
            frtTmp.sizeDelta = Vector2.zero;
            fgoTmp.AddComponent<UnityEngine.EventSystems.EventSystem>();
            fgoTmp.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            //UIcamera
            GameObject fcameraGo = new GameObject("UICamera");
            fcameraGo.transform.SetParent(fCanvas);
            fcameraGo.transform.localPosition = new Vector3(0, 0, -1000);
            Camera fcamera = fcameraGo.AddComponent<Camera>();
            fcamera.cullingMask = LayerMask.GetMask("UI");
            fcamera.orthographic = true;
            fcamera.depth = cameraDepth;
            sObj.FindProperty("fUICamera").objectReferenceValue = fcamera.gameObject;

            //Canvas
            Canvas fCanvasComp = fCanvas.gameObject.AddComponent<Canvas>();
	        fCanvasComp.renderMode = RenderMode.ScreenSpaceCamera;
	        fCanvasComp.worldCamera = fcamera;
	        fCanvasComp.sortingOrder = 100;
            sObj.FindProperty("fCanvas").objectReferenceValue = fCanvasComp.gameObject;

            //CanvasScaler
            CanvasScaler fscaler = fCanvas.gameObject.AddComponent<CanvasScaler>();
            fscaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            fscaler.referenceResolution = referenceResolution;
            fscaler.screenMatchMode = MatchMode;
            sObj.FindProperty("fCanvasScaler").objectReferenceValue = fscaler;

            if (!isOnlyUICamera)
            {
                fcamera.clearFlags = CameraClearFlags.Depth;
                fcamera.depth = cameraDepth;
            }
            else
            {
                fcamera.clearFlags = CameraClearFlags.SolidColor;
                fcamera.backgroundColor = Color.black;
            }
            fscaler.matchWidthOrHeight = isVertical ? 1 : 0;

            fCanvas.gameObject.AddComponent<GraphicRaycaster>();
            //重新保存
            /*ReSaveUIManager(mCanvas.gameObject,"MoveCanvas");
            ReSaveUIManager(fCanvas.gameObject,"FixedCanvas");*/

            sObj.ApplyModifiedPropertiesWithoutUndo();
        }

        /*static void ReSaveUIManager(GameObject Go,string name)
        {
            string dirPath = Application.dataPath + "/Resources/UI";
	        string filePath = "Assets/Resources/UIPrefab/" + name + ".prefab";
            Debug.Log(dirPath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            PrefabUtility.CreatePrefab(filePath, Go, ReplacePrefabOptions.ConnectToPrefab);
        }*/
	}
}
