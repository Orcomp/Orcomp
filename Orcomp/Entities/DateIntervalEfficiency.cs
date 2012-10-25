using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Orcomp.Interfaces;

namespace Orcomp.Entities
{
    public sealed class DateIntervalEfficiency : IDateRange
    {
        private DateRange DateRange { get; set; }

        #region Implementation of IDateRange

        public TimeSpan Duration { get { return DateRange.Duration; } }

        public DateTime EndTime { get { return DateRange.EndTime; } }

        public DateTime StartTime { get { return DateRange.StartTime; } }

        public bool Intersect( DateTime dateTime )
        {
            return DateRange.Intersect( dateTime );
        }

        #endregion

        public double Efficiency { get; private set; }

        public int Priority { get; set; }

        public DateIntervalEfficiency(DateTime startTime, DateTime endTime, double efficiency, int priority = 0)
        {
            DateRange = new DateRange( startTime, endTime );
            Efficiency = efficiency;
            Priority = priority;
        }
    }
}
