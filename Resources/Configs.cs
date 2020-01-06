using System;
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
        var config = Resources.Load<Settings>("ResConfig");
        Init(config);
    }

    public static void Init(Settings config)
    {
        switch (config.LoadResourcesStyle)
        {
            case Settings.LoadResStyle.LoadAsset:
                IsEditorLoadAsset = true;
                IsLoadBundle = false;
                IsUseResources = false;
                break;
            case Settings.LoadResStyle.AssetBundle:
                IsEditorLoadAsset = false;
                IsLoadBundle = true;
                IsUseResources = false;
                break;
            case Settings.LoadResStyle.Resources:
                IsEditorLoadAsset = false;
                IsLoadBundle = false;
                IsUseResources = true;
                break;
        }
        IsEditor = config.IsEditor;
        ResourcePathPriorityType = config.ResourcePathPriorityType;
    }

    public const string AssetBundleExt = ".ab";
    public const string EditorResourcesDir  = "Assets/BundleResources";
    public const string EditorAssetBundleDir  = "ResPackage";
    public const string BundlesDirName  = "Bundles";
    public const string AssetsManifestPath = "Manifest";
    public const string AssetsSettingPath = "Setting";
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
[CreateAssetMenu(fileName = "Setting", menuName = "ScriptableObjects/Settings", order = 1)]
public class Settings : ScriptableObject
{

    public Dictionary<string, string> BundleAssets;
    
    public enum LoadResStyle
    {
        LoadAsset,
        AssetBundle,
        Resources,
    }

    private bool isEditor = true;

    [ShowInInspector]
    [LabelText("编辑模式")]
    public bool IsEditor
    {
        get { return isEditor; }
        set
        {
            isEditor = value;
            if (!isEditor && LoadResourcesStyle == LoadResStyle.LoadAsset)
            {
                LoadResourcesStyle = LoadResStyle.AssetBundle;
            }
        }
    }

    [ShowInInspector]
    [InfoBox("勾选IsEditor后，会从Project/ResPackage/Bundles里面加载AB",InfoMessageType.Warning,VisibleIf = "ShowOrHide")]
    [EnumPaging]
    public LoadResStyle LoadResourcesStyle = LoadResStyle.LoadAsset;

    [EnumPaging]
    [LabelText("资源加载优先级")]
    public KResourcePathPriorityType ResourcePathPriorityType =
        KResourcePathPriorityType.PersistentDataPathPriority;

    private bool ShowOrHide()
    {
        return LoadResourcesStyle == LoadResStyle.AssetBundle;
    }
    
}
