namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using NUnit.Framework;

    using Orc.Interval;
    using Orc.Interval.Interface;

    public partial class DateIntervalContainerTestBase
    {
        //     |-|
        //    |---|
        //   |-----|
        //  |-------|
        [Test]
        [Category("Benchmark")]
        public void Query_BenchmarkContainmentIntervals_Test()
        {
            const int numberOfIntervals = 2000;

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

        // |---| |---| |---| |---|
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

            this.RunQueryBenchmarkTestsForSquentialIntervals(intervalContainer, intervalLength, intervalAndSpaceLength, numberOfIntervals);

            var timeElapsedSummary = timeEllapsedReport.ToString();
            Debug.WriteLine(timeElapsedSummary);
        }

        /// |-------------------|
        ///     |-------------------|
        ///          |-------------------|
        ///                |-------------------|
        [Test]
        [Category("Benchmark")]
        public void Query_BenchmarkOverlappingIntervals_Test()
        {
            const int numberOfIntervals = 1000000;
            const int intervalLength = 10;
            const int startToStartLength = 1;

            Debug.Assert(numberOfIntervals % 2 == 0, "The number of intervals must be even.");
            Debug.Assert(intervalLength % startToStartLength == 0, "The interval length number must be wholely divisible by startToStartLength");
            Debug.Assert(numberOfIntervals / intervalLength > 2, "The number of intervals must be at least twice the the interval length value.");

            var intervals = DateIntervalCollectionGenerator.OverlapsWithConstantDuration(
                now, TimeSpan.FromMinutes(intervalLength), TimeSpan.FromMinutes(startToStartLength), numberOfIntervals);

            var intervalContainer = this.RunIntervalContainerBuild(intervals);

            this.RunQueryBenchmarkTestsForOverlappingIntervals(intervalContainer, intervalLength, startToStartLength, numberOfIntervals);

            var timeElapsedSummary = timeEllapsedReport.ToString();
            Debug.WriteLine(timeElapsedSummary);
        }

        [Test]
        [Category("Benchmark")]
        [Timeout(60 * 1000)]
        public void Add_Remove_BenchmarkSequentialIntervals_Test()
        {
            const int numberOfIntervals = 1000;
            const int intervalLength = 5;
            const int spaceLength = 1;
            const int intervalAndSpaceLength = intervalLength + spaceLength;            

            var intervals = GetSequentialDateTimeIntervals(now, numberOfIntervals, intervalLength, spaceLength).ToList();

            var intervalContainer = RunIntervalContainerBuild(intervals);

            this.RunQueryBenchmarkTestsForSquentialIntervals(intervalContainer, intervalLength, intervalAndSpaceLength, numberOfIntervals);

            RemoveHalfOfIntervals(intervalContainer, intervals);

            const int halfNumberOfIntervals = numberOfIntervals / 2;
            this.RunQueryBenchmarkTestsForSquentialIntervals(intervalContainer, intervalLength, intervalAndSpaceLength, halfNumberOfIntervals);

            AddRemovedIntervals(intervalContainer, intervals);

            this.RunQueryBenchmarkTestsForSquentialIntervals(intervalContainer, intervalLength, intervalAndSpaceLength, numberOfIntervals);

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
            timeEllapsedReport.AppendLine(string.Format("Memory used for removing {0} intervals: {1:0.00} MB", halfNumberOfIntervals, BytesToMegabytes(bytesUsed)));
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
            timeEllapsedReport.AppendLine(string.Format("Memory used for adding {0} intervals: {1:0.00} MB", halfNumberOfIntervals, BytesToMegabytes(bytesUsed)));
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
            timeEllapsedReport.AppendLine(string.Format("Memory used to build data structure: {0:0.00} MB", BytesToMegabytes(bytesUsed)));
            timeEllapsedReport.AppendLine();

            return intervalContainer;
        }

        private void RunQueryBenchmarkTestsForSquentialIntervals(IIntervalContainer<DateTime> intervalContainer, int intervalLength, int intervalAndSpaceLength, int numberOfIntervals)
        {
            var beforeBeginnigResult = TestQueryForInterval(
                ToDateTimeInterval(now, -2 * intervalLength, -intervalLength), 
                intervalContainer, 
                "Before The Beginning");

            var atTheBeginnigResult = TestQueryForInterval(
                ToDateTimeInterval(now, -1, 0), 
                intervalContainer, 
                "At The Beginning");

            int middleIntervalIndex = numberOfIntervals / 2;
            var fromTheBeginnigToMiddleResult = TestQueryForInterval(
                ToDateTimeInterval(now, 0, (middleIntervalIndex) * intervalAndSpaceLength - 1),
                intervalContainer,
                "From The Beginning To The Middle");

            var inTheMiddleResult = TestQueryForInterval(
                ToDateTimeInterval(now, (middleIntervalIndex) * intervalAndSpaceLength, (middleIntervalIndex + 1) * intervalAndSpaceLength - 1),
                intervalContainer,
                "In The Middle");

            var fromTheMiddleToEndResult = TestQueryForInterval(
               ToDateTimeInterval(now, (middleIntervalIndex + 1) * intervalAndSpaceLength - 1, (numberOfIntervals + 1) * intervalAndSpaceLength),
               intervalContainer,
               "From The Middle To The End");

            var atTheEndResult = TestQueryForInterval(
                ToDateTimeInterval(now, (numberOfIntervals - 1) * intervalAndSpaceLength, numberOfIntervals * intervalAndSpaceLength),
                intervalContainer,
                "At The End");

            var fromTheBeginningToEndResult = TestQueryForInterval(
               ToDateTimeInterval(now, 0, (numberOfIntervals + 1) * intervalAndSpaceLength),
               intervalContainer,
               "From The Beginning To The End");

            var afterTheEndResult = TestQueryForInterval(
                ToDateTimeInterval(now, (numberOfIntervals + 1) * intervalAndSpaceLength, (numberOfIntervals + 2) * intervalAndSpaceLength),
                intervalContainer,
                "After The End");

            Assert.AreEqual(0, beforeBeginnigResult.Count());

            Assert.AreEqual(1, atTheBeginnigResult.Count());

            Assert.AreEqual(middleIntervalIndex, fromTheBeginnigToMiddleResult.Count());

            Assert.AreEqual(1, inTheMiddleResult.Count());

            Assert.AreEqual(middleIntervalIndex, fromTheMiddleToEndResult.Count());

            Assert.AreEqual(1, atTheEndResult.Count());

            Assert.AreEqual(numberOfIntervals, fromTheBeginningToEndResult.Count());

            Assert.AreEqual(0, afterTheEndResult.Count());
        }


        private void RunQueryBenchmarkTestsForOverlappingIntervals(IIntervalContainer<DateTime> intervalContainer, int intervalLength, int startToStartLength, int numberOfIntervals)
        {
            var beforeBeginnigResult = TestQueryForInterval(
                ToDateTimeInterval(now, -2 * intervalLength, -intervalLength),
                intervalContainer,
                "Before The Beginning");

            var atTheBeginnigResult = TestQueryForInterval(
                ToDateTimeInterval(now, -1, 0),
                intervalContainer,
                "At The Beginning");

            int middleIntervalIndex = numberOfIntervals / 2;
            var fromTheBeginnigToMiddleResult = TestQueryForInterval(
                ToDateTimeInterval(now, 0, startToStartLength * (middleIntervalIndex - 1)),
                intervalContainer,
                "From The Beginning To The Middle");

            var inTheMiddleResult = TestQueryForInterval(
                ToDateTimeInterval(now, startToStartLength * middleIntervalIndex, startToStartLength * middleIntervalIndex + intervalLength),
                intervalContainer,
                "In The Middle");

            // From the end of the middle interval to the end of the last interval
            var fromTheMiddleToEndResult = TestQueryForInterval(
               ToDateTimeInterval(now, startToStartLength * middleIntervalIndex + intervalLength, startToStartLength * numberOfIntervals + intervalLength),
               intervalContainer,
               "From The Middle To The End");

            var atTheEndResult = TestQueryForInterval(
                ToDateTimeInterval(now, startToStartLength * (numberOfIntervals - 1) + intervalLength - 1, startToStartLength * (numberOfIntervals - 1) + intervalLength),
                intervalContainer,
                "At The End");

            var fromTheBeginningToEndResult = TestQueryForInterval(
               ToDateTimeInterval(now, 0, startToStartLength * (numberOfIntervals - 1) + intervalLength),
               intervalContainer,
               "From The Beginning To The End");

            var afterTheEndResult = TestQueryForInterval(
                ToDateTimeInterval(now, startToStartLength * (numberOfIntervals - 1) + intervalLength + 1, startToStartLength * numberOfIntervals + intervalLength + 2),
                intervalContainer,
                "After The End");

            Assert.AreEqual(0, beforeBeginnigResult.Count());

            Assert.AreEqual(1, atTheBeginnigResult.Count());

            Assert.AreEqual(middleIntervalIndex, fromTheBeginnigToMiddleResult.Count());

            Assert.AreEqual(2 * (intervalLength / startToStartLength), inTheMiddleResult.Count());

            Assert.AreEqual(middleIntervalIndex - 1, fromTheMiddleToEndResult.Count());

            Assert.AreEqual(1, atTheEndResult.Count());

            Assert.AreEqual(numberOfIntervals, fromTheBeginningToEndResult.Count());

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
