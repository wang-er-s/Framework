using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace Framework.Prefs
{
    /// <summary>
    /// 
    /// </summary>
    public class BinaryFilePreferencesFactory : AbstractFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public BinaryFilePreferencesFactory() : this(null, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        public BinaryFilePreferencesFactory(ISerializer serializer) : this(serializer, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="encryptor"></param>
        public BinaryFilePreferencesFactory(ISerializer serializer, IEncryptor encryptor) : base(serializer, encryptor)
        {
        }

        /// <summary>
        /// Create an instance of the BinaryFilePreferences.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Preferences Create(string name)
        {
            return new BinaryFilePreferences(name, this.Serializer, this.Encryptor);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BinaryFilePreferences : Preferences
    {
        private string _root;
        /// <summary>
        /// cache.
        /// </summary>
        protected readonly Dictionary<string, string> _dict = new Dictionary<string, string>();

        /// <summary>
        /// serializer
        /// </summary>
        protected readonly ISerializer _serializer;
        /// <summary>
        /// encryptor
        /// </summary>
        protected readonly IEncryptor _encryptor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serializer"></param>
        /// <param name="encryptor"></param>
        public BinaryFilePreferences(string name, ISerializer serializer, IEncryptor encryptor) : base(name)
        {
            this._root = Application.persistentDataPath;
            this._serializer = serializer;
            this._encryptor = encryptor;
            this.Load();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual StringBuilder GetDirectory()
        {
            StringBuilder buf = new StringBuilder(this._root);
            buf.Append("/").Append(this.Name).Append("/");
            return buf;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual StringBuilder GetFullFileName()
        {
            return this.GetDirectory().Append("prefs.dat");
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Load()
        {
            try
            {
                string filename = GetFullFileName().ToString();
                if (!File.Exists(filename))
                    return;
                byte[] data = File.ReadAllBytes(filename);
                if (data == null || data.Length <= 0)
                    return;
                if (this._encryptor != null)
                    data = _encryptor.Decode(data);
                this._dict.Clear();
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            string key = reader.ReadString();
                            string value = reader.ReadString();
                            this._dict.Add(key, value);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning($"Load failed >> {e}");
            }
        }

        public override object GetObject(string key, Type type, object defaultValue)
        {
            if (!this._dict.ContainsKey(key))
                return defaultValue;
            string str = this._dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            return _serializer.Deserialize(str, type);
        }

        public override void SetObject(string key, object value)
        {
            if (value == null)
            {
                this._dict.Remove(key);
                return;
            }
            this._dict[key] = _serializer.Serialize(value);
        }

        public override T GetObject<T>(string key, T defaultValue)
        {
            if (!this._dict.ContainsKey(key))
                return defaultValue;
            string str = this._dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            return (T) _serializer.Deserialize(str, typeof(T));
        }

        public override void SetObject<T>(string key, T value)
        {
            if (value == null)
            {
                this._dict.Remove(key);
                return;
            }
            this._dict[key] = _serializer.Serialize(value);
        }

        public override object[] GetArray(string key, Type type, object[] defaultValue)
        {
            if (!_dict.ContainsKey(key))
                return defaultValue;
            string str = this._dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            string[] items = str.Split(ARRAY_SEPARATOR);
            List<object> list = new List<object>();
            for (int i = 0; i < items.Length; i++)
            {
                string item = items[i];
                if (string.IsNullOrEmpty(item))
                    list.Add(null);
                else
                {
                    list.Add(_serializer.Deserialize(items[i], type));
                }
            }
            return list.ToArray();
        }

        public override void SetArray(string key, object[] values)
        {
            if (values == null || values.Length == 0)
            {
                this._dict.Remove(key);
                return;
            }
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                buf.Append(_serializer.Serialize(value));
                if (i < values.Length - 1)
                    buf.Append(ARRAY_SEPARATOR);
            }
            this._dict[key] = buf.ToString();
        }

        public override T[] GetArray<T>(string key, T[] defaultValue)
        {
            if (!this._dict.ContainsKey(key))
                return defaultValue;
            string str = this._dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            string[] items = str.Split(ARRAY_SEPARATOR);
            List<T> list = new List<T>();
            for (int i = 0; i < items.Length; i++)
            {
                string item = items[i];
                if (string.IsNullOrEmpty(item))
                    list.Add(default(T));
                else
                {
                    list.Add((T) _serializer.Deserialize(items[i], typeof(T)));
                }
            }
            return list.ToArray();
        }

        public override void SetArray<T>(string key, T[] values)
        {
            if (values == null || values.Length == 0)
            {
                this._dict.Remove(key);
                return;
            }
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                buf.Append(_serializer.Serialize(value));
                if (i < values.Length - 1)
                    buf.Append(ARRAY_SEPARATOR);
            }
            this._dict[key] = buf.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool ContainsKey(string key)
        {
            return this._dict.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public override void Remove(string key)
        {
            if (this._dict.ContainsKey(key))
                this._dict.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void RemoveAll()
        {
            this._dict.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Save()
        {
            if (this._dict.Count <= 0)
            {
                this.Delete();
                return;
            }
            Directory.CreateDirectory(this.GetDirectory().ToString());
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(this._dict.Count);
                    foreach (KeyValuePair<string, string> kv in this._dict)
                    {
                        writer.Write(kv.Key);
                        writer.Write(kv.Value);
                    }
                    writer.Flush();
                }
                byte[] data = stream.ToArray();
                if (this._encryptor != null)
                    data = _encryptor.Encode(data);
                var filename = this.GetFullFileName().ToString();
                File.WriteAllBytes(filename, data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Delete()
        {
            this._dict.Clear();
            string filename = this.GetFullFileName().ToString();
            if (File.Exists(filename))
                File.Delete(filename);
        }
    }
}