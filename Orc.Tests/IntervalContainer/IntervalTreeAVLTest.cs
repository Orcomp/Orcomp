namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Entities.IntervalTreeAVL;
    using Orc.Interface;

    [TestFixture]
    public class IntervalTreeAVLTest : DateIntervalContainerTestBase
    {
        protected override IIntervalContainer<DateTime> CreateIntervalContainer()
        {
            return new IntervalTree<DateTime>();
        }

        [Test]
        public void SimplestFailingTest()
        {
            var now = DateTime.Now;

            //Arrange
            var intervals = new List<Interval<DateTime>>();
            //intervals.Add(ToDateTimeInterval(now, -300, -200));
            //intervals.Add(ToDateTimeInterval(now, -3, -2));
            intervals.Add(ToDateTimeInterval(now, 1, 2));
            //intervals.Add(ToDateTimeInterval(now, 3, 6));
            //intervals.Add(ToDateTimeInterval(now, 2, 4));
            //intervals.Add(ToDateTimeInterval(now, 5, 7));
            intervals.Add(ToDateTimeInterval(now, 1, 3));
            //intervals.Add(ToDateTimeInterval(now, 4, 6));
            //intervals.Add(ToDateTimeInterval(now, 8, 9));
            //intervals.Add(ToDateTimeInterval(now, 15, 20));
            //intervals.Add(ToDateTimeInterval(now, 40, 50));
            //intervals.Add(ToDateTimeInterval(now, 49, 60));

            var intervalTree = new IntervalTree<DateTime>();
            intervals.ForEach(x => intervalTree.Add(x));

            //Act
            var overlaps = intervalTree.Query(ToDateTimeInterval(now, 0, 1));

            //Assert
            Assert.AreEqual(2, overlaps.Count());
        }

        private static Interval<DateTime> ToDateTimeInterval(DateTime startTime, int leftEdgeMinutes, int rightEdgeMinutes, bool includeEdges = true)
        {
            return new Interval<DateTime>(startTime.AddMinutes(leftEdgeMinutes), startTime.AddMinutes(rightEdgeMinutes), includeEdges, includeEdges);
        }

        [Test]
        public void SimplestWorkingTest()
        {
            //Arrange
            var intervals = new List<Interval<int>>();
            intervals.Add(new Interval<int>(1, 2));
            intervals.Add(new Interval<int>(1, 3));

            var intervalContainer = new Entities.IntervalTree.IntervalTree<int>();
            intervals.ForEach(intervalContainer.Add);

            //Act
            var intersections = intervalContainer.Query(new Interval<int>(0,1));

            //Assert
            Assert.AreEqual(2, intersections.Count());
        }
    }
}
