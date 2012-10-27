namespace Orc.Tests.RangeTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Entities.RangeTree;

    [TestFixture]
    public class RangeTreeTest2
    {
        private DateTime now;

        protected DateTime inOneHour;

        protected DateTime inTwoHours;

        protected DateTime inThreeHours;

        [SetUp]
        public void Setup()
        {
            this.now = DateTime.Now;
            this.inOneHour = this.now.AddHours(1);
            this.inTwoHours = this.now.AddHours(2);
            this.inThreeHours = this.now.AddHours(3);
        }

        [TestCase(4, 4, 3)]
        [TestCase(4, 5, 4)]
        [TestCase(-1, 10, 7)]
        [TestCase(-1, -1, 0)]
        [TestCase(1, 4, 5)]
        [TestCase(0, 1, 2)]
        [TestCase(10, 12, 0)]
        public void Search_ForDifferentDateIntervals_ShoudReturnCorrectOverlapsCount(int leftEdgeMinutes, int rightEdgeMinutes, int expectedOverlapsCount)
        {
            //Arrange
            var intervals = new List<Interval<DateTime>>();

            intervals.Add(ToDateTimeInterval(this.now, -300, -200));
            intervals.Add(ToDateTimeInterval(this.now, -300, 200));
            intervals.Add(ToDateTimeInterval(this.now, -300, 200));
            intervals.Add(ToDateTimeInterval(this.now, -3, -2));
            intervals.Add(ToDateTimeInterval(this.now, 1, 2));
            intervals.Add(ToDateTimeInterval(this.now, 3, 6));
            intervals.Add(ToDateTimeInterval(this.now, 2, 4));
            intervals.Add(ToDateTimeInterval(this.now, 5, 7));
            intervals.Add(ToDateTimeInterval(this.now, 1, 3));
            intervals.Add(ToDateTimeInterval(this.now, 4, 6));
            intervals.Add(ToDateTimeInterval(this.now, 8, 9));
            intervals.Add(ToDateTimeInterval(this.now, 15, 20));
            intervals.Add(ToDateTimeInterval(this.now, 40, 50));
            intervals.Add(ToDateTimeInterval(this.now, 49, 60));

            var it = new RangeTree<DateTime>();
            intervals.ForEach(x => it.Add(x));

            //Act
            var overlaps = it.Query(ToDateTimeInterval(this.now, leftEdgeMinutes, rightEdgeMinutes));

            //Assert
            Assert.AreEqual(expectedOverlapsCount, overlaps.Count());
        }

        private static Interval<DateTime> ToDateTimeInterval(DateTime startTime, int leftEdgeMinutes, int rightEdgeMinutes)
        {
            return new Interval<DateTime>(startTime.AddMinutes(leftEdgeMinutes), startTime.AddMinutes(rightEdgeMinutes));
        }
    }
}
