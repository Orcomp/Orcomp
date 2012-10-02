using System;

using Orcomp.Interfaces;

namespace Orcomp.Entities
{
    public class Task : ITask, IEquatable<Task>
    {
        private DateRange _dateRange;

        private double _quantity;

        private double _quantityPerHour;

        private string _resourceName;

        private TaskType _taskType;

        private Task()
        {
        }

        public DateRange DateRange
        {
            get
            {
                return _dateRange;
            }
        }

        public double Quantity
        {
            get
            {
                return _quantity;
            }
        }

        public double QuantityPerHour
        {
            get
            {
                return _quantityPerHour;
            }
        }

        public string ResourceName
        {
            get
            {
                return _resourceName;
            }
        }

        public TaskType TaskType
        {
            get
            {
                return _taskType;
            }
        }

        public static Task CreateUsingQuantity( DateTime startTime, DateTime endTime, double quantity, string resourceName )
        {
            var dateRange = new DateRange( startTime, endTime );

            var task = new Task { _taskType = quantity >= 0 ? TaskType.Produce : TaskType.Consume, _dateRange = dateRange, _quantityPerHour = quantity / dateRange.Duration.TotalHours, _quantity = quantity, _resourceName = resourceName };

            return task;
        }

        public static Task CreateUsingQuantity( DateTime startTime, DateTime endTime, double quantity )
        {
            return CreateUsingQuantity( startTime, endTime, quantity, string.Empty );
        }

        public static Task CreateUsingQuantity( DateRange dateRange, double quantity )
        {
            return CreateUsingQuantity( dateRange.StartTime, dateRange.EndTime, quantity, string.Empty );
        }

        public static Task CreateUsingQuantityPerHour( DateTime startTime, DateTime endTime, double quantityPerHour, string resourceName )
        {
            var dateRange = new DateRange( startTime, endTime );

            var task = new Task { _taskType = quantityPerHour >= 0 ? TaskType.Produce : TaskType.Consume, _dateRange = dateRange, _quantityPerHour = quantityPerHour, _quantity = quantityPerHour * dateRange.Duration.TotalHours, _resourceName = resourceName };

            return task;
        }

        public static Task CreateUsingQuantityPerHour( DateTime startTime, DateTime endTime, double quantityPerHour )
        {
            return CreateUsingQuantityPerHour( startTime, endTime, quantityPerHour, string.Empty );
        }

        public static Task CreateUsingQuantityPerHour( DateRange dateRange, double quantityPerHour )
        {
            return CreateUsingQuantityPerHour( dateRange.StartTime, dateRange.EndTime, quantityPerHour, string.Empty );
        }

        public override bool Equals( object obj )
        {
            var task = obj as Task;
            return task != null && Equals( task );
        }

        public bool Equals( Task other )
        {
            return other != null && other.GetType() == GetType() && other.DateRange == DateRange && other.Quantity == Quantity && other.QuantityPerHour == QuantityPerHour && other.ResourceName == ResourceName && other.TaskType == TaskType;
        }

        public override int GetHashCode()
        {
            return DateRange.GetHashCode() ^ Quantity.GetHashCode() ^ QuantityPerHour.GetHashCode() ^ ResourceName.GetHashCode() ^ TaskType.GetHashCode();
        }

        public double GetQuantity( DateTime date )
        {
            if ( date >= DateRange.EndTime )
            {
                return Quantity;
            }

            if ( date <= DateRange.StartTime )
            {
                return 0;
            }

            return QuantityPerHour * (date - DateRange.StartTime).TotalHours;
        }

        public override string ToString()
        {
            return string.Format( "{0}, Quantity: {1}", DateRange, Quantity );
        }
    }
}