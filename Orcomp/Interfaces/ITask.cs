using System;

using Orcomp.Entities;

namespace Orcomp.Interfaces
{
    public interface ITask
    {
        DateRange DateRange { get; }

        double Quantity { get; }

        double QuantityPerHour { get; }

        string ResourceName { get; }

        TaskType TaskType { get; }

        double GetQuantity( DateTime date );
    }
}