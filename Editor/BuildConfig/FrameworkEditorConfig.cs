using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Editor
{
    [ShowOdinSerializedPropertiesInInspector]
    public class FrameworkEditorConfig : ConfigBase 
    {
        [FoldoutGroup("打包设置",expanded: true)]
        [BoxGroup("打包设置/Android设置")]
        [HideLabel]
        [InlinePropertyAttribute]
        public AndroidConfig Android = new AndroidConfig();

        [BoxGroup("打包设置/Ios设置")]
        [HideLabel]
        [InlinePropertyAttribute]
        public IosConfig IosConfig =new IosConfig();

        [FoldoutGroup("UI设置", expanded: true)]
        [HideLabel]
        [InlinePropertyAttribute]
        public UIConfig UIConfig = new UIConfig();
    }
    
    [Serializable]
    public class AndroidConfig
    {
        [LabelText("Keystore路径")]
        [FilePath(Extensions = "keystore", AbsolutePath = false)]
        public  string KeystoreName = "../key.keystore";
        [LabelText("Keystore密码")]
        public  string KeystorePwd = "";
        [LabelText("keyalias")]
        public  string KeyAliasName;
        [LabelText("keyalias密码")]
        public  string KeyAliasPwd;
    }

    [Serializable]
    public class IosConfig
    {
        [LabelText("证书")]
        public string Liscene;
        [LabelText("密钥")]
        public string Keystore;
    }
    
    [Serializable]
    public class UIConfig
    {
        [FolderPath(AbsolutePath = false)]
        [LabelText("自动生成UI代码的路径")]
        public string GenUIScriptsPath;
    }
}