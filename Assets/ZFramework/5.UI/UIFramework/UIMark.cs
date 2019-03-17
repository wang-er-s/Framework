using UnityEngine;
using UnityEngine.UI;

namespace ZFramework
{

    public enum UIMarkType
    {
        DefaultUnityElement,
        Element,
        Component
    }
    
    /// <inheritdoc />
    /// <summary>
    /// UI的标记
    /// </summary>
    public class UIMark : MonoBehaviour
    {
        public string CustomComponentName;

        public virtual string ComponentName
        {
            get
            {
                if ( !string.IsNullOrEmpty ( CustomComponentName ) )
                    return CustomComponentName;
                if ( null != GetComponent<ScrollRect> () )
                    return "ScrollRect";
                if ( null != GetComponent<InputField> () )
                    return "InputField";
                if ( null != GetComponent<Button> () )
                    return "Button";
                if ( null != GetComponent<Text> () )
                    return "Text";
                if ( null != GetComponent<RawImage> () )
                    return "RawImage";
                if ( null != GetComponent<Toggle> () )
                    return "Toggle";
                if ( null != GetComponent<Slider> () )
                    return "Slider";
                if ( null != GetComponent<Scrollbar> () )
                    return "Scrollbar";
                if ( null != GetComponent<Image> () )
                    return "Image";
                if ( null != GetComponent<ToggleGroup> () )
                    return "ToggleGroup";
                if ( null != GetComponent<Animator> () )
                    return "Animator";
                if ( null != GetComponent<Canvas> () )
                    return "Canvas";
                if ( null != GetComponent ( "Empty4Raycast" ) )
                    return "Empty4Raycast";
                if ( null != GetComponent<RectTransform> () )
                    return "RectTransform";

                return "Transform";
            }
        }

        public Transform Transform { get { return transform; } }

        public UIMarkType GetUIMarkType ()
        {
            return UIMarkType.DefaultUnityElement;
        }
    }
}