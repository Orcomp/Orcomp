// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateIntervalCollectionExtensions.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The date interval collection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interval.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The date interval collection extensions.
    /// </summary>
    public static class DateIntervalCollectionExtensions
    {
        /// <summary>
        /// Assumption: The list parameter is already pre sorted.
        /// </summary>
        /// <param name="orderedDateIntervals"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetSortedDateTimes(this List<DateInterval> orderedDateIntervals)
        {
            // Used modified version of c6c
            // RenzeExtended has the fastest implementation when endtimes are ascending but poor on random and descending endtimes.

            var dateIntervals = orderedDateIntervals.ToArray();

            var startDateTimes = new DateTime[dateIntervals.Length];
            var endDateTimes = new DateTime[dateIntervals.Length];

            bool areEndPointsSorted = true;
            DateTime previousEndPoint = DateTime.MinValue;

            for (int i = 0; i < dateIntervals.Length; i++)
            {
                startDateTimes[i] = dateIntervals[i].StartTime;
                endDateTimes[i] = dateIntervals[i].EndTime;

                if (areEndPointsSorted && (previousEndPoint > endDateTimes[i]))
                {
                    areEndPointsSorted = false;
                }

                previousEndPoint = endDateTimes[i];
            }

            if (!areEndPointsSorted)
            {
                Array.Sort(endDateTimes);
            }

            return MergeSortedArrays(startDateTimes, endDateTimes);
        }

        private static IEnumerable<DateTime> MergeSortedArrays(DateTime[] startDateTimes, DateTime[] endDateTimes)
        {
            var resultList = new DateTime[startDateTimes.Length + endDateTimes.Length];

            int i = 0;
            int j = 0;
            int k = 0;

            while (i < startDateTimes.Length && j < endDateTimes.Length)
            {
                if (startDateTimes[i] <= endDateTimes[j])
                {
                    resultList[k++] = startDateTimes[i++];
                }
                else
                {
                    resultList[k++] = endDateTimes[j++];
                }
            }

            while (i < startDateTimes.Length)
            {
                resultList[k++] = startDateTimes[i++];
            }

            while (j < endDateTimes.Length)
            {
                resultList[k++] = endDateTimes[j++];
            }

            return resultList;
        }
    }
}