// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndPointWithIntervalEqualityComparer.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   EndPoint EqualityComparer which includes the interval.
//   To be used with Dictionaries, HashSets etc where we want to differentiate EndPoints by the Interval they belong to.
//   This mainly will happen if we have an IInterval colletion with DateIntervals, EfficiencyIntervals etc...
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Entities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The end point with interval equality comparer.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class EndPointWithIntervalEqualityComparer<T> : IEqualityComparer<EndPoint<T>>
        where T : IComparable<T>
    {
        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(EndPoint<T> x, EndPoint<T> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x.Interval.Equals(y.Interval) && x.EndPointType.Equals(y.EndPointType)
                   && EqualityComparer<T>.Default.Equals(x.Value, y.Value) && x.IsInclusive.Equals(y.IsInclusive))
            ;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetHashCode(EndPoint<T> obj)
        {
            return obj.GetHashCode() ^ obj.Interval.GetHashCode();
        }
    }
}