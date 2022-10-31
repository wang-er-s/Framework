/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace Framework
{

    public class BinaryFilePreferencesFactory : AbstractFactory
    {

        public BinaryFilePreferencesFactory() : this(null)
        {
        }

        public BinaryFilePreferencesFactory(ISerializer serializer, IEncryptor encryptor = null) : base(serializer,
            encryptor)
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


    public class BinaryFilePreferences : Preferences
    {
        private string _root;

        /// <summary>
        /// cache.
        /// </summary>
        protected readonly Dictionary<string, string> Dict = new Dictionary<string, string>();

        /// <summary>
        /// serializer
        /// </summary>
        protected readonly ISerializer Serializer;

        /// <summary>
        /// encryptor
        /// </summary>
        protected readonly IEncryptor Encryptor;

        public BinaryFilePreferences(string name, ISerializer serializer, IEncryptor encryptor) : base(name)
        {
            _root = Application.persistentDataPath;
            Serializer = serializer;
            Encryptor = encryptor;
            Load();
        }

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
                if (data.Length <= 0)
                    return;
                if (this.Encryptor != null)
                    data = Encryptor.Decode(data);
                this.Dict.Clear();
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            string key = reader.ReadString();
                            string value = reader.ReadString();
                            this.Dict.Add(key, value);
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
            if (!this.Dict.ContainsKey(key))
                return defaultValue;
            string str = this.Dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            return Serializer.Deserialize(str, type);
        }

        public override void SetObject(string key, object value)
        {
            if (value == null)
            {
                this.Dict.Remove(key);
                return;
            }

            this.Dict[key] = Serializer.Serialize(value);
        }

        public override T GetObject<T>(string key, T defaultValue)
        {
            if (!this.Dict.ContainsKey(key))
                return defaultValue;
            string str = this.Dict[key];
            if (string.IsNullOrEmpty(str))
                return defaultValue;
            return (T) Serializer.Deserialize(str, typeof(T));
        }

        public override void SetObject<T>(string key, T value)
        {
            if (value == null)
            {
                this.Dict.Remove(key);
                return;
            }

            this.Dict[key] = Serializer.Serialize(value);
        }

        public override IList GetArray(string key, Type type, IList defaultValue)
        {
            if (!Dict.ContainsKey(key))
                return defaultValue;
            string str = this.Dict[key];
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
                    list.Add(Serializer.Deserialize(items[i], type));
                }
            }

            return list;
        }

        public override void SetArray(string key, IList values)
        {
            if (values == null || values.Count == 0)
            {
                this.Dict.Remove(key);
                return;
            }

            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                var value = values[i];
                buf.Append(Serializer.Serialize(value));
                if (i < values.Count - 1)
                    buf.Append(ARRAY_SEPARATOR);
            }

            this.Dict[key] = buf.ToString();
        }

        public override List<T> GetArray<T>(string key, List<T> defaultValue)
        {
            if (!this.Dict.ContainsKey(key))
                return defaultValue;
            string str = this.Dict[key];
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
                    list.Add((T) Serializer.Deserialize(items[i], typeof(T)));
                }
            }

            return list;
        }

        public override void SetArray<T>(string key, T[] values)
        {
            if (values == null || values.Length == 0)
            {
                this.Dict.Remove(key);
                return;
            }

            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                buf.Append(Serializer.Serialize(value));
                if (i < values.Length - 1)
                    buf.Append(ARRAY_SEPARATOR);
            }

            this.Dict[key] = buf.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool ContainsKey(string key)
        {
            return this.Dict.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public override void Remove(string key)
        {
            if (this.Dict.ContainsKey(key))
                this.Dict.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void RemoveAll()
        {
            this.Dict.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Save()
        {
            if (this.Dict.Count <= 0)
            {
                this.Delete();
                return;
            }

            Directory.CreateDirectory(this.GetDirectory().ToString());
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(this.Dict.Count);
                    foreach (KeyValuePair<string, string> kv in this.Dict)
                    {
                        writer.Write(kv.Key);
                        writer.Write(kv.Value);
                    }

                    writer.Flush();
                }

                byte[] data = stream.ToArray();
                if (this.Encryptor != null)
                    data = Encryptor.Encode(data);
                var filename = this.GetFullFileName().ToString();
                File.WriteAllBytes(filename, data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Delete()
        {
            this.Dict.Clear();
            string filename = this.GetFullFileName().ToString();
            if (File.Exists(filename))
                File.Delete(filename);
        }
    }
}