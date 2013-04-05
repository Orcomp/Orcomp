namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.DataStructures.C5.IntervalStaticTree;
    using Orc.DataStructures.IntervalNCList;
    using Orc.DataStructures.RangeTree;
    using Orc.Interval.Interface;

    [TestFixture]
    public class StaticIntervalTree : DateIntervalContainerTestBase
    {
        protected override IIntervalContainer<DateTime> CreateIntervalContainer(IEnumerable<IInterval<DateTime>> intervals)
        {
            return new StaticIntervalTree<DateTime>(intervals);
        }
    }
}
