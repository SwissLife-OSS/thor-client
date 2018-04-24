using System;
using System.Runtime.InteropServices;

namespace Thor.Core.Abstractions
{
    // todo: complete documentation

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Id<TValue>
        : IComparable
        , IComparable<TValue>
        , IEquatable<TValue>
        where TValue : struct, IComparable, IComparable<TValue>, IEquatable<TValue>
    {
        private TValue _value;

        private Id(TValue value)
        {
            _value = value;
        }

        /// <inheritdoc/>
        public static implicit operator Id<TValue>(TValue value)
        {
            return new Id<TValue>(value);
        }

        /// <inheritdoc/>
        public static bool operator ==(Id<TValue> left, Id<TValue> right)
        {
            return left == right;
        }

        /// <inheritdoc/>
        public static bool operator !=(Id<TValue> left, Id<TValue> right)
        {
            return left != right;
        }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            return _value.CompareTo(obj);
        }

        /// <inheritdoc/>
        public int CompareTo(TValue other)
        {
            return _value.CompareTo(other);
        }

        /// <inheritdoc/>
        public bool Equals(TValue other)
        {
            return _value.Equals(other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return _value.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}