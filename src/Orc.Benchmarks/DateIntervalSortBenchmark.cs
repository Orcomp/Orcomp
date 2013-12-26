using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Orc.Benchmarks
{
    using Orc.Extensions;
    using Orc.Interval;
    using Orc.Submissions;
    using Orc.Utilities;

    public static class DateIntervalSortBenchmark
    {
        private static Dictionary<string, IEnumerable<DateInterval>> BenchmarkData { get; set; } 

        static DateIntervalSortBenchmark()
        {
            var numberOfDateIntervals = 40000; // Must be square rootable

            BenchmarkData = new Dictionary<string, IEnumerable<DateInterval>>();
            BenchmarkData.Add("Sorted EndTimes", GetDateIntervalsEndTimesSorted(numberOfDateIntervals));
            BenchmarkData.Add("Random EndTimes", GetDateIntervalsRandomEndTimes(numberOfDateIntervals, 1));
            BenchmarkData.Add("Random EndTimes and StartTimes", GetDateIntervalsRandomStartAndEndTimes(numberOfDateIntervals, 1));
            BenchmarkData.Add("Multi Group descending EndTimes", GetDateIntervalsMultiGroupDescendingEndTimes(numberOfDateIntervals));

            Console.WriteLine("Finished creating benchmark data");
        }

        public static void Run()
        {
            var benchmarkData = BenchmarkData["Sorted EndTimes"].OrderBy(x => x).ToList();
            
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Benchmark Data: " + "Sorted EndTimes");
            Console.WriteLine(string.Empty);

            var results = GetContestantsResults(benchmarkData).OrderByDescending(x => x.Item2).ToList();

            Console.WriteLine(string.Empty);
            results.ForEach((x, i) => PrintResults(i + 1, x));

            Console.ReadLine();
        }

        public static void RunMulti()
        {
            var multiResults = new List<Tuple<string, double, double, double>>();

            foreach (var data in BenchmarkData)
            {
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Benchmark Data: " + data.Key);
                Console.WriteLine(string.Empty);

                var results = GetContestantsResults( data.Value.ToList());

                results = results.OrderByDescending( x => x.Item2 ).ToList();

                multiResults.AddRange( results );

                Console.WriteLine(string.Empty);
                Console.WriteLine("Printing results for: " + data.Key);
                results.ForEach( ( x, i ) => PrintResults( i + 1, x ) );
            }

            // Group results by contestant name
            var groupedResults = multiResults.GroupBy( x => x.Item1 ).ToDictionary( x => x.Key, x => x.Select( y => y.Item2 ).Average() );
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Average for all benchmark Data:");
            groupedResults.OrderByDescending( x => x.Value ).ForEach( (x,i) => Console.WriteLine("{0} - {1} Average: {2}", i, x.Key, x.Value) );
            Console.ReadLine();
        }

        static List<Tuple<string, double, double, double>> GetContestantsResults(List<DateInterval> benchmarkData)
        {
            var results = new List<Tuple<string, double, double, double>>();
            GetSortedDateTimes.Contestants.ForEach(x => results.Add(GetResult(benchmarkData, x.Key, x.Value)));
            
            return results;
        }

        static void PrintResults(int number, Tuple<string, double, double, double> result)
        {
            Console.WriteLine("{0} - {1}, AvgRatio: {2}, SD: {3}, WorstCase: {4}", number, result.Item1, result.Item2.ToStringDecimal(3), result.Item3.ToStringDecimal(3), result.Item4.ToStringDecimal(3));
        }

        public static Tuple<string, double, double, double> GetResult(List<DateInterval> benchmarkData, string contestant, Func<List<DateInterval>, IEnumerable<DateTime>> entry)
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
                    var sortedDateTimes = entry(benchmarkData);
                    sw2.Stop();

                    times1.Add(sw1.ElapsedTicks);
                    times2.Add(sw2.ElapsedTicks);
                } );


            var avgQuickSort = MathUtils.FilterData(times1.Skip(10)).Average();

            ratios = MathUtils.FilterData(times2.Skip(10)).Select(x => avgQuickSort / x).ToList();

            Console.WriteLine("Finished: " + contestant);

            return new Tuple<string, double, double, double>( contestant, ratios.Average(), ratios.StandardDeviation(), ratios.Min() );
        }


        public static IEnumerable<DateInterval> GetDateIntervalsEndTimesSorted(int count)
        {
            var date = DateTime.Now;

            return Enumerable.Range( 1, count ).Select( x => new DateInterval( date.AddMinutes( x ), date.AddMinutes( x + 100 ) ) );
        }

        public static IEnumerable<DateInterval> GetDateIntervalsRandomEndTimes(int count, int seed)
        {
            var date = DateTime.Now;

            var r = new Random(seed);

            return Enumerable.Range(1, count).Select(x => new DateInterval(date.AddMinutes(x), date.AddMinutes(x + r.Next(0, 100)))).OrderBy( x => x );
        }

        public static IEnumerable<DateInterval> GetDateIntervalsAllDescendingEndTimes(int count)
        {
            var date = DateTime.Now;
            var DateIntervals = new DateInterval[count];

            for (int i = 0; i < count; i++)
            {
                DateIntervals[count - i - 1] = new DateInterval(date.AddMinutes(-i), date.AddMinutes(i));
            }

            return new List<DateInterval>(DateIntervals).OrderBy( x => x );
        }

        public static IEnumerable<DateInterval> GetDateIntervalsRandomStartAndEndTimes(int count, int seed)
        {
            var date = DateTime.Now;
            var r = new Random(seed);
            var DateIntervals = new List<DateInterval>(count);

            for (int i = 0; i < count; i++)
            {
                DateTime startTime = date.AddSeconds(r.Next(-count, count));
                DateTime endTime = startTime.AddSeconds(r.Next(0, count));
                DateIntervals.Add(new DateInterval(startTime, endTime));
            }

            DateIntervals.Sort();
            return DateIntervals;
        }

        public static IEnumerable<DateInterval> GetDateIntervalsMultiGroupDescendingEndTimes(int count)
        {
            var n = Math.Sqrt( count );

            var date = DateTime.Now;

            var output = new List<DateInterval>();

            for (int i = 0; i < n; i++ )
            {
                for (int j = 0;j < n; j++ )
                {
                    output.Add(new DateInterval(date.AddMinutes(2*n * i).AddMinutes(j), date.AddMinutes(2*n * i).AddMinutes(2 * n - j)));
                }
            }

            // +-------------------+ +-------------------+
            //    +-------------+       +-------------+
            //        +-----+               +-----+
            //          +-+                   +-+

            return output.OrderBy( x => x);
        }

        public static IEnumerable<DateTime> GetSortedDateTimesQuickSort(List<DateInterval> orderedDateIntervals )
        {
            var dateTimes = new DateTime[2*orderedDateIntervals.Count];

            var DateIntervals = orderedDateIntervals.ToArray();

            for (int i = 0; i < DateIntervals.Length; i++)
            {
                dateTimes[i] = DateIntervals[i].StartTime;
                dateTimes[i+1] = DateIntervals[i].EndTime;
            }

            Array.Sort(dateTimes);

            return dateTimes;
        }
    }
}
