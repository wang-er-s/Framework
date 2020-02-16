using NUnit.Framework;
using Plugins.XAsset;
using Plugins.XAsset.Editor;
using UnityEngine;

namespace Tests
{
    public class 测试AssetBundle
    {
        [Test]
        public void 测试编辑模式加载()
        {
            var setting = BuildScript.GetSettings();
            setting.runtimeMode = false;
            var obj = Assets.Load("Assets/Res/Prefabs/UI/UI_Main", typeof(Object));
            Assert.NotNull(obj);

            obj = Assets.Load("img/Shape 1.png", typeof(Object));
            Assert.NotNull(obj);
        }

        [Test]
        public void 测试bundle模式加载()
        {
            var setting = BuildScript.GetSettings();
            setting.runtimeMode = true;
            var obj = Assets.Load("Assets/Res/Prefabs/UI/UI_Main", typeof(Object));
            Assert.NotNull(obj);

            obj = Assets.Load("img/Shape 1.png", typeof(Object));
            Assert.NotNull(obj);
        }
    }
}