// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndPointMinFirstComparer.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The end point min first comparer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Interval
{
    using System;
    using System.Collections.Generic;

    using Orc.Interval.Interface;

    /// <summary>
    /// The end point min first comparer.
    /// Similar to the standard EndPoint comparer but ensures that if two end points have the
    /// same value and are inclusive the min endpoint will come AFTER the max value.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class EndPointMinFirstComparer<T> : IComparer<IEndPoint<T>>
        where T : IComparable<T>
    {
        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Compare(IEndPoint<T> x, IEndPoint<T> y)
        {
            var result = x.CompareTo(y);

            if (result == 0)
            {
                if (x.IsMin == !y.IsMin && x.IsInclusive && y.IsInclusive)
                {
                    // Min endPoint always comes AFTER Max endPoint
                    result = x.IsMin ? +1 : -1;
                }
            }

            return result;
        }
    }
}