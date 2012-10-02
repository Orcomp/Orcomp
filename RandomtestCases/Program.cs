using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomtestCases
{
    class Program
    {

        static void Main(string[] args)
        {
            var rand = new Random();
            var start = DateTime.Now;
            int found = 0;
            int CurrentMaxDateRanges = 100;
            string[] contestants = new string[] { "MoustafaS", "Zaher", "aus1", "Renze", "V_Tom_R", "SoftwareBender", "ErwinReid", "Mihai", "bawr", "c6c" };
            // string[] contestants = new string[] { "MoustafaS", /* "Zaher", */ "aus1", "Renze", "V_Tom_R", "SoftwareBender", /* "ErwinReid", */ /* "Mihai", */ /* "bawr", */ "c6c" };
            List<Orcomp.Entities.DateRange> SmallestFailingTestCases = null;
            List<DateTime> SmallestReferenceResult = null;
            IEnumerable<DateTime> SmallestSubmittedResult = null;
            List<string> errContestants = null;
            for (int tried = 0; tried != 10000000 && (SmallestFailingTestCases == null || SmallestFailingTestCases.Count() > 10); ++tried )
            {
                var orderedDateRanges = CreatetestCase(rand, start, CurrentMaxDateRanges);
                var Reference = Orcomp.Benchmarks.DateRangeSortBenchmark.GetSortedDateTimesQuickSort(orderedDateRanges);
                List<string> currentErr = new List<string>();
                foreach (var contestant in contestants)
                {

                    IEnumerable<DateTime> Submission = null;
                    try
                    {
                        Submission= Orcomp.Extensions.DateRangeCollectionExtensions.GetSortedDateTimes(orderedDateRanges, contestant);
                    }
                    catch (Exception)
                    { };
                    if (Submission == null || !Reference.SequenceEqual(Submission))
                    {
                        currentErr.Add(contestant);
                        ++found;
                        if (SmallestFailingTestCases == null || SmallestFailingTestCases.Count() > orderedDateRanges.Count())
                        {
                            SmallestFailingTestCases = orderedDateRanges;
                            SmallestReferenceResult = Reference;
                            SmallestSubmittedResult = Submission;
                            CurrentMaxDateRanges = SmallestFailingTestCases.Count();
                            errContestants = currentErr;
                        }
                    }
                }
                if (tried % 1000 == 0)
                {
                    Console.WriteLine("Tried " + tried + ", found " + found + (SmallestFailingTestCases == null ? "" : ", smallest failing test case length: " + SmallestFailingTestCases.Count()));
                }
            }
            Console.WriteLine("Found " + found + (SmallestFailingTestCases == null ? "" : ", smallest failing test case length: " + SmallestFailingTestCases.Count()));
            if ( SmallestFailingTestCases != null)
            {
                Console.WriteLine("Failing test case:");
                SmallestFailingTestCases.ForEach( x => Console.WriteLine(x.StartTime + ", " +  x.EndTime) );
                // SmallestReferenceResult and SmallestSubmittedResult contain the results of both implementations.
                errContestants.ForEach( x => Console.WriteLine(x) );
            }
            Console.ReadLine();
        }

        private static List<Orcomp.Entities.DateRange> CreatetestCase(Random rand, DateTime start, int MaxNumberOfDateRanges)
        {
            var orderedDateRanges = new List<Orcomp.Entities.DateRange>();

            for (int i = rand.Next(MaxNumberOfDateRanges)+10; i != 0; --i)
            {
                var DRStart = start.AddMinutes(rand.Next(1000));
                var DR = new Orcomp.Entities.DateRange(DRStart, DRStart.AddMinutes(rand.Next(1000)));
                orderedDateRanges.Add(DR);
            }
            orderedDateRanges.Sort();
            return orderedDateRanges;
        }
    }
}
