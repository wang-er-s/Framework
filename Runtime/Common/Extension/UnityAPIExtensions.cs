using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class BehaviourExtension
{
    public static void Example()
    {
        var gameObject = new GameObject();
        var component = gameObject.GetComponent<MonoBehaviour>();

        component.Enable(); // component.enabled = true
        component.Disable(); // component.enabled = false
    }

    public static T Enable<T>(this T selfBehaviour) where T : Behaviour
    {
        selfBehaviour.enabled = true;
        return selfBehaviour;
    }

    public static T Disable<T>(this T selfBehaviour) where T : Behaviour
    {
        selfBehaviour.enabled = false;
        return selfBehaviour;
    }
}

public static class CameraExtension
{
    public static void Example()
    {
        var screenshotTexture2D = Camera.main.CaptureCamera(new Rect(0, 0, Screen.width, Screen.height));
        Debug.Log(screenshotTexture2D.width);
    }

    public static Texture2D CaptureCamera(this Camera camera, Rect rect)
    {
        var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        camera.targetTexture = renderTexture;
        camera.Render();

        RenderTexture.active = renderTexture;

        var screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        camera.targetTexture = null;
        RenderTexture.active = null;
        Object.Destroy(renderTexture);

        return screenShot;
    }
}

public static class ColorExtension
{
    public static void Example()
    {
        var color = "#C5563CFF".HtmlStringToColor();
        Debug.Log(color);
    }

    /// <summary>
    /// #C5563CFF -> 197.0f / 255,86.0f / 255,60.0f / 255
    /// </summary>
    /// <param name="htmlString"></param>
    /// <returns></returns>
    public static Color HtmlStringToColor(this string htmlString)
    {
        Color retColor;
        var parseSucceed = ColorUtility.TryParseHtmlString(htmlString, out retColor);
        return parseSucceed ? retColor : Color.black;
    }

    /// <summary>
    /// unity's color always new a color
    /// </summary>
    public static Color White = Color.white;
}

/// <summary>
/// GameObject's Util/Static This Extension
/// </summary>
public static class GameObjectExtension
{
    public static void Example()
    {
        var gameObject = new GameObject();
        var transform = gameObject.transform;
        var selfScript = gameObject.AddComponent<MonoBehaviour>();
        var boxCollider = gameObject.AddComponent<BoxCollider>();

        gameObject.ActiveShow(); // gameObject.SetActive(true)
        selfScript.ActiveShow(); // this.gameObject.SetActive(true)
        boxCollider.ActiveShow(); // boxCollider.gameObject.SetActive(true)
        gameObject.transform.ActiveShow(); // transform.gameObject.SetActive(true)

        gameObject.ActiveHide(); // gameObject.SetActive(false)
        selfScript.ActiveHide(); // this.gameObject.SetActive(false)
        boxCollider.ActiveHide(); // boxCollider.gameObject.SetActive(false)
        transform.ActiveHide(); // transform.gameObject.SetActive(false)

        selfScript.DestroyGameObj();
        boxCollider.DestroyGameObj();
        transform.DestroyGameObj();

        selfScript.DestroyGameObjGracefully();
        boxCollider.DestroyGameObjGracefully();
        transform.DestroyGameObjGracefully();

        selfScript.DestroyGameObjAfterDelay(1.0f);
        boxCollider.DestroyGameObjAfterDelay(1.0f);
        transform.DestroyGameObjAfterDelay(1.0f);

        selfScript.DestroyGameObjAfterDelayGracefully(1.0f);
        boxCollider.DestroyGameObjAfterDelayGracefully(1.0f);
        transform.DestroyGameObjAfterDelayGracefully(1.0f);

        gameObject.Layer(0);
        selfScript.Layer(0);
        boxCollider.Layer(0);
        transform.Layer(0);

        gameObject.Layer("Default");
        selfScript.Layer("Default");
        boxCollider.Layer("Default");
        transform.Layer("Default");
    }
    
    #region CEGO001 Show

    public static GameObject ActiveShow(this GameObject selfObj)
    {
        if (selfObj.activeSelf) return selfObj;
        selfObj.SetActive(true);
        return selfObj;
    }

    public static GameObject ScaleShow(this GameObject selfObj)
    {
        selfObj.GetComponent<Transform>().localScale = Vector3.one;
        return selfObj;
    }

    public static T ActiveShow<T>(this T selfComponent) where T : Component
    {
        selfComponent.gameObject.ActiveShow();
        return selfComponent;
    }

    public static T ScaleShow<T>(this T selfComponent) where T : Component
    {
        selfComponent.gameObject.ScaleShow();
        return selfComponent;
    }

    #endregion

    #region CEGO002 Hide

    public static GameObject ActiveHide(this GameObject selfObj)
    {
        if (selfObj.activeSelf == false) return selfObj;
        selfObj.SetActive(false);
        return selfObj;
    }

    public static GameObject ScaleHide(this GameObject selfObj)
    {
        selfObj.GetComponent<Transform>().localScale = Vector3.zero;
        return selfObj;
    }

    public static T ActiveHide<T>(this T selfComponent) where T : Component
    {
        selfComponent.gameObject.ActiveHide();
        return selfComponent;
    }

    public static T ScaleHide<T>(this T selfComponent) where T : Component
    {
        selfComponent.gameObject.ScaleHide();
        return selfComponent;
    }

    #endregion

    #region CEGO003 DestroyGameObj

    public static void DestroyGameObj<T>(this T selfBehaviour) where T : Component
    {
        selfBehaviour.gameObject.DestroySelf();
    }

    #endregion

    #region CEGO004 DestroyGameObjGracefully

    public static void DestroyGameObjGracefully<T>(this T selfBehaviour) where T : Component
    {
        if (selfBehaviour && selfBehaviour.gameObject)
        {
            selfBehaviour.gameObject.DestroySelfGracefully();
        }
    }

    #endregion

    #region CEGO005 DestroyGameObjGracefully

    public static T DestroyGameObjAfterDelay<T>(this T selfBehaviour, float delay) where T : Component
    {
        selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
        return selfBehaviour;
    }

    public static T DestroyGameObjAfterDelayGracefully<T>(this T selfBehaviour, float delay) where T : Component
    {
        if (selfBehaviour && selfBehaviour.gameObject)
        {
            selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
        }

        return selfBehaviour;
    }

    #endregion

    #region CEGO006 Layer

    public static GameObject Layer(this GameObject selfObj, int layer)
    {
        selfObj.layer = layer;
        return selfObj;
    }

    public static T Layer<T>(this T selfComponent, int layer) where T : Component
    {
        selfComponent.gameObject.layer = layer;
        return selfComponent;
    }

    public static GameObject Layer(this GameObject selfObj, string layerName)
    {
        selfObj.layer = LayerMask.NameToLayer(layerName);
        return selfObj;
    }

    public static T Layer<T>(this T selfComponent, string layerName) where T : Component
    {
        selfComponent.gameObject.layer = LayerMask.NameToLayer(layerName);
        return selfComponent;
    }

    #endregion

    #region CEGO007 Component

    public static T GetOrAddComponent<T>(this GameObject selfComponent) where T : Component
    {
        var comp = selfComponent.gameObject.GetComponent<T>();
        return comp ? comp : selfComponent.gameObject.AddComponent<T>();
    }

    public static Component GetOrAddComponent(this GameObject selfComponent, Type type)
    {
        var comp = selfComponent.gameObject.GetComponent(type);
        return comp ? comp : selfComponent.gameObject.AddComponent(type);
    }
    #endregion
}

public static class GraphicExtension
{
    public static void Example()
    {
        var gameObject = new GameObject();
        var image = gameObject.AddComponent<Image>();
        var rawImage = gameObject.AddComponent<RawImage>();

        // image.color = new Color(image.color.r,image.color.g,image.color.b,1.0f);
        image.ColorAlpha(1.0f);
        rawImage.ColorAlpha(1.0f);
    }

    public static T ColorAlpha<T>(this T selfGraphic, float alpha) where T : Graphic
    {
        var color = selfGraphic.color;
        color.a = alpha;
        selfGraphic.color = color;
        return selfGraphic;
    }
}

public static class ImageExtension
{
    public static void Example()
    {
        var gameObject = new GameObject();
        var image1 = gameObject.AddComponent<Image>();

        image1.FillAmount(0.0f); // image1.fillAmount = 0.0f;
    }

    public static Image FillAmount(this Image selfImage, float fillamount)
    {
        selfImage.fillAmount = fillamount;
        return selfImage;
    }

    public static Image ChangeColor(this Image selfImage, Color color)
    {
        selfImage.gameObject.GetComponent<CanvasRenderer>().SetColor(color);
        return selfImage;
    }

    public static Image ColorHide(this Image selfImage)
    {
        selfImage.gameObject.GetComponent<CanvasRenderer>()
            .SetColor(new Color(selfImage.color.r, selfImage.color.g, selfImage.color.b, 0));
        return selfImage;
    }

    public static Image ColorShow(this Image selfImage)
    {
        selfImage.gameObject.GetComponent<CanvasRenderer>()
            .SetColor(new Color(selfImage.color.r, selfImage.color.g, selfImage.color.b, 1));
        return selfImage;
    }
}

public static class TextExtension
{
    public static Text ChangeColor(this Text selfTxt, Color color)
    {
        selfTxt.gameObject.GetComponent<CanvasRenderer>().SetColor(color);
        return selfTxt;
    }
}

public static class ComponentExtension
{
    public static Component SetAllTag(this Component self, string targetTag)
    {
#if UNITY_EDITOR
        if (!UnityEditorInternal.InternalEditorUtility.tags.Contains(targetTag))
            Debug.LogError($"场景中不包含{targetTag}的标签");
#endif
        self.tag = targetTag;
        GetAllChild(self.transform, (child) => child.tag = targetTag);
        return self;
    }

    static void GetAllChild(Transform self, Action<Transform> action)
    {
        foreach (Transform child in self)
        {
            if (child.childCount > 0) GetAllChild(child, action);
            action?.Invoke(child);
        }
    }
}

public static class LightmapExtension
{
    public static void SetAmbientLightHTMLStringColor(string htmlStringColor)
    {
        RenderSettings.ambientLight = htmlStringColor.HtmlStringToColor();
    }
}

public static class ObjectExtension
{
    public static void Example()
    {
        var gameObject = new GameObject();

        gameObject.Instantiate().Name("ExtensionExample").DestroySelf();

        gameObject.Instantiate().DestroySelfGracefully();

        gameObject.Instantiate().DestroySelfAfterDelay(1.0f);

        gameObject.Instantiate().DestroySelfAfterDelayGracefully(1.0f);

        gameObject.ApplySelfTo(selfObj => Debug.Log(selfObj.name)).Name("TestObj")
            .ApplySelfTo(selfObj => Debug.Log(selfObj.name)).Name("ExtensionExample").DontDestroyOnLoad();
    }

    #region CEUO001 Instantiate

    public static T Instantiate<T>(this T selfObj) where T : Object
    {
        return Object.Instantiate(selfObj);
    }

    #endregion

    #region CEUO002 Instantiate

    public static T Name<T>(this T selfObj, string name) where T : Object
    {
        selfObj.name = name;
        return selfObj;
    }

    #endregion

    #region CEUO003 Destroy Self

    public static void DestroySelf<T>(this T selfObj) where T : Object
    {
        Object.Destroy(selfObj);
    }

    public static T DestroySelfGracefully<T>(this T selfObj) where T : Object
    {
        if (selfObj)
        {
            Object.Destroy(selfObj);
        }

        return selfObj;
    }

    #endregion

    #region CEUO004 Destroy Self AfterDelay

    public static T DestroySelfAfterDelay<T>(this T selfObj, float afterDelay) where T : Object
    {
        Object.Destroy(selfObj, afterDelay);
        return selfObj;
    }

    public static T DestroySelfAfterDelayGracefully<T>(this T selfObj, float delay) where T : Object
    {
        if (selfObj)
        {
            Object.Destroy(selfObj, delay);
        }

        return selfObj;
    }

    #endregion

    #region CEUO005 Apply Self To

    public static T ApplySelfTo<T>(this T selfObj, System.Action<T> toFunction) where T : Object
    {
        toFunction?.Invoke(selfObj);
        return selfObj;
    }

    #endregion

    #region CEUO006 DontDestroyOnLoad

    public static T DontDestroyOnLoad<T>(this T selfObj) where T : Object
    {
        Object.DontDestroyOnLoad(selfObj);
        return selfObj;
    }

    #endregion

    public static T As<T>(this Object selfObj) where T : Object
    {
        return selfObj as T;
    }
}

public static class RectTransformExtension
{
    public static Vector2 GetPosInRootTrans(this RectTransform selfRectTransform, Transform rootTrans)
    {
        return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, selfRectTransform).center;
    }

    public static RectTransform AnchorPosX(this RectTransform selfRectTrans, float anchorPosX)
    {
        var anchorPos = selfRectTrans.anchoredPosition;
        anchorPos.x = anchorPosX;
        selfRectTrans.anchoredPosition = anchorPos;
        return selfRectTrans;
    }

    public static RectTransform AnchorPosY(this RectTransform selfRectTrans, float anchorPosY)
    {
        var anchorPos = selfRectTrans.anchoredPosition;
        anchorPos.y = anchorPosY;
        selfRectTrans.anchoredPosition = anchorPos;
        return selfRectTrans;
    }

    public static RectTransform SetSizeWidth(this RectTransform selfRectTrans, float sizeWidth)
    {
        var sizeDelta = selfRectTrans.sizeDelta;
        sizeDelta.x = sizeWidth;
        selfRectTrans.sizeDelta = sizeDelta;
        return selfRectTrans;
    }

    public static RectTransform SetSizeHeight(this RectTransform selfRectTrans, float sizeHeight)
    {
        var sizeDelta = selfRectTrans.sizeDelta;
        sizeDelta.y = sizeHeight;
        selfRectTrans.sizeDelta = sizeDelta;
        return selfRectTrans;
    }

    public static RectTransform SetLeft(this RectTransform selfRectTrans, float left)
    {
        selfRectTrans.offsetMin = new Vector2(left, selfRectTrans.offsetMin.y);
        return selfRectTrans;
    }

    public static RectTransform SetRight(this RectTransform selfRectTrans, float right)
    {
        selfRectTrans.offsetMax = new Vector2(-right, selfRectTrans.offsetMax.y);
        return selfRectTrans;
    }

    public static RectTransform SetTop(this RectTransform selfRectTrans, float top)
    {
        selfRectTrans.offsetMax = new Vector2(selfRectTrans.offsetMax.x, -top);
        return selfRectTrans;
    }

    public static RectTransform SetBottom(this RectTransform selfRectTrans, float bottom)
    {
        selfRectTrans.offsetMin = new Vector2(selfRectTrans.offsetMin.x, bottom);
        return selfRectTrans;
    }

    public static Vector2 GetWorldSize(this RectTransform selfRectTrans)
    {
        return RectTransformUtility.CalculateRelativeRectTransformBounds(selfRectTrans).size;
    }
}

public static class SelectableExtension
{
    public static T EnableInteract<T>(this T selfSelectable) where T : Selectable
    {
        selfSelectable.interactable = true;
        return selfSelectable;
    }

    public static T DisableInteract<T>(this T selfSelectable) where T : Selectable
    {
        selfSelectable.interactable = false;
        return selfSelectable;
    }

    public static T CancalAllTransitions<T>(this T selfSelectable) where T : Selectable
    {
        selfSelectable.transition = Selectable.Transition.None;
        return selfSelectable;
    }
}

public static class ToggleExtension
{
    public static void RegOnValueChangedEvent(this Toggle selfToggle, UnityAction<bool> onValueChangedEvent)
    {
        selfToggle.onValueChanged.AddListener(onValueChangedEvent);
    }
}

/// <summary>
/// Transform's Extension
/// </summary>
public static class TransformExtension
{
    public static void Example()
    {
        var selfScript = new GameObject().AddComponent<MonoBehaviour>();
        var transform = selfScript.transform;

        transform.Parent(null).LocalIdentity().LocalPositionIdentity().LocalRotationIdentity().LocalScaleIdentity()
            .LocalPosition(Vector3.zero).LocalPosition(0, 0, 0).LocalPosition(0, 0).LocalPositionX(0).LocalPositionY(0)
            .LocalPositionZ(0).LocalRotation(Quaternion.identity).LocalScale(Vector3.one).LocalScaleX(1.0f)
            .LocalScaleY(1.0f).Identity().PositionIdentity().RotationIdentity().Position(Vector3.zero).PositionX(0)
            .PositionY(0).PositionZ(0).Rotation(Quaternion.identity).DestroyAllChild().AsLastSibling().AsFirstSibling()
            .SiblingIndex(0);

        selfScript.Parent(null).LocalIdentity().LocalPositionIdentity().LocalRotationIdentity().LocalScaleIdentity()
            .LocalPosition(Vector3.zero).LocalPosition(0, 0, 0).LocalPosition(0, 0).LocalPositionX(0).LocalPositionY(0)
            .LocalPositionZ(0).LocalRotation(Quaternion.identity).LocalScale(Vector3.one).LocalScaleX(1.0f)
            .LocalScaleY(1.0f).Identity().PositionIdentity().RotationIdentity().Position(Vector3.zero).PositionX(0)
            .PositionY(0).PositionZ(0).Rotation(Quaternion.identity).DestroyAllChild().AsLastSibling().AsFirstSibling()
            .SiblingIndex(0);
    }

    /// <summary>
    /// 缓存的一些变量,免得每次声明
    /// </summary>
    private static Vector3 mLocalPos;

    private static Vector3 mScale;
    private static Vector3 mPos;

    #region CETR001 Parent

    public static T Parent<T>(this T selfComponent, Component parentComponent) where T : Component
    {
        selfComponent.transform.SetParent(parentComponent == null ? null : parentComponent.transform);
        return selfComponent;
    }

    public static Transform FindInAllChild(this Transform selfTransform, string name)
    {
        foreach (Transform child in selfTransform)
        {
            if (child.name == name)
                return child;
        }

        Transform result = null;
        foreach (Transform child in selfTransform)
        {
            if (result = child.FindInAllChild(name)) break;
        }

        return result;
    }

    public static Transform FindInAllChild(this Component selfComponent, string name)
    {
        return selfComponent.transform.FindInAllChild(name);
    }

    /// <summary>
    /// 设置成为顶端 Transform
    /// </summary>
    /// <returns>The root transform.</returns>
    /// <param name="selfComponent">Self component.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T AsRootTransform<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.SetParent(null);
        return selfComponent;
    }

    public static void SetParentIdentically(this Transform trans, Transform parent)
    {
        trans.SetParent(parent, false);
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    #endregion

    #region CETR002 LocalIdentity

    public static T LocalIdentity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.localPosition = Vector3.zero;
        selfComponent.transform.localRotation = Quaternion.identity;
        selfComponent.transform.localScale = Vector3.one;
        return selfComponent;
    }

    #endregion

    #region CETR003 LocalPosition

    public static T LocalPosition<T>(this T selfComponent, Vector3 localPos) where T : Component
    {
        selfComponent.transform.localPosition = localPos;
        return selfComponent;
    }

    public static Vector3 GetLocalPosition<T>(this T selfComponent) where T : Component
    {
        return selfComponent.transform.localPosition;
    }

    public static T LocalPosition<T>(this T selfComponent, float x, float y, float z) where T : Component
    {
        selfComponent.transform.localPosition = new Vector3(x, y, z);
        return selfComponent;
    }

    public static T LocalPosition<T>(this T selfComponent, float x, float y) where T : Component
    {
        mLocalPos = selfComponent.transform.localPosition;
        mLocalPos.x = x;
        mLocalPos.y = y;
        selfComponent.transform.localPosition = mLocalPos;
        return selfComponent;
    }

    public static T LocalPositionX<T>(this T selfComponent, float x) where T : Component
    {
        mLocalPos = selfComponent.transform.localPosition;
        mLocalPos.x = x;
        selfComponent.transform.localPosition = mLocalPos;
        return selfComponent;
    }

    public static T LocalPositionY<T>(this T selfComponent, float y) where T : Component
    {
        mLocalPos = selfComponent.transform.localPosition;
        mLocalPos.y = y;
        selfComponent.transform.localPosition = mLocalPos;
        return selfComponent;
    }

    public static T LocalPositionZ<T>(this T selfComponent, float z) where T : Component
    {
        mLocalPos = selfComponent.transform.localPosition;
        mLocalPos.z = z;
        selfComponent.transform.localPosition = mLocalPos;
        return selfComponent;
    }

    public static T LocalPositionIdentity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.localPosition = Vector3.zero;
        return selfComponent;
    }

    #endregion

    #region CETR004 LocalRotation

    public static Quaternion GetLocalRotation<T>(this T selfComponent) where T : Component
    {
        return selfComponent.transform.localRotation;
    }

    public static T LocalRotation<T>(this T selfComponent, Quaternion localRotation) where T : Component
    {
        selfComponent.transform.localRotation = localRotation;
        return selfComponent;
    }

    public static T LocalRotationIdentity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.localRotation = Quaternion.identity;
        return selfComponent;
    }

    #endregion

    #region CETR005 LocalScale

    public static T LocalScale<T>(this T selfComponent, Vector3 scale) where T : Component
    {
        selfComponent.transform.localScale = scale;
        return selfComponent;
    }

    public static Vector3 GetLocalScale<T>(this T selfComponent) where T : Component
    {
        return selfComponent.transform.localScale;
    }

    public static T LocalScale<T>(this T selfComponent, float xyz) where T : Component
    {
        selfComponent.transform.localScale = Vector3.one * xyz;
        return selfComponent;
    }

    public static T LocalScale<T>(this T selfComponent, float x, float y, float z) where T : Component
    {
        mScale = selfComponent.transform.localScale;
        mScale.x = x;
        mScale.y = y;
        mScale.z = z;
        selfComponent.transform.localScale = mScale;
        return selfComponent;
    }

    public static T LocalScale<T>(this T selfComponent, float x, float y) where T : Component
    {
        mScale = selfComponent.transform.localScale;
        mScale.x = x;
        mScale.y = y;
        selfComponent.transform.localScale = mScale;
        return selfComponent;
    }

    public static T LocalScaleX<T>(this T selfComponent, float x) where T : Component
    {
        mScale = selfComponent.transform.localScale;
        mScale.x = x;
        selfComponent.transform.localScale = mScale;
        return selfComponent;
    }

    public static T LocalScaleY<T>(this T selfComponent, float y) where T : Component
    {
        mScale = selfComponent.transform.localScale;
        mScale.y = y;
        selfComponent.transform.localScale = mScale;
        return selfComponent;
    }

    public static T LocalScaleZ<T>(this T selfComponent, float z) where T : Component
    {
        mScale = selfComponent.transform.localScale;
        mScale.z = z;
        selfComponent.transform.localScale = mScale;
        return selfComponent;
    }

    public static T LocalScaleIdentity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.localScale = Vector3.one;
        return selfComponent;
    }

    #endregion

    #region CETR006 Identity

    public static T Identity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.position = Vector3.zero;
        selfComponent.transform.rotation = Quaternion.identity;
        selfComponent.transform.localScale = Vector3.one;
        return selfComponent;
    }

    #endregion

    #region CETR007 Position

    public static T Position<T>(this T selfComponent, Vector3 position) where T : Component
    {
        selfComponent.transform.position = position;
        return selfComponent;
    }

    public static Vector3 GetPosition<T>(this T selfComponent) where T : Component
    {
        return selfComponent.transform.position;
    }

    public static T Position<T>(this T selfComponent, float x, float y, float z) where T : Component
    {
        selfComponent.transform.position = new Vector3(x, y, z);
        return selfComponent;
    }

    public static T Position<T>(this T selfComponent, float x, float y) where T : Component
    {
        mPos = selfComponent.transform.position;
        mPos.x = x;
        mPos.y = y;
        selfComponent.transform.position = mPos;
        return selfComponent;
    }

    public static T PositionIdentity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.position = Vector3.zero;
        return selfComponent;
    }

    public static T PositionX<T>(this T selfComponent, float x) where T : Component
    {
        mPos = selfComponent.transform.position;
        mPos.x = x;
        selfComponent.transform.position = mPos;
        return selfComponent;
    }

    public static T PositionX<T>(this T selfComponent, Func<float, float> xSetter) where T : Component
    {
        mPos = selfComponent.transform.position;
        mPos.x = xSetter(mPos.x);
        selfComponent.transform.position = mPos;
        return selfComponent;
    }

    public static T PositionY<T>(this T selfComponent, float y) where T : Component
    {
        mPos = selfComponent.transform.position;
        mPos.y = y;
        selfComponent.transform.position = mPos;
        return selfComponent;
    }

    public static T PositionY<T>(this T selfComponent, Func<float, float> ySetter) where T : Component
    {
        mPos = selfComponent.transform.position;
        mPos.y = ySetter(mPos.y);
        selfComponent.transform.position = mPos;
        return selfComponent;
    }

    public static T PositionZ<T>(this T selfComponent, float z) where T : Component
    {
        mPos = selfComponent.transform.position;
        mPos.z = z;
        selfComponent.transform.position = mPos;
        return selfComponent;
    }

    public static T PositionZ<T>(this T selfComponent, Func<float, float> zSetter) where T : Component
    {
        mPos = selfComponent.transform.position;
        mPos.z = zSetter(mPos.z);
        selfComponent.transform.position = mPos;
        return selfComponent;
    }

    #endregion

    #region CETR008 Rotation

    public static T RotationIdentity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.rotation = Quaternion.identity;
        return selfComponent;
    }

    public static T Rotation<T>(this T selfComponent, Quaternion rotation) where T : Component
    {
        selfComponent.transform.rotation = rotation;
        return selfComponent;
    }

    public static Quaternion GetRotation<T>(this T selfComponent) where T : Component
    {
        return selfComponent.transform.rotation;
    }

    #endregion

    #region CETR009 WorldScale/LossyScale/GlobalScale/Scale

    public static Vector3 GetGlobalScale<T>(this T selfComponent) where T : Component
    {
        return selfComponent.transform.lossyScale;
    }

    public static Vector3 GetScale<T>(this T selfComponent) where T : Component
    {
        return selfComponent.transform.lossyScale;
    }

    public static Vector3 GetWorldScale<T>(this T selfComponent) where T : Component
    {
        return selfComponent.transform.lossyScale;
    }

    public static Vector3 GetLossyScale<T>(this T selfComponent) where T : Component
    {
        return selfComponent.transform.lossyScale;
    }

    #endregion

    #region CETR010 Destroy All Child

    public static T DestroyAllChild<T>(this T selfComponent) where T : Component
    {
        var childCount = selfComponent.transform.childCount;

        for (var i = 0; i < childCount; i++)
        {
            selfComponent.transform.GetChild(i).DestroyGameObjGracefully();
        }

        return selfComponent;
    }

    public static GameObject DestroyAllChild(this GameObject selfGameObj)
    {
        var childCount = selfGameObj.transform.childCount;

        for (var i = 0; i < childCount; i++)
        {
            selfGameObj.transform.GetChild(i).DestroyGameObjGracefully();
        }

        return selfGameObj;
    }

    #endregion

    #region CETR011 Sibling Index

    public static T AsLastSibling<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.SetAsLastSibling();
        return selfComponent;
    }

    public static T AsFirstSibling<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.SetAsFirstSibling();
        return selfComponent;
    }

    public static T SiblingIndex<T>(this T selfComponent, int index) where T : Component
    {
        selfComponent.transform.SetSiblingIndex(index);
        return selfComponent;
    }

    #endregion

    public static Transform FindByPath(this Transform selfTrans, string path)
    {
        return selfTrans.Find(path.Replace(".", "/"));
    }

    public static Transform SeekTrans(this Transform selfTransform, string uniqueName)
    {
        var childTrans = selfTransform.Find(uniqueName);

        if (null != childTrans)
            return childTrans;

        foreach (Transform trans in selfTransform)
        {
            childTrans = trans.SeekTrans(uniqueName);

            if (null != childTrans)
                return childTrans;
        }

        return null;
    }

    public static T ShowChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
    {
        selfComponent.transform.Find(tranformPath).gameObject.ActiveShow();
        return selfComponent;
    }

    public static T HideChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
    {
        selfComponent.transform.Find(tranformPath).ActiveHide();
        return selfComponent;
    }

    public static void CopyDataFromTrans(this Transform selfTrans, Transform fromTrans)
    {
        selfTrans.SetParent(fromTrans.parent);
        selfTrans.localPosition = fromTrans.localPosition;
        selfTrans.localRotation = fromTrans.localRotation;
        selfTrans.localScale = fromTrans.localScale;
    }

    /// <summary>
    /// 递归遍历子物体，并调用函数
    /// </summary>
    /// <param name="tfParent"></param>
    /// <param name="action"></param>
    public static void ActionRecursion(this Transform tfParent, Action<Transform> action)
    {
        action(tfParent);
        foreach (Transform tfChild in tfParent)
        {
            tfChild.ActionRecursion(action);
        }
    }

    /// <summary>
    /// 递归获取所有子物体
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="result"></param>
    public static void GetAllChildRecursion(this Transform parent, out List<Transform> result)
    {
        result = new List<Transform>();
        foreach (Transform child in parent)
        {
            result.Add(child);
            GetAllChildRecursion(child, out var childChild);
            result.AddRange(childChild);
        }
    }
    
    /// <summary>
    /// 递归遍历查找指定的名字的子物体
    /// </summary>
    /// <param name="tfParent">当前Transform</param>
    /// <param name="name">目标名</param>
    /// <param name="stringComparison">字符串比较规则</param>
    /// <returns></returns>
    public static Transform FindChildRecursion(this Transform tfParent, string name,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        if (tfParent.name.Equals(name, stringComparison))
        {
            //Debug.Log("Hit " + tfParent.name);
            return tfParent;
        }

        foreach (Transform tfChild in tfParent)
        {
            Transform tfFinal = null;
            tfFinal = tfChild.FindChildRecursion(name, stringComparison);
            if (tfFinal)
            {
                return tfFinal;
            }
        }

        return null;
    }

    /// <summary>
    /// 递归遍历查找相应条件的子物体
    /// </summary>
    /// <param name="tfParent">当前Transform</param>
    /// <param name="predicate">条件</param>
    /// <returns></returns>
    public static Transform FindChildRecursion(this Transform tfParent, Func<Transform, bool> predicate)
    {
        if (predicate(tfParent))
        {
            Debug.Log("Hit " + tfParent.name);
            return tfParent;
        }

        foreach (Transform tfChild in tfParent)
        {
            Transform tfFinal = null;
            tfFinal = tfChild.FindChildRecursion(predicate);
            if (tfFinal)
            {
                return tfFinal;
            }
        }

        return null;
    }

    public static string GetPathFromRoot(this Transform transform)
    {
        var sb = new System.Text.StringBuilder();
        var t = transform;
        while (true)
        {
            sb.Insert(0, t.name);
            t = t.parent;
            if (t)
            {
                sb.Insert(0, "/");
            }
            else
            {
                return sb.ToString();
            }
        }
    }

    public static string GetPathFromTarget(this Transform transform, Transform root)
    {
        var sb = new System.Text.StringBuilder();
        var t = transform;
        while (true)
        {
            sb.Insert(0, t.name);
            t = t.parent;
            if (t == root) return sb.ToString();
            if (t)
            {
                sb.Insert(0, "/");
            }
            else
            {
                return sb.ToString();
            }
        }
    }

    public static Transform GetRootParent(this Transform transform)
    {
        if (transform.parent == null)
            return transform;
        return GetRootParent(transform.parent);
    }
}

public static class UnityActionExtension
{
    public static void AddOnlyOneListener(this UnityEvent self, UnityAction action)
    {
        self.RemoveAllListeners();
        self.AddListener(action);
    }
}

public static class Vector3Extension
{
    public static Vector3 GetRandomVector3(this Vector3 self, float floatRang)
    {
        return new Vector3(
            UnityEngine.Random.Range(self.x - floatRang, self.x + floatRang),
            UnityEngine.Random.Range(self.y - floatRang, self.y + floatRang),
            UnityEngine.Random.Range(self.z - floatRang, self.z + floatRang));
    }

    public static Vector3 SetX(this Vector3 self, float xValue)
    {
        return new Vector3(xValue, self.y, self.z);
    }
    
    public static float SqrDistance(this Vector3 self, Vector3 other)
    {
        float num1 = self.x - other.x;
        float num2 = self.y - other.y;
        float num3 = self.z - other.z;
        return (float) (num1 * (double) num1 + num2 * (double) num2 + num3 * (double) num3);
    }

    public static Vector3 SetY(this Vector3 self, float yValue)
    {
        return new Vector3(self.x, yValue, self.z);
    }

    public static Vector3 SetZ(this Vector3 self, float zValue)
    {
        return new Vector3(self.x, self.y, zValue);
    }
}