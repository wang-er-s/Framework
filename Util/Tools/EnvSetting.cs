using System;
using System.Collections;
using System.Reflection;
using Framework.BaseUtil;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework
{
    [CreateAssetMenu(menuName = "Game/Framework/EnvSetting",fileName = EnvSetting.ASSET_NAME)]
    public class EnvSetting : ScriptableObject
    {
        public const string ASSET_NAME = "Assets/EnvSetting";
        public bool isDev;//开发模式
        public bool useBundleInEditor;
        public string editorResPath;
        public bool useTrdBundlePath;//第三方bundle路径，用于调试，仅编辑器生效
        public string trdBundlePath;//第三方bundle路径，用于调试，仅编辑器生效
        public bool useOriginalData;
        public bool useStreamingAssetsInEditor;
        [LabelTextAttribute("Log模式")]
        public bool resVerbose;

        public EnvSetting LoadFromJson(string json)
        {
            var data = JsonMapper.ToObject(json);
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                if (data.HasField(field.Name))
                {
                    if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(this, data[field.Name].GetJsonBool());
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        field.SetValue(this, data[field.Name].GetJsonInt());
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(this, data[field.Name].GetJsonFloat());
                    }
                    else if (field.FieldType == typeof(double))
                    {
                        field.SetValue(this, data[field.Name].GetJsonDouble());
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(this, data.GetJsonStrField(field.Name));
                    }
                }
            }

            return this;
        }

        public static EnvSetting FromJson(string json)
        {
            var env = CreateInstance<EnvSetting>();
            return env.LoadFromJson(json);
        }
    }
}