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

namespace Framework.Configuration
{
    public class CompositeConfiguration : ConfigurationBase
    {
        private readonly List<IConfiguration> _configurations = new List<IConfiguration>();

        private readonly IConfiguration _memoryConfiguration;

        public CompositeConfiguration() : this(null)
        {
        }

        /// <summary>
        /// Multiple Configuration
        /// </summary>
        /// <param name="configurations">list of IConfiguration</param>
        public CompositeConfiguration(List<IConfiguration> configurations)
        {
            this._memoryConfiguration = new MemoryConfiguration();
            this._configurations.Add(_memoryConfiguration);

            if (configurations != null && configurations.Count > 0)
            {
                foreach (var config in configurations)
                {
                    if (config == null)
                        continue;

                    AddConfiguration(config);
                }
            }
        }

        /// <summary>
        /// Get the first configuration with a given key.
        /// </summary>
        /// <param name="key">the key to be checked</param>
        /// <exception cref="ArgumentException">if the source configuration cannot be determined</exception>
        /// <returns>the source configuration of this key</returns>
        public IConfiguration GetFirstConfiguration(string key)
        {
            if (key == null)
                throw new ArgumentException("Key must not be null!");

            foreach (var config in _configurations)
            {
                if (config != null && config.ContainsKey(key))
                    return config;
            }

            return null;
        }

        /// <summary>
        /// Return the configuration at the specified index.
        /// </summary>
        /// <param name="index">The index of the configuration to retrieve</param>
        /// <returns>the configuration at this index</returns>
        public IConfiguration GetConfiguration(int index)
        {
            if (index < 0 || index >= _configurations.Count)
                return null;

            return _configurations[index];
        }

        /// <summary>
        /// Returns the memory configuration. In this configuration changes are stored.
        /// </summary>
        /// <returns>the in memory configuration</returns>
        public IConfiguration GetMemoryConfiguration()
        {
            return _memoryConfiguration;
        }

        /// <summary>
        /// Add a new configuration, the new configuration has a higher priority.
        /// </summary>
        /// <param name="configuration"></param>
        public void AddConfiguration(IConfiguration configuration)
        {
            if (!_configurations.Contains(configuration))
            {
                _configurations.Insert(1, configuration);
            }
        }

        public void RemoveConfiguration(IConfiguration configuration)
        {
            if (!configuration.Equals(_memoryConfiguration))
            {
                _configurations.Remove(configuration);
            }
        }

        public int GetNumberOfConfigurations()
        {
            return _configurations.Count;
        }

        public override bool IsEmpty
        {
            get
            {
                foreach (var config in _configurations)
                {
                    if (config != null && !config.IsEmpty)
                        return false;
                }

                return true;
            }
        }

        public override bool ContainsKey(string key)
        {
            foreach (var config in _configurations)
            {
                if (config != null && config.ContainsKey(key))
                    return true;
            }

            return false;
        }

        public override IEnumerator<string> GetKeys()
        {
            List<string> keys = new List<string>();
            foreach (var config in _configurations)
            {
                if (config == null)
                    continue;

                IEnumerator<string> j = config.GetKeys();
                while (j.MoveNext())
                {
                    string key = j.Current;
                    if (!keys.Contains(key))
                        keys.Add(key);
                }
            }

            return keys.GetEnumerator();
        }

        public override object GetProperty(string key)
        {
            foreach (var config in _configurations)
            {
                if (config != null && config.ContainsKey(key))
                    return config.GetProperty(key);
            }

            return null;
        }

        public override void AddProperty(string key, object value)
        {
            _memoryConfiguration.AddProperty(key, value);
        }

        public override void SetProperty(string key, object value)
        {
            _memoryConfiguration.SetProperty(key, value);
        }

        public override void RemoveProperty(string key)
        {
            _memoryConfiguration.RemoveProperty(key);
        }

        public override void Clear()
        {
            _memoryConfiguration.Clear();
            for (int i = _configurations.Count - 1; i > 0; i--)
            {
                _configurations.RemoveAt(i);
            }
        }
    }
}
