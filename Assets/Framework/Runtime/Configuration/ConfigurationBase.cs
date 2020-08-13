using System;
using System.Collections.Generic;
using System.Text;
using Framework.Helper;

namespace Framework.Configuration
{
    public abstract class ConfigurationBase : IConfiguration
    {

        private static readonly DefaultTypeConverter defaultTypeConverter = new DefaultTypeConverter();

        protected static readonly string KEY_DELIMITER = ".";
        protected static readonly Version DEFAULT_VERSION = new Version("1.0.0");
        protected static readonly DateTime DEFAULT_DATETIME = new DateTime();

        private readonly List<ITypeConverter> _converters = new List<ITypeConverter>();

        public ConfigurationBase() : this(null)
        {
        }

        public ConfigurationBase(ITypeConverter[] converters)
        {
            _converters.Add(defaultTypeConverter);
            if (converters != null && converters.Length > 0)
            {
                foreach (var converter in converters)
                {
                    _converters.Insert(0, converter);
                }
            }
        }

        protected virtual T GetProperty<T>(string key, T defaultValue)
        {
            object value = GetProperty(key);
            if (value == null)
                return defaultValue;

            return (T) ConvertTo(typeof(T), value);
        }

        protected virtual object GetProperty(string key, Type type, object defaultValue)
        {
            object value = GetProperty(key);
            return value == null ? defaultValue : ConvertTo(type, value);
        }

        protected virtual object ConvertTo(Type type, object value)
        {
            try
            {
                foreach (var converter in _converters)
                {
                    if (!converter.Support(type))
                        continue;

                    return converter.Convert(type, value);
                }
            }
            catch (Exception e)
            {
                Log.Warning($"This value \"{value}\" cannot be converted to type \"{type.Name}\"");
                throw new FormatException($"This value \"{value}\" cannot be converted to the type \"{type.Name}\"", e);
            }

            throw new NotSupportedException($"This value \"{value}\" cannot be converted to the type \"{type.Name}\"");
        }

        public virtual void AddTypeConverter(ITypeConverter converter)
        {
            _converters.Insert(0, converter);
        }

        public virtual IConfiguration Subset(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("the prefix is null or empty", nameof(prefix));

            return new SubsetConfiguration(this, prefix);
        }

        public virtual bool IsEmpty => !GetKeys().MoveNext();

        public virtual IEnumerator<string> GetKeys(string prefix)
        {
            return new FilterEnumerator<string>(GetKeys(), (it) => it.StartsWith(prefix + KEY_DELIMITER));
        }

        public bool GetBoolean(string key)
        {
            return GetBoolean(key, false);
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public float GetFloat(string key)
        {
            return GetFloat(key, 0);
        }

        public float GetFloat(string key, float defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public double GetDouble(string key)
        {
            return GetDouble(key, 0);
        }

        public double GetDouble(string key, double defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public short GetShort(string key)
        {
            return GetShort(key, 0);
        }

        public short GetShort(string key, short defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public int GetInt(string key)
        {
            return GetInt(key, 0);
        }

        public int GetInt(string key, int defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public long GetLong(string key)
        {
            return GetLong(key, 0);
        }

        public long GetLong(string key, long defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public string GetString(string key)
        {
            return GetString(key, null);
        }

        public string GetString(string key, string defaultValue)
        {
            return GetProperty<string>(key, defaultValue);
        }

        public string GetFormattedString(string key, params object[] args)
        {
            string format = GetString(key, null);
            return format == null ? null : string.Format(format, args);
        }

        public DateTime GetDateTime(string key)
        {
            return GetDateTime(key, DEFAULT_DATETIME);
        }

        public DateTime GetDateTime(string key, DateTime defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public Version GetVersion(string key)
        {
            return GetVersion(key, DEFAULT_VERSION);
        }

        public virtual Version GetVersion(string key, Version defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public T GetObject<T>(string key)
        {
            return GetObject(key, default(T));
        }

        public virtual T GetObject<T>(string key, T defaultValue)
        {
            return GetProperty(key, defaultValue);
        }

        public object[] GetArray(string key, Type type)
        {
            return GetArray(key, type, new object[0]);
        }

        public object[] GetArray(string key, Type type, object[] defaultValue)
        {
            object value = GetProperty(key);
            if (value == null)
                return defaultValue;

            if (value is string)
            {
                string str = (string) value;
                if (string.IsNullOrEmpty(str))
                    return defaultValue;

                List<object> list = new List<object>();
                string[] items = str.Split(',');
                foreach (string item in items)
                {
                    object ret = null;
                    try
                    {
                        ret = ConvertTo(type, item);
                    }
                    catch (NotSupportedException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                    }

                    list.Add(ret);
                }

                return list.ToArray();
            }

            if (value is Array array)
            {
                List<object> list = new List<object>();
                for (int i = 0; i < array.Length; i++)
                {
                    var item = array.GetValue(i);
                    object ret = null;
                    try
                    {
                        ret = ConvertTo(type, item);
                    }
                    catch (NotSupportedException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                    }

                    list.Add(ret);
                }

                return list.ToArray();
            }

            throw new FormatException($"This value \"{value}\" cannot be converted to an \"{type.Name}\" array.");
        }

        public T[] GetArray<T>(string key)
        {
            return GetArray(key, new T[0]);
        }

        public virtual T[] GetArray<T>(string key, T[] defaultValue)
        {
            object value = GetProperty(key);
            if (value == null)
                return defaultValue;

            if (value is string str)
            {
                if (string.IsNullOrEmpty(str))
                    return defaultValue;

                List<T> list = new List<T>();
                string[] items = str.Split(',');
                foreach (string item in items)
                {
                    T ret = default(T);
                    try
                    {
                        ret = (T) ConvertTo(typeof(T), item);
                    }
                    catch (NotSupportedException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                    }

                    list.Add(ret);
                }

                return list.ToArray();
            }

            if (value is T[] ts)
                return ts;

            if (value is Array array)
            {
                List<T> list = new List<T>();
                for (int i = 0; i < array.Length; i++)
                {
                    var item = array.GetValue(i);
                    T ret = default(T);
                    try
                    {
                        ret = (T) ConvertTo(typeof(T), item);
                    }
                    catch (NotSupportedException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                    }

                    list.Add(ret);
                }

                return list.ToArray();
            }

            throw new FormatException($"This value \"{value}\" cannot be converted to an \"{typeof(T).Name}\" array.");
        }

        public override string ToString()
        {
            IEnumerator<string> it = GetKeys();
            StringBuilder buf = new StringBuilder();
            buf.Append(GetType().Name).Append("{ \r\n");
            while (it.MoveNext())
            {
                string key = it.Current;
                buf.AppendFormat("  {0} = {1}\r\n", key, GetProperty(key));
            }

            buf.Append("}");
            return buf.ToString();
        }

        public abstract IEnumerator<string> GetKeys();

        public abstract bool ContainsKey(string key);

        public abstract object GetProperty(string key);

        public abstract void AddProperty(string key, object value);

        public abstract void RemoveProperty(string key);

        public abstract void SetProperty(string key, object value);

        public abstract void Clear();
    }
}