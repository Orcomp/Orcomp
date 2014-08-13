namespace Orc.Entities
{
    using System;

    using Orc.Entities.Interface;
    using Orc.Interval;

    public class Task : ITask, IEquatable<Task>
    {
        private DateInterval _dateInterval;

        private double _quantity;

        private double _quantityPerHour;

        private string _resourceName;

        private TaskType _taskType;

        private Task()
        {
        }

        public DateInterval DateInterval
        {
            get
            {
                return this._dateInterval;
            }
        }

        public double Quantity
        {
            get
            {
                return this._quantity;
            }
        }

        public double QuantityPerHour
        {
            get
            {
                return this._quantityPerHour;
            }
        }

        public string ResourceName
        {
            get
            {
                return this._resourceName;
            }
        }

        public TaskType TaskType
        {
            get
            {
                return this._taskType;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this.DateInterval.Min.Value;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this.DateInterval.Max.Value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return this.DateInterval.Duration;
            }
        }

        public static Task CreateUsingQuantity( DateTime startTime, DateTime endTime, double quantity, string resourceName )
        {
            var dateRange = new DateInterval(startTime, endTime);

            var task = new Task { _taskType = quantity >= 0 ? TaskType.Produce : TaskType.Consume, _dateInterval = dateRange, _quantityPerHour = quantity / dateRange.Duration.TotalHours, _quantity = quantity, _resourceName = resourceName };

            return task;
        }

        public static Task CreateUsingQuantity( DateTime startTime, DateTime endTime, double quantity )
        {
            return CreateUsingQuantity( startTime, endTime, quantity, string.Empty );
        }

        public static Task CreateUsingQuantity(DateInterval dateRange, double quantity)
        {
            return CreateUsingQuantity( dateRange.Min.Value, dateRange.Max.Value, quantity, string.Empty );
        }

        public static Task CreateUsingQuantityPerHour( DateTime startTime, DateTime endTime, double quantityPerHour, string resourceName )
        {
            var dateRange = new DateInterval(startTime, endTime);

            var task = new Task { _taskType = quantityPerHour >= 0 ? TaskType.Produce : TaskType.Consume, _dateInterval = dateRange, _quantityPerHour = quantityPerHour, _quantity = quantityPerHour * dateRange.Duration.TotalHours, _resourceName = resourceName };

            return task;
        }

        public static Task CreateUsingQuantityPerHour( DateTime startTime, DateTime endTime, double quantityPerHour )
        {
            return CreateUsingQuantityPerHour( startTime, endTime, quantityPerHour, string.Empty );
        }

        public static Task CreateUsingQuantityPerHour(DateInterval dateRange, double quantityPerHour)
        {
            return CreateUsingQuantityPerHour( dateRange.Min.Value, dateRange.Max.Value, quantityPerHour, string.Empty );
        }

        public override bool Equals( object obj )
        {
            var task = obj as Task;
            return task != null && this.Equals( task );
        }

        public bool Equals( Task other )
        {
            return other != null && other.GetType() == this.GetType() && other.DateInterval == this.DateInterval && other.Quantity == this.Quantity && other.QuantityPerHour == this.QuantityPerHour && other.ResourceName == this.ResourceName && other.TaskType == this.TaskType;
        }

        public override int GetHashCode()
        {
            return this.DateInterval.GetHashCode() ^ this.Quantity.GetHashCode() ^ this.QuantityPerHour.GetHashCode() ^ this.ResourceName.GetHashCode() ^ this.TaskType.GetHashCode();
        }

        public double GetQuantity( DateTime date )
        {
            if ( date >= this.DateInterval.Max.Value )
            {
                return this.Quantity;
            }

            if ( date <= this.DateInterval.Min.Value )
            {
                return 0;
            }

            return this.QuantityPerHour * (date - this.DateInterval.Min.Value).TotalHours;
        }

        public override string ToString()
        {
            return string.Format( "{0}, Quantity: {1}", this.DateInterval, this.Quantity );
        }
    }
}