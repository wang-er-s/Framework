using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Editor
{
    [ShowOdinSerializedPropertiesInInspector]
    public class EditorConfig : ConfigBase 
    {
        [FoldoutGroup("打包设置",expanded: false)]
        [BoxGroup("打包设置/Android设置")]
        [HideLabel]
        [InlinePropertyAttribute]
        public AndroidConfig Android = new AndroidConfig();

        [BoxGroup("打包设置/Ios设置")]
        [HideLabel]
        [InlinePropertyAttribute]
        public IosConfig IosConfig =new IosConfig();
        
        [FoldoutGroup("UI设置", expanded: false)]
        [FolderPath(AbsolutePath = false)]
        public string GenUIScriptsPath;
    }
    
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


    public class IosConfig
    {
        [LabelText("证书")]
        public string Liscene;
        [LabelText("密钥")]
        public string Keystore;
    }
}