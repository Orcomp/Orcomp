namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Orc.DataStructures.IntervalTreeRB;
    using Orc.Interval;
    using Orc.Interval.Interface;

    [TestFixture, Ignore]
    public class IntervalTreeRBTest : DateIntervalContainerTestBase
    {
        protected override IIntervalContainer<DateTime> CreateIntervalContainer(IEnumerable<IInterval<DateTime>> intervals)
        {
            return new IntervalTree<DateTime>(intervals);
        }

        [Test]
        public void SimplestFailingTest()
        {
            // Container: [-----] [------] [----------]
            //                      [---]     [----]
            // Query:                             [---]

            //Arrange
            var intervals = new List<Interval<int>>();
            intervals.Add(new Interval<int>(4, 10));
            intervals.Add(new Interval<int>(21, 35));
            intervals.Add(new Interval<int>(25, 31));
            intervals.Add(new Interval<int>(42, 56));            
            intervals.Add(new Interval<int>(46, 52));

            var intervalContainer = new IntervalTree<int>();
            intervals.ForEach(intervalContainer.Add);

            //Act
            var intersections = intervalContainer.Query(new Interval<int>(49, 56));

            //Assert
            Assert.AreEqual(2, intersections.Count());
        }
    }
}
