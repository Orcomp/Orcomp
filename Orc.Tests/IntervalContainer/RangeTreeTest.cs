namespace Orc.Tests.IntervalContainer
{
    using System;

    using NUnit.Framework;

    using Orc.Entities.RangeTree;
    using Orc.Interface;

    [TestFixture]
    public class RangeTree : DateIntervalContainerTestBase
    {
        protected override IIntervalContainer<DateTime> CreateIntervalContainer()
        {
            return new RangeTree<DateTime>();
        }
    }
}
