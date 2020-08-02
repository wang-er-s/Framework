using System;

namespace Framework.Prefs
{
    public class VersionTypeEncoder : ITypeEncoder
    {
        public int Priority { get; set; } = 999;

        public bool IsSupport(Type type)
        {
            return type == typeof(Version);
        }

        public object Decode(Type type, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return new Version(value);
        }

        public string Encode(object value)
        {
            return ((Version) value).ToString();
        }
    }
}
