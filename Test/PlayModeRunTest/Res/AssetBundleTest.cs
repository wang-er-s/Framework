using Framework;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class 测试AssetBundle
    {
        [Test]
        public void 测试编辑模式加载()
        {
            //AppEnv.Init();
            SingleAssetObj<GameObject> obj = new SingleAssetObj<GameObject>();
            obj.LoadSync("");
        }

        [Test]
        public void 测试bundle模式加载()
        {
            
        }
    }
}