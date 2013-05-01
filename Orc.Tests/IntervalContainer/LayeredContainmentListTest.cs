namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.DataStructures.C5;
    using Orc.Interval.Interface;

    [TestFixture]
    public class LayeredContainmentList : DateIntervalContainerTestBase
    {
        protected override IIntervalContainer<DateTime> CreateIntervalContainer(IEnumerable<IInterval<DateTime>> intervals)
        {
            return new LayeredContainmentList<DateTime>(intervals);
        }
    }
}
