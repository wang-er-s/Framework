using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Framework.Prefs
{

    public class PlayerPrefsPreferencesFactory : AbstractFactory
    {

        public PlayerPrefsPreferencesFactory() : this(null, null)
        {
        }

        public PlayerPrefsPreferencesFactory(ISerializer serializer) : this(serializer, null)
        {
        }

        public PlayerPrefsPreferencesFactory(ISerializer serializer, IEncryptor encryptor) : base(serializer, encryptor)
        {
        }

        public override Preferences Create(string name)
        {
            return new PlayerPrefsPreferences(name, Serializer, Encryptor);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PlayerPrefsPreferences : Preferences
    {
        /// <summary>
        /// Default key
        /// </summary>
        protected static readonly string KEYS = "_KEYS_";

        protected readonly ISerializer Serializer;

        protected readonly IEncryptor Encryptor;

        protected readonly List<string> Keys = new List<string>();

        public PlayerPrefsPreferences(string name, ISerializer serializer, IEncryptor encryptor) : base(name)
        {
            Serializer = serializer;
            Encryptor = encryptor;
            Load();
        }

        protected override void Load()
        {
            LoadKeys();
        }

        protected string Key(string key)
        {
            StringBuilder buf = new StringBuilder(Name);
            buf.Append(".").Append(key);
            return buf.ToString();
        }

        protected virtual void LoadKeys()
        {
            if (!PlayerPrefs.HasKey(Key(KEYS)))
                return;

            string value = PlayerPrefs.GetString(Key(KEYS));
            if (string.IsNullOrEmpty(value))
                return;

            string[] keyValues = value.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string key in keyValues)
            {
                if (string.IsNullOrEmpty(key))
                    continue;

                Keys.Add(key);
            }
        }

        protected virtual void SaveKeys()
        {
            if (Keys == null || Keys.Count <= 0)
            {
                PlayerPrefs.DeleteKey(Key(KEYS));
                return;
            }

            string[] values = Keys.ToArray();

            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                if (string.IsNullOrEmpty(values[i]))
                    continue;

                buf.Append(values[i]);
                if (i < values.Length - 1)
                    buf.Append(",");
            }

            PlayerPrefs.SetString(Key(KEYS), buf.ToString());
        }

        public override object GetObject(string key, Type type, object defaultValue)
        {
            if (!PlayerPrefs.HasKey(Key(key)))
                return defaultValue;

            string str = PlayerPrefs.GetString(Key(key));
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            if (Encryptor != null)
            {
                byte[] data = Convert.FromBase64String(str);
                data = Encryptor.Decode(data);
                str = Encoding.UTF8.GetString(data);
            }

            return Serializer.Deserialize(str, type);
        }

        public override void SetObject(string key, object value)
        {
            string str = value == null ? "" : Serializer.Serialize(value);
            if (Encryptor != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(str);
                data = Encryptor.Encode(data);
                str = Convert.ToBase64String(data);
            }

            PlayerPrefs.SetString(Key(key), str);

            if (!Keys.Contains(key))
            {
                Keys.Add(key);
                SaveKeys();
            }
        }

        public override T GetObject<T>(string key, T defaultValue)
        {
            if (!PlayerPrefs.HasKey(Key(key)))
                return defaultValue;

            string str = PlayerPrefs.GetString(Key(key));
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            if (Encryptor != null)
            {
                byte[] data = Convert.FromBase64String(str);
                data = Encryptor.Decode(data);
                str = Encoding.UTF8.GetString(data);
            }

            return (T) Serializer.Deserialize(str, typeof(T));
        }

        public override void SetObject<T>(string key, T value)
        {
            string str = value == null ? "" : Serializer.Serialize(value);
            if (Encryptor != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(str);
                data = Encryptor.Encode(data);
                str = Convert.ToBase64String(data);
            }

            PlayerPrefs.SetString(Key(key), str);

            if (!Keys.Contains(key))
            {
                Keys.Add(key);
                SaveKeys();
            }
        }

        public override object[] GetArray(string key, Type type, object[] defaultValue)
        {
            if (!PlayerPrefs.HasKey(Key(key)))
                return defaultValue;

            string str = PlayerPrefs.GetString(Key(key));
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            if (Encryptor != null)
            {
                byte[] data = Convert.FromBase64String(str);
                data = Encryptor.Decode(data);
                str = Encoding.UTF8.GetString(data);
            }

            string[] items = str.Split(ARRAY_SEPARATOR);
            List<object> list = new List<object>();
            for (int i = 0; i < items.Length; i++)
            {
                string item = items[i];
                if (string.IsNullOrEmpty(item))
                    list.Add(null);
                else
                {
                    list.Add(Serializer.Deserialize(items[i], type));
                }
            }

            return list.ToArray();
        }

        public override void SetArray(string key, object[] values)
        {
            StringBuilder buf = new StringBuilder();
            if (values != null && values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    buf.Append(Serializer.Serialize(value));
                    if (i < values.Length - 1)
                        buf.Append(ARRAY_SEPARATOR);
                }
            }

            string str = buf.ToString();
            if (Encryptor != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(str);
                data = Encryptor.Encode(data);
                str = Convert.ToBase64String(data);
            }

            PlayerPrefs.SetString(Key(key), str);

            if (!Keys.Contains(key))
            {
                Keys.Add(key);
                SaveKeys();
            }
        }

        public override T[] GetArray<T>(string key, T[] defaultValue)
        {
            if (!PlayerPrefs.HasKey(Key(key)))
                return defaultValue;

            string str = PlayerPrefs.GetString(Key(key));
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            if (Encryptor != null)
            {
                byte[] data = Convert.FromBase64String(str);
                data = Encryptor.Decode(data);
                str = Encoding.UTF8.GetString(data);
            }

            string[] items = str.Split(ARRAY_SEPARATOR);
            List<T> list = new List<T>();
            for (int i = 0; i < items.Length; i++)
            {
                string item = items[i];
                if (string.IsNullOrEmpty(item))
                    list.Add(default(T));
                else
                {
                    list.Add((T) Serializer.Deserialize(items[i], typeof(T)));
                }
            }

            return list.ToArray();
        }

        public override void SetArray<T>(string key, T[] values)
        {
            StringBuilder buf = new StringBuilder();
            if (values != null && values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    buf.Append(Serializer.Serialize(value));
                    if (i < values.Length - 1)
                        buf.Append(ARRAY_SEPARATOR);
                }
            }

            string str = buf.ToString();
            if (Encryptor != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(str);
                data = Encryptor.Encode(data);
                str = Convert.ToBase64String(data);
            }

            PlayerPrefs.SetString(Key(key), str);

            if (!Keys.Contains(key))
            {
                Keys.Add(key);
                SaveKeys();
            }
        }

        public override bool ContainsKey(string key)
        {
            return PlayerPrefs.HasKey(Key(key));
        }

        public override void Remove(string key)
        {
            PlayerPrefs.DeleteKey(Key(key));
            if (Keys.Contains(key))
            {
                Keys.Remove(key);
                SaveKeys();
            }
        }

        public override void RemoveAll()
        {
            foreach (string key in Keys)
            {
                PlayerPrefs.DeleteKey(Key(key));
            }

            PlayerPrefs.DeleteKey(Key(KEYS));
            Keys.Clear();
        }

        public override void Save()
        {
            PlayerPrefs.Save();
        }

        public override void Delete()
        {
            RemoveAll();
            PlayerPrefs.Save();
        }
    }
}