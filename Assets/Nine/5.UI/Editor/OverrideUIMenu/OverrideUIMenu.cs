using System;
using Nine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

#pragma warning disable
public class OverrideUIMenu
{
    [MenuItem ( "GameObject/UI/@@ Text" )]
    private static void CreateText ()
    {
        Text text = CreateDefaultUI<Text> ();
        ChangeComponent<Text, DText> ( text );
    }

    [MenuItem ( "GameObject/UI/@@ Image" )]
    private static void CreateImage ()
    {
        Image image = CreateDefaultUI<Image> ();
        ChangeComponent<Image, DImage> ( image );
    }

    [MenuItem ( "GameObject/UI/@@ Raw Image" )]
    private static void CreateRawImage ()
    {
        RawImage img = CreateDefaultUI<RawImage> ( "Raw Image" );
        ChangeComponent<RawImage, DRawImage> ( img );
    }

    [MenuItem ( "GameObject/UI/@@ Button" )]
    private static void CreateButton ()
    {
        Button button = CreateDefaultUI<Button> ();
        DImage dImage = ChangeComponent<Image, DImage> ( button.GetComponent<Image> () );
        DButton dButton = ChangeComponent<Button, DButton> ( button );
        dButton.targetGraphic = dImage;
        DText dText = ChangeComponent<Text, DText> ( dButton.GetComponentInChildren<Text> () );
    }

    [MenuItem ( "GameObject/UI/@@ Toggle" )]
    private static void CreateToggle ()
    {
        Toggle toggle = CreateDefaultUI<Toggle> ();
        DText dText = ChangeComponent<Text, DText> ( toggle.GetComponentInChildren<Text> () );
        DImage checkmark = ChangeComponent<Image, DImage> (
            toggle.transform.GetChild ( 0 ).Find ( "Checkmark" ).GetComponent<Image> () );
        DImage bg = ChangeComponent<Image, DImage> ( toggle.transform.Find ( "Background" ).GetComponent<Image> () );
        DToggle dToggle = ChangeComponent<Toggle, DToggle> ( toggle );
        dToggle.targetGraphic = bg;
        dToggle.graphic       = checkmark;
    }

    [MenuItem ( "GameObject/UI/@@ Slider" )]
    private static void CreateSlider ()
    {
        Slider slider = CreateDefaultUI<Slider> ();
        DSlider dSlider = ChangeComponent<Slider, DSlider> ( slider );
        DImage handle = ChangeComponent<Image, DImage> ( dSlider.FindInAllChild ( "Handle" ).GetComponent<Image> () );
        DImage fill = ChangeComponent<Image, DImage> ( dSlider.FindInAllChild ( "Fill" ).GetComponent<Image> () );
        ChangeComponent<Image, DImage> ( dSlider.FindInAllChild ( "Background" ).GetComponent<Image> () );
        dSlider.targetGraphic = handle;
        dSlider.fillRect = fill.rectTransform;
        dSlider.handleRect = handle.rectTransform;
    }

    private static void SetTransform ( Transform transform )
    {
        Transform activeTransform = Selection.activeTransform;
        GameObject canvasObj = SecurityCheck ();
        if ( !activeTransform ) // 在根文件夹创建的， 自己主动移动到 Canvas下
        {
            transform.SetParent ( canvasObj.transform, false );
            transform.gameObject.layer = canvasObj.layer;
        }
        else
        {
            if ( !activeTransform.GetComponentInParent<Canvas> () ) // 没有在UI树下
            {
                transform.SetParent ( canvasObj.transform, false );
                transform.gameObject.layer = canvasObj.layer;
            }
            else
            {
                transform.SetParent ( activeTransform, false );
                transform.gameObject.layer = activeTransform.gameObject.layer;
            }
        }
        Selection.activeObject = transform;
    }

    private static TResult ChangeComponent<TSource, TResult> ( TSource source ) where TResult : Component
                                                                                where TSource : Component
    {
        GameObject go = source.gameObject;
        List<object> fieldValues;
        List<object> propertyValues;
        GetComponentValues ( source, out fieldValues, out propertyValues );
        Object.DestroyImmediate ( source );
        TResult result = go.AddComponent<TResult> ();
        SetComponetValues ( result, fieldValues, propertyValues );
        return result;
        ;
    }

    private static void GetComponentValues<T> ( T component, out List<object> fieldValues,
                                               out List<object> propertyValues ) where T : Component
    {
        var fields = typeof ( T ).GetFields ();
        var propertys = typeof ( T ).GetProperties ();
        fieldValues    = new List<object> ();
        propertyValues = new List<object> ();
        foreach ( FieldInfo field in fields )
        {
            fieldValues.Add ( field.GetValue ( component ) );
        }
        foreach ( PropertyInfo property in propertys )
        {
            if ( !property.CanWrite ) continue;
            propertyValues.Add ( property.GetValue ( component ) );
        }
    }

    private static void SetComponetValues<T> ( T component, List<object> fieldValues, List<object> propertyValues )
        where T : Component
    {
        var fields = typeof ( T ).GetFields ();
        var propertys = typeof ( T ).GetProperties ();
        for ( int i = 0; i < fields.Length; i++ )
        {
            fields[ i ].SetValue ( component, fieldValues[ i ] );
        }
        int index = 0;
        foreach ( PropertyInfo property in propertys )
        {
            if ( !property.CanWrite ) continue;
            property.SetValue ( component, propertyValues[ index ] );
            index++;
        }
    }

    private static DImage ChangeImage ( Image img )
    {
        GameObject go = img.gameObject;
        Sprite sprite = img.sprite;
        Image.Type type = img.type;
        bool fillCenter = img.fillCenter;
        Object.DestroyImmediate ( img );
        DImage dImage = go.AddComponent<DImage> ();
        dImage.sprite     = sprite;
        dImage.type       = type;
        dImage.fillCenter = fillCenter;
        return dImage;
    }

    private static Sprite CreateBuildInSprite ( string name )
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite> ( $"UI/Skin/{name}.psd" );
    }

    private static T CreateDefaultUI<T> ( string menuItem = "" ) where T : Component
    {
        string item = string.IsNullOrEmpty ( menuItem ) ? typeof ( T ).Name : menuItem;
        EditorApplication.ExecuteMenuItem ( $"GameObject/UI/{item}" );
        return Selection.activeGameObject.GetComponent<T> ();
    }

    // 假设第一次创建UI元素 可能没有 Canvas、EventSystem对象！
    private static GameObject SecurityCheck ()
    {
        Canvas cv = Object.FindObjectOfType<Canvas> ();
        GameObject canvas;
        if ( !cv )
        {
            canvas = new GameObject ( "Canvas", typeof ( Canvas ) );
            Undo.RegisterCreatedObjectUndo ( canvas, "Canvas" );
        }
        else
        {
            canvas = cv.gameObject;
        }

        if ( !Object.FindObjectOfType<EventSystem> () )
        {
            GameObject go = new GameObject ( "EventSystem", typeof ( EventSystem ) );
            Undo.RegisterCreatedObjectUndo ( go, "EventSystem" );
        }

        canvas.layer = LayerMask.NameToLayer ( "UI" );
        return canvas;
    }

    private static Color DefaultColor = new Color ( 50f / 255, 50f / 255, 50f / 255 );
    private const string CHECKMARK = "Checkmark";
    private const string UISPRITE = "UISprite";
}