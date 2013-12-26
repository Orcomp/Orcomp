// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDateInterval.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The DateInterval interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interval.Interface
{
    using System;

    /// <summary>
    /// The DateInterval interface.
    /// </summary>
    public interface IDateInterval : IInterval<DateTime>
    {
        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        TimeSpan Duration { get; }

        /// <summary>
        /// DateInterval Start time
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// DateInterval End time
        /// </summary>
        DateTime EndTime { get; }
    }
}