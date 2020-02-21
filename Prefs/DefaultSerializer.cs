using System;
using System.Collections.Generic;

namespace Framework.Prefs
{
    public class DefaultSerializer : ISerializer
    {
        private readonly object _lock = new object();
        private static readonly ComparerImpl<ITypeEncoder> _comparer = new ComparerImpl<ITypeEncoder>();
        private List<ITypeEncoder> _encoders = new List<ITypeEncoder>();

        public DefaultSerializer()
        {
            AddTypeEncoder(new PrimitiveTypeEncoder());
            AddTypeEncoder(new VersionTypeEncoder());
            AddTypeEncoder(new JsonTypeEncoder());
        }

        public virtual void AddTypeEncoder(ITypeEncoder encoder)
        {
            lock (_lock)
            {
                if (_encoders.Contains(encoder))
                    return;

                _encoders.Add(encoder);
                _encoders.Sort(_comparer);
            }
        }

        public virtual void RemoveTypeEncoder(ITypeEncoder encoder)
        {
            lock (_lock)
            {
                if (!_encoders.Contains(encoder))
                    return;

                _encoders.Remove(encoder);
            }
        }

        public virtual object Deserialize(string input, Type type)
        {
            lock (_lock)
            {
                for (int i = 0; i < _encoders.Count; i++)
                {
                    try
                    {
                        ITypeEncoder encoder = _encoders[i];
                        if (!encoder.IsSupport(type))
                            continue;

                        return encoder.Decode(type, input);
                    }
                    catch (Exception) { }
                }

            }
            throw new NotSupportedException(string.Format("This value \"{0}\" cannot be converted to the type \"{1}\"", input, type.Name));
        }

        public virtual string Serialize(object value)
        {
            lock (_lock)
            {
                for (int i = 0; i < _encoders.Count; i++)
                {
                    try
                    {
                        ITypeEncoder encoder = _encoders[i];
                        if (!encoder.IsSupport(value.GetType()))
                            continue;

                        return encoder.Encode(value);
                    }
                    catch (Exception) { }
                }
            }
            throw new NotSupportedException(string.Format("Unsupported type, this value \"{0}\" cannot be serialized", value));
        }

        class ComparerImpl<T> : IComparer<T> where T : ITypeEncoder
        {
            public int Compare(T x, T y)
            {
                return y.Priority.CompareTo(x.Priority);
            }
        }
    }
}
