namespace Orc.Tests.IntervalContainer
{
    using System;

    using NUnit.Framework;

    using Orc.DataStructures.RangeTree;
    using Orc.Interval.Interface;

    [TestFixture]
    public class RangeTree : DateIntervalContainerTestBase
    {
        protected override IIntervalContainer<DateTime> CreateIntervalContainer()
        {
            return new RangeTree<DateTime>();
        }
    }
}
