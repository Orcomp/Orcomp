using System;

namespace Orcomp.Interfaces
{
    public interface IDateRange
    {
        TimeSpan Duration { get; }

        DateTime EndTime { get; }

        DateTime StartTime { get; }

        bool Intersect( DateTime dateTime );
    }
}