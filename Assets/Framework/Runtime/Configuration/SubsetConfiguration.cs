using System;
using System.Collections.Generic;
using Framework.CommonHelper;

namespace Framework.Configuration
{
    class SubsetConfiguration : ConfigurationBase
    {
        private readonly string _prefix;
        private readonly ConfigurationBase _parent;

        public SubsetConfiguration(ConfigurationBase parent, string prefix)
        {
            this._parent = parent;
            this._prefix = prefix;
        }

        protected string GetParentKey(string key)
        {
            if ("".Equals(key) || key == null)
                throw new ArgumentNullException(key);

            return _prefix + KEY_DELIMITER + key;
        }

        protected string GetChildKey(string key)
        {
            if (!key.StartsWith(_prefix))
                throw new ArgumentException($"The parent key '{key}' is not in the subset.");

            return key.Substring(_prefix.Length + KEY_DELIMITER.Length);
        }

        public override IConfiguration Subset(string prefix)
        {
            return _parent.Subset(GetParentKey(prefix));
        }

        public override bool ContainsKey(string key)
        {
            return _parent.ContainsKey(GetParentKey(key));
        }

        public override IEnumerator<string> GetKeys()
        {
            return new TransformEnumerator<string, string>(_parent.GetKeys(_prefix), GetChildKey);
        }

        public override object GetProperty(string key)
        {
            return _parent.GetProperty(GetParentKey(key));
        }

        public override void AddProperty(string key, object value)
        {
            _parent.AddProperty(GetParentKey(key), value);
        }

        public override void SetProperty(string key, object value)
        {
            _parent.SetProperty(GetParentKey(key), value);
        }

        public override void RemoveProperty(string key)
        {
            _parent.RemoveProperty(GetParentKey(key));
        }

        public override void Clear()
        {
            IEnumerator<string> it = GetKeys();
            for (; it.MoveNext();)
            {
                RemoveProperty(it.Current);
            }
        }
    }
}
