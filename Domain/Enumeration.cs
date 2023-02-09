using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Depra.Domain
{
    public abstract class Enumeration : IComparable, IEquatable<Enumeration>
    {
        public int Id { get; }

        public string Name { get; }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration => typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

        protected Enumeration(int id, string name) =>
            (Id, Name) = (id, name);

        public override string ToString() => Name;

        public bool Equals(Enumeration other) =>
            Name == other!.Name && Id == other.Id;

        public override bool Equals(object obj)
        {
            if (obj is Enumeration otherValue)
            {
                var typeMatches = GetType() == obj.GetType();
                var valueMatches = Id.Equals(otherValue.Id);

                return typeMatches && valueMatches;
            }

            return false;
        }

        public override int GetHashCode() =>
            HashCode.Combine(Name, Id);

        public int CompareTo(object other) =>
            Id.CompareTo(((Enumeration)other).Id);
    }
}