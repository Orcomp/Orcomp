// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalCollectionExtensions.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval collection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interval.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Interval.Interface;

    /// <summary>
    /// The date interval collection extensions.
    /// </summary>
    public static class DateIntervalCollectionExtensions
    {
        /// <summary>
        /// Assumption: The list parameter is already pre sorted.
        /// </summary>
        /// <param name="orderedDateIntervals"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetSortedDateTimes(this IEnumerable<IInterval<DateTime>> orderedDateIntervals)
        {

            return orderedDateIntervals.GetSortedEndPoints().Select(x => x.Value);
        }
    }
}