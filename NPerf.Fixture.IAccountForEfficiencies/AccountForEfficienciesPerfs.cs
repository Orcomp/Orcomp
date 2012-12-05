namespace NPerf.Fixture.IAccountForEfficiencies
{
    using System;
    using System.Linq;

    using NPerf.Framework;

    using Nperf.Fixture.IAccountForEfficiencies;

    using Orc.Interval;
    using Orc.Interval.Interface;

    [PerfTester(typeof(IAccountForEfficiencies), 2, Description = "Date Interval AccountForEfficiencies() method tests", FeatureDescription = "Intervals count")]
    public class AccountForEfficienciesPerfs
    {
        private const int IntevalStep = 500;

        protected readonly DateTime now = DateTime.Now;

        protected int numberOfIntervals;

        protected int CollectionCount(int testIndex)
        {
            return IntevalStep * (testIndex + 2); //because testIndex initial value is -1
        }

        [PerfSetUp]
        public void SetUp(int testIndex, IAccountForEfficiencies accountForEfficienciesCalculator)
        {
            this.numberOfIntervals = this.CollectionCount(testIndex);
        }

        [PerfRunDescriptor]
        public double RunDescription(int testIndex)
        {
            return this.CollectionCount(testIndex);
        }

        [PerfTest]
        public void FixedStartPointNoOverlapsWithoutOffset(IAccountForEfficiencies accountForEfficienciesCalculator)
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(this.now, TimeSpan.FromMinutes(1), TimeSpan.Zero, this.numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(this.now, dateIntervalEfficiencies.Last().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedEndPointNoOverlapsWithoutOffset(IAccountForEfficiencies accountForEfficienciesCalculator)
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(this.now, TimeSpan.FromMinutes(1), TimeSpan.Zero, this.numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(this.now, dateIntervalEfficiencies.Last().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Max);
        }

        [PerfTest]
        public void FixedStartPointNoOverlapsWithOffset(IAccountForEfficiencies accountForEfficienciesCalculator)
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(this.now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), this.numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(this.now, dateIntervalEfficiencies.Last().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedEndPointNoOverlapsWithOffset(IAccountForEfficiencies accountForEfficienciesCalculator)
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(this.now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), this.numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(this.now, dateIntervalEfficiencies.Last().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Max);
        }

        [PerfTest]
        public void FixedStartPointNoOverlapsWithOffsetWithSpan(IAccountForEfficiencies accountForEfficienciesCalculator)
        {
            // |--------------------------------------| 100 % priority 1
            // |--|  |--|  |--|  |--|  |--|  |--|  |--| 1% priority 0


            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(this.now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), this.numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(this.now, dateIntervalEfficiencies.Last().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, 100, 1));

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedStartPointNoOverlapsWithOffsetWithSpan2(IAccountForEfficiencies accountForEfficienciesCalculator)
        {
            // |--------------------------------------| 100 %
            // |--|  |--|  |--|  |--|  |--|  |--|  |--|
            //  50   150    50   150   50    150

            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(this.now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), this.numberOfIntervals)
                .Select((interval, i) => new DateIntervalEfficiency(interval, i % 2 == 0 ? 50 : 150)).ToList();

            var initialInterval = new DateInterval(this.now, dateIntervalEfficiencies.Last().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, 100));

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedStartPointWithOverlaps(IAccountForEfficiencies accountForEfficienciesCalculator)
        {
            // With decreasing efficiency
            // |-------------------|  90
            //   |---------------|    80
            //      |---------|       70
            //        |-----|         60

            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.OverlapsWithDecreasingDuration(this.now, TimeSpan.FromMinutes(1), this.numberOfIntervals)
                .Select((interval, i) => new DateIntervalEfficiency(interval, this.numberOfIntervals - i)).ToList();

            var initialInterval = new DateInterval(this.now, dateIntervalEfficiencies.First().Max.Value);

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }

        [PerfTest]
        public void FixedStartPointWithOverlapsWithSpan(IAccountForEfficiencies accountForEfficienciesCalculator)
        {
            // |-------------------|  100% with highest priority
            // |-------------------|  90
            //   |---------------|    80
            //      |---------|       70
            //        |-----|         60

            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.OverlapsWithDecreasingDuration(this.now, TimeSpan.FromMinutes(1), this.numberOfIntervals)
                .Select((interval, i) => new DateIntervalEfficiency(interval, this.numberOfIntervals - i)).ToList();

            var initialInterval = new DateInterval(this.now, dateIntervalEfficiencies.First().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, 100, 1));

            accountForEfficienciesCalculator.AccountForEfficiencies(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min);
        }
    }
}
