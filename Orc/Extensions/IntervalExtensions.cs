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
    public static class IntervalExtensions
    {
        /// <summary>
        /// The get edges.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<IEndPoint<DateTime>> GetEndPoints(this IInterval<DateTime> source)
        {
            return new List<IEndPoint<DateTime>> { source.Min, source.Max };
        }
    }
}