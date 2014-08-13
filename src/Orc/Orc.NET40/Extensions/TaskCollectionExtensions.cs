namespace Orc.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Orc.Entities;
    using Orc.Entities.Interface;
    using Orc.Interval;

    public static class TaskCollectionExtensions
    {
        public static IList<ITask> Collapse(this IEnumerable<ITask> taskCollection)
        {
            var taskList = taskCollection.ToList();

            if (taskCollection == null)
            {
                return null;
            }

            var output = new Collection<ITask>();

            // Grabbing all time split points and sorting them
            List<DateTime> timeEvents = taskList.SelectMany(task => new[] { task.StartTime, task.EndTime })
                                                     .Distinct()
                                                     .OrderBy(x => x)
                                                     .ToList();


            for (var i = 0; i < timeEvents.Count - 1; i++)
            {
                var newSpan = new DateInterval(timeEvents[i], timeEvents[i + 1]);

                var overlappingTasks = taskList.Where(x => x.DateInterval.Overlaps(newSpan));

                if (overlappingTasks.Any())
                {
                    var quantityPerHour = overlappingTasks.Sum(x => x.QuantityPerHour);
                    output.Add(Task.CreateUsingQuantityPerHour(newSpan, quantityPerHour));
                }
            }

            return output;
        }

        public static IEnumerable<KeyValuePair<DateTime, double>> GetDataPoints( this IEnumerable<ITask> taskCollection, double initialLevel )
        {
            double runningLevel = initialLevel;

            var dataPoints = new List<KeyValuePair<DateTime, double>>();

            IList<ITask> collapsedTasks = taskCollection.Collapse();

            if ( !collapsedTasks.Any() )
            {
                return dataPoints;
            }

            DateTime lastEndTime = DateTime.MinValue;

            foreach ( ITask item in collapsedTasks )
            {
                if ( item.StartTime > lastEndTime )
                {
                    dataPoints.Add( new KeyValuePair<DateTime, double>( item.StartTime, runningLevel ) );
                }

                runningLevel += item.Quantity;
                dataPoints.Add( new KeyValuePair<DateTime, double>( item.EndTime, runningLevel ) );
            }

            return dataPoints;
        }

        private static int MAX_ITER = 1000;

        public static double GetQuantity( this IEnumerable<ITask> taskCollection, DateTime date )
        {
            return taskCollection.Sum( task => task.GetQuantity( date ) );
        }

        public static bool WillBreachMinLevel(this IEnumerable<ITask> taskCollection, double minLevel, double initialValue)
        {
            if (initialValue < minLevel)
                throw new ArgumentException(string.Format("InitialValue: {0}, must be greater than or equal to MinLevel: {1}", initialValue, minLevel));
            else
                return taskCollection.GetDataPoints(initialValue).Any(kv => kv.Value < minLevel);
        }

        public static bool WillBreachMaxLevel(this IEnumerable<ITask> taskCollection, double maxLevel, double initialValue)
        {
            if (initialValue > maxLevel)
                throw new ArgumentException(string.Format("InitialValue: {0}, must be less than or equal to MaxLevel: {1}", initialValue, maxLevel));
            else
                return taskCollection.GetDataPoints(initialValue).Any(kv => kv.Value > maxLevel);
        }

        public static bool WillStayWithinRange(this IEnumerable<ITask> taskCollection, double minLevel, double maxLevel, double initialValue)
        {
            return !taskCollection.WillBreachMinLevel(minLevel, initialValue) && !taskCollection.WillBreachMaxLevel(maxLevel, initialValue);
        }

        public static IEnumerable<ITask> TryToMakeTasksComply(this IEnumerable<ITask> taskCollection, double minLevel, double maxLevel, double initialValue)
        {
            //Check if initial value is inside allowed range
            if ((initialValue > maxLevel) || (initialValue < minLevel))
                throw new ArgumentException(string.Format("InitialValue: {0}, must be between MinLevel: {1} and MaxLevel: {2}", initialValue, minLevel, maxLevel));

            //Check if final quantity cannot be inside allowed range
            double finalQuantity = initialValue + taskCollection.Sum(task => task.Quantity);
            if ((finalQuantity > maxLevel) || (finalQuantity < minLevel))
                return null;

            var dataPoints = taskCollection.GetDataPoints(initialValue);

            //Find the first point that is outside of allowed range
            KeyValuePair<DateTime, double> outOfRangePoint;
            if (!FindOutOfRangePoint(dataPoints, minLevel, maxLevel, out outOfRangePoint))
                return taskCollection;

            //Calculate how much quantity overshoot/undershoot we have
            double diffQ = CalcQuantityDifference(outOfRangePoint, minLevel, maxLevel);

            //Find where to move the end time of the tasks
            DateTime moveToTime;
            if (!FindMoveToTime(dataPoints, outOfRangePoint, minLevel, maxLevel, out moveToTime))
                return null;

            Collection<ITask> newTaskCollection = new Collection<ITask>(taskCollection.ToList());

            //Cache indexes of the collection for fast retrieval
            Dictionary<int, int> indexDict = new Dictionary<int, int>();
            for (int i = 0; i < newTaskCollection.Count; i++)
                indexDict.Add(RuntimeHelpers.GetHashCode(newTaskCollection[i]),  i);

            int iterations = 0;

            //Loop until the overshoot/undershoot quantity is zero
            while (diffQ != 0)
            {
                //check for infinite iterations
                if (iterations >= MAX_ITER)
                    return null;

                int sign = diffQ > 0 ? 1 : -1;

                //Find the tasks that should be moved 
                var tasksToMove = newTaskCollection.Where(task => (task.Quantity * sign > 0) && (task.StartTime < outOfRangePoint.Key))
                                                                                    .OrderByDescending(task => task.StartTime);

                if (!tasksToMove.Any())
                    return null;

                //Loop through found tasks to decrease the overshoot/undershoot quantity
                foreach (Task task in tasksToMove)
                {
                    //Last task cannot be moved
                    if (indexDict[RuntimeHelpers.GetHashCode(task)] == newTaskCollection.Count - 1)
                        continue;

                    double oldQuantity = task.GetQuantity(outOfRangePoint.Key);
                    Task newTask;

                    DateTime newStartTime;

                    if (task.EndTime < moveToTime)
                        newStartTime = moveToTime.AddHours(-1 * task.Duration.TotalHours);
                    else
                        if (oldQuantity * sign <= diffQ * sign)
                            newStartTime = outOfRangePoint.Key;
                        else
                            newStartTime = task.StartTime.AddHours(diffQ / task.QuantityPerHour);

                    //Do not move task further than the next task of the same type in sequence
                    DateTime maxAllowedStartTime = GetMaxAllowedStartTime(task, newTaskCollection, indexDict);
                    if (newStartTime > maxAllowedStartTime)
                        newStartTime = maxAllowedStartTime;

                    if (newStartTime > task.StartTime)
                    {
                        newTask = task.ChangeTimes(newStartTime);
                        diffQ -= oldQuantity - newTask.GetQuantity(outOfRangePoint.Key);

                        //Replace the old task with the new one
                        int oldTaskId = RuntimeHelpers.GetHashCode(task);
                        int index = indexDict[oldTaskId];
                        newTaskCollection[index] = newTask;
                        indexDict.Remove(oldTaskId);
                        indexDict.Add(RuntimeHelpers.GetHashCode(newTask), index);

                        //If overshoot/undershoot quantity is zero then exit loop
                        if (diffQ * sign <= 0)
                            break;
                    }
                }

                //Recalculate new graph and prepare for next iteration
                dataPoints = newTaskCollection.GetDataPoints(initialValue);

                if (!FindOutOfRangePoint(dataPoints, minLevel, maxLevel, out outOfRangePoint))
                    return newTaskCollection;

                diffQ = CalcQuantityDifference(outOfRangePoint, minLevel, maxLevel);

                if (!FindMoveToTime(dataPoints, outOfRangePoint, minLevel, maxLevel, out moveToTime))
                    return null;

                iterations++;
            }

            return newTaskCollection;
        }

        private static bool FindOutOfRangePoint(IEnumerable<KeyValuePair<DateTime, double>> dataPoints, double minLevel, double maxLevel, out KeyValuePair<DateTime, double> outOfRangePoint)
        {
            outOfRangePoint = dataPoints.FirstOrDefault(kv => (kv.Value < minLevel) || (kv.Value > maxLevel));
            if (outOfRangePoint.Equals(default(KeyValuePair<DateTime, double>)))
                return false;
            else
                return true;
        }

        private static double CalcQuantityDifference(KeyValuePair<DateTime, double> outOfRangePoint, double minLevel, double maxLevel)
        {
            return outOfRangePoint.Value > maxLevel ? outOfRangePoint.Value - maxLevel : (outOfRangePoint.Value < minLevel ? outOfRangePoint.Value - minLevel : 0);
        }

        private static bool FindMoveToTime(IEnumerable<KeyValuePair<DateTime, double>> dataPoints, KeyValuePair<DateTime, double> outOfRangePoint, double minLevel, double maxLevel, out DateTime moveToTime)
        {
            moveToTime = DateTime.MaxValue;

            //Find the first point after overshoot/undershoot that is inside the allowed range or outside but in the opposite direction
            KeyValuePair<DateTime, double> candidatePoint = dataPoints.FirstOrDefault(kv => (kv.Key > outOfRangePoint.Key) && (((outOfRangePoint.Value < minLevel) && (kv.Value >= minLevel)) || ((outOfRangePoint.Value > maxLevel) && (kv.Value <= maxLevel))));
            if (candidatePoint.Equals(default(KeyValuePair<DateTime, double>)))
                return false;

            //Check if the found point lies on the maxLevel or minLevel line depending on what we have (overshoot or undershoot)
            if (((candidatePoint.Value == maxLevel) && (outOfRangePoint.Value > maxLevel)) || ((candidatePoint.Value == minLevel) && (outOfRangePoint.Value < minLevel)))
                moveToTime = candidatePoint.Key;
            else
            {
                //Point is inside allowed range, so calculate where the graph crosses the maxLevel or minLevel line, it must be earlier in time
                KeyValuePair<DateTime, double> onLimitPoint = dataPoints.LastOrDefault(kv => kv.Key < candidatePoint.Key);
                if (onLimitPoint.Equals(default(KeyValuePair<DateTime, double>)))
                    return false;
                moveToTime = onLimitPoint.Key.AddHours((candidatePoint.Key - onLimitPoint.Key).TotalHours * ((onLimitPoint.Value - (outOfRangePoint.Value > maxLevel ? maxLevel : minLevel)) / (onLimitPoint.Value - candidatePoint.Value)));
            }

            return true;
        }

        private static DateTime GetMaxAllowedStartTime(ITask task, Collection<ITask> taskCollection, IDictionary<int, int> indexDict)
        {
            int taskIndex = indexDict[RuntimeHelpers.GetHashCode(task)];

            for (int i = taskIndex + 1; i < taskCollection.Count; i++)
            {
                if (taskCollection[i].TaskType == task.TaskType)
                    return taskCollection[i].StartTime;
            }

            return DateTime.MaxValue;
        }
    }
}