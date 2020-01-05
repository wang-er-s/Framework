using System.Collections;
using System.Collections.Generic;
using AD;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public static class Configs
{
    static Configs()
    {
        var config = Resources.Load<ResConfig>("ResConfig");
        Init(config);
    }

    public static void Init(ResConfig config)
    {
        IsEditor = config.IsEditor;
        IsLoadBundle = config.IsLoadBundle;
        IsEditorLoadAsset = config.IsEditorLoadAsset;
        IsUseResources = config.IsUseResources;
        ResourcePathPriorityType = config.ResourcePathPriorityType;
    }

    public static string AssetBundleExt { get; private set; } = ".ab";
    public static string EditorResourcesDir { get; private set; } = "BundleResources";
    public static string BundlesDirName { get; private set; } = "Bundles";
    public static bool IsEditor { get; private set; } = true;
    public static bool IsLoadBundle { get; private set; } = false;
    public static bool IsUseResources { get; private set; } = true;
    public static bool ApplicationQuite { get; private set; } = false;
    public static bool IsEditorLoadAsset { get; private set; } = false;
    /// <summary>
    /// 是否優先找下載的資源?還是app本身資源優先. 优先下载的资源，即采用热更新的资源
    /// </summary>
    public static KResourcePathPriorityType ResourcePathPriorityType =
        KResourcePathPriorityType.PersistentDataPathPriority;
}

[ShowOdinSerializedPropertiesInInspector]
public class ResConfig : ScriptableObject
{
    public ResConfig(bool isEditor = true,bool isEditorLoadAsset = false,bool isLoadBundle = false, bool isUseResources = true)
    {
        IsEditor = isEditor;
        IsEditorLoadAsset = isEditorLoadAsset;
        IsLoadBundle = isLoadBundle;
        IsUseResources = isUseResources;
    }


    private bool isEditor = false;

    [ShowInInspector]
    [LabelText("编辑模式")]
    public bool IsEditor
    {
        get { return isEditor; }
        set
        {
            isEditor = value;
            if (!isEditor)
            {
                IsEditorLoadAsset = false;
            }
        }
    }

    private bool isEditorLoadAsset = false;

    [ShowInInspector]
    [DisableIf("@IsEditor==false")]
    [LabelText("使用LoadAssetAtPath加载")]
    public bool IsEditorLoadAsset
    {
        get { return isEditorLoadAsset; }
        set { isEditorLoadAsset = value; }
    }

    private bool isLoadBundle = false;

    [ShowInInspector]
    [InfoBox("勾选IsEditor后，会从Project/ResPackage/Bundles里面加载AB",InfoMessageType.Warning,VisibleIf = "IsLoadBundle")]
    [LabelText("使用AssetBundle")]
    public bool IsLoadBundle
    {
        get { return isLoadBundle; }
        set { isLoadBundle = value; }
    }

    private bool isUseResources = false;

    [ShowInInspector]
    [LabelText("使用Resources")]
    public bool IsUseResources
    {
        get { return isUseResources; }
        set { isUseResources = value; }
    }

    [EnumPaging]
    [LabelText("资源加载优先级")]
    public KResourcePathPriorityType ResourcePathPriorityType =
        KResourcePathPriorityType.PersistentDataPathPriority;

}
