namespace Orc.Tests.IntervalContainer.NPerf
{
    using System;

    using Orc.Interface;

    using global::NPerf.Framework;

    [PerfTester(typeof(IIntervalContainer<DateTime>), 2, Description = "Interval Container Add method benchmark tests for DateTime interval", FeatureDescription = "Intervals count")]
    public class DateIntervalContainerAddTests : DateIntervalContainerBenchmarkBase
    {
        [PerfSetUp]
        public void SetUp(int testIndex, IIntervalContainer<DateTime> intervalContainer)
        {
            numberOfIntervals = CollectionCount(testIndex);
        }

        [PerfRunDescriptor]
        public double RunDescription(int testIndex)
        {
            return CollectionCount(testIndex);
        }        

        [PerfTest]
        public void Add_Interval(IIntervalContainer<DateTime> container)
        {
            const int intervalLength = 4;
            const int spaceLength = 1;
            const int intervalAndSpaceLength = intervalLength + spaceLength;
            for (int i = 0; i < numberOfIntervals; i++)
            {
                var intervalToAdd = ToDateTimeInterval(now, i * intervalAndSpaceLength, ((i + 1) * intervalAndSpaceLength) - spaceLength);
                container.Add(intervalToAdd);
            }
        }
    }
}
