using System;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Framework
{
    public static class ResNameRedirect
    {
        private static string NAME_ENCRYPT_KEY = "Name encrypt";
        private static byte[] NAME_ENCRYPT_KEY_CODE = null;
        private static byte[] GetFileEncryptKeyCode()
        {
            if (NAME_ENCRYPT_KEY_CODE == null)
                NAME_ENCRYPT_KEY_CODE = Encoding.UTF8.GetBytes(NAME_ENCRYPT_KEY);
            return NAME_ENCRYPT_KEY_CODE;
        } 
        private static void EncryptBytes(byte[] body,int len = -1)
        {
            if (len < 0)
                len = body.Length;
            byte[] key = GetFileEncryptKeyCode();
            int keyLen = key.Length;
            for (int i = 0; i < len; ++i)
            {
                body[i] = (byte)(body[i] ^ (key[i % keyLen]));
            }
        }

        public static string GetRedirectName(string name)
        {
            if (null == name) return null;
            //var encrypt = GameEnv.Instance.NameEncrypt;
            //if(string.IsNullOrEmpty(encrypt))
            string pathId = PathIdProfile.Ins.GetPathId(name);
            return pathId ?? name;
            //return string.Format(encrypt, name[0], name);
            /*
            byte[] body = Encoding.UTF8.GetBytes(name);
            EncryptBytes(body);
            string ret = Convert.ToBase64String(body);
            return ret.Replace('/', '-').ToLower();
            */
        }
    }
}