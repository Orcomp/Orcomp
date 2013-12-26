namespace Orc.Submissions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Interval;

    public static class GetSortedDateTimes
    {
        public static Dictionary<string, Func<List<DateInterval>, IEnumerable<DateTime>>> Contestants { get; set; }

        static GetSortedDateTimes()
        {
            Contestants = new Dictionary<string, Func<List<DateInterval>, IEnumerable<DateTime>>>();
            //Contestants.Add("MoustafaS", MoustafaS);
            //Contestants.Add("Zaher", Zaher);
            //Contestants.Add("aus1", Aus1);
            //Contestants.Add("Renze", Renze);
            //Contestants.Add("RenzeExtended", RenzeExtended);
            //Contestants.Add("V_Tom_R", V_Tom_R);
            //Contestants.Add("SoftwareBender", SoftwareBender);
            //Contestants.Add("ErwinReid", ErwinReid);
            //Contestants.Add("Mihai", Mihai);
            //Contestants.Add("bawr", Bawr);
            Contestants.Add("c6c", C6c);
            Contestants.Add("c6c_mod", C6c_mod);
        }

        public static IEnumerable<DateTime> Run(this List<DateInterval> orderedDateIntervals, string contestant)
        {
            if (Contestants.ContainsKey(contestant))
            {
                return Contestants[contestant](orderedDateIntervals);
            }

            throw new ArgumentException("Contestant: " + contestant + ", does not exist.");
        }

        public static IEnumerable<DateTime> MoustafaS(this List<DateInterval> orderedDateIntervals)
        {
            var dates = orderedDateIntervals.ToArray();
            int c = orderedDateIntervals.Count();
            var dateTimes = new DateTime[2 * c];

            DateTime[] startings = new DateTime[c], endings = new DateTime[c];
            for (int i = c - 1; i >= 0; --i)
            {
                startings[i] = dates[i].StartTime;
                endings[i] = dates[i].EndTime;
            }
            bool change = false;
            do
            {
                change = false;
                var to = c - 1;
                for (int i = 0; i < to; ++i)
                {
                    if (endings[i] > endings[i + 1])
                    {
                        change = true;

                        var t = endings[i];
                        endings[i] = endings[i + 1];
                        endings[i + 1] = t;
                    }
                }
            } while (change);

            int sIndex = 0, eIndex = 0, arrIndex = 0;
            while (sIndex < c && eIndex < c)
            {
                if (startings[sIndex] < endings[eIndex])
                    dateTimes[arrIndex++] = startings[sIndex++];
                else
                    dateTimes[arrIndex++] = endings[eIndex++];
            }
            if (sIndex < c)
                Array.Copy(startings, sIndex, dateTimes, arrIndex, c - sIndex);
            else if (eIndex < c)
                Array.Copy(endings, eIndex, dateTimes, arrIndex, c - eIndex);
            return dateTimes;
        }

        public static IEnumerable<DateTime> Zaher(this List<DateInterval> orderedDateIntervals)
        {
            // This code is optimized for the standard algorithm defined in the Orcomp.Benchmarks project
            // Get the length of the list.
            var odr = orderedDateIntervals as List<DateInterval>;
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

        public static IEnumerable<DateTime> Aus1(this List<DateInterval> orderedDateIntervals)
        {
            int dateTimeCount = orderedDateIntervals.Count<DateInterval>();
            DateTime[] dateTimeEnd = new DateTime[dateTimeCount];
            int idx = 0;
            orderedDateIntervals.ForEach(x =>
            {
                dateTimeEnd[idx] = x.EndTime;
                idx++;
            });
            sortElements(ref dateTimeEnd);
            List<DateTime> result = new List<DateTime>();
            int endIdx = 0;
            // will have same number of times for "end" and "start" times
            DateTime currentEndItem = DateTime.MinValue;
            DateTime currentStartItem = DateTime.MinValue;
            bool doneEndList = (endIdx >= dateTimeCount);
            if (!doneEndList)
            {
                currentEndItem = dateTimeEnd[endIdx];
            }
            orderedDateIntervals.ForEach(x =>
            {
                currentStartItem = x.StartTime;
                // add from end list to result while item in end list is an earlier time
                while ((currentEndItem < currentStartItem) && (!doneEndList))
                {
                    result.Add(currentEndItem);
                    ++endIdx;
                    doneEndList = (endIdx >= dateTimeCount);
                    if (!doneEndList)
                    {
                        currentEndItem = dateTimeEnd[endIdx];
                    }
                }
                // now add item from start list
                result.Add(currentStartItem);
            });
            // add any remaining in end list to result
            while (endIdx < dateTimeCount)
            {
                result.Add(dateTimeEnd[endIdx]);
                ++endIdx;
            }
            return result;
        }

        public static IEnumerable<DateTime> Renze(this List<DateInterval> orderedDateIntervals)
        {

            var result = new DateTime[orderedDateIntervals.Count() * 2];
            int r = -1;
            var enumStart = orderedDateIntervals.GetEnumerator();
            var enumEnd = orderedDateIntervals.GetEnumerator();
            if (!enumStart.MoveNext())
            {
                // Empty list in, so empty list out.
                return result;
            }
            var startValue = enumStart.Current.StartTime;
            enumEnd.MoveNext();
            var endValue = enumEnd.Current.EndTime;
            while (true)
            {
                int comp = DateTime.Compare(startValue, endValue);
                if (comp == 0)
                {
                    result[++r] = endValue;
                    result[++r] = startValue;
                    enumEnd.MoveNext();
                    if (enumStart.MoveNext())
                    {
                        startValue = enumStart.Current.StartTime;
                        endValue = enumEnd.Current.EndTime;
                        continue;
                    }
                    break;
                }
                else if (comp < 0)
                {
                    result[++r] = startValue;
                    if (enumStart.MoveNext())
                    {
                        startValue = enumStart.Current.StartTime;
                        continue;
                    }
                    break;
                }
                else
                {
                    AddEndTime(result, r++, endValue);
                    enumEnd.MoveNext();
                    endValue = enumEnd.Current.EndTime;
                }
            }
            for (int t = r + 1; t < result.Length; t++)
            {
                AddEndTime(result, t - 1, enumEnd.Current.EndTime);
                enumEnd.MoveNext();
            }
            return result;
        }

        public static IEnumerable<DateTime> RenzeExtended(this List<DateInterval> orderedDateIntervals)
        {
            Action<DateTime[], int, DateTime> MyAddEndTime =
            delegate(DateTime[] resultIn, int rIn, DateTime endValueIn)
            {
                int n = rIn;
                while (resultIn[n] > endValueIn)
                {
                    resultIn[n + 1] = resultIn[n];
                    --n;
                }
                // result[n] <= endValue, result[n+1] does not contain a relevant value.
                resultIn[n + 1] = endValueIn;
            };
            var inp = orderedDateIntervals.ToArray();
            var result = new DateTime[inp.Count() * 2];
            int r = -1;
            var enumStart = 0;
            var enumEnd = 0;
            if (enumStart == inp.Length)
            {
                return result;
            }
            var startValue = inp[enumStart].StartTime;
            var endValue = inp[enumEnd].EndTime;
            while (true)
            {
                int comp = DateTime.Compare(startValue, endValue);
                if (comp == 0)
                {
                    result[++r] = endValue;
                    result[++r] = startValue;
                    enumEnd++;
                    if (++enumStart != inp.Length)
                    {
                        startValue = inp[enumStart].StartTime;
                        endValue = inp[enumEnd].EndTime;
                        continue;
                    }
                    break;
                }
                else if (comp < 0)
                {
                    result[++r] = startValue;
                    if (++enumStart != inp.Length)
                    {
                        startValue = inp[enumStart].StartTime;
                        continue;
                    }
                    break;
                }
                else
                {
                    MyAddEndTime(result, r++, endValue);
                    ++enumEnd;
                    endValue = inp[enumEnd].EndTime;
                }
            }
            for (int t = r + 1; t < result.Length; t++)
            {
                MyAddEndTime(result, t - 1, inp[enumEnd++].EndTime);
            }
            return result;
        }

        public static IEnumerable<DateTime> V_Tom_R(this List<DateInterval> orderedDateIntervals)
        {
            int currentPosition = 0;
            int startIndex = 0;
            int endIndex = 0;
            int index;
            int capacity = orderedDateIntervals.Count();

            DateTime[] endDateTimes;
            DateTime[] dateTimes = null;

            DateTime tmp;

            if (capacity > 0)
            {
                endDateTimes = new DateTime[capacity];
                dateTimes = new DateTime[2 * capacity];

                foreach (DateInterval DateInterval in orderedDateIntervals)
                {
                    while (currentPosition < endIndex
                           && endDateTimes[currentPosition] <= DateInterval.StartTime)
                    {
                        dateTimes[startIndex++] = endDateTimes[currentPosition++];
                    }
                    dateTimes[startIndex++] = DateInterval.StartTime;
                    endDateTimes[endIndex++] = DateInterval.EndTime;

                    //if current EndTime>maximum EndTime from endDateTimes, then apply bubble sort
                    //on current range
                    index = endIndex - 1;
                    while (index > currentPosition
                           && endDateTimes[index] < endDateTimes[index - 1])
                    {
                        tmp = endDateTimes[index - 1];
                        endDateTimes[index - 1] = endDateTimes[index];
                        endDateTimes[index] = tmp;
                        index--;
                    }
                }
                while (currentPosition < capacity)
                {
                    dateTimes[startIndex++] = endDateTimes[currentPosition++];
                }
            }
            return dateTimes;
        }

        public static IEnumerable<DateTime> SoftwareBender(this List<DateInterval> orderedDateIntervals)
        {

            List<DateInterval> list = orderedDateIntervals.ToList();
            int len = list.Count;
            DateTime[] result = new DateTime[2 * len];

            for (int i = 0, p = 0, first = len, end = len; i < len; i++)
            {
                DateTime startTime = list[i].StartTime, endTime = list[i].EndTime;

                while (end - first > 0 && result[first] <= startTime)
                {
                    result[p++] = result[first++];
                }

                result[p++] = startTime;

                result[end] = endTime;

                for (int j = end++; j >= first && result[j - 1] > result[j]; j--)
                {
                    Swap(ref result[j - 1], ref result[j]);
                }
            }

            return result;
        }

        public static IEnumerable<DateTime> ErwinReid(this List<DateInterval> orderedDateIntervals)
        {
            var l = orderedDateIntervals.Count<DateInterval>();

            var en = new DateTime[l];
            var st = new DateTime[l];

            int t = 0;
            var rge = orderedDateIntervals.ToArray<DateInterval>();
            foreach (var d in rge)
            {
                st[t] = d.StartTime;
                en[t++] = d.EndTime;
            }

            /*foreach (var d in orderedDateIntervals)
            {
                str[t] = d.StartTime;
                end[t++] = d.EndTime;
            }*/

            // Now we will use Insertion sort (http://xbfish.com/2011/11/03/insertion-sort-in-c/)
            DateTime tmp; int k;
            for (var i = 1; i < en.Length; i++)
            {
                tmp = en[i];
                k = i - 1;
                while (k >= 0 && en[k] > tmp)
                {
                    en[k + 1] = en[k];
                    k--;
                }
                en[k + 1] = tmp;
            }

            var res = new DateTime[l * 2];

            res[0] = st[0];
            var sP = 1;
            var eP = 0;
            var lP = l - 1;

            for (var i = 1; i < l; i++)
            {
                res[sP++] = st[i];

                while (eP < l && (i == lP || st[i + 1] > en[eP]))
                {
                    res[sP++] = en[eP++];
                }
            }

            /*
            var c = res[0];
            for (int i = 1; i < res.Length; i++)
            {
                if (res[i] < c)
                {
                    var error = 1;
                }
                c = res[i];
            }*/

            return res;
        }

        public static IEnumerable<DateTime> Mihai(this List<DateInterval> orderedDateIntervals)
        {
            return AreEndTimesSorted(orderedDateIntervals)
                       ? FastMerge(orderedDateIntervals.ToArray())
                       : NormalSort(orderedDateIntervals);

        }

        //public static IEnumerable<DateTime> Bawr(this List<DateInterval> orderedDateIntervals)
        //{
        //    var source = orderedDateIntervals as List<DateInterval>;
        //    var output = new long[2 * source.Count()];

        //    if (MergeTry(source, output) == false)
        //    {
        //        output = MergeAny(source, output);
        //    }

        //    return output.Select(t => new DateTime(t)).ToArray();

        //}

        public static IEnumerable<DateTime> C6c(this List<DateInterval> orderedDateIntervals)
        {
            var DateIntervals = orderedDateIntervals.ToArray();
            var startDateTimes = new DateTime[DateIntervals.Length];
            var endDateTimes = new DateTime[DateIntervals.Length];
            for (int i = 0; i < DateIntervals.Length; i++)
            {
                startDateTimes[i] = DateIntervals[i].StartTime;
                endDateTimes[i] = DateIntervals[i].EndTime;
            }

            if (!CheckSorted(endDateTimes))
                Array.Sort(endDateTimes);
            return MergeSortedArrays(startDateTimes, endDateTimes);
        }

        public static IEnumerable<DateTime> C6c_mod(this List<DateInterval> orderedDateIntervals)
        {
            // c6c version modified

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

        #region Zaher

        private static DateTime addEndTimes(List<DateInterval> odr, DateTime[] sorted, DateTime ti, ref int iti, ref int i)
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

        #endregion

        #region aus1
        // http://www.softwareandfinance.com/CSharp/Insertion_Sort.html

        private static void sortElements(ref DateTime[] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                int j = i;
                while (j > 0)
                {
                    if (arr[j - 1] > arr[j])
                    {
                        DateTime temp = arr[j - 1];
                        arr[j - 1] = arr[j];
                        arr[j] = temp;
                        j--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region Renze

        private static void AddEndTime(DateTime[] result, int r, DateTime endValue)
        {
            int n = r;
            while (result[n] > endValue)
            {
                result[n + 1] = result[n];
                --n;
            }
            // result[n] <= endValue, result[n+1] does not contain a relevant value.
            result[n + 1] = endValue;
        }

        #endregion

        #region SoftwareBender

        private static void Swap(ref DateTime time1, ref DateTime time2)
        {
            DateTime temp = time1;
            time1 = time2;
            time2 = temp;
        }

        #endregion

        #region Mihai

        private static IEnumerable<DateTime> FastMerge(DateInterval[] DateIntervals)
        {
            var count = DateIntervals.Length;
            var c = new DateTime[count * 2];
            int aindex = 0, bindex = 0, cindex = 0;
            DateTime first, second;
            while (bindex < count)
            {
                if (aindex < count)
                {
                    first = DateIntervals[aindex].StartTime;
                    second = DateIntervals[bindex].EndTime;
                    if (first < second)
                    {
                        c[cindex] = first;
                    }
                    else
                    {
                        c[cindex] = second;
                    }
                    cindex++;
                    aindex++;
                    continue;
                }
                c[cindex++] = DateIntervals[bindex].EndTime;
                bindex++;
            }
            return c;
        }

        private static IEnumerable<DateTime> NormalSort(IEnumerable<DateInterval> orderedDateIntervals)
        {
            var arrayLen = orderedDateIntervals.Count();

            var startTimes = new List<DateTime>(arrayLen);
            var endTimes = new List<DateTime>(arrayLen);

            var orderedRangeArray = orderedDateIntervals.ToList();
            for (var pos = 0; pos < arrayLen; pos++)
            {
                startTimes.Add(orderedRangeArray[pos].StartTime);
                endTimes.Add(orderedRangeArray[pos].EndTime);
            }
            endTimes.Sort();
            var mergedTimes = Merge(startTimes, endTimes);
            return mergedTimes;
        }

        private static bool AreEndTimesSorted(IEnumerable<DateInterval> orderedDateIntervals)
        {
            var enu = orderedDateIntervals.GetEnumerator();
            enu.MoveNext();
            var prev = enu.Current;
            while (enu.MoveNext())
            {
                if (enu.Current.EndTime < prev.EndTime)
                {
                    return false;
                }
            }
            return true;
        }

        private static List<T> Merge<T>(List<T> a, List<T> b) where T : IComparable
        {
            var c = new List<T>(a.Count + b.Count);
            int aindex = 0, bindex = 0, cindex = 0;
            while ((aindex < a.Count) || (bindex < b.Count))
            {
                if ((bindex >= b.Count) || ((aindex < a.Count) && (a[aindex].CompareTo(b[bindex]) < 0)))
                    c.Insert(cindex++, a[aindex++]);
                else
                    c.Insert(cindex++, b[bindex++]);
            }
            return c;
        }

        #endregion

        #region bawr
        // Problem with CountDownThreading not implemented in .NET 3.5

        //public static bool MergeTry(List<DateInterval> source, long[] output)
        //{
        //    var iter_a = source.GetEnumerator();
        //    var have_a = iter_a.MoveNext();

        //    var iter_b = source.GetEnumerator();
        //    var have_b = iter_b.MoveNext();

        //    int b = 0;
        //    int c = 2 * source.Count();

        //    int i = 0;
        //    int j = 0;
        //    int k = 0;

        //    var l = 0L;

        //    while (have_a)
        //    {
        //        var time_a = iter_a.Current.StartTime.Ticks;
        //        var time_b = iter_b.Current.EndTime.Ticks;

        //        if (time_a < time_b)
        //        {
        //            output[i++] = time_a;
        //            have_a = iter_a.MoveNext();
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    if (c <= i * 3)
        //    {
        //        return false;
        //    }


        //    b = i;
        //    j = b + 1;
        //    k = b * 3 + 1;
        //    while (j < k)
        //    {
        //        long time_b = iter_b.Current.EndTime.Ticks;
        //        output[j] = time_b;
        //        l = (time_b < l) ? long.MaxValue : time_b;
        //        j += 2;
        //        iter_b.MoveNext();
        //        time_b = iter_b.Current.EndTime.Ticks;
        //        output[j] = time_b;
        //        iter_b.MoveNext();
        //        l = (time_b < l) ? long.MaxValue : time_b;
        //        j += 2;
        //    }
        //    if (l == long.MaxValue)
        //    {
        //        return false;
        //    }

        //    j = b * 3 + 1;
        //    k = c - b;
        //    while (j < k)
        //    {
        //        long time_a = iter_a.Current.StartTime.Ticks;
        //        long time_b = iter_a.Current.EndTime.Ticks;
        //        output[i] = time_a;
        //        output[j] = time_b;
        //        iter_a.MoveNext();
        //        l = (time_b < l) ? long.MaxValue : time_b;
        //        i += 2;
        //        j += 2;
        //    }
        //    if (l == long.MaxValue)
        //    {
        //        return false;
        //    }

        //    j = c - b;
        //    k = c;
        //    while (j < k)
        //    {
        //        long time_a = iter_a.Current.StartTime.Ticks;
        //        long time_b = iter_a.Current.EndTime.Ticks;
        //        output[i] = time_a;
        //        output[j] = time_b;
        //        iter_a.MoveNext();
        //        l = (time_b < l) ? long.MaxValue : time_b;
        //        i += 2;
        //        j += 1;
        //    }
        //    if (l == long.MaxValue)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //private static long[] MergeAny(IEnumerable<DateInterval> source, long[] output)
        //{
        //    var bounds = new LinkedList<int>();

        //    int i = 0;
        //    int j = source.Count();

        //    output[j - 1] = 0;
        //    foreach (var v in source)
        //    {
        //        output[i] = v.StartTime.Ticks;
        //        output[j] = v.EndTime.Ticks;
        //        if (output[j] < output[j - 1])
        //        {
        //            bounds.AddLast(j);
        //        }
        //        i++;
        //        j++;
        //    }

        //    j = source.Count();
        //    if (output[j] < output[j - 1])
        //    {
        //        bounds.AddFirst(j);
        //    }

        //    if (bounds.IsEmpty())
        //    {
        //        return output;
        //    }
        //    else
        //    {
        //        bounds.AddFirst(0);
        //        bounds.AddLast(output.Length);
        //    }

        //    return MergeAll(output, bounds);
        //}

        //private static long[] MergeAll(long[] output, LinkedList<int> bounds)
        //{
        //    var buffer = new long[output.Count()];

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
        //                    MergeTwo(tmp_src, tmp_buf, (MergeWindow)w);
        //                    counter.Signal();
        //                },
        //                new MergeWindow(w_node)
        //                );
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

        //private static void MergeTwo(long[] source, long[] buffer, MergeWindow window)
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
        //        var time_d = time_a - time_b;

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
        //        this.bound_a = node.Previous.Value;
        //        this.bound_b = node.Value;
        //        this.bound_c = node.Next.Value;
        //    }
        //}

        #endregion

        #region c6c

        private static bool CheckSorted(DateTime[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i] > array[i + 1])
                {
                    return false;
                }
            }
            return true;
        }

        private static DateTime[] MergeSortedArrays(DateTime[] startDateTimes, DateTime[] endDateTimes)
        {
            var resultList = new DateTime[startDateTimes.Length + endDateTimes.Length];
            int i = 0;
            int j = 0;
            int k = 0;
            while (i < startDateTimes.Length && j < endDateTimes.Length)
            {
                if (startDateTimes[i] <= endDateTimes[j])
                    resultList[k++] = startDateTimes[i++];
                else
                    resultList[k++] = endDateTimes[j++];
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

        #endregion
    }
}
