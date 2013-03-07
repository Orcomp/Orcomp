namespace Orc.Submissions.AccountForEfficiencies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using C5;

    using Orc.DataStructures.RangeTree;
    using Orc.Entities;
    using Orc.DataStructures.IntervalTreeRB;
    using Orc.DataStructures.RangeTree;
    using Orc.Interface;
    using Orc.Interval;
    using Orc.Interval.Interface;
    using Orc.Submissions.AccountForEfficiencies.SpecialIntervalTree;

    public static class AccountForEfficiencies
    {
        #region thcristo
        public static DateInterval thcristo(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
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

        public static DateInterval thcristo2(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            if (dateIntervalEfficiencies.Count == 0)
                return dateInterval;

            DateIntervalCollection intvColl = new DateIntervalCollection(true);
            IntervalTree<DateTime> intvTree = new IntervalTree<DateTime>();

            //Put efficiency intervals in a collection and a range tree
            foreach (DateIntervalEfficiency eff in dateIntervalEfficiencies)
            {
                intvColl.Add(eff);
                intvTree.Add(eff);
            }

            //Add a default efficiency of 100% before the first
            //and after the last efficiency interval
            DateIntervalEfficiency effStart = new DateIntervalEfficiency(DateTime.MinValue, intvColl.DateEdges.First().Value);
            DateIntervalEfficiency effEnd = new DateIntervalEfficiency(intvColl.DateEdges.Last().Value, DateTime.MaxValue);
            intvColl.Add(effStart);
            intvColl.Add(effEnd);
            intvTree.Add(effStart);
            intvTree.Add(effEnd);

            int numEdges = intvColl.DateEdges.Count;

            int startEdge, endEdge, sign;
            DateTime currentDate;

            //Find the range of edges that are of interest
            if (fixedEndPoint == FixedEndPoint.Min)
            {
                currentDate = dateInterval.Min.Value;
                startEdge = intvColl.DateEdges.BinarySearch(dateInterval.Min);
                if (startEdge < 0)
                    startEdge = ~startEdge - 1;
                endEdge = numEdges - 1;
                sign = 1;
            }
            else
            {
                currentDate = dateInterval.Max.Value;
                startEdge = intvColl.DateEdges.BinarySearch(dateInterval.Max);
                if (startEdge < 0)
                    startEdge = ~startEdge;
                endEdge = 0;
                sign = -1;
            }

            //We have to "consume" this time
            double totalDays = dateInterval.Duration.TotalDays;

            //This is the actual time used
            double equivDays = 0;

            DateInterval newInterval = dateInterval;
            int currentEdge = startEdge;

            //Loop through the edges of interest
            //and connect them with a new efficiency interval
            //by taking into account the priorities.
            //At the same time, try to "consume" totalDays of 100% efficiency
            //by increasing equivDays of actual efficiency
            while (currentEdge != endEdge)
            {
                //We need to ignore an edge that coincides with the next one
                if (intvColl.DateEdges[currentEdge].Value != intvColl.DateEdges[currentEdge + sign].Value)
                {
                    int minEdge = (sign == 1 ? currentEdge : currentEdge - 1);
                    int maxEdge = (sign == 1 ? currentEdge + 1 : currentEdge);

                    DateInterval currentInterval = new DateInterval(intvColl.DateEdges[minEdge].Value, intvColl.DateEdges[maxEdge].Value);
                    IEnumerable<DateIntervalEfficiency> overlaps = intvTree.Query(currentInterval).Cast<DateIntervalEfficiency>();

                    double efficiency = 100;

                    if (overlaps.Any())
                        efficiency = (overlaps
                                        .OrderByDescending(intv => intv.Priority)
                                        .ThenBy(intv => intv.Efficiency)
                                        .First()).Efficiency;

                    double diffDays = Double.MaxValue;
                    DateTime newDate = (sign == 1 ? DateTime.MaxValue : DateTime.MinValue);

                    if (efficiency != 0)
                    {
                        //Compute how much time is needed to complete the remaining task 
                        //given the current interval's efficiency
                        diffDays = totalDays * 100.0 / efficiency;
                        newDate = currentDate.AddDays(sign * diffDays);
                    }

                    if (efficiency == 0)
                    {
                        //No actual work is done, 
                        //so just jump to the end of the interval
                        newDate = (sign == 1 ? currentInterval.Max.Value : currentInterval.Min.Value);
                        equivDays += (newDate - currentDate).TotalDays * sign;
                        currentDate = newDate;
                    }
                    else if ((sign > 0 && newDate > currentInterval.Max.Value) || (sign < 0 && newDate < currentInterval.Min.Value))
                    {
                        //This interval is not sufficient to complete the task,
                        //so use all the interval's available time
                        newDate = (sign == 1 ? currentInterval.Max.Value : currentInterval.Min.Value);
                        equivDays += (newDate - currentDate).TotalDays * sign;

                        //Decrease the remaining task time and update the current point in time
                        totalDays -= (newDate - currentDate).TotalDays * sign * efficiency / 100.0;
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

                currentEdge += sign;
            }

            return newInterval;
        }

        /// <summary>
        /// This class compares object IDs of efficiency intervals 
        /// based on priority, efficiency and object id.
        /// </summary>
        private class DateIntervalEfficiencyComparer : System.Collections.Generic.Comparer<int>
        {
            private Dictionary<int, DateIntervalEfficiency> dictIntervals;

            public DateIntervalEfficiencyComparer(Dictionary<int, DateIntervalEfficiency> dictIntervals)
            {
                this.dictIntervals = dictIntervals;
            }

            public override int Compare(int x, int y)
            {
                DateIntervalEfficiency intervalx = this.dictIntervals[x];
                DateIntervalEfficiency intervaly = this.dictIntervals[y];

                if (intervalx.Priority != intervaly.Priority)
                    return (-1) * intervalx.Priority.CompareTo(intervaly.Priority);
                else if (intervalx.Efficiency != intervaly.Efficiency)
                    return intervalx.Efficiency.CompareTo(intervaly.Efficiency);
                else
                    return x.CompareTo(y);
            }
        }

        /// <summary>
        /// This method will accept a dateInterval and calcualte a new dateInterval taking into account the dateIntervalEfficiencies
        /// ASSUMPTION: the input dateInterval has an efficiency of 100%.
        /// </summary>
        /// <param name="dateInterval">
        /// </param>
        /// <param name="dateIntervalEfficiencies">
        /// </param>
        /// <param name="fixedEndPoint">
        /// </param>
        /// <returns>
        /// The <see cref="DateInterval"/>.
        /// </returns>
        public static DateInterval thcristo3(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            if (dateIntervalEfficiencies.Count == 0)
                return dateInterval;

            DateIntervalCollection intvColl = new DateIntervalCollection(true);

            Dictionary<int, DateIntervalEfficiency> dictIntervals = new Dictionary<int, DateIntervalEfficiency>();

            //Add efficiency intervals in a collection to sort their edges
            //Also create a dictionary of unique IDs to intervals
            foreach (DateIntervalEfficiency eff in dateIntervalEfficiencies)
            {
                intvColl.Add(eff);
                dictIntervals.Add(RuntimeHelpers.GetHashCode(eff), eff);
            }

            //In this list we will store the resulting efficiency intervals
            List<DateIntervalEfficiency> efficiencyList = new List<DateIntervalEfficiency>();

            //Comparer to use for comparison between intervals
            DateIntervalEfficiencyComparer comparer = new DateIntervalEfficiencyComparer(dictIntervals);

            //We add and remove intervals in this dictionary as they come and go 
#if (SILVERLIGHT)
            TreeDictionary<int, DateIntervalEfficiency> currentIntervals = new TreeDictionary<int, DateIntervalEfficiency>(comparer);
#else
            SortedDictionary<int, DateIntervalEfficiency> currentIntervals = new SortedDictionary<int, DateIntervalEfficiency>(comparer);
#endif

            //This is the interval to use for efficiency calculation
            DateIntervalEfficiency priorityInterval = null;

            //This is the last time when the efficiency changed
            DateTime prevTime = DateTime.MinValue;

            int numEdges = intvColl.DateEdges.Count;
            int currentEdge = 0;

            //Loop through all interval edges
            while (currentEdge < numEdges)
            {
                DateTime currentTime = intvColl.DateEdges[currentEdge].Value;

                //Process all edges at this point in time
                while (currentEdge < numEdges && intvColl.DateEdges[currentEdge].Value == currentTime)
                {
                    IEndPoint<DateTime> currentEndPoint = intvColl.DateEdges[currentEdge];
                    int intervalID = RuntimeHelpers.GetHashCode(currentEndPoint.Interval);

                    if (currentEndPoint.IsMin)
                        //This interval starts, so add it to the sorted dictionary
                        currentIntervals.Add(intervalID, dictIntervals[intervalID]);
                    else
                        //This interval ended, so remove it from the sorted dictionary
                        currentIntervals.Remove(intervalID);

                    currentEdge++;
                }

                double oldEfficiency = (priorityInterval != null ? priorityInterval.Efficiency : 100);
                double newEfficiency = (currentIntervals.Count > 0 ? currentIntervals.Values.First().Efficiency : 100);

                //Check if we are at the end
                if (currentEdge == numEdges)
                {
                    if (oldEfficiency == newEfficiency)
                        //Previous efficiency is 100%, add only one interval to the end
                        efficiencyList.Add(new DateIntervalEfficiency(prevTime, DateTime.MaxValue, oldEfficiency));
                    else
                    {
                        //Previous efficiency is not 100%, add two intervals to the end
                        efficiencyList.Add(new DateIntervalEfficiency(prevTime, intvColl.DateEdges[currentEdge - 1].Value, oldEfficiency));
                        efficiencyList.Add(new DateIntervalEfficiency(intvColl.DateEdges[currentEdge - 1].Value, DateTime.MaxValue, newEfficiency));
                    }
                }
                else
                {
                    //Add a new interval only when efficiency changes
                    if (oldEfficiency != newEfficiency)
                    {
                        efficiencyList.Add(new DateIntervalEfficiency(prevTime, intvColl.DateEdges[currentEdge - 1].Value, oldEfficiency));
                        prevTime = intvColl.DateEdges[currentEdge - 1].Value;
                    }

                    //The interval to use is always the first (if any) in the sorted dictionary
                    priorityInterval = (currentIntervals.Count > 0 ? currentIntervals.Values.First() : null);
                }
            }

            //Create a new list to hold the points where efficiency changes
            List<IEndPoint<DateTime>> changePoints = new List<IEndPoint<DateTime>>();

            //efficiencyList contains already sorted intervals
            foreach (DateIntervalEfficiency eff in efficiencyList)
                changePoints.Add(eff.Min);

            changePoints.Add(efficiencyList.Last().Max);
            numEdges = changePoints.Count;

            int startEdge, endEdge, sign;
            DateTime currentDate;

            //Find the range of edges that are of interest
            if (fixedEndPoint == FixedEndPoint.Min)
            {
                currentDate = dateInterval.Min.Value;
                startEdge = changePoints.BinarySearch(dateInterval.Min);
                if (startEdge < 0)
                    startEdge = ~startEdge - 1;
                endEdge = numEdges - 1;
                sign = 1;
            }
            else
            {
                currentDate = dateInterval.Max.Value;
                startEdge = changePoints.BinarySearch(dateInterval.Max);
                if (startEdge < 0)
                    startEdge = ~startEdge;
                endEdge = 0;
                sign = -1;
            }

            //We have to "consume" this time
            double totalDays = dateInterval.Duration.TotalDays;

            //This is the actual time used
            double equivDays = 0;

            DateInterval newInterval = dateInterval;
            currentEdge = startEdge;

            //Loop through the time points 
            //and calculate the actual work done
            //by taking into account the efficiencies.
            while (currentEdge != endEdge)
            {
                int minEdge = (sign == 1 ? currentEdge : currentEdge - 1);
                int maxEdge = (sign == 1 ? currentEdge + 1 : currentEdge);

                DateTime minEdgeDate = changePoints[minEdge].Value;
                DateTime maxEdgeDate = changePoints[maxEdge].Value;

                double diffDays = Double.MaxValue;
                DateTime newDate = (sign == 1 ? DateTime.MaxValue : DateTime.MinValue);

                double currentEfficiency = (changePoints[minEdge].Interval as DateIntervalEfficiency).Efficiency;

                if (currentEfficiency != 0)
                {
                    //Compute how much time is needed to complete the remaining task 
                    //given the current interval's efficiency
                    diffDays = totalDays * 100.0 / currentEfficiency;
                    newDate = currentDate.AddDays(sign * diffDays);
                }

                if (currentEfficiency == 0)
                {
                    //No actual work is done, 
                    //so just jump to the end of the interval
                    newDate = (sign == 1 ? maxEdgeDate : minEdgeDate);
                    equivDays += (newDate - currentDate).TotalDays * sign;
                    currentDate = newDate;
                }
                else if ((sign > 0 && newDate > maxEdgeDate) || (sign < 0 && newDate < minEdgeDate))
                {
                    //This interval is not sufficient to complete the task,
                    //so use all the interval's available time
                    newDate = (sign == 1 ? maxEdgeDate : minEdgeDate);
                    equivDays += (newDate - currentDate).TotalDays * sign;

                    //Decrease the remaining task time and update the current point in time
                    totalDays -= (newDate - currentDate).TotalDays * sign * currentEfficiency / 100.0;
                    currentDate = newDate;
                }
                else
                {
                    //The task can be completed in this interval
                    equivDays += diffDays;
                    newInterval = new DateInterval((sign == 1 ? dateInterval.Min.Value : dateInterval.Max.Value.AddDays(sign * equivDays)), (sign == 1 ? dateInterval.Min.Value.AddDays(sign * equivDays) : dateInterval.Max.Value));
                    break;
                }

                currentEdge += sign;
            }

            return newInterval;
        }

        /// <summary>
        /// This class compares object IDs of efficiency intervals 
        /// based on priority, efficiency and object id.
        /// </summary>
        private class DateIntervalEfficiencyComparer4 : System.Collections.Generic.Comparer<int>
        {
            private Dictionary<int, DateIntervalEfficiency> dictIntervals;

            public DateIntervalEfficiencyComparer4(Dictionary<int, DateIntervalEfficiency> dictIntervals)
            {
                this.dictIntervals = dictIntervals;
            }

            public override int Compare(int x, int y)
            {
                DateIntervalEfficiency intervalx = dictIntervals[x];
                DateIntervalEfficiency intervaly = dictIntervals[y];

                if (intervalx.Priority != intervaly.Priority)
                    return (-1) * intervalx.Priority.CompareTo(intervaly.Priority);
                else if (intervalx.Efficiency != intervaly.Efficiency)
                    return intervalx.Efficiency.CompareTo(intervaly.Efficiency);
                else
                    return x.CompareTo(y);
            }
        }

        /// <summary>
        /// This method will accept a dateInterval and calcualte a new dateInterval taking into account the dateIntervalEfficiencies
        /// ASSUMPTION: the input dateInterval has an efficiency of 100%.
        /// </summary>
        /// <param name="dateInterval">
        /// </param>
        /// <param name="dateIntervalEfficiencies">
        /// </param>
        /// <param name="fixedEndPoint">
        /// </param>
        /// <returns>
        /// The <see cref="DateInterval"/>.
        /// </returns>
        public static DateInterval thcristo4(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            if (dateIntervalEfficiencies.Count == 0)
                return dateInterval;

            List<IEndPoint<DateTime>> dateEdges = new List<IEndPoint<DateTime>>(2 * dateIntervalEfficiencies.Count);

            Dictionary<int, DateIntervalEfficiency> dictIntervals = new Dictionary<int, DateIntervalEfficiency>(dateIntervalEfficiencies.Count);

            //Add the min and max of each interval in a list to sort them
            //Also create a dictionary of unique IDs to intervals
            foreach (DateIntervalEfficiency eff in dateIntervalEfficiencies)
            {
                //Add only intervals that affect our calculation
                if ((fixedEndPoint == FixedEndPoint.Min && eff.Max.Value > dateInterval.Min.Value) ||
                     (fixedEndPoint == FixedEndPoint.Max && eff.Min.Value < dateInterval.Max.Value))
                {
                    dateEdges.Add(eff.Min);
                    dateEdges.Add(eff.Max);
                    dictIntervals.Add(RuntimeHelpers.GetHashCode(eff), eff);
                }
            }

            //Sort the edges of intervals
            dateEdges.Sort();

            //Comparer to use for comparison between intervals
            DateIntervalEfficiencyComparer4 comparer = new DateIntervalEfficiencyComparer4(dictIntervals);

            //We add and remove intervals in this dictionary as they come and go 
#if (SILVERLIGHT)
            TreeDictionary<int, DateIntervalEfficiency> currentIntervals = new TreeDictionary<int, DateIntervalEfficiency>(comparer);
#else
            SortedDictionary<int, DateIntervalEfficiency> currentIntervals = new SortedDictionary<int, DateIntervalEfficiency>(comparer);
#endif

            //This is the interval having greatest priority or least efficiency
            DateIntervalEfficiency priorityInterval = null;

            int numEdges = dateEdges.Count;

            int startEdge, endEdge, sign;
            DateTime prevDate;

            //Setup the direction of move through edges
            if (fixedEndPoint == FixedEndPoint.Min)
            {
                //move forward
                prevDate = dateInterval.Min.Value;
                startEdge = 0;
                endEdge = numEdges - 1;
                sign = 1;
            }
            else
            {
                //move backward
                prevDate = dateInterval.Max.Value;
                startEdge = numEdges - 1;
                endEdge = 0;
                sign = -1;
            }

            //We have to "consume" this time
            double totalDays = dateInterval.Duration.TotalDays;

            //This is the actual time needed
            double equivDays = 0;

            DateInterval newInterval = dateInterval;

            int currentEdge = startEdge;

            //Loop through all edges
            while (sign * (currentEdge - endEdge) <= 0)
            {
                DateTime currentEdgeDate = dateEdges[currentEdge].Value;

                if ((sign > 0 && currentEdgeDate > dateInterval.Min.Value) ||
                     (sign < 0 && currentEdgeDate < dateInterval.Max.Value))
                {
                    DateTime minEdgeDate = (sign > 0 ? prevDate : currentEdgeDate);
                    DateTime maxEdgeDate = (sign > 0 ? currentEdgeDate : prevDate);

                    double diffDays = Double.MaxValue;
                    DateTime newDate = (sign > 0 ? DateTime.MaxValue : DateTime.MinValue);
                    double currentEfficiency = (priorityInterval != null ? priorityInterval.Efficiency : 100);

                    if (currentEfficiency != 0)
                    {
                        //Compute how much time is needed to complete the remaining task 
                        //given the current interval's efficiency
                        diffDays = totalDays * 100.0 / currentEfficiency;
                        newDate = prevDate.AddDays(sign * diffDays);
                    }

                    if (currentEfficiency == 0)
                    {
                        //No actual work is done, 
                        //so just jump to the end of the interval
                        newDate = (sign > 0 ? maxEdgeDate : minEdgeDate);
                        equivDays += (newDate - prevDate).TotalDays * sign;
                        prevDate = newDate;
                    }
                    else if ((sign > 0 && newDate > maxEdgeDate) || (sign < 0 && newDate < minEdgeDate))
                    {
                        //This interval is not sufficient to complete the task,
                        //so use all the interval's available time
                        newDate = (sign > 0 ? maxEdgeDate : minEdgeDate);
                        equivDays += (newDate - prevDate).TotalDays * sign;

                        //Decrease the remaining task time and update the current point in time
                        totalDays -= (newDate - prevDate).TotalDays * sign * currentEfficiency / 100.0;
                        prevDate = newDate;
                    }
                    else
                    {
                        //The task can be completed in this interval
                        equivDays += diffDays;
                        totalDays = 0;
                        newInterval = new DateInterval((sign > 0 ? dateInterval.Min.Value : dateInterval.Max.Value.AddDays(sign * equivDays)), (sign > 0 ? dateInterval.Min.Value.AddDays(sign * equivDays) : dateInterval.Max.Value));
                        break;
                    }
                }

                //Process all edges at this point in time
                while ((sign * (currentEdge - endEdge) <= 0) &&
                        (dateEdges[currentEdge].Value == currentEdgeDate))
                {
                    IEndPoint<DateTime> currentEndPoint = dateEdges[currentEdge];
                    int intervalID = RuntimeHelpers.GetHashCode(currentEndPoint.Interval);

                    if ((sign > 0 && currentEndPoint.IsMin) ||
                         (sign < 0 && !currentEndPoint.IsMin))
                        //This interval starts, so add it to the sorted dictionary
                        currentIntervals.Add(intervalID, dictIntervals[intervalID]);
                    else
                        //This interval ended, so remove it from the sorted dictionary
                        currentIntervals.Remove(intervalID);

                    currentEdge += sign;
                }

                //The interval to use is always the first (if any) in the sorted dictionary
                priorityInterval = (currentIntervals.Count > 0 ? currentIntervals.Values.First() : null);
            }

            //If there still exists any remaining work to be done, use 100% efficiency
            if (totalDays > 0)
            {
                equivDays += totalDays;
                newInterval = new DateInterval((sign > 0 ? dateInterval.Min.Value : dateInterval.Max.Value.AddDays(sign * equivDays)), (sign > 0 ? dateInterval.Min.Value.AddDays(sign * equivDays) : dateInterval.Max.Value));
            }

            return newInterval;
        }

        #endregion

        #region Quicks01ver

        public class IntervalBorder
        {
            public DateTime time;
            public bool isStart;
            public DateIntervalEfficiency efficiency;

            public IntervalBorder(DateTime time, bool isStart, DateIntervalEfficiency efficiency)
            {
                this.time = time;
                this.isStart = isStart;
                this.efficiency = efficiency;
            }
        }

        public class TimeComparer<T> : IComparer<T> where T : IntervalBorder
        {
            public int Compare(T obj1, T obj2)
            {
                return obj1.time.CompareTo(obj2.time);
            }
        }

        public class EfficiencyComparer<T> : IComparer<T> where T : DateIntervalEfficiency
        {
            public int Compare(T obj1, T obj2)
            {
                if (obj1.Priority == obj2.Priority)
                    return obj2.Efficiency.CompareTo(obj1.Efficiency);

                return obj1.Priority.CompareTo(obj2.Priority);
            }
        }

        public static DateInterval Quicks01ver(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            IntervalBorder[] borders = new IntervalBorder[2 * dateIntervalEfficiencies.Count];
            IntervalHeap<DateIntervalEfficiency> efficiencies = new IntervalHeap<DateIntervalEfficiency>(dateIntervalEfficiencies.Count, new EfficiencyComparer<DateIntervalEfficiency>());

            DateTime startTime = fixedEndPoint == FixedEndPoint.Min ? dateInterval.Min.Value : dateInterval.Max.Value;
            DateTime currentTime = startTime;
            long remainingTicks = dateInterval.Duration.Ticks;
            bool flip = fixedEndPoint == FixedEndPoint.Max;

            int i = 0;

            if (flip)
            { // if (fixedEndPoint == FixedEndPoint.Max)
                DateTime dblStartTime = new DateTime(2 * startTime.Ticks);
                foreach (var eff in dateIntervalEfficiencies)
                {
                    if (eff.Min.Value < startTime)
                    {
                        borders[i++] = new IntervalBorder(eff.Max.Value < startTime ? dblStartTime.AddTicks(-eff.Max.Value.Ticks) : startTime, true, eff);
                        borders[i++] = new IntervalBorder(dblStartTime.AddTicks(-eff.Min.Value.Ticks), false, eff);
                    }
                }
            }

            else
            { // if (fixedEndPoint == FixedEndPoint.Min)
                foreach (var eff in dateIntervalEfficiencies)
                {
                    if (startTime < eff.Max.Value)
                    {
                        borders[i++] = new IntervalBorder(startTime < eff.Min.Value ? eff.Min.Value : startTime, true, eff);
                        borders[i++] = new IntervalBorder(eff.Max.Value, false, eff);
                    }
                }
            }

            int last = i;

            if (last > 0)
            {
                Array.Sort(borders, 0, last, new TimeComparer<IntervalBorder>());

                i = 0;
                while (i < last)
                {
                    var b = borders[i];

                    if (b.time == currentTime)
                    {
                        if (b.isStart) efficiencies.Add(ref b.efficiency.handle, b.efficiency);
                        else efficiencies.Delete(b.efficiency.handle);

                        ++i;
                    }

                    else
                    {
                        TimeSpan period = b.time - currentTime;
                        long progress = efficiencies.IsEmpty ? period.Ticks : (long)(period.Ticks * 0.01 * efficiencies.FindMax().Efficiency);

                        if (progress >= remainingTicks)
                        {
                            long overDone = progress - remainingTicks;
                            remainingTicks = 0;
                            currentTime = b.time;
                            if (overDone > 0)
                                currentTime -= new TimeSpan(efficiencies.IsEmpty ? period.Ticks : (long)(overDone / (0.01 * efficiencies.FindMax().Efficiency)));

                            break;
                        }

                        remainingTicks -= progress;
                        currentTime = b.time;
                    }
                }
            }

            if (remainingTicks > 0)
                currentTime += new TimeSpan(remainingTicks);

            if (flip)
                return new DateInterval(startTime.AddTicks(startTime.Ticks - currentTime.Ticks), startTime);

            return new DateInterval(startTime, currentTime);
        }

        public class IndexInterval
        {
            public int startIdx;
            public int endIdx;

            public IndexInterval(int start = 0, int end = 0)
            {
                this.startIdx = start;
                this.endIdx = end;
            }
        }

        public class ExtendedEfficiency
        {
            public DateIntervalEfficiency efficiency;
            public IndexInterval indexInterval;

            public ExtendedEfficiency(DateIntervalEfficiency eff, int startIdx, int endIdx)
            {
                this.efficiency = eff;
                this.indexInterval = new IndexInterval(startIdx, endIdx);
            }
        }

        public class IntervalTreeNode
        {
            public IntervalTreeNode parent;
            public IndexInterval interval;

            public IntervalTreeNode(IntervalTreeNode parent, IndexInterval ii)
            {
                this.parent = parent;
                this.interval = ii;
            }
        }

        public class EfficiencyComparer2<T> : IComparer<T> where T : ExtendedEfficiency
        {
            public int Compare(T obj1, T obj2)
            {
                if (obj1.efficiency.Priority == obj2.efficiency.Priority)
                    return -obj2.efficiency.Efficiency.CompareTo(obj1.efficiency.Efficiency);

                return -obj1.efficiency.Priority.CompareTo(obj2.efficiency.Priority);
            }
        }

        public static DateInterval Quicks01lver2(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            // Check for empty list

            if (dateIntervalEfficiencies == null || dateIntervalEfficiencies.Count == 0)
                return dateInterval;

            // Data preprocessing

            bool flip = fixedEndPoint == FixedEndPoint.Max;
            long fixedTimePoint = flip ? dateInterval.Max.Value.Ticks : dateInterval.Min.Value.Ticks;

            // Trim and flip if necessary

            List<DateIntervalEfficiency> trimmedEfficiencies = new List<DateIntervalEfficiency>(dateIntervalEfficiencies.Count);

            DateTime fixedPointDate;
            if (flip)
            {
                foreach (var eff in dateIntervalEfficiencies)
                {
                    long fixed_2 = 2 * fixedTimePoint;
                    if (fixedTimePoint <= eff.Max.Value.Ticks)
                    {
                        if (fixedTimePoint <= eff.Min.Value.Ticks)
                            continue;

                        fixedPointDate = new DateTime(fixedTimePoint);
                    }
                    else
                        fixedPointDate = new DateTime(fixed_2 - eff.Max.Value.Ticks);

                    trimmedEfficiencies.Add(new DateIntervalEfficiency(new DateInterval(fixedPointDate, new DateTime(fixed_2 - eff.Min.Value.Ticks)), eff.Efficiency, eff.Priority));
                }
            }
            else
            {
                foreach (var eff in dateIntervalEfficiencies)
                {
                    if (eff.Min.Value.Ticks <= fixedTimePoint)
                    {
                        if (eff.Max.Value.Ticks <= fixedTimePoint)
                            continue;

                        fixedPointDate = new DateTime(fixedTimePoint);
                    }
                    else
                        fixedPointDate = eff.Min.Value;

                    trimmedEfficiencies.Add(new DateIntervalEfficiency(new DateInterval(fixedPointDate, eff.Max.Value), eff.Efficiency, eff.Priority));
                }
            }

            // Determine unique "support" points in time

            var uniqueTimePoints = new System.Collections.Generic.HashSet<long>();
            uniqueTimePoints.Add(fixedTimePoint);

            foreach (var eff in trimmedEfficiencies)
            {
                uniqueTimePoints.Add(eff.Min.Value.Ticks);
                uniqueTimePoints.Add(eff.Max.Value.Ticks);
            }

            var timePoints = new long[uniqueTimePoints.Count];
            uniqueTimePoints.CopyTo(timePoints);
            Array.Sort(timePoints);

            var ticksToSupportIndices = new Dictionary<long, int>(2 * trimmedEfficiencies.Count);
            var effectiveTicks = new long[timePoints.Length];

            // Create 'time -> support point index' mapping

            ticksToSupportIndices.Add(timePoints[0], 0);
            effectiveTicks[0] = 0;
            for (int i = 1; i < timePoints.Length; ++i)
            {
                ticksToSupportIndices.Add(timePoints[i], i);
                effectiveTicks[i] = timePoints[i] - timePoints[i - 1];
            }

            // Add 'support border index' info to the efficiencies

            var sortedEfficiencies = new List<ExtendedEfficiency>(trimmedEfficiencies.Count);
            foreach (var eff in trimmedEfficiencies)
                sortedEfficiencies.Add(new ExtendedEfficiency(eff, ticksToSupportIndices[eff.Min.Value.Ticks], ticksToSupportIndices[eff.Max.Value.Ticks]));
            sortedEfficiencies.Sort(new EfficiencyComparer2<ExtendedEfficiency>());


            // Core algorithm

            var supportPoints = new IntervalTreeNode[timePoints.Length];
            var tree = new IntervalTreeNode[2 * timePoints.Length];

            int maxNode = 0;
            foreach (var exEff in sortedEfficiencies)
            {
                int leftIdx = exEff.indexInterval.startIdx;
                int rightIdx = exEff.indexInterval.endIdx;

                IntervalTreeNode leftNode = supportPoints[leftIdx];
                IntervalTreeNode rightNode = supportPoints[rightIdx];
                IntervalTreeNode newParentNode = null;

                // Get Left Edge Node
                if (leftNode != null)
                {
                    IntervalTreeNode node = leftNode.parent;
                    if (node != null)
                    {
                        while (node.parent != null)
                            node = node.parent;

                        // Update "direct" links
                        while (leftNode.parent != node)
                        {
                            IntervalTreeNode next = leftNode.parent;
                            leftNode.parent = node;
                            leftNode = next;
                        }
                        leftNode = node;
                    }
                }

                // Get Right Edge Node
                if (rightNode != null)
                {
                    IntervalTreeNode parentNode = rightNode.parent;
                    if (parentNode != null)
                    {
                        while (parentNode.parent != null)
                            parentNode = parentNode.parent;

                        // Update "direct" links
                        while (rightNode.parent != parentNode)
                        {
                            IntervalTreeNode next = rightNode.parent;
                            rightNode.parent = parentNode;
                            rightNode = next;
                        }
                        rightNode = parentNode;
                    }
                }

                if (leftNode != null)
                {
                    // if node is the same on both sides the current interval is fully covered
                    if (leftNode == rightNode)
                        continue;

                    newParentNode = new IntervalTreeNode(null, new IndexInterval(leftNode.interval.startIdx, rightIdx));
                    tree[maxNode++] = newParentNode;

                    // Adjust left marker
                    leftIdx = leftNode.interval.endIdx;
                }

                if (rightNode != null)
                {
                    if (newParentNode == null)
                    {
                        newParentNode = new IntervalTreeNode(null, new IndexInterval(leftIdx, rightNode.interval.endIdx));
                        tree[maxNode++] = newParentNode;
                    }
                    else
                        newParentNode.interval.endIdx = rightNode.interval.endIdx;

                    // Adjust right marker
                    rightIdx = rightNode.interval.startIdx;
                }

                // Create new leaf node and include the free points from 'leftIdx' to 'rightIdx'
                IntervalTreeNode newLeafNode = new IntervalTreeNode(null, new IndexInterval(leftIdx, rightIdx));
                tree[maxNode++] = newLeafNode;

                // Scanning from left to right

                // Marking the start point, but skipping the integration, since it contains the ticks from the previous delta interval
                supportPoints[leftIdx] = newLeafNode;

                while (++leftIdx <= rightIdx)
                {
                    // Integrate the delta interval
                    effectiveTicks[leftIdx] = (long)(effectiveTicks[leftIdx] * 0.01 * exEff.efficiency.Efficiency);

                    if (supportPoints[leftIdx] != null)
                    {
                        IntervalTreeNode parentNode = supportPoints[leftIdx].parent;
                        if (parentNode != null)
                        {
                            while (parentNode.parent != null)
                                parentNode = parentNode.parent;

                            // Update "direct" links
                            IntervalTreeNode node = supportPoints[leftIdx].parent;
                            while (parentNode != null && node.parent != parentNode)
                            {
                                IntervalTreeNode next = node.parent;
                                node.parent = parentNode;
                                node = next;
                            }
                            supportPoints[leftIdx] = parentNode;
                        }

                        // Jump over existent interval
                        leftIdx = supportPoints[leftIdx].interval.endIdx;
                    }
                    else
                    {
                        // "Brand" point with the current node
                        supportPoints[leftIdx] = newLeafNode;
                    }
                }

                if (newParentNode != null)
                {
                    if (leftNode != null) leftNode.parent = newParentNode;
                    if (rightNode != null) rightNode.parent = newParentNode;
                    newLeafNode.parent = newParentNode;
                }
            }


            // All time pieces integrated at this point, let's walk over them and accumulate the "effective" ticks

            int idx = 0;
            var remainingTicks = dateInterval.Duration.Ticks;
            while (++idx < effectiveTicks.Length && remainingTicks > 0)
                remainingTicks -= effectiveTicks[idx];
            --idx;

            // Check if end is before last time point

            if (remainingTicks < 0)
            {
                double frac = -remainingTicks / (double)effectiveTicks[idx];
                long overTime = (long)(frac * (timePoints[idx] - timePoints[idx - 1]));

                // FixedEndPoint.Max
                if (flip)
                    return new DateInterval(new DateTime(2 * fixedTimePoint - timePoints[idx] + overTime), new DateTime(fixedTimePoint));

                // FixedEndPoint.Min
                return new DateInterval(new DateTime(fixedTimePoint), new DateTime(timePoints[idx] - overTime));
            }

            // FixedEndPoint.Max
            if (flip)
                return new DateInterval(new DateTime(2 * fixedTimePoint - timePoints[idx] - remainingTicks), new DateTime(fixedTimePoint));

            // FixedEndPoint.Min
            return new DateInterval(new DateTime(fixedTimePoint), new DateTime(timePoints[idx] + remainingTicks));
        }

        //---------------------------------------------------------------------------------------------------------------
        // SUBMISSION 3
        public class EfficiencyComparer3<T> : IComparer<T> where T : DateIntervalEfficiency
        {
            public int Compare(T obj1, T obj2)
            {
                if (obj1.Priority == obj2.Priority)
                    return -obj2.Efficiency.CompareTo(obj1.Efficiency);

                return -obj1.Priority.CompareTo(obj2.Priority);
            }
        }


        private static List<DateIntervalEfficiency> TrimEfficiencies(List<DateIntervalEfficiency> efficiencies, long fixedTimePoint, bool flip)
        {
            // Trim and flip if necessary
            List<DateIntervalEfficiency> trimmedEfficiencies = new List<DateIntervalEfficiency>(efficiencies.Count);

            DateTime fixedPointDate;
            if (flip)
            {
                foreach (var eff in efficiencies)
                {
                    long fixed_2 = 2 * fixedTimePoint;
                    if (fixedTimePoint <= eff.Max.Value.Ticks)
                    {
                        if (fixedTimePoint <= eff.Min.Value.Ticks)
                            continue;

                        fixedPointDate = new DateTime(fixedTimePoint);
                    }
                    else
                        fixedPointDate = new DateTime(fixed_2 - eff.Max.Value.Ticks);

                    trimmedEfficiencies.Add(new DateIntervalEfficiency(new DateInterval(fixedPointDate, new DateTime(fixed_2 - eff.Min.Value.Ticks)), eff.Efficiency, eff.Priority));
                }
            }
            else
            {
                foreach (var eff in efficiencies)
                {
                    if (eff.Min.Value.Ticks <= fixedTimePoint)
                    {
                        if (eff.Max.Value.Ticks <= fixedTimePoint)
                            continue;

                        fixedPointDate = new DateTime(fixedTimePoint);
                    }
                    else
                        fixedPointDate = eff.Min.Value;

                    trimmedEfficiencies.Add(new DateIntervalEfficiency(new DateInterval(fixedPointDate, eff.Max.Value), eff.Efficiency, eff.Priority));
                }
            }

            return trimmedEfficiencies;
        }


        public static DateInterval Quicks01lver3(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            /* Preprocessing */

            // Check for empty list
            if (dateIntervalEfficiencies == null || dateIntervalEfficiencies.Count == 0)
                return dateInterval;

            // Trim and flip if necessary
            bool flip = fixedEndPoint == FixedEndPoint.Max;
            long fixedTimePoint = flip ? dateInterval.Max.Value.Ticks : dateInterval.Min.Value.Ticks;
            List<DateIntervalEfficiency> trimmedEfficiencies = TrimEfficiencies(dateIntervalEfficiencies, fixedTimePoint, flip);
            if (trimmedEfficiencies.Count == 0)
                return dateInterval;

            // Create our special tree
            IntervalTree tree = new IntervalTree();

            // Determine unique "support" points in time, including period's start point
            tree.AddSupportPoint(fixedTimePoint);
            foreach (var eff in trimmedEfficiencies)
            {
                tree.AddSupportPoint(eff.Min.Value.Ticks);
                tree.AddSupportPoint(eff.Max.Value.Ticks);
            }

            // Initialize support points and create 'time -> support point index' mapping & integrate at 100%
            tree.InitSupportAndMapping();

            // Add 'support border index' info to the efficiencies
            var sortedEfficiencies = new List<DateIntervalEfficiency>(trimmedEfficiencies.Count);
            foreach (var eff in trimmedEfficiencies)
                sortedEfficiencies.Add(eff);
            sortedEfficiencies.Sort(new EfficiencyComparer3<DateIntervalEfficiency>());


            /* Core algorithm */

            // Merge intervals into data structure and 
            foreach (var eff in sortedEfficiencies)
            {
                tree.MergeAndIntegrate(eff);
                if (tree.IsFullyCovered)
                    break;
            }

            // All time pieces integrated at this point, let's walk over them and accumulate the "effective" ticks
            long effectiveTicks = tree.AccumulateEffectiveTime(dateInterval.Duration.Ticks);

            if (flip)
                // FixedEndPoint.Max
                return new DateInterval(new DateTime(fixedTimePoint - effectiveTicks), new DateTime(fixedTimePoint));

            // FixedEndPoint.Min
            return new DateInterval(new DateTime(fixedTimePoint), new DateTime(fixedTimePoint + effectiveTicks));
        }
        #endregion

        # region vrstks
        public static DateInterval vrstks(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
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

        #region vrstks2
        public static DateInterval vrstks2(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            //----- STEP 1. Creating timeline
            List<DateIntervalEfficiency> timeline = new List<DateIntervalEfficiency>();

            int insertPoint = 0; //we may skip intervals before this point in future
            foreach (var current in dateIntervalEfficiencies.OrderBy(di => di.Min))
            {
                bool added = false;
                for (int i = insertPoint; i < timeline.Count; i++)
                {
                    if (timeline[i].Overlaps(current))
                    {
                        var nonOverlapped = FixOverlap2(timeline[i], current);

                        //replace old item with non-overlapped intervals
                        timeline.RemoveAt(i);
                        timeline.InsertRange(i, nonOverlapped);
                        insertPoint = i;

                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    //Not overlaps any prevous interval - add to the end
                    insertPoint = timeline.Count;
                    timeline.Add(current);
                }
            }

            //----- STEP 2. Prepare time navigation depending on the fixed point side
            TimeSpan duration = TimeSpan.Zero,
                     notAffected = dateInterval.Duration;

            Func<DateIntervalEfficiency, TimeSpan> getEffectDuration, getGapDuration;

            if (fixedEndPoint == FixedEndPoint.Min)
            {
                //Start from the minimum point of interval and move forward
                //So, 'NOW' is (dateInterval.Min.Value + duration)
                //  start |----NOW>----| end
                getEffectDuration = (calendar) => calendar.Max.Value - (dateInterval.Min.Value + duration);
                //  NOW>| gap |--calendar--|
                getGapDuration = (calendar) => calendar.Min.Value - (dateInterval.Min.Value + duration);
            }
            else
            {
                //Start from the maximum point of interval and move backward
                //So, 'NOW' is (dateInterval.Max.Value - duration)
                //  end |----<NOW----| start
                getEffectDuration = (calendar) => (dateInterval.Max.Value - duration) - calendar.Min.Value;
                //  |--calendar--| gap |<NOW
                getGapDuration = (calendar) => (dateInterval.Max.Value - duration) - calendar.Max.Value;

                //reverse timeline in this case
                timeline.Reverse();
            }

            //remove outside efficiencies
            while (timeline.Count > 0 && getEffectDuration(timeline[0]) < TimeSpan.Zero)
                timeline.RemoveAt(0);

            //----- STEP 3. Moving through the timeline and applying efficiencies
            foreach (var calendar in timeline)
            {
                if (notAffected == TimeSpan.Zero)
                    break;

                //Check for a gap in the schedule - 100% efficiency
                TimeSpan gap = getGapDuration(calendar);
                if (gap > TimeSpan.Zero)
                {
                    duration = duration + gap;
                    notAffected = notAffected - gap;

                    if (notAffected == TimeSpan.Zero)
                        break;
                }

                TimeSpan effectTime = getEffectDuration(calendar);
                TimeSpan timeAt100 = effectTime.Scale(calendar.Efficiency / 100);

                if (notAffected < timeAt100 && calendar.Efficiency != 0)
                {
                    //Overtime. Take only the rest of interval and convert it back according to current efficiency
                    timeAt100 = notAffected;
                    effectTime = timeAt100.Scale(100 / calendar.Efficiency);
                }

                duration = duration + effectTime;
                notAffected = notAffected - timeAt100;
            }

            //Add not affected duration (in fact, a gap after last calendar) to result and return
            duration = duration + notAffected;

            if (fixedEndPoint == FixedEndPoint.Min)
            {
                return new DateInterval(dateInterval.Min.Value, dateInterval.Min.Value + duration, dateInterval.Min.IsInclusive, dateInterval.Max.IsInclusive);
            }
            else
            {
                return new DateInterval(dateInterval.Max.Value - duration, dateInterval.Max.Value, dateInterval.Min.IsInclusive, dateInterval.Max.IsInclusive);
            }
        }

        private static TimeSpan Scale(this TimeSpan time, double factor)
        {
            return TimeSpan.FromTicks((long)(time.Ticks * factor));
        }

        private static List<DateIntervalEfficiency> FixOverlap2(DateIntervalEfficiency previous, DateIntervalEfficiency current)
        {
            List<DateIntervalEfficiency> result = new List<DateIntervalEfficiency>(3);

            //head
            if (previous.Min.Value < current.Min.Value)
            {
                // Previous  |-----------|
                // Current       |-----------|
                // Interval  |---|
                var interval = new DateIntervalEfficiency(previous.Min.Value, current.Min.Value, previous.Efficiency, previous.Priority, previous.Min.IsInclusive, current.Min.IsInclusive);
                result.Add(interval);
            }

            //overlap
            var overlap = previous.GetOverlap(current);
            if (overlap.Max.Value > overlap.Min.Value)
            {
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
            if (previous.Max.Value > current.Max.Value)
            {
                // Previous  |------------------|
                // Current       |-----------|
                // Interval                  |--|, previous priority and efficiency

                var interval = new DateIntervalEfficiency(current.Max.Value, previous.Max.Value, previous.Efficiency, previous.Priority, current.Max.IsInclusive, previous.Max.IsInclusive);
                result.Add(interval);
            }
            else if (current.Max.Value > previous.Max.Value)
            {
                // Previous  |-----------|
                // Current       |-----------|
                // Interval              |---|, current priority and efficiency

                var interval = new DateIntervalEfficiency(previous.Max.Value, current.Max.Value, current.Efficiency, current.Priority, previous.Max.IsInclusive, current.Max.IsInclusive);
                result.Add(interval);
            }
            return result;
        }
        #endregion

        #region MoustafaS
        public static DateInterval MoustafaS(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            if (dateIntervalEfficiencies == null || dateIntervalEfficiencies.Count == 0)
                return dateInterval;
            if (dateInterval == null)
                return null;
            var split = dateIntervalEfficiencies.Collapse();
            if (split.Count == 0)
                return dateInterval;
            var constantMin = fixedEndPoint == FixedEndPoint.Min;
            long resultantTicks = 0;
            long totalTicks = dateInterval.Duration.Ticks,
                 acc = 0;
            DateTime start = dateInterval.Min.Value;
            DateTime end = dateInterval.Max.Value;
            var d = split.Select((x, i)
                => ((i < split.Count - 1) && (split[i + 1].Min != x.Max))
                ?
                split[i + 1].Min.Value.Ticks - x.Max.Value.Ticks : 0)
                .Sum();
            acc += d;
            resultantTicks += d;
            if (constantMin && split[0].Min.Value > dateInterval.Min.Value)
            {
                var z = split[0].Min.Value.Ticks - dateInterval.Min.Value.Ticks;
                acc += z;
                resultantTicks += z;
            }
            foreach (var over in split)
            {
                if (constantMin && over.Max.Value <= dateInterval.Min.Value)
                    continue;
                var eff = over.Efficiency;
                if (eff == 0.0)
                    continue;
                var diff = (long)(over.Max.Value.Ticks - over.Min.Value.Ticks);
                var last = diff * eff / 100;
                if ((double)resultantTicks + last < totalTicks)
                {
                    resultantTicks += (long)last;
                    acc += diff;
                }
                else
                {
                    last = totalTicks - resultantTicks;
                    var t = (long)((double)last * 100 / eff);
                    resultantTicks += (long)last;
                    acc += t;
                    break;
                }
            }
            if (constantMin)
                end = end.AddTicks((acc - resultantTicks));
            else
                start = start.AddTicks(-(acc - resultantTicks));
            if (split.Any(x => x.Efficiency == 0.0))
            {
                var zeros = split.Where(x => x.Efficiency == 0.0);
                zeros = constantMin ? zeros : zeros.OrderByDescending(x => x.Min);
                foreach (var item in zeros)
                {
                    var tmp = new DateInterval(start, end).GetOverlap(item);
                    if (tmp != null)
                    {
                        long ticks;
                        if (constantMin)
                        {
                            ticks = end.Ticks >= item.Max.Value.Ticks ? (tmp.Max.Value.Ticks - tmp.Min.Value.Ticks) : ((item.Max.Value.Ticks - end.Ticks) + (tmp.Max.Value.Ticks - tmp.Min.Value.Ticks));
                            end = end.AddTicks(ticks);
                        }
                        else
                        {
                            ticks = start.Ticks <= item.Min.Value.Ticks ? (tmp.Max.Value.Ticks - tmp.Min.Value.Ticks) : ((start.Ticks - item.Min.Value.Ticks) + (tmp.Max.Value.Ticks - tmp.Min.Value.Ticks));
                            start = start.AddTicks(-ticks);
                        }
                    }
                }
            }
            var result = new DateInterval(start, end);
            return result;
        }

        public static List<DateIntervalEfficiency> Collapse(this IEnumerable<DateIntervalEfficiency> intervalCollection)
        {
            if (intervalCollection == null)
            {
                return null;
            }
            var output = new List<DateIntervalEfficiency>();

            // Grabbing all time split points and sorting them
            List<DateTime> timeEvents = intervalCollection.SelectMany(task => new[] { task.Min.Value, task.Max.Value })
                                                     .Distinct()
                                                     .OrderBy(x => x)
                                                     .ToList();
            for (var i = 0; i < timeEvents.Count - 1; i++)
            {
                var overlappingEfficiencies = intervalCollection.Where(x => x.Overlaps(new DateInterval(timeEvents[i], timeEvents[i + 1])));

                if (overlappingEfficiencies.Any())
                {
                    var eff = overlappingEfficiencies.GroupBy(x => x.Priority).OrderByDescending(x => x.Key).Select(x => x.Min(y => y.Efficiency)).First();
                    output.Add(new DateIntervalEfficiency(timeEvents[i], timeEvents[i + 1], eff));
                }
            }
            return output;
        }

        public static DateInterval MoustafaS2(this DateInterval dateInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint = FixedEndPoint.Min)
        {
            if (dateIntervalEfficiencies == null || dateIntervalEfficiencies.Count == 0)
                return dateInterval;
            if (dateInterval == null)
                return null;
            var split = dateIntervalEfficiencies.Collapse2();
            if (split.Count == 0)
                return dateInterval;
            var constantMin = fixedEndPoint == FixedEndPoint.Min;
            long resultantTicks = 0;
            long totalTicks = dateInterval.Duration.Ticks,
                 acc = 0;
            DateTime start = dateInterval.Min.Value;
            DateTime end = dateInterval.Max.Value;
            var d = split.Select((x, i)
                => ((i < split.Count - 1) && (split[i + 1].Min.Value.Ticks != x.Max.Value.Ticks) && (split[i + 1].Min.Value.Ticks >= dateInterval.Min.Value.Ticks && x.Max.Value.Ticks <= dateInterval.Max.Value.Ticks))
                ?
                split[i + 1].Min.Value.Ticks - x.Max.Value.Ticks : 0)
                .Sum();
            acc += d;
            resultantTicks += d;
            if (constantMin && split[0].Min.Value.Ticks > dateInterval.Min.Value.Ticks)
            {
                var z = split[0].Min.Value.Ticks - dateInterval.Min.Value.Ticks;
                acc += z;
                resultantTicks += z;
            }
            foreach (var over in split)
            {
                if (constantMin && over.Max.Value.Ticks <= dateInterval.Min.Value.Ticks)
                    continue;
                var eff = over.Efficiency;
                if (eff == 0.0)
                    continue;
                var diff = (long)(over.Max.Value.Ticks - over.Min.Value.Ticks);
                var last = diff * eff / 100;
                if (resultantTicks + last < totalTicks)
                {
                    resultantTicks += (long)last;
                    acc += diff;
                }
                else
                {
                    last = totalTicks - resultantTicks;
                    var t = (long)((double)last * 100 / eff);
                    resultantTicks += (long)last;
                    acc += t;
                    break;
                }
            }
            if (constantMin)
                end = end.AddTicks((acc - resultantTicks));
            else
                start = start.AddTicks(-(acc - resultantTicks));

            var zeros = split.Where(x => x.Efficiency == 0.0);
            if (zeros.Any())
            {
                zeros = constantMin ? zeros : zeros.OrderByDescending(x => x.Min);
                foreach (var item in zeros)
                {
                    var tmp = new DateInterval(start, end).GetOverlap(item);
                    if (tmp != null)
                    {
                        long ticks;
                        if (constantMin)
                        {
                            ticks = end.Ticks >= item.Max.Value.Ticks ? (tmp.Max.Value.Ticks - tmp.Min.Value.Ticks) : ((item.Max.Value.Ticks - end.Ticks) + (tmp.Max.Value.Ticks - tmp.Min.Value.Ticks));
                            end = end.AddTicks(ticks);
                        }
                        else
                        {
                            ticks = start.Ticks <= item.Min.Value.Ticks ? (tmp.Max.Value.Ticks - tmp.Min.Value.Ticks) : ((start.Ticks - item.Min.Value.Ticks) + (tmp.Max.Value.Ticks - tmp.Min.Value.Ticks));
                            start = start.AddTicks(-ticks);
                        }
                    }
                }
            }
            var result = new DateInterval(start, end);
            return result;
        }
        public static System.Collections.Generic.IList<DateIntervalEfficiency> Collapse2(this IEnumerable<DateIntervalEfficiency> intervalCollection)
        {
            if (intervalCollection == null)
            {
                return null;
            }
            //var output = new List<DateIntervalEfficiency>();

            // Grabbing all time split points and sorting them
            var output = new List<DateIntervalEfficiency>();

            // Grabbing all time split points and sorting them
            List<DateTime> timeEvents = intervalCollection.SelectMany(task => new[] { task.Min.Value, task.Max.Value })
                                                     .Distinct()
                                                     .OrderBy(x => x)
                                                     .ToList();
            for (var i = 0; i < timeEvents.Count - 1; i++)
            {
                var overlappingEfficiencies = intervalCollection.Where(x => x.Overlaps(new DateInterval(timeEvents[i], timeEvents[i + 1])));

                if (overlappingEfficiencies.Any())
                {
                    var eff = overlappingEfficiencies.GroupBy(x => x.Priority).OrderByDescending(x => x.Key).Select(x => x.Min(y => y.Efficiency)).First();
                    //var max = overlappingEfficiencies.Max(x => x.Priority);
                    //var eff = overlappingEfficiencies.Where(x => x.Priority == max).Min(x => x.Efficiency);
                    output.Add(new DateIntervalEfficiency(timeEvents[i], timeEvents[i + 1], eff));
                }
            }
            return output;
        }
        #endregion
    }
}
