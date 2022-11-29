using System;
using System.Collections.Generic;

namespace Framework
{
    public readonly struct Range<T> : IEquatable<Range<T>> where T : IComparable<T>
    {
        public T Min { get; }
        public T Max { get; }

        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public bool InRange(T val, bool includeMin = true, bool includeMax = true)
        {
            if (includeMin && includeMax)
                return val.CompareTo(Min) >= 0 && val.CompareTo(Max) <= 0;
            if (includeMin && !includeMax)
                return val.CompareTo(Min) >= 0 && val.CompareTo(Max) < 0;
            if (!includeMin && includeMax)
                return val.CompareTo(Min) > 0 && val.CompareTo(Max) <= 0;

            return val.CompareTo(Min) > 0 && val.CompareTo(Max) < 0;
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