// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalEfficiency.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Date Interval with efficiency and Priority
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Entities
{
    using System;

    /// <summary>
    /// The date interval efficiency.
    /// </summary>
    public class DateIntervalEfficiency : DateInterval
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateIntervalEfficiency"/> class.
        /// </summary>
        /// <param name="minValue">
        /// The min value.
        /// </param>
        /// <param name="maxValue">
        /// The max value.
        /// </param>
        /// <param name="efficiency">
        /// The efficiency.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// <param name="isMinInclusive">
        /// The is min inclusive.
        /// </param>
        /// <param name="isMaxInclusive">
        /// The is max inclusive.
        /// </param>
        public DateIntervalEfficiency(
            DateTime minValue, 
            DateTime maxValue, 
            double efficiency = 100, 
            int priority = 0, 
            bool isMinInclusive = true, 
            bool isMaxInclusive = false)
            : base(minValue, maxValue, isMinInclusive, isMaxInclusive)
        {
            this.Efficiency = efficiency;
            this.Priority = priority;
        }


        public DateIntervalEfficiency(DateInterval dateInterval, double efficiency = 100, int priority = 0)
            :base(dateInterval.Min.Value, dateInterval.Max.Value, dateInterval.Min.IsInclusive, dateInterval.Max.IsInclusive)
        {
            this.Efficiency = efficiency;
            this.Priority = priority;
        }

        /// <summary>
        /// Gets or sets the efficiency.
        /// </summary>
        /// <value>
        /// The efficiency.
        /// </value>
        public double Efficiency { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; set; }
    }
}