using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Framework.Configuration
{
    public class PropertiesConfiguration : ConfigurationBase
    {
        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();

        public PropertiesConfiguration(string text)
        {
            this.Load(text);
        }

        protected void Load(string text)
        {
            StringReader reader = new StringReader(text);
            string line = null;
            while (null != (line = reader.ReadLine()))
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (Regex.IsMatch(line, @"^((#)|(//))"))
                    continue;

                int index = line.IndexOf("=", StringComparison.Ordinal);
                if (index <= 0 || (index + 1) >= line.Length)
                    throw new FormatException($"This line is not formatted correctly.line:{line}");

                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1).Trim();
                if (string.IsNullOrEmpty(key))
                    throw new FormatException($"The key is null or empty.line:{line}");

                _dict.Add(key, value);
            }
        }

        public override bool IsEmpty => _dict.Count == 0;

        public override bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        public override IEnumerator<string> GetKeys()
        {
            return _dict.Keys.GetEnumerator();
        }

        public override object GetProperty(string key)
        {
            _dict.TryGetValue(key, out var value);
            return value;
        }

        public override void AddProperty(string key, object value)
        {
            _dict.Add(key, value);
        }

        public override void SetProperty(string key, object value)
        {
            _dict[key] = value;
        }

        public override void RemoveProperty(string key)
        {
            _dict.Remove(key);
        }

        public override void Clear()
        {
            _dict.Clear();
        }
    }
}
