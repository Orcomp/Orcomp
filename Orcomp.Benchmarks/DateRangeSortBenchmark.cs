using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Orcomp.Entities;
using Orcomp.Extensions;
using Orcomp.Utilities;

namespace Orcomp.Benchmarks
{
    public static class DateRangeSortBenchmark
    {
        public static List<DateRange> GetBenchmarkData(int numberOfDateRanges)
        {
            var dateRanges = GetDateRanges(numberOfDateRanges);
            // var dateRanges = GetDateRanges3();

            // Our assumption is that the list of DateRanges is already pre-sorted.
            return dateRanges.OrderBy(x => x).ToList();
        }

        public static Tuple<string, double, double, double> Run(List<DateRange> benchmarkData, string contestant)
        {
            var numberOfIterations = 20;

            var ratios = new List<double>();

            Stopwatch sw1 = Stopwatch.StartNew();
            Stopwatch sw2 = Stopwatch.StartNew();

            var times1 = new List<double>();
            var times2 = new List<double>();

            Enumerable.Range(1, numberOfIterations).ForEach(i =>
                {
                    GC.Collect();

                    sw1.Reset();
                    sw1.Start();
                    var sortedDateTimesQuickSort = GetSortedDateTimesQuickSort(benchmarkData);
                    sw1.Stop();

                    GC.Collect();

                    sw2.Reset();
                    sw2.Start();
                    var sortedDateTimes = benchmarkData.GetSortedDateTimes(contestant);
                    sw2.Stop();

                    //Console.WriteLine("Loop " + i +  " - Elasped time: " + sw2.ElapsedMilliseconds);

                    // check that the two results are identical.
                    // Xunit.Assert.Equal( sortedDateTimesQuickSort, sortedDateTimes );

                    times1.Add((double)sw1.ElapsedTicks);
                    times2.Add((double)sw2.ElapsedTicks);

                    //double ratio = (double) sw1.ElapsedTicks / (double) sw2.ElapsedTicks;
                    //ratios.Add( ratio );
                    //Console.WriteLine(  "Loop " + i + ": Ratio: " + ratio );
                } );


            var avgQuickSort = MathUtils.FilterData(times1.Skip(10)).Average();

            ratios = MathUtils.FilterData(times2.Skip(10)).Select(x => avgQuickSort / x).ToList();

            Console.WriteLine("Finished: " + contestant);

            return new Tuple<string, double, double, double>( contestant, ratios.Average(), ratios.StandardDeviation(), ratios.Min() );

            //Console.WriteLine(">> " + contestant + ", AvgRatio: " + ratios.Average().ToStringDecimal(3) + ", SD: " + ratios.StandardDeviation().ToStringDecimal(3) + ", WorstCase: " + ratios.Min().ToStringDecimal(3));
           // Console.ReadLine();
        }


        public static IEnumerable<DateRange> GetDateRanges(int count)
        {
            var date = DateTime.Now;

            return Enumerable.Range( 1, count ).Select( x => new DateRange( date.AddMinutes( x ), date.AddMinutes( x + 100 ) ) );
        }

        public static IEnumerable<DateRange> GetDateRanges2(int count)
        {
            var date = DateTime.Now;

            var r = new Random();

            return Enumerable.Range(1, count).Select(x => new DateRange(date.AddMinutes(x), date.AddMinutes(x + r.Next(0, 100))));
        }

        public static IEnumerable<DateRange> GetDateRanges3()
        {
            var n = 500;
            var date = DateTime.Now;

            var output = new List<DateRange>();

            for (int i = 0; i < n; i++ )
            {
                for (int j = 0;j < n; j++ )
                {
                    output.Add(new DateRange(date.AddMinutes(2*n * i).AddMinutes(j), date.AddMinutes(2*n * i).AddMinutes(2 * n - j)));
                }
            }

            return output;
        }

        public static List<DateTime> GetSortedDateTimesQuickSort(List<DateRange> orderedDateRanges )
        {
            var dateTimes = new List<DateTime>(2*orderedDateRanges.Count);

            orderedDateRanges.ForEach(x => dateTimes.AddRange(new List<DateTime> { x.StartTime, x.EndTime }));

            dateTimes.Sort();

            return dateTimes;
        }

        public static List<DateTime> GetSortedDateTimes(List<DateRange> orderedDateRanges)
        {
            // Various approaches I have tried.

            // NOTE: Union does not keep duplicates, so is a bad implementation.

            var dateTimes = new List<DateTime>();

            //orderedDateRanges.ForEach(x => dateTimes.AddRange(new List<DateTime> { x.StartTime, x.EndTime }));

            //-------------------------------------------------------------------------------
            // Loop through the orderedDateRanges list twice, so this will nearly twice as slow.
            //dateTimes.AddRange(orderedDateRanges.Select(x => x.StartTime));
            //dateTimes.AddRange(orderedDateRanges.Select(x => x.EndTime));


            //-------------------------------------------------------------------------------
            var dateTimesStart = new List<DateTime>();
            var dateTimesEnd = new List<DateTime>();

            orderedDateRanges.ForEach(x =>
            {
                dateTimesStart.Add(x.StartTime);
                dateTimesEnd.Add(x.EndTime);
            });

            dateTimes = dateTimesStart.Union(dateTimesEnd).ToList();
            //dateTimes = dateTimesEnd.Union(dateTimesStart).ToList();


            //-------------------------------------------------------------------------------
            //var dateTimesStart = new DateTime[orderedDateRanges.Count];
            //var dateTimesEnd = new DateTime[orderedDateRanges.Count];

            //orderedDateRanges.ForEach((x, i) =>
            //{
            //    dateTimesStart[i] = x.StartTime;
            //    dateTimesEnd[i] = x.EndTime;
            //});

            //dateTimes = dateTimesStart.Union(dateTimesEnd).ToList();

            //-------------------------------------------------------------------------------
            //var dateTimesFull = new DateTime[2 * orderedDateRanges.Count];

            //orderedDateRanges.ForEach((x, i) =>
            //{
            //    dateTimesFull[i] = x.StartTime;
            //    dateTimesFull[orderedDateRanges.Count + i] = x.EndTime;
            //});

            //dateTimes = dateTimesEnd.Union( dateTimesStart ).ToList();


            // Concat function is also slow.

            //dateTimes.AddRange( dateTimesStart );
            //dateTimes.AddRange( dateTimesEnd );

            dateTimes = dateTimes.OrderBy( x => x ).ToList();

            //dateTimes = InsertSort( dateTimes.ToArray() );

            return dateTimes;
        }
    }
}
