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
