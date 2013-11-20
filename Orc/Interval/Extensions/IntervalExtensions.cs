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
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<IEndPoint<T>> GetEndPoints<T>(this IInterval<T> source)
            where T : IComparable<T>
        {
            return new List<IEndPoint<T>> { source.Min, source.Max };
        }
    }
}