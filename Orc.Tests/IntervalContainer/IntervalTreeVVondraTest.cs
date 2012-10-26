namespace Orc.Tests.IntervalContainer
{
    using System;

    using NUnit.Framework;

    using Orc.Entities.IntervalTreeVvondra;
    using Orc.Interface;

    [TestFixture]
    public class IntervalTreeVVondraTest : DateIntervalContainerTestBase
    {
        protected override IIntervalContainer<DateTime> CreateIntervalContainer()
        {
            return new IntervalTree<DateTime>();
        }
    }
}
