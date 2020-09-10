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
