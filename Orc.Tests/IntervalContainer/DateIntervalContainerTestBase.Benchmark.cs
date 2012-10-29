namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Interface;

    public partial class DateIntervalContainerTestBase
    {
        [Test]
        [Category("Benchmark")]
        public void Query_BenchmarkIncludedOneInAnotherIntervals_Test()
        {
            const int numberOfIntervals = 10000;

            var intervals = GetDateRangesAllDescendingEndTimes(now, numberOfIntervals).ToList();

            stopwatch = Stopwatch.StartNew();

            var intervalContainer = this.CreateIntervalContainer(intervals);

            stopwatch.Stop();
            timeEllapsedReport.AppendLine(string.Format("Time taken to build data structure: {0} ms", this.stopwatch.ElapsedMilliseconds));

            var result1 = this.TestQueryForInterval(ToDateTimeInterval(now, 0, numberOfIntervals), intervalContainer, "Mid Point to Max Spanning Interval");

            var result2 = this.TestQueryForInterval(ToDateTimeInterval(now, -1, 1), intervalContainer, "Mid Point +/- 1");

            var result3 = this.TestQueryForInterval(ToDateTimeInterval(now, -numberOfIntervals, numberOfIntervals), intervalContainer, "Min to Max Spanning Interval");

            var result4 = this.TestQueryForInterval(ToDateTimeInterval(now, numberOfIntervals - 1, numberOfIntervals), intervalContainer, "Max Spanning interval -1 to Max Spanning Interval");

            Assert.AreEqual(numberOfIntervals, result1.Count());

            Assert.AreEqual(numberOfIntervals, result2.Count());

            Assert.AreEqual(numberOfIntervals, result3.Count());

            Assert.AreEqual(2, result4.Count());

            var timeElapsedSummary = timeEllapsedReport.ToString();
            Debug.WriteLine(timeElapsedSummary);
        }

        private IIntervalContainer<DateTime> CreateIntervalContainer(IEnumerable<Interval<DateTime>> intervals)
        {
            IIntervalContainer<DateTime> intervalContainer = CreateIntervalContainer();
            foreach (var interval in intervals)
            {
                intervalContainer.Add(interval);
            }
            return intervalContainer;
        }

        private IEnumerable<IInterval<DateTime>> TestQueryForInterval(Interval<DateTime> intervalToQuery, IIntervalContainer<DateTime> queryIn, string testName)
        {
            stopwatch.Reset();
            stopwatch.Start();

            var foundIntervals = queryIn.Query(intervalToQuery).ToList();

            stopwatch.Stop();
            timeEllapsedReport.AppendLine(string.Format("Time taken for {0}: {1} ms", testName, stopwatch.ElapsedMilliseconds));
            return foundIntervals;
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
