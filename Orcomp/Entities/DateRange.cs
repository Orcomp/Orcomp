using System;

using Orcomp.Interfaces;

namespace Orcomp.Entities
{
    public sealed class DateRange : IDateRange, IComparable, IEquatable<DateRange>
    {
        private readonly TimeSpan _duration;

        private readonly DateTime _endTime;

        private readonly DateTime _startTime;

        public DateRange( DateTime startTime, DateTime endTime )
        {
            if ( endTime < startTime )
            {
                throw new ArgumentException( string.Format( "EndTime: {0}, must be greater than StartTime: {1}", endTime, startTime ) );
            }

            _startTime = startTime;
            _endTime = endTime;
            _duration = endTime - startTime;
        }

        public DateRange( DateTime startTime, TimeSpan duration )
            : this( startTime, startTime.Add( duration ) )
        {
        }

        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        }

        public static bool operator ==( DateRange a, DateRange b )
        {
            // If both are null, or both are same instance, return true.
            if ( ReferenceEquals( a, b ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ( ((object) a == null) || ((object) b == null) )
            {
                return false;
            }

            // Return true if the fields match:
            return a.StartTime == b.StartTime && a.EndTime == b.EndTime;
        }

        public static bool operator !=( DateRange a, DateRange b )
        {
            return !(a == b);
        }

        /// <summary>
        ///  Earliest start dates come first. If two date ranges have the same start date, the one with the 
        /// shortest duration comes first.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo( object obj )
        {
            if ( obj == null )
            {
                return 1;
            }

            var otherDateRange = obj as DateRange;

            if(otherDateRange == null)
            {
                return 1;
            }

            if ( StartTime < otherDateRange.StartTime )
            {
                return -1;
            }

            if ( otherDateRange.StartTime == StartTime )
            {
                if ( Duration < otherDateRange.Duration )
                {
                    return -1;
                }

                return Duration > otherDateRange.Duration ? 1 : 0;
            }

            return 1;
        }

        public override bool Equals( object obj )
        {
            var dateRange = obj as DateRange;
            return dateRange != null && Equals( dateRange );
        }

        public bool Equals( DateRange other )
        {
            return other != null && other.GetType() == GetType() && other.StartTime == StartTime && other.EndTime == EndTime;
        }

        public override int GetHashCode()
        {
            return StartTime.GetHashCode() ^ EndTime.GetHashCode();
        }

        public DateRange GetOverlap( DateRange dateRange )
        {
            if ( Overlaps( dateRange ) )
            {
                return new DateRange( StartTime > dateRange.StartTime ? StartTime : dateRange.StartTime, EndTime < dateRange.EndTime ? EndTime : dateRange.EndTime );
            }

            return null;
        }

        public bool Intersect( DateTime dateTime )
        {
            return (dateTime >= StartTime) && (dateTime <= EndTime);
        }

        public bool Overlaps( DateRange dateRange )
        {
            return ((EndTime > dateRange.StartTime) && (dateRange.EndTime > StartTime));
        }

        public override string ToString()
        {
            return string.Format( "{0} - {1}", StartTime, EndTime );
        }
    }
}