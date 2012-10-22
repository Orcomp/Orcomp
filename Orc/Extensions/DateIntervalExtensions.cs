// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalExtensions.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Orc.Entities;
    using Orc.Interface;

    /// <summary>
    /// The date interval extensions.
    /// </summary>
    public static class DateIntervalExtensions
    {
        /// <summary>
        /// Return the overlap between two intervals.
        /// </summary>
        /// <param name="source">
        /// </param>
        /// <param name="other">
        /// </param>
        /// <returns>
        /// The <see cref="DateInterval"/>.
        /// </returns>
        public static DateInterval GetOverlap(this DateInterval source, DateInterval other)
        {
            if (source.Overlaps(other) == false)
            {
                return null;
            }

            var newMin = source.Min.CompareTo(other.Min) > 0 ? source.Min : other.Min;
            var newMax = source.Max.CompareTo(other.Max) > 0 ? other.Max : source.Max;

            if (newMin.Value.CompareTo(newMax.Value) == 0 && !newMax.IsInclusive)
            {
                return null;
            }

            return new DateInterval(newMin.Value, newMax.Value, newMin.IsInclusive, newMax.IsInclusive);
        }

        /// <summary>
        /// The get edges.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<IEndPoint<DateTime>> GetEndPoints(this DateInterval source)
        {
            return new List<IEndPoint<DateTime>> { source.Min, source.Max };
        }
    }
}