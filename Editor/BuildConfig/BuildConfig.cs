using System.IO;
using Framework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Editor
{
    [ShowOdinSerializedPropertiesInInspector]
    public class BuildConfig : ConfigBase 
    {

        [BoxGroup("Android设置")]
        [HideLabel]
        [InlinePropertyAttribute]
        public AndroidConfig Android = new AndroidConfig();
        
        [BoxGroup("Ios设置")]
        [HideLabel]
        [InlinePropertyAttribute]
        public IosConfig IosConfig =new IosConfig();
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