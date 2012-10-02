using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Orcomp.Entities;
using Orcomp.Extensions;

namespace Orcomp.Benchmarks
{
    public delegate IEnumerable<DateTime> GetSortedDateTimesSoftwareBender(IEnumerable<Orcomp.Entities.DateRange> orderedDateRanges);
    class Program
    {
        static void Main(string[] args)
        {
            // ListWeaverBenchmark.Run();

            var contestants = new Dictionary<string, Func<List<DateRange>, IEnumerable<DateTime>>>();
            contestants.Add("MoustafaS",Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesMoustafaS);
            contestants.Add("Zaher", Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesZaher);
            contestants.Add("aus1", Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesAus1);
            contestants.Add("Renze", Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesRenze);
            contestants.Add("V_Tom_R", Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesV_Tom_R);
            contestants.Add("SoftwareBender", Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesSoftwareBender);
            contestants.Add("ErwinReid", Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesErwinReid);
            contestants.Add("Mihai", Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesMihai);
            contestants.Add("bawr", Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesBawr);
            contestants.Add("c6c",Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimesC6c);


            var numberOfDateRanges = 1000000;
            var benchmarkData = DateRangeSortBenchmark.GetBenchmarkData(numberOfDateRanges);
            Console.WriteLine("Finished creating benchmark data");

            var results = new List<Tuple<string, double, double, double>>();
            contestants.ForEach(x => results.Add(DateRangeSortBenchmark.Run(benchmarkData, x.Key,x.Value)));
            results = results.OrderByDescending( x => x.Item2 ).ToList();
            results.ForEach((x, i) => PrintResults(i + 1, x));

            results = new List<Tuple<string, double, double, double>>();
            contestants.Reverse();
            contestants.ForEach(x => results.Add(DateRangeSortBenchmark.Run(benchmarkData, x.Key, x.Value)));
            results = results.OrderByDescending(x => x.Item2).ToList();
            results.ForEach((x,i) => PrintResults(i+1, x));

            Console.ReadLine();
        }

        static void PrintResults(int number, Tuple<string, double, double, double> result)
        {
            Console.WriteLine("{0} - {1}, AvgRatio: {2}, SD: {3}, WorstCase: {4}", number, result.Item1, result.Item2.ToStringDecimal(3), result.Item3.ToStringDecimal(3), result.Item4.ToStringDecimal(3) );
        }

    }
}
