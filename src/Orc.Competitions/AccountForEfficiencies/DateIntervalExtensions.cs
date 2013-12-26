﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalExtensions.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Competitions.AccountForEfficiencies
{
    using System.Collections.Generic;

    using Orc.Interval;

    /// <summary>
    /// The date interval extensions.
    /// </summary>
    public static class DateIntervalExtensions
    {
        /// <summary>
        /// This method will accept a dateInterval and calcualte a new dateInterval taking into account the dateIntervalEfficiencies
        /// ASSUMPTION: the input dateInterval has an efficiency of 100%.
        /// </summary>
        /// <param name="dateInterval">
        /// </param>
        /// <param name="dateIntervalEfficiencies">
        /// </param>
        /// <param name="fixedEndPoint">
        /// </param>
        /// <returns>
        /// The <see cref="DateInterval"/>.
        /// </returns>
        public static DateInterval AccountForEfficiencies(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            return Orc.Submissions.AccountForEfficiencies.AccountForEfficiencies.Quicks01lver3(dateInterval, dateIntervalEfficiencies, fixedEndPoint);
        }
    }
}