using System.Collections.Generic;

namespace Framework.Configuration
{
    public class MemoryConfiguration : ConfigurationBase
    {
        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();

        public MemoryConfiguration()
        {
        }

        public MemoryConfiguration(Dictionary<string, object> dict)
        {
            if (dict != null && dict.Count > 0)
            {
                foreach (var kv in dict)
                {
                    dict.Add(kv.Key, kv.Value);
                }
            }
        }

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