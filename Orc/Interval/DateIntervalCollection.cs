// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalCollection.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interval
{
    using System;
    using System.Collections.Generic;

    using Orc.Interval.Interface;

    /// <summary>
    /// The date interval collection.
    /// </summary>
    public class DateIntervalCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateIntervalCollection"/> class.
        /// </summary>
        /// <param name="canOverlap">
        /// The can overlap.
        /// </param>
        public DateIntervalCollection(bool canOverlap = false)
        {
            this.DateIntervals = new List<DateInterval>();
            this.DateEdges = new List<IEndPoint<DateTime>>();

            this.CanOverlap = canOverlap;
        }

        /// <summary>
        /// Gets the date intervals.
        /// </summary>
        /// <value>
        /// The date intervals.
        /// </value>
        public List<DateInterval> DateIntervals { get; private set; }

        /// <summary>
        /// Gets the date edges.
        /// </summary>
        /// <value>
        /// The date edges.
        /// </value>
        public List<IEndPoint<DateTime>> DateEdges { get; private set; }

        /// <summary>
        /// Gets a value indicating whether can overlap.
        /// </summary>
        /// <value>
        /// The can overlap.
        /// </value>
        public bool CanOverlap { get; private set; }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public int Depth { get; private set; }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Add(DateInterval item)
        {
            this.DateIntervals.Add(item);

            var min = item.Min;
            var max = item.Max;

            // Add these items into the DateEdges list
            int index = this.DateEdges.BinarySearch(min);
            int startIndex = index + 1;

            if (index < 0)
            {
                startIndex = ~index;
                this.DateEdges.Insert(startIndex, min);
            }
            else
            {
                this.DateEdges.Insert(startIndex, min);
            }

            index = this.DateEdges.BinarySearch(startIndex, this.DateEdges.Count - startIndex, max, null);

            if (index < 0)
            {
                int endIndex = ~index;
                this.DateEdges.Insert(endIndex, max);
            }
            else
            {
                int endIndex = index + 1;
                this.DateEdges.Insert(endIndex, max);
            }
        }
    }
}