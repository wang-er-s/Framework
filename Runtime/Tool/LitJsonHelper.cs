using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using UnityEngine;

namespace Framework.Helper
{
    public static class LitJsonHelper
    {
        public static string GetMD5(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return GetMD5(bytes);
        }
        
        public static void WriteProperty(this JsonWriter w,string name,long value){
            w.WritePropertyName(name);
            w.Write(value);
        }

        public static void WriteProperty(this JsonWriter w,string name,string value){
            w.WritePropertyName(name);
            w.Write(value);
        }

        public static void WriteProperty(this JsonWriter w,string name,bool value){
            w.WritePropertyName(name);
            w.Write(value);
        }

        public static void WriteProperty(this JsonWriter w,string name,double value){
            w.WritePropertyName(name);
            w.Write(value);
        }

        public static string GetMD5(byte[] bytes)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] retBytes = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", retBytes[i]);
            }

            return sb.ToString();
        }

        public static string GetBase64(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        #region Json Help

        public static string GetJsonStrField(this JsonData data, string field)
        {
            if (data.HasField(field))
            {
                IJsonWrapper strData = (IJsonWrapper) data[field];
                return strData.GetString();
            }

            return null;
        }

        public static string GetJsonString(this JsonData data)
        {
            return ((IJsonWrapper) data).GetString();
        }

        public static void SetJsonString(this JsonData data, string val)
        {
            ((IJsonWrapper) data).SetString(val);
        }

        public static int GetJsonInt(this JsonData data)
        {
            return ((IJsonWrapper) data).GetInt();
        }

        public static void SetJsonInt(this JsonData data, int val)
        {
            ((IJsonWrapper) data).SetInt(val);
        }

        public static bool GetJsonBool(this JsonData data)
        {
            return ((IJsonWrapper) data).GetBoolean();
        }

        public static void SetJsonBool(this JsonData data, bool val)
        {
            ((IJsonWrapper) data).SetBoolean(val);
        }

        public static long GetJsonLong(this JsonData data)
        {
            return ((IJsonWrapper) data).GetLong();
        }

        public static double GetJsonDouble(this JsonData data)
        {
            IJsonWrapper id = data;
            if (id.IsInt)
                return id.GetInt();
            else if (id.IsLong)
                return id.GetLong();
            return id.GetDouble();
        }

        public static float GetJsonFloat(this JsonData data)
        {
            return Convert.ToSingle(GetJsonDouble(data));
        }

        public static void SetJsonFloat(this JsonData data, float val)
        {
            ((IJsonWrapper) data).SetDouble(val);
        }

        public static bool HasField(this JsonData data, string key)
        {
            return ((IDictionary) data).Contains(key);
        }

        public static void AddField(this JsonData data, string key, object val)
        {
            ((IDictionary) data).Add(key, val);
        }

        public static void RemoveField(this JsonData data, string key)
        {
            ((IDictionary) data).Remove(key);
        }

        public static string ToFormatJson(this JsonData data)
        {
            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonWriter(sw);
            writer.Validate = false;
            writer.PrettyPrint = true;
            data.ToJson(writer);
            return sw.ToString();
        }

        #endregion

        #region RegisterUnityType
        
        static bool registerd;

        public static void Register()
        {

            if (registerd) return;
            registerd = true;


            // 注册Type类型的Exporter
            JsonMapper.RegisterExporter<Type>((v, w) =>
            {
                w.Write(v.FullName);
            });

            JsonMapper.RegisterImporter<string, Type>((s) =>
            {
                return Type.GetType(s);
            });

            // 注册Vector2类型的Exporter
            Action<Vector2, JsonWriter> writeVector2 = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteObjectEnd();
            };

            JsonMapper.RegisterExporter<Vector2>((v, w) =>
            {
                writeVector2(v, w);
            });

            // 注册Vector3类型的Exporter
            Action<Vector3, JsonWriter> writeVector3 = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteObjectEnd();
            };

            JsonMapper.RegisterExporter<Vector3>((v, w) =>
            {
                writeVector3(v, w);
            });

            // 注册Vector4类型的Exporter
            JsonMapper.RegisterExporter<Vector4>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });

            // 注册Quaternion类型的Exporter
            JsonMapper.RegisterExporter<Quaternion>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });

            // 注册Color类型的Exporter
            JsonMapper.RegisterExporter<Color>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });

            // 注册Color32类型的Exporter
            JsonMapper.RegisterExporter<Color32>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });

            // 注册Bounds类型的Exporter
            JsonMapper.RegisterExporter<Bounds>((v, w) =>
            {
                w.WriteObjectStart();

                w.WritePropertyName("center");
                writeVector3(v.center, w);

                w.WritePropertyName("size");
                writeVector3(v.size, w);

                w.WriteObjectEnd();
            });

            // 注册Rect类型的Exporter
            JsonMapper.RegisterExporter<Rect>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("width", v.width);
                w.WriteProperty("height", v.height);
                w.WriteObjectEnd();
            });

            // 注册RectOffset类型的Exporter
            JsonMapper.RegisterExporter<RectOffset>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("top", v.top);
                w.WriteProperty("left", v.left);
                w.WriteProperty("bottom", v.bottom);
                w.WriteProperty("right", v.right);
                w.WriteObjectEnd();
            });
        }
        
        #endregion
    }
}