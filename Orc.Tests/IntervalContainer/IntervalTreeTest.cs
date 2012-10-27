namespace Orc.Tests.IntervalContainer
{
    using System;

    using NUnit.Framework;

    using Orc.Entities.IntervalTree;
    using Orc.Interface;

    [TestFixture]
    public class IntervalTreeTest : DateIntervalContainerTestBase
    {
        protected override IIntervalContainer<DateTime> CreateIntervalContainer()
        {
            return new IntervalTree<DateTime>();
        }
    }
}
