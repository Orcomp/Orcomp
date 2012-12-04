namespace NPerf.Fixture.IIntervalContainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Entities;
    using Orc.Interface;

    using NPerf.Framework;

    [PerfTester(typeof(IIntervalContainer<DateTime>), 2, Description = "Interval Container Query method benchmark tests for DateTime interval", FeatureDescription = "Intervals count")]
    public class DateIntervalContainerIncludedOneInOneQueryPerfs : DateIntervalContainerBenchmarkBase
    {
        [PerfSetUp]
        public void SetUp(int testIndex, IIntervalContainer<DateTime> intervalContainer)
        {
            this.numberOfIntervals = this.CollectionCount(testIndex);
            var intervals = GenerateIncludedOneIntoAnotherIntervals(DateTime.Now, this.numberOfIntervals);
            foreach (var interval in intervals)
            {
                intervalContainer.Add(interval);
            }
        }

        [PerfRunDescriptor]
        public double RunDescription(int testIndex)
        {
            return this.CollectionCount(testIndex);
        }

        [PerfTest]
        public void Query_MidPointToMaxSpanningInterval(IIntervalContainer<DateTime> container)
        {
            Interval<DateTime> queryInterval = this.ToDateTimeInterval(DateTime.Now, 0, this.numberOfIntervals);
            container.Query(queryInterval).ToList();
        }

        [PerfTest]
        public void Query_MidInterval(IIntervalContainer<DateTime> container)
        {
            Interval<DateTime> queryInterval = this.ToDateTimeInterval(DateTime.Now, -1, 1);
            container.Query(queryInterval).ToList();
        }

        [PerfTest]
        public void Query_MinToMaxSpanningInterval(IIntervalContainer<DateTime> container)
        {
            Interval<DateTime> queryInterval = this.ToDateTimeInterval(DateTime.Now, -this.numberOfIntervals, this.numberOfIntervals);
            container.Query(queryInterval).ToList();
        }

        [PerfTest]
        public void Query_RigthEndInterval(IIntervalContainer<DateTime> container)
        {
            Interval<DateTime> queryInterval = this.ToDateTimeInterval(DateTime.Now, this.numberOfIntervals - 1, this.numberOfIntervals);
            container.Query(queryInterval).ToList();
        }

        private static IEnumerable<Interval<DateTime>> GenerateIncludedOneIntoAnotherIntervals(DateTime date, int count)
        {
            //              ...            
            //       [--------------]      
            //    [--------------------]   
            // [--------------------------]
            //              ...            

            var dateRanges = new Interval<DateTime>[count];

            for (int i = 1; i <= count; i++)
            {
                dateRanges[i - 1] = new Interval<DateTime>(date.AddMinutes(-i), date.AddMinutes(i));
            }

            return new List<Interval<DateTime>>(dateRanges).OrderBy(x => x.Min.Value);
        }
    }
}
