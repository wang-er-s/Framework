using System;
using System.Collections.Generic;

namespace Framework
{
    public struct Range<T> : IEquatable<Range<T>>
    {
        public T Min { get; }
        public T Max { get; }

        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public bool Equals(Range<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Min, other.Min) &&
                   EqualityComparer<T>.Default.Equals(Max, other.Max);
        }

        public override bool Equals(object obj)
        {
            return obj is Range<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

        public static bool operator ==(Range<T> d1, Range<T> d2)
        {
            return d1.Equals(d2);
        }

        public static bool operator !=(Range<T> d1, Range<T> d2)
        {
            return !(d1 == d2);
        }
    }
}