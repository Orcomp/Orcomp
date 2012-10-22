// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interval.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The interval.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Entities
{
    using System;
    using System.Collections.Generic;

    using Orc.Interface;

    /// <summary>
    /// The interval.
    /// </summary>
    /// <typeparam name="T">
    /// T must be comparable.
    /// </typeparam>
    public class Interval<T> : IInterval<T>
        where T : IComparable<T>
    {
        /// <summary>
        /// The _comparer.
        /// </summary>
        private readonly Comparer<T> comparer = Comparer<T>.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval{T}"/> class.
        /// </summary>
        public Interval()
            : this(default(T), default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval{T}"/> class.
        /// </summary>
        /// <param name="minValue">
        /// The minValue.
        /// </param>
        /// <param name="maxValue">
        /// The maxValue.
        /// </param>
        /// <param name="isMinInclusive">
        /// The is minValue inclusive.
        /// </param>
        /// <param name="isMaxInclusive">
        /// The is maxValue inclusive.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Min must be lesser or equal to Max.
        /// </exception>
        public Interval(T minValue, T maxValue, bool isMinInclusive = true, bool isMaxInclusive = false)
        {
            if (this.comparer.Compare(minValue, maxValue) > 0)
            {
                // NOTE: We are not interested in moments in time. I.e intervals with the same end point values with different inclusiveness
                throw new ArgumentException(string.Format("Min value: {0} can't be greater than Max value: {1}", minValue, maxValue));
            }

            this.Min = new EndPoint<T>(minValue, EndPointType.Min, isMinInclusive, this);
            this.Max = new EndPoint<T>(maxValue, EndPointType.Max, isMaxInclusive, this);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(IInterval<T> other)
        {
            return (this.GetType() == other.GetType()) && (this.CompareTo(other) == 0);
        }

        /// <summary>
        /// Compare two IInterval types.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// Returns the result of comparing two intervals together.
        /// </returns>
        public int CompareTo(IInterval<T> other)
        {
            if (other == null)
            {
                return 1;
            }

            var result = this.Min.CompareTo(other.Min);

            if (result == 0)
            {
                result = this.Max.CompareTo(other.Max);
            }

            return result;
        }

        /// <summary>
        /// Gets the Min.
        /// </summary>
        /// <value>
        /// The Min.
        /// </value>
        public IEndPoint<T> Min { get; private set; }

        /// <summary>
        /// Gets the Max.
        /// </summary>
        /// <value>
        /// The Max.
        /// </value>
        public IEndPoint<T> Max { get; private set; }

        /// <summary>
        /// The intersects.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Intersects(T value)
        {
            var minResult = this.comparer.Compare(this.Min.Value, value);
            var maxResult = this.comparer.Compare(this.Max.Value, value);

            var isAfterMin = this.Min.IsInclusive ? (minResult <= 0) : (minResult < 0);
            var isBeforeMax = this.Max.IsInclusive ? (maxResult >= 0) : (maxResult > 0);

            return isAfterMin && isBeforeMax;
        }

        /// <summary>
        /// Returns whether two intervals overlap each other.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// Returns a boolean on whether two intervals overlap each other.
        /// </returns>
        public bool Overlaps(IInterval<T> other)
        {
            if (other == null)
            {
                return false;
            }

            var startsBeforeOtherEnds = this.Min.CompareTo(other.Max) <= 0;
            var endsAfterOtherStart = this.Max.CompareTo(other.Min) >= 0;

            return startsBeforeOtherEnds && endsAfterOtherStart;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as IInterval<T>);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Min.GetHashCode() ^ this.Max.GetHashCode();
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} : {1}", this.Min, this.Max);
        }
    }
}