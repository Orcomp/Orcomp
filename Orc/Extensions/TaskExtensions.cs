namespace Orc.Extensions
{
    using System;

    using Orc.Entities;

    public static class TaskExtensions
    {
        public static Task ChangeEndTime( this Task task, DateTime endTime )
        {
            return Task.CreateUsingQuantity( task.StartTime, endTime, task.Quantity, task.ResourceName );
        }

        public static Task ChangeStartTime( this Task task, DateTime startTime )
        {
            return Task.CreateUsingQuantity( startTime, task.EndTime, task.Quantity, task.ResourceName );
        }

        public static Task ChangeTimes( this Task task, DateTime startTime, DateTime endTime )
        {
            return Task.CreateUsingQuantity( startTime, endTime, task.Quantity, task.ResourceName );
        }

        public static Task ChangeTimes( this Task task, DateTime startTime )
        {
            // In this case the duration stays the same.
            return Task.CreateUsingQuantity( startTime, startTime.Add( task.DateInterval.Duration ), task.Quantity, task.ResourceName );
        }

        public static Task ChangeTimesBy( this Task task, TimeSpan delay )
        {
            // In this case the duration stays the same.
            return Task.CreateUsingQuantity( task.StartTime.Add( delay ), task.EndTime.Add( delay ), task.Quantity, task.ResourceName );
        }
    }
}