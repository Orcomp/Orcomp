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
        public void Query_BenchmarkIncludedOneIntoAnotherIntervals_Test()
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

        private static IEnumerable<Interval<DateTime>> GetDateRangesAllDescendingEndTimes(DateTime date, int count)
        {
            var dateRanges = new Interval<DateTime>[count];

            for (int i = 1; i <= count; i++)
            {
                dateRanges[i - 1] = new Interval<DateTime>(date.AddMinutes(-i), date.AddMinutes(i));
            }

            return new List<Interval<DateTime>>(dateRanges).OrderBy(x => x.Min.Value);
        }

        [Test]
        [Category("Benchmark")]
        public void Query_BenchmarkSequentialIntervals_Test()
        {
            const int numberOfIntervals = 1000000;
            const int intervalLength = 5;
            const int spaceLength = 1;
            const int intervalAndSpaceLength = intervalLength + spaceLength;

            var intervals = GetSequentialDateTimeIntervals(now, numberOfIntervals, intervalLength, spaceLength).ToList();

            stopwatch = Stopwatch.StartNew();

            var intervalContainer = this.CreateIntervalContainer(intervals);

            stopwatch.Stop();
            timeEllapsedReport.AppendLine(string.Format("Time taken to build data structure: {0} ms", this.stopwatch.ElapsedMilliseconds));

            var beforeBeginnigResult = this.TestQueryForInterval(
                ToDateTimeInterval(now, -2 * intervalLength, -intervalLength),
                intervalContainer,
                "Before The Beginning");

            var atTheBeginnigResult = this.TestQueryForInterval(
                ToDateTimeInterval(now, -1, 1), 
                intervalContainer, 
                "At The Beginning");

            const int middleIntervalIndex = numberOfIntervals / 2;
            var inTheMiddleResult = this.TestQueryForInterval(
                ToDateTimeInterval(now, (middleIntervalIndex) * intervalAndSpaceLength, ((middleIntervalIndex) + 1) * intervalAndSpaceLength - 1), 
                intervalContainer, 
                "In The Middle");

            var atTheEndResult = this.TestQueryForInterval(
                ToDateTimeInterval(now, (numberOfIntervals - 1) * intervalAndSpaceLength, numberOfIntervals * intervalAndSpaceLength), 
                intervalContainer, 
                "At The End");

            var afterTheEndResult = this.TestQueryForInterval(
                ToDateTimeInterval(now, (numberOfIntervals + 1) * intervalAndSpaceLength, (numberOfIntervals + 2) * intervalAndSpaceLength), 
                intervalContainer, 
                "After The End");

            Assert.AreEqual(0, beforeBeginnigResult.Count());

            Assert.AreEqual(1, atTheBeginnigResult.Count());

            Assert.AreEqual(1, inTheMiddleResult.Count());

            Assert.AreEqual(1, atTheEndResult.Count());

            Assert.AreEqual(0, afterTheEndResult.Count());

            var timeElapsedSummary = timeEllapsedReport.ToString();
            Debug.WriteLine(timeElapsedSummary);
        }

        private IEnumerable<Interval<DateTime>> GetSequentialDateTimeIntervals(DateTime startTime, int numberOfIntervals, int intervalLength, int spaceLength)
        {
            int intervalAndSpaceLength = intervalLength + spaceLength;

            var dateIntervals = new Interval<DateTime>[numberOfIntervals];

            for (int i = 0; i < numberOfIntervals; i++)
            {
                dateIntervals[i] = ToDateTimeInterval(startTime, i * intervalAndSpaceLength, (i + 1) * intervalAndSpaceLength - spaceLength);
            }

            return dateIntervals.OrderBy(i => i.Min).ToList();
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
    }
}
