namespace Orc.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static DateTime Ceil( this DateTime date, TimeSpan span )
        {
            long ticks = (date.Ticks + span.Ticks - 1) / span.Ticks;
            return new DateTime( ticks * span.Ticks );
        }

        public static DateTime Floor( this DateTime date, TimeSpan span )
        {
            long ticks = date.Ticks / span.Ticks;
            return new DateTime( ticks * span.Ticks );
        }

        public static bool IsEqualTo( this DateTime d1, DateTime d2, int accuracyInMilliseconds )
        {
            return Math.Abs( d1.Ticks - d2.Ticks ) <= accuracyInMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        public static bool IsGreaterThan( this DateTime d1, DateTime d2, int accuracyInMilliseconds )
        {
            return (d1.Ticks - d2.Ticks) > accuracyInMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        public static bool IsLessThan( this DateTime d1, DateTime d2, int accuracyInMilliseconds )
        {
            return (d2.Ticks - d1.Ticks) > accuracyInMilliseconds * TimeSpan.TicksPerMillisecond;
        }

        // Found this code on http://stackoverflow.com/questions/1393696/c-sharp-rounding-datetime-objects
        public static DateTime Round( this DateTime date, TimeSpan span )
        {
            long ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime( ticks * span.Ticks );
        }

        public static DateTime ToNearestSecond( this DateTime d1 )
        {
            return d1.Round( new TimeSpan( 0, 0, 0, 1 ) );
        }

        public static DateTime TruncateToSecond( this DateTime original )
        {
            return new DateTime( original.Year, original.Month, original.Day, original.Hour, original.Minute, original.Second );
        }
    }
}