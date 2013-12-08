// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalExtensions.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interval.Extensions
{
    using System;
    using System.Collections.Generic;

    using Orc.Interval.Interface;

    /// <summary>
    /// The date interval extensions.
    /// </summary>
    public static class IntervalExtensions
    {
        /// <summary>
        /// The get edges.
        /// </summary>
        /// <param name="interval">
        /// The interval.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<IEndPoint<T>> GetEndPoints<T>(this IInterval<T> interval)
            where T : IComparable<T>
        {
            return new List<IEndPoint<T>> { interval.Min, interval.Max };
        }

        /// <summary>
        /// Returns a sorted collection of end points from a collection of sorted intervals
        /// </summary>
        /// <param name="sortedIntervals">
        /// The ordered intervals.
        /// </param>
        /// <param name="comparer">
        /// The endpoint comparer.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<IEndPoint<T>> GetSortedEndPoints<T>(this IEnumerable<IInterval<T>> sortedIntervals, IComparer<IEndPoint<T>> comparer=null)
            where T : IComparable<T>
        {
            // TODO: this can be done as a lazy generator, just assume intervals are min-sorted (raise an exception if they're not), and keep max-endpoints on a min-heap
            // TODO: add a parameter that asserts that end points are already sorted, use it in FindOverlapsMultiple

            if (comparer == null)
            {
                comparer = Comparer<IEndPoint<T>>.Default;
            }

            var minEndPoints = new List<IEndPoint<T>>();
            var maxEndPoints = new List<IEndPoint<T>>();

            bool areMaxEndPointsSorted = true;

            IEndPoint<T> previousMaxEndPoint = new EndPoint<T>(default(T), EndPointType.Max, false, null);

            foreach(var interval in sortedIntervals)
            {
                var maxEndPoint = interval.Max;

                minEndPoints.Add(interval.Min);
                maxEndPoints.Add(interval.Max);

                if (areMaxEndPointsSorted && (comparer.Compare(previousMaxEndPoint, maxEndPoint) == +1))
                {
                    areMaxEndPointsSorted = false;
                }

                previousMaxEndPoint = maxEndPoint;
            }

            if (!areMaxEndPointsSorted)
            {
                // Replace with TimSort
                maxEndPoints.Sort(comparer);
            }

            return Orc.Extensions.MiscExtensions.MergeSorted(minEndPoints, maxEndPoints, comparer);
        }
    }
}
