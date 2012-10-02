using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Orcomp.Extensions;

namespace Orcomp.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            // ListWeaverBenchmark.Run();

            var contestants = new List<string>();
            contestants.Add("MoustafaS");
            contestants.Add("Zaher");
            contestants.Add("aus1");
            contestants.Add("Renze");
            contestants.Add("V_Tom_R");
            contestants.Add("SoftwareBender");
            contestants.Add("ErwinReid");
            contestants.Add("Mihai");
            contestants.Add("bawr");
            contestants.Add("c6c");

            var numberOfDateRanges = 100000;
            var benchmarkData = DateRangeSortBenchmark.GetBenchmarkData(numberOfDateRanges);
            Console.WriteLine("Finished creating benchmark data");

            var results = new List<Tuple<string, double, double, double>>();
            contestants.ForEach(x => results.Add(DateRangeSortBenchmark.Run(benchmarkData, x)));
            results = results.OrderByDescending( x => x.Item2 ).ToList();
            results.ForEach((x, i) => PrintResults(i + 1, x));

            results = new List<Tuple<string, double, double, double>>();
            contestants.Reverse();
            contestants.ForEach(x => results.Add(DateRangeSortBenchmark.Run(benchmarkData, x)));
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
