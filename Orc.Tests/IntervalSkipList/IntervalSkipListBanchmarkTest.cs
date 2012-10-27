namespace Orc.Tests.IntervalSkipList
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Entities.IntervalSkipList;

    [Ignore("All interval container tests should be implemented via DateIntervalContainerTestBase class inheritance")]
    [TestFixture]
    [Category("Benchmark")]
    public class IntervalSkipListBanchmarkTest
    {
        private Stopwatch stopwatch;

        private StringBuilder timeEllapsedReport;

        [SetUp]
        public void Setup()
        {
            stopwatch = null;
            timeEllapsedReport = new StringBuilder();
        }

        [Test]        
        public void Search_Benchmark_Test()
        {
            var date = DateTime.Now;

            const int numberOfIntervals = 10000;

            var intervals = GetDateRangesAllDescendingEndTimes(date, numberOfIntervals).ToList();
            
            stopwatch = Stopwatch.StartNew();

            var intervalSkipList = new IntervalSkipList<DateTime>(intervals);

            stopwatch.Stop();
            timeEllapsedReport.AppendLine("Time taken to build tree: " + stopwatch.ElapsedMilliseconds);

            var result1 = TestSearchForInterval(date, date.AddMinutes(numberOfIntervals), intervalSkipList, "first");

            var result2 = TestSearchForInterval(date.AddMinutes(-1), date.AddMinutes(1), intervalSkipList, "second");

            var result3 = TestSearchForInterval(date.AddMinutes(-numberOfIntervals), date.AddMinutes(numberOfIntervals), intervalSkipList, "third");

            var result4 = TestSearchForInterval(date.AddMinutes(numberOfIntervals - 1), date.AddMinutes(numberOfIntervals), intervalSkipList, "fourth");            

            Assert.AreEqual(numberOfIntervals, result1.Count);

            Assert.AreEqual(numberOfIntervals, result2.Count);

            Assert.AreEqual(numberOfIntervals, result3.Count);

            Assert.AreEqual(2, result4.Count);
            
            var timeElapsedSummary = timeEllapsedReport.ToString();
            Debug.WriteLine(timeElapsedSummary);
        }

        private LinkedList<Interval<DateTime>> TestSearchForInterval(DateTime startEdge, DateTime endEdge, IntervalSkipList<DateTime> searchIn, string testName)
        {
            stopwatch.Reset();
            stopwatch.Start();

            var result = searchIn.Search(new Interval<DateTime>(startEdge, endEdge));

            stopwatch.Stop();
            timeEllapsedReport.AppendLine(string.Format("Time taken for {0}: {1}", testName, stopwatch.ElapsedMilliseconds));
            return result;
        }

        private static IEnumerable<Interval<DateTime>> GetDateRangesAllDescendingEndTimes(DateTime date, int count)
        {
            var dateRanges = new Interval<DateTime>[count];

            for (int i = 1; i <= count; i++)
            {
                dateRanges[i - 1] = new Interval<DateTime>(date.AddMinutes(-i), date.AddMinutes(i));
            }

            return new List<Interval<DateTime>>(dateRanges).OrderBy(x => x.Min.Value);
        }
    }
}
