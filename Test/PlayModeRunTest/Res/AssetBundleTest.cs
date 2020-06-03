using Framework;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class 测试AssetBundle
    {
        [Test]
        public void 测试编辑模式加载()
        {
            var set = AssetDatabase.LoadAssetAtPath<EnvSetting>("Assets/Setting/EnvSetting.asset");
            set.useBundleInEditor = false;
            AppEnv.Init(set);
            BundleMgr.Instance.ResetDep(true);
            var path = @"Assets/SoUtil/Test/PlayModeRunTest/Res/EditorRes/Sphere.prefab";
            SingleAssetObj<GameObject> obj = new SingleAssetObj<GameObject>();
            obj.Load(path, () => Debug.Log(obj.asset));
        }

        [Test]
        public void 测试bundle模式加载()
        {
            var set = AssetDatabase.LoadAssetAtPath<EnvSetting>("Assets/Setting/EnvSetting.asset");
            AppEnv.Init(set);
            BundleMgr.Instance.ResetDep(true);
            var path = @"Assets/SoUtil/Test/PlayModeRunTest/Res/EditorRes/Sphere.prefab";
            SingleAssetObj<GameObject> obj = new SingleAssetObj<GameObject>();
            obj.Load(path, () => Debug.Log(obj.asset));
        }
    }
}