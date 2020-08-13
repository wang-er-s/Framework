using System;
using System.Collections.Generic;

namespace Framework.Prefs
{
    public class DefaultSerializer : ISerializer
    {
        private readonly object _lock = new object();
        private static readonly ComparerImpl<ITypeEncoder> comparer = new ComparerImpl<ITypeEncoder>();
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
                _encoders.Sort(comparer);
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
                    catch (Exception)
                    {
                    }
                }

            }

            throw new NotSupportedException($"This value \"{input}\" cannot be converted to the type \"{type.Name}\"");
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
                    catch (Exception)
                    {
                    }
                }
            }

            throw new NotSupportedException($"Unsupported type, this value \"{value}\" cannot be serialized");
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
