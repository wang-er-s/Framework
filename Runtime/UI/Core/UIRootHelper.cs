using UnityEngine;

namespace Framework
{
    public class UIRootHelper
    {
        public static Transform GetTargetRoot(Scene root, UILevel level)
        {
            switch (level)
            {
                case UILevel.Bg:
                    return root.GetComponent<GlobalReferenceComponent>().BgUIRoot;
                case UILevel.Common:
                    return root.GetComponent<GlobalReferenceComponent>().CommonUIRoot;
                case UILevel.Pop:
                    return root.GetComponent<GlobalReferenceComponent>().PopUIRoot;
                case UILevel.Guide:
                    return root.GetComponent<GlobalReferenceComponent>().GuideUIRoot;
                case UILevel.FullScreen:
                    return root.GetComponent<GlobalReferenceComponent>().FullScreenUIRoot;
                default:
                    Log.Error("uiroot type is error: " + level.ToString());
                    return null;
            }
        }
    }
}