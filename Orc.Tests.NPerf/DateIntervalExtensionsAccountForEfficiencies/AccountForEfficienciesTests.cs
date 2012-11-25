namespace Orc.Tests.NPerf.DateIntervalExtensionsAccountForEfficiencies
{
    using System;
    using System.Linq;

    using Orc.Entities;   
    using Orc.Submissions;

    using global::NPerf.Framework;
    
    [PerfTester(typeof(IAccountForEfficienciesWrapper), 2, Description = "Date Interval AccountForEfficiencies() method tests", FeatureDescription = "Intervals count")]
    public class AccountForEfficienciesTests
    {
        private const int IntevalStep = 500;

        protected readonly DateTime now = DateTime.Now;

        protected int numberOfIntervals;

        protected int CollectionCount(int testIndex)
        {
            return IntevalStep * (testIndex + 2); //because testIndex initial value is -1
        }

        [PerfSetUp]
        public void SetUp(int testIndex, IAccountForEfficienciesWrapper accountForEfficienciesCalculator)
        {
            numberOfIntervals = CollectionCount(testIndex);
        }

        [PerfRunDescriptor]
        public double RunDescription(int testIndex)
        {
            return CollectionCount(testIndex);
        }

        [PerfTest]
        public void FixedStartPointNoOverlapsWithoutOffset(IAccountForEfficienciesWrapper accountForEfficienciesCalculator)
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.Zero, numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedEndPointNoOverlapsWithoutOffset(IAccountForEfficienciesWrapper accountForEfficienciesCalculator)
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.Zero, numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Max);
        }

        [PerfTest]
        public void FixedStartPointNoOverlapsWithOffset(IAccountForEfficienciesWrapper accountForEfficienciesCalculator)
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedEndPointNoOverlapsWithOffset(IAccountForEfficienciesWrapper accountForEfficienciesCalculator)
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Max);
        }

        [PerfTest]
        public void FixedStartPointNoOverlapsWithOffsetWithSpan(IAccountForEfficienciesWrapper accountForEfficienciesCalculator)
        {
            // |--------------------------------------| 100 % priority 1
            // |--|  |--|  |--|  |--|  |--|  |--|  |--| 1% priority 0


            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, 100, 1));

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedStartPointNoOverlapsWithOffsetWithSpan2(IAccountForEfficienciesWrapper accountForEfficienciesCalculator)
        {
            // |--------------------------------------| 100 %
            // |--|  |--|  |--|  |--|  |--|  |--|  |--|
            //  50   150    50   150   50    150

            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), numberOfIntervals)
                .Select((interval, i) => new DateIntervalEfficiency(interval, i % 2 == 0 ? 50 : 150)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, 100));

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedStartPointWithOverlaps(IAccountForEfficienciesWrapper accountForEfficienciesCalculator)
        {
            // With decreasing efficiency
            // |-------------------|  90
            //   |---------------|    80
            //      |---------|       70
            //        |-----|         60

            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.OverlapsWithDecreasingDuration(now, TimeSpan.FromMinutes(1), numberOfIntervals)
                .Select((interval, i) => new DateIntervalEfficiency(interval, numberOfIntervals - i)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.First().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedStartPointWithOverlapsWithSpan(IAccountForEfficienciesWrapper accountForEfficienciesCalculator)
        {
            // |-------------------|  100% with highest priority
            // |-------------------|  90
            //   |---------------|    80
            //      |---------|       70
            //        |-----|         60

            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.OverlapsWithDecreasingDuration(now, TimeSpan.FromMinutes(1), numberOfIntervals)
                .Select((interval, i) => new DateIntervalEfficiency(interval, numberOfIntervals - i)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.First().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, 100, 1));

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }
    }
}
