// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateInterval.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interval
{
    using System;

    using Orc.Interval.Interface;

    /// <summary>
    /// The date interval.
    /// By Default the min endpoint is inclusive and the max is exclusive
    /// </summary>
    public class DateInterval : Interval<DateTime>, IDateInterval, IComparable<DateInterval>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateInterval"/> class.
        /// By Default the min point is inclusive and the max point is exclusive.
        /// </summary>
        /// <param name="minValue">
        /// Left most value (or Min).
        /// </param>
        /// <param name="maxValue">
        /// Right most value (or Max).
        /// </param>
        /// <param name="isMinInclusive">
        /// The left most point is inclusive.
        /// </param>
        /// <param name="isMaxInclusive">
        /// The right most point is inclusive.
        /// </param>
        public DateInterval(
            DateTime minValue, DateTime maxValue, bool isMinInclusive = true, bool isMaxInclusive = false)
            : base(minValue, maxValue, isMinInclusive, isMaxInclusive)
        {
            this.Duration = maxValue - minValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateInterval"/> class.
        /// By Default the min point is inclusive and the max point is exclusive.
        /// </summary>
        /// <param name="minValue">
        /// Left most value (or Min).
        /// </param>
        /// <param name="duration">
        /// Interval duration
        /// </param>
        /// <param name="isMinInclusive">
        /// The left most point is inclusive.
        /// </param>
        /// <param name="isMaxInclusive">
        /// The right most point is inclusive.
        /// </param>
        public DateInterval(
            DateTime minValue, TimeSpan duration, bool isMinInclusive = true, bool isMaxInclusive = false)
            : base(minValue, minValue.Add(duration), isMinInclusive, isMaxInclusive)
        {
            this.Duration = duration;
        }

        /// <summary>
        /// Instantiate DateInterval from Interval{DateTime}
        /// </summary>
        /// <param name="dateTimeInterval"></param>
        public DateInterval(IInterval<DateTime> dateTimeInterval )
            : this(dateTimeInterval.Min.Value, dateTimeInterval.Max.Value, dateTimeInterval.Min.IsInclusive, dateTimeInterval.Max.IsInclusive)
        { 
        }

        /// <summary>
        /// Instantiate DateInterval from EndPoints data.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public DateInterval(DateEndPoint start, DateEndPoint end)
            :this(start.Value, end.Value, start.IsInclusive, end.IsInclusive)
        {
        }

        /// <summary>
        /// Return the overlap with other interval
        /// </summary>
        /// <param name="other">
        /// </param>
        /// <returns>
        /// The <see cref="DateInterval"/>.
        /// </returns>
        public new IInterval<DateTime> GetOverlap(IInterval<DateTime> other)
        {
            var dateTimeInterval = base.GetOverlap(other);

            return dateTimeInterval == null ? null : new DateInterval(dateTimeInterval);
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Get the Start time
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return Min.Value;
            }
        }

        /// <summary>
        /// Get the End time
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return Max.Value;
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// Smallest Min comes first. If two Min have the same value. Smallest Max comes first.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(DateInterval other)
        {
            if (other == null)
            {
                return 1;
            }

            var result = this.Min.CompareTo(other.Min);

            if (result == 0)
            {
                result = this.Max.CompareTo(other.Max);
            }

            return result;
        }
    }
}