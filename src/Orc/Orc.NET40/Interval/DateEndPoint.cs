// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateEdge.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date edge.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interval
{
    using System;

    /// <summary>
    /// The date edge.
    /// </summary>
    public class DateEndPoint : EndPoint<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateEndPoint"/> class.
        /// </summary>
        /// <param name="interval">
        /// The interval.
        /// </param>
        /// <param name="endPointType">
        /// The edge type.
        /// </param>
        public DateEndPoint(DateInterval interval, EndPointType endPointType)
            : base(interval, endPointType)
        {
        }
    }
}