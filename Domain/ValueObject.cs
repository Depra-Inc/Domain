using System;
using System.Collections.Generic;
using System.Linq;

namespace Depra.Domain
{
    public abstract class ValueObject : IComparable, IComparable<ValueObject>
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((ValueObject) obj);
        }

        public override int GetHashCode() => GetHashCodePrivate();

        public int CompareTo(ValueObject other) => CompareTo(other as object);

        public int CompareTo(object obj)
        {
            var thisType = GetType();
            var otherType = obj.GetType();

            if (thisType != otherType)
            {
                return string.Compare(thisType.ToString(), otherType.ToString(), StringComparison.Ordinal);
            }

            var otherValueObject = (ValueObject) obj;
            var components = GetEqualityComponents().ToArray();
            var otherComponents = otherValueObject.GetEqualityComponents().ToArray();

            for (var i = 0; i < components.Length; i++)
            {
                var comparisonResult = CompareComponents(components[i], otherComponents[i]);
                if (comparisonResult != 0)
                {
                    return comparisonResult;
                }
            }

            return 0;
        }

        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject a, ValueObject b) => !(a == b);

#if ALLOW_UNSAFE_CODE
        private int GetHashCodePrivate() => GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
#else
        private int GetHashCodePrivate() => GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
#endif

        private bool Equals(ValueObject other) => GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

        private static int CompareComponents(object object1, object object2)
        {
            if (object1 is null && object2 is null)
            {
                return 0;
            }

            if (object1 is null)
            {
                return -1;
            }

            if (object2 is null)
            {
                return 1;
            }

            if (object1 is IComparable comparable1 && object2 is IComparable comparable2)
            {
                return comparable1.CompareTo(comparable2);
            }

            return object1.Equals(object2) ? 0 : -1;
        }
    }

    public abstract class ValueObject<T> : ValueObject
    {
        public T Value { get; }

        protected ValueObject(T value) => Value = value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value?.ToString();

        public static implicit operator T(ValueObject<T> valueObject) => valueObject.Value;
    }
}