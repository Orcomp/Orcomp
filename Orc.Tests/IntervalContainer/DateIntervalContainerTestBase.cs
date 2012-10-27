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

        [TestCase(0, 2, 1)]
        [TestCase(0, 7, 2)]
        [TestCase(0, 23, 3)]
        [TestCase(0, 28, 4)]
        [TestCase(0, 44, 5)]
        [TestCase(0, 49, 6)]
        [TestCase(54, 56, 1)]
        [TestCase(49, 56, 2)]
        [TestCase(33, 56, 3)]
        [TestCase(28, 56, 4)]
        [TestCase(12, 56, 5)]
        [TestCase(7, 56, 6)]
        public void Query_ForDifferentSideDateIntervals_ShoudReturnCorrectOverlapsCount(int leftEdgeMinutes, int rightEdgeMinutes, int expectedOverlapsCount)
        {
            //Arrange
            #region Specification
            // *************************************************************
            // | X axis:                                                   |
            // | 0    5    10   15   20   25   30   35   40   45   50    56|
            // | |    |    |    |    |    |    |    |    |    |    |     | |
            // | Tree intervals:                                           |
            // | [-------------]      [-------------]      [-------------] |
            // |     [-----]              [-----]              [-----]     |
            // | Test intervals:                                           |
            // | [-]                                                       |
            // | [------]                                                  |
            // | [----------------------]                                  |
            // | [---------------------------]                             |
            // | [-------------------------------------------]             |
            // | [------------------------------------------------]        |
            // |                                                       [-] |
            // |                                                  [------] |
            // |                                  [----------------------] |
            // |                             [---------------------------] |
            // |             [-------------------------------------------] |
            // |        [------------------------------------------------] |
            // | X axis:                                                   |
            // | |    |    |    |    |    |    |    |    |    |    |     | |
            // | 0    5    10   15   20   25   30   35   40   45   50    56|
            // *************************************************************
            #endregion

            var intervals = new List<Interval<DateTime>>();

            intervals.Add(ToDateTimeInterval(now, 0, 14));
            intervals.Add(ToDateTimeInterval(now, 4, 10));
            intervals.Add(ToDateTimeInterval(now, 21, 35));
            intervals.Add(ToDateTimeInterval(now, 25, 31));
            intervals.Add(ToDateTimeInterval(now, 42, 56));
            intervals.Add(ToDateTimeInterval(now, 46, 52));

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
        public void Query_InclusiveIntervalBetweenTwoInclusiveIntervals_ShouldReturnBothIntervals()
        {
            //Arrange
            // Tree: [----]    [----]
            // Test:      [----]     

            var intervals = new List<Interval<DateTime>>();
            intervals.Add(new Interval<DateTime>(now, inOneHour));
            intervals.Add(new Interval<DateTime>(inTwoHours, inThreeHours));

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(new Interval<DateTime>(inOneHour, inTwoHours));

            //Assert
            CollectionAssert.AreEquivalent(intervals, intersections.ToList());
        }

        [Test]
        public void Query_NotInclusiveIntervalBetweenTwoInclusiveIntervals_ShouldReturnEmptyList()
        {
            //Arrange            
            // Tree: [----]    [----]
            // Test:      ]----[     

            var intervals = new List<Interval<DateTime>>();
            intervals.Add(new Interval<DateTime>(now, inOneHour));
            intervals.Add(new Interval<DateTime>(inTwoHours, inThreeHours));

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(new Interval<DateTime>(inOneHour, inTwoHours, false, false));

            //Assert
            Assert.AreEqual(0, intersections.Count());
        }

        [Test]
        public void Query_InclusiveIntervalBetweenTwoNotInclusiveIntervals_ShouldReturnEmptyList()
        {
            //Arrange
            // Tree: [----[    ]----]
            // Test:      [----]     

            var intervals = new List<Interval<DateTime>>();
            intervals.Add(new Interval<DateTime>(now, inOneHour, isMaxInclusive: false));
            intervals.Add(new Interval<DateTime>(inTwoHours, inThreeHours, isMinInclusive: false));

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(new Interval<DateTime>(inOneHour, inTwoHours));

            //Assert
            Assert.AreEqual(0, intersections.Count());
        }

        [Test]
        public void Query_NotInclusiveIntervalBetweenTwoNotInclusiveIntervals_ShouldReturnEmptyList()
        {
            //Arrange
            // Tree: [----[    ]----]
            // Test:      ]----[     

            var intervals = new List<Interval<DateTime>>();
            intervals.Add(new Interval<DateTime>(now, inOneHour, isMaxInclusive: false));
            intervals.Add(new Interval<DateTime>(inTwoHours, inThreeHours, isMinInclusive: false));

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
            timeEllapsedReport.AppendLine(string.Format("Time taken to build data structure: {0} ms", this.stopwatch.ElapsedMilliseconds));

            var result1 = TestSearchForInterval(now, now.AddMinutes(numberOfIntervals), intervalContainer, "Mid Point to Max Spanning Interval");

            var result2 = TestSearchForInterval(now.AddMinutes(-1), now.AddMinutes(1), intervalContainer, "Mid Point +/- 1");

            var result3 = TestSearchForInterval(now.AddMinutes(-numberOfIntervals), now.AddMinutes(numberOfIntervals), intervalContainer, "Min to Max Spanning Interval");

            var result4 = TestSearchForInterval(now.AddMinutes(numberOfIntervals - 1), now.AddMinutes(numberOfIntervals), intervalContainer, "Max Spanning interval -1 to Max Spanning Interval");            

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
		#endregion

        private static Interval<DateTime> ToDateTimeInterval(DateTime startTime, int leftEdgeMinutes, int rightEdgeMinutes)
        {
            return new Interval<DateTime>(startTime.AddMinutes(leftEdgeMinutes), startTime.AddMinutes(rightEdgeMinutes));
        }
	}
}
