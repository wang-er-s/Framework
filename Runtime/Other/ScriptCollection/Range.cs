using System;
using System.Collections.Generic;

namespace Framework
{
    [Serializable]
    public struct Range<T> : IEquatable<Range<T>>
    {
        public T Min;
        public T Max;

        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public bool InRange(T val, bool includeMin = true, bool includeMax = true, Comparison<T> comparison = null)
        {
            if (comparison != null)
            {
                var comparer = new FunctorComparer<T>(comparison);
                return InRangeComparer(comparer, val, includeMin, includeMax);
            }

            if (val is IComparable<T> comparable)
            {
                if (includeMin && includeMax)
                    return comparable.CompareTo(Min) >= 0 && comparable.CompareTo(Max) <= 0;
                if (includeMin && !includeMax)
                    return comparable.CompareTo(Min) >= 0 && comparable.CompareTo(Max) < 0;
                if (!includeMin && includeMax)
                    return comparable.CompareTo(Min) > 0 && comparable.CompareTo(Max) <= 0;

                return comparable.CompareTo(Min) > 0 && comparable.CompareTo(Max) < 0;
            }

            throw new Exception($"{typeof(T)}没有实现IComparable<{typeof(T)}>接口，也没有提供比较器，没法比较");
        }

        private bool InRangeComparer(IComparer<T> comparer, T val, bool includeMin , bool includeMax)
        {
            if (includeMin && includeMax)
                return comparer.Compare(val, Min) >= 0 && comparer.Compare(val, Max) <= 0;
            if (includeMin && !includeMax)
                return comparer.Compare(val, Min) >= 0 && comparer.Compare(val, Max) < 0;
            if (!includeMin && includeMax)
                return comparer.Compare(val, Min) > 0 && comparer.Compare(val, Max) <= 0;
 
            return comparer.Compare(val, Min) > 0 && comparer.Compare(val, Max) < 0;         
        }

        public bool Equals(Range<T> other)
        {
            if (Min is IEquatable<T> equatableMin && Max is IEquatable<T> equatableMax)
            {
                return equatableMin.Equals(other.Min) && equatableMax.Equals(other.Max);
            }
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

        public override string ToString()
        {
            return $"min:{Min} max:{Max}";
        }
    }
}