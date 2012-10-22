// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalCollectionExtensions.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval collection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Entities;

    /// <summary>
    /// The date interval collection extensions.
    /// </summary>
    public static class DateIntervalCollectionExtensions
    {
        /// <summary>
        /// The overlaps with.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="dateInterval">
        /// The date interval.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<DateInterval> OverlapsWith(
            this DateIntervalCollection source, DateInterval dateInterval)
        {
            // TODO: Replace with IntervalTree

            // Get indices of start and end edge
            var startEdge = dateInterval.Min;
            var endEdge = dateInterval.Max;

            // Add these items into the DateEdges list
            int index = source.DateEdges.BinarySearch(startEdge);
            int startIndex = index;

            if (index < 0)
            {
                startIndex = ~index;
            }

            index = source.DateEdges.BinarySearch(startIndex, source.DateEdges.Count - startIndex, endEdge, null);
            int endIndex = index;

            if (index < 0)
            {
                endIndex = ~index - 1;
            }

            var dateIntervals = new HashSet<DateInterval>();

            if (startIndex == endIndex)
            {
                // We are contained so we need to move outwards
            }
            else
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    dateIntervals.Add(source.DateEdges[i].Interval as DateInterval);
                }
            }

            return dateIntervals.OrderBy(x => x.Min);
        }
    }
}