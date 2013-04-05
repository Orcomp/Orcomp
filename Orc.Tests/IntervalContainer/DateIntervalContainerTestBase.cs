namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Orc.Interval;
    using Orc.Interval.Interface;

    public abstract partial class DateIntervalContainerTestBase
    {
        private Stopwatch stopwatch;

        private StringBuilder timeEllapsedReport;

        private DateTime now;

        protected DateTime inOneHour;

        protected DateTime inTwoHours;

        protected DateTime inThreeHours;

        protected abstract IIntervalContainer<DateTime> CreateIntervalContainer(IEnumerable<IInterval<DateTime>> intervals = null);

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

        #region Query

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
            // Container: [----]    [----]
            //      Test:      [----]     

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
            // Container: [----]    [----]
            //      Test:      ]----[     

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
            // Container: [----[    ]----]
            //      Test:      [----]     

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
            // Container: [----[    ]----]
            //      Test:      ]----[     

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
        public void Query_InclusiveIntervalInListWhichContainsOneTheSameInterval_ShouldReturnThisInterval()
        {
            //Arrange
            var interval = new Interval<DateTime>(now, inOneHour);
            var intervals = new List<Interval<DateTime>> { interval };

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(interval).ToList();

            //Assert
            Assert.AreEqual(1, intersections.Count);
            Assert.AreEqual(interval, intersections[0]);
        }

        [Test]
        public void Query_InclusiveIntervalInListWhichContainsOneTheSameButExclusiveInterval_ShouldReturnThisInterval()
        {
            //Arrange
            var interval = new Interval<DateTime>(now, inOneHour, false, false);
            var intervals = new List<Interval<DateTime>> { interval };

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(new Interval<DateTime>(now, inOneHour, true, true)).ToList();

            //Assert
            Assert.AreEqual(1, intersections.Count);
            Assert.AreEqual(interval, intersections[0]);
        }

        [Test]
        public void Query_IntervalWhichIsCompletellyInListOfOneLargeInterval_ShouldReturnLargeInterval()
        {
            //Arrange
            // Container: [--------------]
            //      Test:      [----]     

            //Arrange
            var interval = new Interval<DateTime>(now, inThreeHours);
            var intervals = new List<Interval<DateTime>> { interval };

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(new Interval<DateTime>(inOneHour, inTwoHours)).ToList();

            //Assert
            Assert.AreEqual(1, intersections.Count);
            Assert.AreEqual(interval, intersections[0]);
        }

        [Test]
        public void Query_IntervalWhichCompletellyCoversListOfOneSmallInterval_ShouldReturnSmallInterval()
        {
            //Arrange
            // Container:      [----]
            //      Test: [--------------] 

            //Arrange
            var interval = new Interval<DateTime>(inOneHour, inTwoHours);
            var intervals = new List<Interval<DateTime>> { interval };

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(new Interval<DateTime>(now, inThreeHours)).ToList();

            //Assert
            Assert.AreEqual(1, intersections.Count);
            Assert.AreEqual(interval, intersections[0]);
        }

        [Test]
        public void Query_EmptyIntervalContainer_ShouldReturnEmptyResults()
        {
            //Arrange
            var emptyIntervalContainer = CreateIntervalContainer(new Interval<DateTime>[0]);

            //Act
            var intersections = emptyIntervalContainer.Query(new Interval<DateTime>(now, inOneHour));

            //Assert
            Assert.NotNull(intersections);
            Assert.AreEqual(0, intersections.Count());
        }

        #region General Count Intervals Tests
        [Test]
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
        [TestCase(0, 2, 1)]
        [TestCase(0, 7, 2)]
        [TestCase(0, 23, 3)]
        [TestCase(0, 28, 4)]
        [TestCase(0, 44, 5)]
        [TestCase(0, 49, 6)]
        [TestCase(0, 56, 6)]
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
            // | Container intervals:                                      |
            // | [-------------]      [-------------]      [-------------] |
            // |     [-----]              [-----]              [-----]     |
            // | Test intervals:                                           |
            // | [-]                                                       |
            // | [------]                                                  |
            // | [----------------------]                                  |
            // | [---------------------------]                             |
            // | [-------------------------------------------]             |
            // | [------------------------------------------------]        |
            // | [-------------------------------------------------------] |
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
        #endregion

        #endregion

        #region Add/Remove
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullInterval_ShouldTrowArgumentNullException()
        {
            //Arrange
            var intervalContainer = CreateIntervalContainer();

            //Act
            intervalContainer.Add(null);
        }

        [Test]        
        public void Add_Interval_ShouldBeAvailable()
        {
            //Arrange
            var interval = new Interval<DateTime>(now, inTwoHours);
            var intervalContainer = CreateIntervalContainer();

            //Act
            var preAddIntersections = intervalContainer.Query(interval);
            intervalContainer.Add(interval);
            var afterAddIntersections = intervalContainer.Query(interval);

            //Assert
            Assert.AreEqual(0, preAddIntersections.Count());
            Assert.AreEqual(1, afterAddIntersections.Count());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Remove_NullInterval_ShouldTrowArgumentNullException()
        {
            //Arrange
            var intervalContainer = CreateIntervalContainer();

            //Act
            intervalContainer.Remove(null);
        }

        [Test]
        public void Remove_Interval_ShouldNotBeAvailable()
        {
            //Arrange
            var interval = new Interval<DateTime>(now, inTwoHours);
            var intervalContainer = CreateIntervalContainer();
            intervalContainer.Add(interval);

            //Act
            var preRemoveIntersections = intervalContainer.Query(interval);
            intervalContainer.Remove(interval);
            var afterRemoveIntersections = intervalContainer.Query(interval);

            //Assert
            Assert.AreEqual(1, preRemoveIntersections.Count());
            Assert.AreEqual(0, afterRemoveIntersections.Count());
        }

        #endregion

        /// <summary>
        /// Tests the query for interval with expected interval indexes.
        /// </summary>
        /// <param name="intervals">The intervals to query in.</param>
        /// <param name="queryFor">The interval to query for.</param>
        /// <param name="intervalsIndexesExpectedInResult">The intervals indexes expected in results. Indexes correspond to intervals in 'intervals' parameter</param>
        private void TestQueryForIntervalWithExpectedIntervalIndexes(List<Interval<DateTime>> intervals, Interval<DateTime> queryFor, int[] intervalsIndexesExpectedInResult)
        {
            //Arrange
            //for expercted results based on expected indexes from initial interval list
            IEnumerable<Interval<DateTime>> expectedResult = new List<Interval<DateTime>>();
            if (intervalsIndexesExpectedInResult != null)
            {
                expectedResult = intervals.Where(i => intervalsIndexesExpectedInResult.Contains(intervals.IndexOf(i)));
            }

            var intervalContainer = CreateIntervalContainer(intervals);

            //Act
            var intersections = intervalContainer.Query(queryFor);

            //Assert            
            CollectionAssert.AreEquivalent(expectedResult, intersections);
        }

        //private IIntervalContainer<DateTime> CreateIntervalContainer(IEnumerable<Interval<DateTime>> intervals)
        //{
        //    IIntervalContainer<DateTime> intervalContainer = CreateIntervalContainer(intervals);
        //    //foreach (var interval in intervals)
        //    //{
        //    //    intervalContainer.Add(interval);
        //    //}
        //    return intervalContainer;
        //}

        private static Interval<DateTime> ToDateTimeInterval(DateTime startTime, int leftEdgeMinutes, int rightEdgeMinutes, bool includeEdges = true)
        {
            return ToDateTimeInterval(startTime, leftEdgeMinutes, rightEdgeMinutes, includeEdges, includeEdges);
        }

        private static Interval<DateTime> ToDateTimeInterval(DateTime startTime, int leftEdgeMinutes, int rightEdgeMinutes, bool includeLefEdge, bool includeRigthEdge)
        {
            return new Interval<DateTime>(startTime.AddMinutes(leftEdgeMinutes), startTime.AddMinutes(rightEdgeMinutes), includeLefEdge, includeRigthEdge);
        }
	}
}
