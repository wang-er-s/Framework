using System;
using Framework;
using Framework.Prefs;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class 数据持久化
    {
        [Test]
        public void 测试存储GlobalPreferences()
        {
            Preferences.Factory = new BinaryFilePreferencesFactory();

            //lobalPreferences
            var globalPreferences = Preferences.GetGlobalPreferences();
            globalPreferences.SetInt("Level", 1);
            globalPreferences.SetObject("DATA_VERSION", new Version(1, 1));
            globalPreferences.Save();
            
            Preferences.Factory = new PlayerPrefsPreferencesFactory();
            globalPreferences = Preferences.GetGlobalPreferences();
            Assert.AreEqual(globalPreferences.GetInt("Level"), 1);
            
            //创建自己的preference
            var playerPreferences = Preferences.GetPreferences("soso");
            playerPreferences.SetInt("Level", 12);
            playerPreferences.Save();
            Assert.AreEqual(playerPreferences.GetInt("Level"), 12);
            
            //自定义TypeEncode
            var binFactory = new BinaryFilePreferencesFactory();
            binFactory.Serializer.AddTypeEncoder(new SelfTypeEncoder());
            Preferences.Factory = binFactory;
        }

        class SelfTypeEncoder : ITypeEncoder
        {
            //更改优先级
            public int Priority { get; set; } = 1;

            public bool IsSupport(Type type)
            {
                if (type == typeof(MyType))
                    return true;
                return false;
            }

            public string Encode(object value)
            {
                return value.ToString();
            }

            public object Decode(Type type, string value)
            {
                return Activator.CreateInstance(type, value);
            }
        }

        class MyType
        {
            private string name;
            
            private MyType(string name)
            {
                this.name = name;
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}