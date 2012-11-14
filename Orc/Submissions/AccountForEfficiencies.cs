using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orc.Submissions
{
    using Orc.Entities;
    using Orc.Entities.RangeTree;
    using Orc.Interface;

    public static class AccountForEfficiencies
    {
        public static DateInterval thcristo(this DateInterval dateInterval, IList<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            if (dateIntervalEfficiencies.Count == 0)
                return dateInterval;

            DateIntervalCollection intvColl = new DateIntervalCollection(true);
            RangeTree<DateTime> intvTree = new RangeTree<DateTime>();

            //Put efficiency intervals in a collection and a range tree
            foreach (DateIntervalEfficiency eff in dateIntervalEfficiencies)
            {
                intvColl.Add(eff);
                intvTree.Add(eff);
            }

            //This is the interval tree which will contain the actual efficiencies to be used
            RangeTree<DateTime> effectiveTree = new RangeTree<DateTime>();

            //Add a default efficiency of 100% before the first efficiency interval
            effectiveTree.Add(new DateIntervalEfficiency(DateTime.MinValue, intvColl.DateEdges.First().Value));

            int nEdges = intvColl.DateEdges.Count;

            //Loop through the edges of all efficiency intervals
            //and connect them with a new efficiency interval
            //by taking into account the priorities
            for (int i = 0; i < nEdges; i++)
            {

                //We need to ignore an edge that coincides with the next one
                if ((i < nEdges - 1) && (intvColl.DateEdges[i].Value != intvColl.DateEdges[i + 1].Value))
                {
                    DateIntervalEfficiency eff = new DateIntervalEfficiency(intvColl.DateEdges[i].Value, intvColl.DateEdges[i + 1].Value);
                    var overlaps = intvTree.Query(eff).Cast<DateIntervalEfficiency>();
                    if (overlaps.Any())
                        eff.Efficiency = (overlaps
                                        .OrderByDescending(intv => (intv).Priority)
                                        .ThenBy(intv => (intv).Efficiency)
                                        .First()).Efficiency;
                    else
                        eff.Efficiency = 100;

                    effectiveTree.Add(eff);
                }
            }

            //Add a default efficiency of 100% after the last efficiency interval
            effectiveTree.Add(new DateIntervalEfficiency(intvColl.DateEdges.Last().Value, DateTime.MaxValue));

            DateInterval checkInterval;

            //Find the maximum interval where the action can take place
            if (fixedEndPoint == FixedEndPoint.Min)
                checkInterval = new DateInterval(dateInterval.Min.Value, DateTime.MaxValue);
            else
                checkInterval = new DateInterval(DateTime.MinValue, dateInterval.Max.Value);

            //Query the interval tree to find the necessary efficiency intervals 
            IEnumerable<IInterval<DateTime>> effectiveIntervals = effectiveTree.Query(checkInterval).OrderBy(intv => intv.Min);

            //If fixed point is at the end then take the intervals with reverse order
            if (fixedEndPoint == FixedEndPoint.Max)
                effectiveIntervals = effectiveIntervals.Reverse();

            //We have to "consume" this time
            double totalDays = dateInterval.Duration.TotalDays;

            //This is the actual time used
            double equivDays = 0;

            //This is the starting point in time
            DateTime currentDate = (fixedEndPoint == FixedEndPoint.Min ? dateInterval.Min.Value : dateInterval.Max.Value);

            int sign = (fixedEndPoint == FixedEndPoint.Min ? 1 : -1);

            DateInterval newInterval = dateInterval;

            //Loop through all efficiency intervals and try to "consume" totalDays of 100% efficiency
            //by increasing equivDays of actual efficiency
            foreach (DateIntervalEfficiency interval in effectiveIntervals)
            {
                double diffDays = Double.MaxValue;
                DateTime newDate = (sign == 1 ? DateTime.MaxValue : DateTime.MinValue);

                if (interval.Efficiency != 0)
                {
                    //Compute how much time is needed to complete the remaining task 
                    //given the current interval's efficiency
                    diffDays = totalDays * 100.0 / interval.Efficiency;
                    newDate = currentDate.AddDays(sign * diffDays);
                }

                if (interval.Efficiency == 0)
                {
                    //No actual work is done, 
                    //so just jump to the end of the interval
                    newDate = (sign == 1 ? interval.Max.Value : interval.Min.Value);
                    equivDays += (newDate - currentDate).TotalDays * sign;
                    currentDate = newDate;
                }
                else if ((sign > 0 && newDate > interval.Max.Value) || (sign < 0 && newDate < interval.Min.Value))
                {
                    //This interval is not sufficient to complete the task,
                    //so use all the interval's available time
                    newDate = (sign == 1 ? interval.Max.Value : interval.Min.Value);
                    equivDays += (newDate - currentDate).TotalDays * sign;

                    //Decrease the remaining task time and update the current point in time
                    totalDays -= (newDate - currentDate).TotalDays * sign * interval.Efficiency / 100.0;
                    currentDate = newDate;
                }
                else
                {
                    //The task can be completed in this interval
                    equivDays += diffDays;
                    newInterval = new DateInterval((sign == 1 ? dateInterval.Min.Value : dateInterval.Max.Value.AddDays(sign * equivDays)), (sign == 1 ? dateInterval.Min.Value.AddDays(sign * equivDays) : dateInterval.Max.Value));
                    break;
                }

            }

            return newInterval;
        }


        # region vrstks
        public static DateInterval vrstks(this DateInterval dateInterval, IList<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
              //--- STEP 1. Creating timeline

            List<DateIntervalEfficiency> timeline = new List<DateIntervalEfficiency>();

            int insertPoint = 0;
            foreach (var current in dateIntervalEfficiencies.OrderBy(i => i.Min))
            {
                bool added = false;
                for (int i = insertPoint; i < timeline.Count; i++)
                {
                    if (timeline[i].Overlaps(current))
                    {
                        // Previous  |-------|
                        // Current     |---|

                        List<DateIntervalEfficiency> fix = FixOverlap(timeline[i], current);
                            
                        //replace item with fixed intervals
                        timeline.RemoveAt(i);
                        insertPoint = i;
                        timeline.InsertRange(insertPoint, fix);
                        added = true;
                        break;
                    }
                }
                    
                if (!added)
                {
                    //Out of timeline - add to the end
                    insertPoint = timeline.Count;
                    timeline.Add(current);
                }
            }

//            if(timeline.Count == 0)
//                return dateInterval; //not affected by schedule


            //--- STEP 2. Time normalization
            if (fixedEndPoint == FixedEndPoint.Max)
                timeline.Reverse();

            DateTime now;
            if (fixedEndPoint == FixedEndPoint.Min)
            {
                now = dateInterval.Min.Value;
                while (timeline.Count > 0 && timeline[0].Max.Value < now)
                {
                    timeline.RemoveAt(0);
                }
            }
            else
            {
                now = dateInterval.Max.Value;
                while (timeline.Count > 0 && timeline[0].Min.Value > now)
                {
                    timeline.RemoveAt(0);
                }
            }

            // STEP 3. Time travel

            TimeSpan ZERO = TimeSpan.FromTicks(0);
            TimeSpan workDuration = ZERO;
            TimeSpan notAffected = dateInterval.Duration;

            foreach (var current in timeline)
            {
                TimeSpan scheduledTime;

                if (fixedEndPoint == FixedEndPoint.Min)
                {
                    if (now < current.Min.Value)
                    {
                        //We have a gap in the schedule - fill it
                        var gap = new DateIntervalEfficiency(now, current.Min.Value, 100);
                        notAffected -= gap.Duration;
                        workDuration += gap.Duration;
                        now = current.Min.Value;
                    }

                    if (notAffected > ZERO)
                        scheduledTime = current.Max.Value - now;
                    else
                        scheduledTime = ZERO; //work already completed
                }
                else
                {
                    if (now > current.Max.Value)
                    {
                        //We have a gap in the schedule - fill it
                        var gap = new DateIntervalEfficiency(current.Max.Value, now, 100);
                        notAffected -= gap.Duration;
                        workDuration += gap.Duration;
                        now = current.Max.Value;
                    }

                    if(notAffected > ZERO)
                        scheduledTime = now - current.Min.Value;
                    else
                        scheduledTime = ZERO; //work already completed
                }

                if (scheduledTime > ZERO)
                {
                    TimeSpan timeAt100;
                    if (current.Efficiency > 0)
                    {
                        //scheduled time -> real time
                        timeAt100 = TimeSpan.FromTicks((long)(scheduledTime.Ticks * current.Efficiency / 100));

                        if (notAffected > timeAt100)
                        {
                            notAffected -= timeAt100;
                        }
                        else
                        {
                            //Schedule rest of date interval
                            scheduledTime = TimeSpan.FromTicks((long)(notAffected.Ticks * 100 / current.Efficiency));
                            notAffected = ZERO;
                        }
                    }
                    else
                    {
                        //Zero efficiency - no real work done
                        timeAt100 = ZERO;
                    }

                    //Increase duration by 
                    workDuration += scheduledTime;

                    //If work completed
                    if (notAffected == ZERO)
                        break;

                    //Shift current time by effective time to the right direction
                    if (fixedEndPoint == FixedEndPoint.Min)
                    {
                        now += scheduledTime;
                    }
                    else
                        now -= scheduledTime;
                }
            }

            // Add not affected intervals to duration (not scheduled parts)
            workDuration += notAffected;

            if (fixedEndPoint == FixedEndPoint.Min)
            {
                return new DateInterval(dateInterval.Min.Value, dateInterval.Min.Value + workDuration, dateInterval.Min.IsInclusive, dateInterval.Max.IsInclusive);
            }
            else
            {
                return new DateInterval(dateInterval.Max.Value - workDuration, dateInterval.Max.Value, dateInterval.Min.IsInclusive, dateInterval.Max.IsInclusive);
            }
        }

        private static List<DateIntervalEfficiency> FixOverlap(DateIntervalEfficiency previous, DateIntervalEfficiency current)
        {
            var overlap = previous.GetOverlap(current);

            List<DateIntervalEfficiency> result = new List<DateIntervalEfficiency>();

            //head
            if (previous.Min.CompareTo(overlap.Min) < 0)
            {
                // Previous  |-----------|
                // Current       |-----------|
                // Result    |---|
                var interval = new DateIntervalEfficiency(previous.Min.Value, overlap.Min.Value, previous.Efficiency, previous.Priority, previous.Min.IsInclusive, overlap.Min.IsInclusive);
                result.Add(interval);
            }

            //overlap
            if (overlap.Max.CompareTo(overlap.Min) > 0)
            {
                // Previous  |-----------|
                // Current       |-----------|
                // Result        |-------|

                var interval = new DateIntervalEfficiency(overlap.Min.Value, overlap.Max.Value, previous.Efficiency, previous.Priority, overlap.Min.IsInclusive, overlap.Max.IsInclusive);

                //If efficiency intervals overlap the one with the highest priority will be used for the calculation
                if (previous.Priority < current.Priority)
                {
                    interval.Priority = current.Priority;
                    interval.Efficiency = current.Efficiency;
                }
                else if (previous.Priority == current.Priority)
                {
                    //If two efficiency intervals have the same priority the one with the lowest efficiency has precedence and will be used in the calculation
                    if (previous.Efficiency > current.Efficiency)
                        interval.Efficiency = current.Efficiency;
                }

                result.Add(interval);
            }

            //tail
            if (previous.Max.CompareTo(overlap.Max) > 0)
            {
                // Previous  |------------------|
                // Current       |-----------|
                // Result                    |--|, previous priority and efficiency

                var interval = new DateIntervalEfficiency(overlap.Max.Value, previous.Max.Value, previous.Efficiency, previous.Priority, overlap.Max.IsInclusive, previous.Max.IsInclusive);
                result.Add(interval);
            }
            else if (current.Max.CompareTo(overlap.Max) > 0)
            {
                // Previous  |-----------|
                // Current       |-----------|
                // Result                |---|, current priority and efficiency

                var interval = new DateIntervalEfficiency(overlap.Max.Value, current.Max.Value, current.Efficiency, current.Priority, overlap.Max.IsInclusive, current.Max.IsInclusive);
                result.Add(interval);
            }
            return result;
        }
        #endregion
    }
}
