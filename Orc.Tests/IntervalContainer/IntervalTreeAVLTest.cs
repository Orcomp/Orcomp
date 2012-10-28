namespace Orc.Tests.IntervalContainer
{
    using System;

    using NUnit.Framework;

    using Orc.Entities.IntervalTreeAVL;
    using Orc.Interface;

    [TestFixture]
    public class IntervalTreeAVLTest : DateIntervalContainerTestBase
    {
        protected override IIntervalContainer<DateTime> CreateIntervalContainer()
        {
            return new IntervalTree<DateTime>();
        }
    }
}
