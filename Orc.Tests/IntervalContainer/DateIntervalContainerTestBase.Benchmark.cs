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

            var intervalContainer = RunIntervalContainerBuild(intervals);

            var result1 = TestQueryForInterval(ToDateTimeInterval(now, 0, numberOfIntervals), intervalContainer, "Mid Point to Max Spanning Interval");

            var result2 = TestQueryForInterval(ToDateTimeInterval(now, -1, 1), intervalContainer, "Mid Point +/- 1");

            var result3 = TestQueryForInterval(ToDateTimeInterval(now, -numberOfIntervals, numberOfIntervals), intervalContainer, "Min to Max Spanning Interval");

            var result4 = TestQueryForInterval(ToDateTimeInterval(now, numberOfIntervals - 1, numberOfIntervals), intervalContainer, "Max Spanning interval -1 to Max Spanning Interval");

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

            var intervalContainer = RunIntervalContainerBuild(intervals);

            RunQueryBenchmarkTestsForSquentialInterval(intervalContainer, intervalLength, intervalAndSpaceLength, numberOfIntervals);

            var timeElapsedSummary = timeEllapsedReport.ToString();
            Debug.WriteLine(timeElapsedSummary);
        }

        [Test]
        [Category("Benchmark")]
        [Timeout(60 * 1000)]
        public void Add_Remove_BenchmarkSequentialIntervals_Test()
        {
            const int numberOfIntervals = 1000000;
            const int intervalLength = 5;
            const int spaceLength = 1;
            const int intervalAndSpaceLength = intervalLength + spaceLength;            

            var intervals = GetSequentialDateTimeIntervals(now, numberOfIntervals, intervalLength, spaceLength).ToList();

            var intervalContainer = RunIntervalContainerBuild(intervals);

            RunQueryBenchmarkTestsForSquentialInterval(intervalContainer, intervalLength, intervalAndSpaceLength, numberOfIntervals);

            RemoveHalfOfIntervals(intervalContainer, intervals);

            const int halfNumberOfIntervals = numberOfIntervals / 2;
            RunQueryBenchmarkTestsForSquentialInterval(intervalContainer, intervalLength, intervalAndSpaceLength, halfNumberOfIntervals);

            AddRemovedIntervals(intervalContainer, intervals);

            RunQueryBenchmarkTestsForSquentialInterval(intervalContainer, intervalLength, intervalAndSpaceLength, numberOfIntervals);

            var timeElapsedSummary = timeEllapsedReport.ToString();
            Debug.WriteLine(timeElapsedSummary);
        }

        private void RemoveHalfOfIntervals(IIntervalContainer<DateTime> intervalContainer, List<Interval<DateTime>> intervals)
        {
            stopwatch.Reset();
            long initalMemory = GC.GetTotalMemory(true);
            stopwatch.Start();

            //remove second part of intervals
            int halfNumberOfIntervals = intervals.Count / 2;
            for (int i = 0; i < halfNumberOfIntervals; i++)
            {
                intervalContainer.Remove(intervals[halfNumberOfIntervals + i]);
            }

            stopwatch.Stop();
            long endRemoveIntervalsMemory = GC.GetTotalMemory(true);
            long bytesUsed = endRemoveIntervalsMemory - initalMemory;

            timeEllapsedReport.AppendLine();
            timeEllapsedReport.AppendLine(string.Format("Time taken for removing {0} intervals: {1} ms", halfNumberOfIntervals, stopwatch.ElapsedMilliseconds));
            timeEllapsedReport.AppendLine(string.Format("Memory used for removing {0} intervals: {1:00} MB", halfNumberOfIntervals, BytesToMegabytes(bytesUsed)));
            timeEllapsedReport.AppendLine();
        }

        private void AddRemovedIntervals(IIntervalContainer<DateTime> intervalContainer, List<Interval<DateTime>> intervals)
        {
            stopwatch.Reset();
            long initalMemory = GC.GetTotalMemory(true);
            stopwatch.Start();

            //add second part of intervals
            int halfNumberOfIntervals = intervals.Count / 2;
            for (int i = 0; i < halfNumberOfIntervals; i++)
            {
                intervalContainer.Add(intervals[halfNumberOfIntervals + i]);
            }

            stopwatch.Stop();
            long endAddIntervalsMemory = GC.GetTotalMemory(true);
            long bytesUsed = endAddIntervalsMemory - initalMemory;

            timeEllapsedReport.AppendLine();
            timeEllapsedReport.AppendLine(string.Format("Time taken for adding {0} intervals: {1} ms", halfNumberOfIntervals, stopwatch.ElapsedMilliseconds));
            timeEllapsedReport.AppendLine(string.Format("Memory used for adding {0} intervals: {1:00} MB", halfNumberOfIntervals, BytesToMegabytes(bytesUsed)));
            timeEllapsedReport.AppendLine();
        }        

        private IIntervalContainer<DateTime> RunIntervalContainerBuild(IEnumerable<Interval<DateTime>> intervals)
        {
            long initalMemory = GC.GetTotalMemory(true);
            stopwatch = Stopwatch.StartNew();            

            var intervalContainer = CreateIntervalContainer(intervals);
                        
            stopwatch.Stop();
            long endCreateMemory = GC.GetTotalMemory(true);
            long bytesUsed = endCreateMemory - initalMemory;

            timeEllapsedReport.AppendLine();
            timeEllapsedReport.AppendLine(string.Format("Time taken to build data structure: {0} ms", stopwatch.ElapsedMilliseconds));
            timeEllapsedReport.AppendLine(string.Format("Memory used to build data structure: {0:00} MB", BytesToMegabytes(bytesUsed)));
            timeEllapsedReport.AppendLine();

            return intervalContainer;
        }

        private void RunQueryBenchmarkTestsForSquentialInterval(IIntervalContainer<DateTime> intervalContainer, int intervalLength, int intervalAndSpaceLength, int numberOfIntervals)
        {
            var beforeBeginnigResult = TestQueryForInterval(
                ToDateTimeInterval(now, -2 * intervalLength, -intervalLength), 
                intervalContainer, 
                "Before The Beginning");

            var atTheBeginnigResult = TestQueryForInterval(
                ToDateTimeInterval(now, -1, 1), 
                intervalContainer, 
                "At The Beginning");

            int middleIntervalIndex = numberOfIntervals / 2;
            var inTheMiddleResult = TestQueryForInterval(
                ToDateTimeInterval(now, (middleIntervalIndex) * intervalAndSpaceLength, ((middleIntervalIndex) + 1) * intervalAndSpaceLength - 1),
                intervalContainer,
                "In The Middle");

            var atTheEndResult = TestQueryForInterval(
                ToDateTimeInterval(now, (numberOfIntervals - 1) * intervalAndSpaceLength, numberOfIntervals * intervalAndSpaceLength),
                intervalContainer,
                "At The End");

            var afterTheEndResult = TestQueryForInterval(
                ToDateTimeInterval(now, (numberOfIntervals + 1) * intervalAndSpaceLength, (numberOfIntervals + 2) * intervalAndSpaceLength),
                intervalContainer,
                "After The End");

            Assert.AreEqual(0, beforeBeginnigResult.Count());

            Assert.AreEqual(1, atTheBeginnigResult.Count());

            Assert.AreEqual(1, inTheMiddleResult.Count());

            Assert.AreEqual(1, atTheEndResult.Count());

            Assert.AreEqual(0, afterTheEndResult.Count());
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

        private static double BytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}
