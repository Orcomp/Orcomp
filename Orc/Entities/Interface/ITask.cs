namespace Orc.Entities.Interface
{
    using System;

    using Orc.Interval;

    public interface ITask
    {
        DateInterval DateInterval { get; }

        double Quantity { get; }

        double QuantityPerHour { get; }

        string ResourceName { get; }

        TaskType TaskType { get; }

        double GetQuantity( DateTime date );

        DateTime StartTime { get; }

        DateTime EndTime { get; }

        TimeSpan Duration { get; }
    }
}