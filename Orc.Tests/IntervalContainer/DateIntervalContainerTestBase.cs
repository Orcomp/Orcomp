namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Interface;

    public abstract class DateIntervalContainerTestBase
    {
        private Stopwatch stopwatch;

        private StringBuilder timeEllapsedReport;

        private DateTime now;

        protected DateTime inOneHour;

        protected DateTime inTwoHours;

        protected DateTime inThreeHours;

        protected abstract IIntervalContainer<DateTime> CreateIntervalContainer();

        [SetUp]
        public void Setup()
        {
            stopwatch = null;
			timeEllapsedReport = new StringBuilder();

            now = DateTime.Now;
            inOneHour = now.AddHours(1);
            inTwoHours = now.AddHours(2);
            inThreeHours = now.AddHours(3);

            Debug.WriteLine(string.Format("Now: {0}", now));
        }

        [TestCase(4, 4, 3)]
        [TestCase(4, 5, 4)]
        [TestCase(-1, 10, 7)]
        [TestCase(-1, -1, 0)]
        [TestCase(1, 4, 5)]
        [TestCase(0, 1, 2)]
        [TestCase(10, 12, 0)]
        public void Query_ForDifferentDateIntervals_ShoudReturnCorrectOverlapsCount(int leftEdgeMinutes, int rightEdgeMinutes, int expectedOverlapsCount)
        {
            //Arrange
            var intervals = new List<Interval<DateTime>>();

            intervals.Add(ToDateTimeInterval(now, -300, -200));
            intervals.Add(ToDateTimeInterval(now, -3, -2));
            intervals.Add(ToDateTimeInterval(now, 1, 2));
            intervals.Add(ToDateTimeInterval(now, 3, 6));
            intervals.Add(ToDateTimeInterval(now, 2, 4));
            intervals.Add(ToDateTimeInterval(now, 5, 7));
            intervals.Add(ToDateTimeInterval(now, 1, 3));
            intervals.Add(ToDateTimeInterval(now, 4, 6));
            intervals.Add(ToDateTimeInterval(now, 8, 9));
            intervals.Add(ToDateTimeInterval(now, 15, 20));
            intervals.Add(ToDateTimeInterval(now, 40, 50));
            intervals.Add(ToDateTimeInterval(now, 49, 60));

            IIntervalContainer<DateTime> intervalContainer = this.CreateIntervalContainer(intervals);

            //Act
            var overlaps = intervalContainer.Query(ToDateTimeInterval(now, leftEdgeMinutes, rightEdgeMinutes));

            //Assert
            Assert.AreEqual(expectedOverlapsCount, overlaps.Count());
        }

        [Test]
        public void Query_ForNullInterval_ShouldReturnEmptyList()
        {
            //Arrange
            var intervals = new List<Interval<DateTime>>();
            intervals.Add(new Interval<DateTime>(now, inOneHour));

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(null);

            //Assert
            Assert.AreEqual(0, intersections.Count());
        }

        [Test]
        public void Query_IntervalBetweenTwoInclusiveIntervals_ShouldReturnBothIntervals()
        {
            //Arrange
            var intervals = new List<Interval<DateTime>>();
            intervals.Add(new Interval<DateTime>(now, inOneHour, isMaxInclusive: true));
            intervals.Add(new Interval<DateTime>(inTwoHours, inThreeHours, isMinInclusive: true));

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(new Interval<DateTime>(inOneHour, inTwoHours));

            //Assert
            CollectionAssert.AreEquivalent(intervals, intersections.ToList());
        }

        [Test]
        public void Query_IntervalBetweenTwoNotInclusiveIntervals_ShouldReturnEmptyList()
        {
            //Arrange
            var intervals = new List<Interval<DateTime>>();
            intervals.Add(new DateInterval(now, inOneHour, isMaxInclusive: false));
            intervals.Add(new DateInterval(inTwoHours, inThreeHours, isMinInclusive: false));

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(new Interval<DateTime>(inOneHour, inTwoHours));

            //Assert
            Assert.AreEqual(0, intersections.Count());
        }

        #region Benchmark

        [Test]
        [Category("Benchmark")]
        public void Query_Benchmark_Test()
        {
            const int numberOfIntervals = 10000;

            var intervals = GetDateRangesAllDescendingEndTimes(now, numberOfIntervals).ToList();
            
            stopwatch = Stopwatch.StartNew();

			var intervalContainer = this.CreateIntervalContainer(intervals);

            stopwatch.Stop();
            timeEllapsedReport.AppendLine("Time taken to build data structure: " + stopwatch.ElapsedMilliseconds);

            var result1 = TestSearchForInterval(now, now.AddMinutes(numberOfIntervals), intervalContainer, "first");

            var result2 = TestSearchForInterval(now.AddMinutes(-1), now.AddMinutes(1), intervalContainer, "second");

            var result3 = TestSearchForInterval(now.AddMinutes(-numberOfIntervals), now.AddMinutes(numberOfIntervals), intervalContainer, "third");

            var result4 = TestSearchForInterval(now.AddMinutes(numberOfIntervals - 1), now.AddMinutes(numberOfIntervals), intervalContainer, "fourth");            

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

        private IEnumerable<IInterval<DateTime>> TestSearchForInterval(DateTime startEdge, DateTime endEdge, IIntervalContainer<DateTime> searchIn, string testName)
        {
            stopwatch.Reset();
            stopwatch.Start();

            var foundIntervals = searchIn.Query(new Interval<DateTime>(startEdge, endEdge));

            stopwatch.Stop();
            timeEllapsedReport.AppendLine(string.Format("Time taken for {0}: {1}", testName, stopwatch.ElapsedMilliseconds));
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
		#endregion

        private static Interval<DateTime> ToDateTimeInterval(DateTime startTime, int leftEdgeMinutes, int rightEdgeMinutes)
        {
            return new Interval<DateTime>(startTime.AddMinutes(leftEdgeMinutes), startTime.AddMinutes(rightEdgeMinutes));
        }
	}
}
