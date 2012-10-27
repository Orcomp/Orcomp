// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interval.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The interval.
//   We want this class to be immutable:
//   http://www.bluebytesoftware.com/blog/2007/11/11/ImmutableTypesForC.aspx
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Entities
{
    using System;
    using System.Collections.Generic;

    using Orc.Interface;

    /// <summary>
    /// The interval.
    /// By Default the end points are inclusive.
    /// </summary>
    /// <typeparam name="T">
    /// T must be comparable.
    /// </typeparam>
    public class Interval<T> : IInterval<T>, IComparable<Interval<T>>, IEquatable<Interval<T>>
        where T : IComparable<T>
    {
        /// <summary>
        /// The _comparer.
        /// </summary>
        private readonly Comparer<T> comparer = Comparer<T>.Default;

        /// <summary>
        /// Backing field for the Max property
        /// </summary>
        private readonly IEndPoint<T> max;

        /// <summary>
        /// Backing field for the Min property
        /// </summary>
        private readonly IEndPoint<T> min;

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
        public Interval(T minValue, T maxValue, bool isMinInclusive = true, bool isMaxInclusive = true)
        {
            if (this.comparer.Compare(minValue, maxValue) > 0)
            {
                // NOTE: We are not interested in moments in time. I.e intervals with the same end point values with different inclusiveness
                throw new ArgumentException(string.Format("Min value: {0} can't be greater than Max value: {1}", minValue, maxValue));
            }

            // NOTE: Should not really be leaking "this" in the constructor, if we really want this class to be completely immutable.
            // See the reference in the header.
            this.min = new EndPoint<T>(minValue, EndPointType.Min, isMinInclusive, this);
            this.max = new EndPoint<T>(maxValue, EndPointType.Max, isMaxInclusive, this);
        }

        /// <summary>
        /// Gets the Max.
        /// </summary>
        /// <value>
        /// The Max.
        /// </value>
        public IEndPoint<T> Max
        {
            get
            {
                return this.max;
            }
        }

        /// <summary>
        /// Gets the Min.
        /// </summary>
        /// <value>
        /// The Min.
        /// </value>
        public IEndPoint<T> Min
        {
            get
            {
                return this.min;
            }
        }

        public static bool operator !=(Interval<T> left, Interval<T> right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(Interval<T> left, Interval<T> right)
        {
            return Equals(left, right);
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
        public int CompareTo(Interval<T> other)
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
            return CompareTo(other as Interval<T>);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Interval<T>)obj);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param>
        /// The other. <name>Interval{T}</name> </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(Interval<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(this.max, other.max) && Equals(this.min, other.min);
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
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            // NOTE: Not sure whether to include type comparision yet. Leave comment below.
            // return (this.GetType() == other.GetType()) && (this.CompareTo(other) == 0);

            return this.CompareTo(other) == 0;
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
        /// Checks whether a point is contained within the interval
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(T value)
        {
            var minResult = this.comparer.Compare(this.Min.Value, value);
            var maxResult = this.comparer.Compare(this.Max.Value, value);

            var isAfterMin = this.Min.IsInclusive ? (minResult <= 0) : (minResult < 0);
            var isBeforeMax = this.Max.IsInclusive ? (maxResult >= 0) : (maxResult > 0);

            return isAfterMin && isBeforeMax;
        }

        /// <summary>
        ///  Checks whether an interval is contained within the parent
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Contains(IInterval<T> other)
        {
            var minResult = this.Min.CompareTo(other.Min);
            var maxResult = this.Max.CompareTo(other.Max);

            return minResult <= 0 && maxResult >= 0;
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
        /// Return the overlap with other interval
        /// </summary>
        /// <param name="other">
        /// </param>
        /// <returns>
        /// The <see cref="DateInterval"/>.
        /// </returns>
        public IInterval<T> GetOverlap(IInterval<T> other)
        {
            if (this.Overlaps(other) == false)
            {
                return null;
            }

            var newMin = this.Min.CompareTo(other.Min) > 0 ? this.Min : other.Min;
            var newMax = this.Max.CompareTo(other.Max) > 0 ? other.Max : this.Max;

            if (newMin.Value.CompareTo(newMax.Value) == 0 && !newMax.IsInclusive)
            {
                return null;
            }

            return  new Interval<T>(newMin.Value, newMax.Value, newMin.IsInclusive, newMax.IsInclusive);
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