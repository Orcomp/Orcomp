using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomtestCases
{
    using Orc.Interval;

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
            List<DateInterval> SmallestFailingTestCases = null;
            List<DateTime> SmallestReferenceResult = null;
            IEnumerable<DateTime> SmallestSubmittedResult = null;
            List<string> errContestants = null;
            for (int tried = 0; tried != 10000000 && (SmallestFailingTestCases == null || SmallestFailingTestCases.Count() > 10); ++tried )
            {
                var orderedDateRanges = CreatetestCase(rand, start, CurrentMaxDateRanges);
                var Reference = Orc.Benchmarks.DateIntervalSortBenchmark.GetSortedDateTimesQuickSort(orderedDateRanges);

                List<string> currentErr = new List<string>();
                foreach (var contestant in contestants)
                {

                    IEnumerable<DateTime> Submission = null;
                    try
                    {
                        Submission= Orc.Submissions.GetSortedDateTimes.Run(orderedDateRanges, contestant);
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
                            SmallestReferenceResult = Reference.ToList();
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

        private static List<DateInterval> CreatetestCase(Random rand, DateTime start, int MaxNumberOfDateRanges)
        {
            var orderedDateRanges = new List<DateInterval>();

            for (int i = rand.Next(MaxNumberOfDateRanges)+10; i != 0; --i)
            {
                var DRStart = start.AddMinutes(rand.Next(1000));
                var DR = new DateInterval(DRStart, DRStart.AddMinutes(rand.Next(1000)));
                orderedDateRanges.Add(DR);
            }
            orderedDateRanges.Sort();
            return orderedDateRanges;
        }
    }
}
