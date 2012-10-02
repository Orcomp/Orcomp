using System;

using Orcomp.Entities;

namespace Orcomp.Extensions
{
    public static class TaskExtensions
    {
        public static Task ChangeEndTime( this Task task, DateTime endTime )
        {
            return Task.CreateUsingQuantity( task.DateRange.StartTime, endTime, task.Quantity, task.ResourceName );
        }

        public static Task ChangeStartTime( this Task task, DateTime startTime )
        {
            return Task.CreateUsingQuantity( startTime, task.DateRange.EndTime, task.Quantity, task.ResourceName );
        }

        public static Task ChangeTimes( this Task task, DateTime startTime, DateTime endTime )
        {
            return Task.CreateUsingQuantity( startTime, endTime, task.Quantity, task.ResourceName );
        }

        public static Task ChangeTimes( this Task task, DateTime startTime )
        {
            // In this case the duration stays the same.
            return Task.CreateUsingQuantity( startTime, startTime.Add( task.DateRange.Duration ), task.Quantity, task.ResourceName );
        }

        public static Task ChangeTimesBy( this Task task, TimeSpan delay )
        {
            // In this case the duration stays the same.
            return Task.CreateUsingQuantity( task.DateRange.StartTime.Add( delay ), task.DateRange.EndTime.Add( delay ), task.Quantity, task.ResourceName );
        }
    }
}