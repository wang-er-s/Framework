using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Framework.BaseUtil
{
    public static class LitJsonUtility
    {
        public static string GetMD5(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return GetMD5(bytes);
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
            return ((IJsonWrapper)data).GetString();
        }

        public static void SetJsonString(this JsonData data, string val)
        {
            ((IJsonWrapper)data).SetString(val);
        }
        public static int GetJsonInt(this JsonData data)
        {
            return ((IJsonWrapper)data).GetInt();
        }
        public static void SetJsonInt(this JsonData data, int val)
        {
            ((IJsonWrapper)data).SetInt(val);
        }
        public static bool GetJsonBool(this JsonData data)
        {
            return ((IJsonWrapper)data).GetBoolean();
        }
        public static void SetJsonBool(this JsonData data, bool val)
        {
            ((IJsonWrapper)data).SetBoolean(val);
        }
        public static long GetJsonLong(this JsonData data)
        {
            return ((IJsonWrapper)data).GetLong();
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
            ((IJsonWrapper)data).SetDouble(val);
        }
        public static bool HasField(this JsonData data, string key)
        {
            return ((IDictionary)data).Contains(key);
        }
        public static void AddField(this JsonData data, string key, object val)
        {
            ((IDictionary)data).Add(key, val);
        }

        public static void RemoveField(this JsonData data, string key)
        {
            ((IDictionary)data).Remove(key);
        }

        public static string ToFormatJson(this JsonData data)
        {
            StringWriter sw = new StringWriter ();
            JsonWriter writer = new JsonWriter (sw);
            writer.Validate = false;
            writer.PrettyPrint = true;
            data.ToJson(writer);
            return sw.ToString ();
        }
        #endregion
    }
}
