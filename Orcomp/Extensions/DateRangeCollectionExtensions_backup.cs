using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Orcomp.Entities;

namespace Orcomp.Extensions
{
    public static class DateRangeCollectionExtensions
    {
        public static IEnumerable<DateTime> GetSortedDateTimes(this IEnumerable<DateRange> orderedDateRanges)
        {
            // This code is optimized for the standard algorithm defined in the Orcomp.Benchmarks project
            // Get the length of the list.
            List<DateRange> odr = orderedDateRanges as List<DateRange>;
            int length = odr.Count;
            // Create the sorted array
            DateTime[] sorted = new DateTime[length * 2];
            DateTime ti, lti;
            int iti, i;
            // Add the first start time.
            sorted[0] = odr[0].StartTime;
            lti = odr[0].EndTime;
            iti = 0;
            i = 1;
            // Loop through the list to add start times and conforming end times.
            for (int ist = 1; ist < length; ist++)
            {
                ti = odr[ist].StartTime;
                if (ti >= lti)
                {
                    // There is an end time that can be added before this start time.
                    sorted[i] = lti;
                    i++;
                    iti++;
                    lti = odr[iti].EndTime;
                    // Checked if we still have other conforming end times
                    if (lti <= ti)
                    {
                        // Loop through the other conforming end times and add them.
                        lti = addEndTimes(odr, sorted, ti, ref iti, ref i);
                    }
                }
                // Add the start time.
                sorted[i] = ti;
                i++;
            }
            // Continue adding other end times.
            while (iti < length)
            {
                ti = odr[iti].EndTime;
                if (ti >= lti)
                {
                    // Add the end time to the current place in the sorted array.
                    sorted[i] = ti;
                    lti = ti;
                }
                else
                {
                    // Insert the end time at the correct place in the sorted array.
                    searchAndInsert(sorted, i, ti);
                }
                i++;
                iti++;
            }
            // Done, return the sorted array.
            return sorted;
        }


        private static DateTime addEndTimes(List<DateRange> odr, DateTime[] sorted, DateTime ti, ref int iti, ref int i)
        {
            DateTime lti = sorted[i - 1];
            DateTime et;
            // Loop through end times that can be added before this start time.
            while ((et = odr[iti].EndTime) <= ti)
            {
                if (et < lti)
                {
                    // This end time is not in the correct order, add it to the correct location in the sorted list.
                    searchAndInsert(sorted, i, et);
                }
                else
                {
                    // Add the end time to the current location in the sorted list.
                    sorted[i] = et;
                    lti = et;
                }
                i++;
                iti++;
            }
            // Return the last end time that will be used for coparisons with start times.
            return et;
        }

        private static void searchAndInsert(DateTime[] sorted, int i, DateTime ti)
        {
            // Search for the correct place for this end item
            int il = i - 2;
            while (sorted[il] > ti)
            {
                il--;
            }
            // Shift the items in the array.
            for (int ii = i - 1; ii > il; ii--)
            {
                sorted[ii + 1] = sorted[ii];
            }
            // Add the end time.
            sorted[il + 1] = ti;
        }

        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetCurrentThread();
        //[DllImport("kernel32.dll")]
        //static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

        //--------------------------------------------------------------
        //public static IEnumerable<DateTime> GetSortedDateTimes(this IEnumerable<DateRange> source)
        //{
        //    var output = new DateTime[2 * source.Count()];
        //    var bounds = new LinkedList<int>();

        //    var iter_a = source.GetEnumerator();
        //    var have_a = iter_a.MoveNext();

        //    var iter_b = source.GetEnumerator();
        //    var have_b = iter_b.MoveNext();
        //    var last_v = DateTime.MinValue.Ticks;

        //    int i = 0;

        //    while (have_a && have_b)
        //    {
        //        var time_a = iter_a.Current.StartTime;
        //        var time_b = iter_b.Current.EndTime;

        //        var date_d = time_a.Ticks - time_b.Ticks;

        //        if (date_d > 0)
        //        {
        //            output[i++] = time_b;
        //            have_b = iter_b.MoveNext();
        //            if (last_v > time_b.Ticks)
        //            {
        //                bounds.AddLast(i - 1);
        //            }
        //            last_v = time_b.Ticks;
        //            continue;
        //        }

        //        if (date_d < 0)
        //        {
        //            output[i++] = time_a;
        //            have_a = iter_a.MoveNext();
        //            continue;
        //        }

        //        if (date_d == 0)
        //        {
        //            output[i++] = time_a;
        //            output[i++] = time_b;
        //            have_a = iter_a.MoveNext();
        //            have_b = iter_b.MoveNext();
        //            last_v = time_b.Ticks;
        //            continue;
        //        }
        //    }

        //    while (have_a)
        //    {
        //        var time_a = iter_a.Current.StartTime;
        //        output[i++] = time_a;
        //        have_a = iter_a.MoveNext();
        //    }

        //    while (have_b)
        //    {
        //        var time_b = iter_b.Current.EndTime;
        //        output[i++] = time_b;
        //        have_b = iter_b.MoveNext();
        //        if (last_v > time_b.Ticks)
        //        {
        //            bounds.AddLast(i - 1);
        //        }
        //        last_v = time_b.Ticks;
        //    }

        //    if (bounds.Count == 0)
        //    {
        //        return output;
        //    }

        //    bounds.AddFirst(0);
        //    bounds.AddLast(i);

        //    var buffer = new DateTime[source.Count()];

        //    while (bounds.Count > 2)
        //    {
        //        var tmp_src = output;
        //        var tmp_buf = buffer;

        //        var w_node = bounds.First.Next;
        //        var w_copy = (bounds.Count - 1) % 2 == 1;
        //        int merges = (bounds.Count - 1) / 2;

        //        var counter = new System.Threading.CountdownEvent(merges);

        //        while (merges > 0)
        //        {
        //            System.Threading.ThreadPool.QueueUserWorkItem(
        //                w =>
        //                {
        //                    Merge(tmp_src, tmp_buf, (MergeWindow)w);
        //                    counter.Signal();
        //                },
        //                new MergeWindow(w_node)
        //            );
        //            var w_next = w_node.Next.Next;
        //            bounds.Remove(w_node);
        //            w_node = w_next;
        //            merges--;
        //        }

        //        if (w_copy)
        //        {
        //            var w_prev = w_node.Previous;
        //            Array.Copy(tmp_src, w_prev.Value, tmp_buf, w_prev.Value, w_node.Value - w_prev.Value);
        //        }

        //        counter.Wait();

        //        output = tmp_buf;
        //        buffer = tmp_src;
        //    }

        //    return output;
        //}

        //private static void Merge(DateTime[] source, DateTime[] buffer, MergeWindow window)
        //{
        //    var iter_a = window.bound_a;
        //    var have_a = true;

        //    var iter_b = window.bound_b;
        //    var have_b = true;

        //    int i = window.bound_a;

        //    while (have_a && have_b)
        //    {
        //        var time_a = source[iter_a];
        //        var time_b = source[iter_b];
        //        var time_d = time_a.Ticks - time_b.Ticks;

        //        if (time_d > 0)
        //        {
        //            buffer[i++] = time_b;
        //            have_b = ++iter_b < window.bound_c;
        //            continue;
        //        }

        //        if (time_d < 0)
        //        {
        //            buffer[i++] = time_a;
        //            have_a = ++iter_a < window.bound_b;
        //            continue;
        //        }

        //        if (time_d == 0)
        //        {
        //            buffer[i++] = time_a;
        //            buffer[i++] = time_b;
        //            have_a = ++iter_a < window.bound_b;
        //            have_b = ++iter_b < window.bound_c;
        //            continue;
        //        }
        //    }

        //    while (have_a)
        //    {
        //        var time_a = source[iter_a];
        //        buffer[i++] = time_a;
        //        have_a = ++iter_a < window.bound_b;
        //    }

        //    while (have_b)
        //    {
        //        var time_b = source[iter_b];
        //        buffer[i++] = time_b;
        //        have_b = ++iter_b < window.bound_c;
        //    }
        //}

        //private struct MergeWindow
        //{
        //    public readonly int bound_a;
        //    public readonly int bound_b;
        //    public readonly int bound_c;

        //    public MergeWindow(LinkedListNode<int> node)
        //    {
        //        bound_a = node.Previous.Value;
        //        bound_b = node.Value;
        //        bound_c = node.Next.Value;
        //    }
        //}

        //----------------------------------------
        //public static IEnumerable<DateTime> GetSortedDateTimes(this IEnumerable<DateRange> source)
        //{
        //    var output = new DateTime[2 * source.Count()];
        //    var points = new LinkedList<int>();

        //    var iter_a = source.GetEnumerator();
        //    var have_a = iter_a.MoveNext();

        //    var iter_b = source.GetEnumerator();
        //    var have_b = iter_b.MoveNext();
        //    var last_b = DateTime.MinValue.Ticks;

        //    int i = 0;

        //    while (have_a && have_b)
        //    {
        //        var time_a = iter_a.Current.StartTime;
        //        var time_b = iter_b.Current.EndTime;

        //        var diff = time_a.Ticks - time_b.Ticks;
        //        if (diff == 0)
        //        {
        //            output[i++] = time_a;
        //            output[i++] = time_b;
        //            have_a = iter_a.MoveNext();
        //            have_b = iter_b.MoveNext();
        //            last_b = time_b.Ticks;
        //        }
        //        else if (diff < 0)
        //        {
        //            output[i++] = time_a;
        //            have_a = iter_a.MoveNext();
        //        }
        //        else
        //        {
        //            output[i++] = time_b;
        //            have_b = iter_b.MoveNext();
        //            if (last_b <= time_b.Ticks)
        //            {
        //                last_b = time_b.Ticks;
        //            }
        //            else
        //            {
        //                points.AddLast(i - 1);
        //            }
        //        }
        //    }

        //    while (have_a)
        //    {
        //        output[i++] = iter_a.Current.StartTime;
        //        have_a = iter_a.MoveNext();
        //    }

        //    while (have_b)
        //    {
        //        output[i++] = iter_b.Current.EndTime;
        //        have_b = iter_b.MoveNext();
        //    }

        //    if (points.Count > 0)
        //    {
        //        var buffer = new DateTime[2 * source.Count()];

        //        points.AddLast(i);
        //        Merge(ref output, ref buffer, points);

        //        points.AddFirst(0);
        //        Merge(ref output, ref buffer, points);
        //    }

        //    return output;
        //}

        //public static void Merge(ref DateTime[] src, ref DateTime[] dst, LinkedList<int> pts)
        //{
        //    while (pts.Count > 2)
        //    {
        //        DateTime[] tmp_src = src;
        //        DateTime[] tmp_dst = dst;

        //        var node = pts.First.Next;
        //        var next = node;

        //        while (node != null && node.Next != null)
        //        {
        //            Merge(tmp_src, tmp_dst, node.Previous.Value, node.Value, node.Next.Value);
        //            next = node.Next.Next;
        //            pts.Remove(node);
        //            node = next;
        //        }

        //        if (node != null)
        //        {
        //            Array.Copy(tmp_src, node.Previous.Value, tmp_dst, node.Previous.Value, node.Value - node.Previous.Value);
        //        }

        //        src = tmp_dst;
        //        dst = tmp_src;
        //    }
        //}

        //public static void Merge(DateTime[] src, DateTime[] dst, int s_0, int s_1, int end)
        //{
        //    int i_d = s_0;

        //    int i_0 = s_0;
        //    int e_0 = s_1;
        //    int c_0 = e_0 - i_0;

        //    int i_1 = s_1;
        //    int e_1 = end;
        //    int c_1 = e_1 - i_1;

        //    int c_2 = c_0 + c_1;

        //    while (i_0 < e_0 && i_1 < e_1)
        //    {
        //        if (src[i_0] <= src[i_1])
        //        {
        //            dst[i_d++] = src[i_0++];
        //        }
        //        else
        //        {
        //            dst[i_d++] = src[i_1++];
        //        }
        //    }

        //    while (i_0 < e_0)
        //    {
        //        dst[i_d++] = src[i_0++];
        //    }

        //    while (i_1 < e_1)
        //    {
        //        dst[i_d++] = src[i_1++];
        //    }
        //}

        //public static IEnumerable<DateTime> GetSortedDateTimes(this IEnumerable<DateRange> orderedDateRanges)
        //{
        //    int numDateRanges = orderedDateRanges.Count();

        //    DateTime[] result = new DateTime[numDateRanges * 2];

        //    var startDateTimes = new DateTime[numDateRanges];
        //    var endDateTimes = new DateTime[numDateRanges];

        //    int index = 0;

        //    foreach (var x in orderedDateRanges)
        //    {
        //        startDateTimes[index] = x.StartTime;
        //        endDateTimes[index++] = x.EndTime;
        //    }

        //    BubbleSort(endDateTimes);

        //    return startDateTimes.MergeSort(endDateTimes);
        //}

        public static IEnumerable<DateTime> MergeSort(this DateTime[] startTimes, DateTime[] endTimes)
        {
            int numDateRanges = startTimes.Count();
            var result = new DateTime[numDateRanges * 2];

            int indexStart = 0;
            int indexEnd = 0;
            int resultindex = 0;

            while (numDateRanges != indexStart)
            {
                if (startTimes[indexStart] <= endTimes[indexEnd])
                {
                    result[resultindex++] = startTimes[indexStart++];
                }
                else
                {
                    result[resultindex++] = endTimes[indexEnd++];
                }
            }
            while (numDateRanges != indexEnd)
            {
                result[resultindex++] = endTimes[indexEnd++];
            }

            return result;
        }

        //public static IEnumerable<DateTime> GetSortedDateTimes(this IEnumerable<DateRange> orderedDateRanges)
        //{
        //    var orderedDateRangesCollection = orderedDateRanges.ToList();
        //    var numDateRanges = orderedDateRangesCollection.Count;

        //    var tempDateTimes = new DateTime[numDateRanges * 2];

        //    DateTime endTime = DateTime.MinValue;

        //    var tempDateTimesCollection = new List<List<DateTime>>();

        //    for (int i = 0; i < numDateRanges; i++ )
        //    {
        //        if (orderedDateRangesCollection[i].StartTime > endTime)
        //        {
        //            //tempDateTimesCollection.Add(tempDateTimes);
        //            //tempDateTimes = new List<DateTime>();

        //            endTime = orderedDateRangesCollection[i].EndTime;
        //        }

        //        tempDateTimes[i*2]= orderedDateRangesCollection[i].StartTime;
        //        tempDateTimes[i*2+1] = orderedDateRangesCollection[i].EndTime;
        //    }

        //    //tempDateTimesCollection.Add(tempDateTimes);

        //    return tempDateTimes;
        //}

        //public static IList<T> InsertSort<T>(this IList<T> list) where T : IComparable<T>
        //{
        //    int i, j;

        //    for (i = 1; i < list.Count; i++)
        //    {
        //        T value = list[i];
        //        j = i - 1;
        //        while ((j >= 0) && (list[j].CompareTo(value) > 0))
        //        {
        //            list[j + 1] = list[j];
        //            j--;
        //        }
        //        list[j + 1] = value;
        //    }

        //    return list;
        //}
    }
}
