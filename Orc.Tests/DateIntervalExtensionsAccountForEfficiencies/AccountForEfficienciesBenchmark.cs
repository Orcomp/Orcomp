namespace Orc.Tests.DateIntervalExtensionsAccountForEfficiencies
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using NUnit.Framework;

    using Orc.Extensions;
    using Orc.Interval;
    using Orc.Interval.Extensions;

    public class AccountForEfficienciesBenchmark
    {
        private DateTime now;

        int numberOfIntervals;

        [SetUp]
        public void Init()
        {
            numberOfIntervals = 10000;
            now = DateTime.FromOADate(0);
        }

		[Test]
		[Category("Benchmark")]
		[TestCase(1, 0, FixedEndPoint.Min, TestName = "FixedStartPointNoOverlapsWithoutOffset")]
		[TestCase(1, 0, FixedEndPoint.Max, TestName = "FixedEndPointNoOverlapsWithoutOffset")]
		[TestCase(1, 1, FixedEndPoint.Min, TestName = "FixedStartPointNoOverlapsWithOffset")]
		[TestCase(1, 1, FixedEndPoint.Max, TestName = "FixedEndPointNoOverlapsWithOffset")]
		public void NoOverlaps(int durationInMinutes, int offSetinMinutes, FixedEndPoint fixedEndPoint)
		{
			// 1% efficiency
			var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(durationInMinutes), TimeSpan.FromMinutes(offSetinMinutes), numberOfIntervals)
				.Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

			var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            RunAccountForEfficienciesBenchmarkTest(initialInterval, dateIntervalEfficiencies, fixedEndPoint, assertIntervalsEquality: false);
		}

        [Test]
        [TestCase(100, 1, true)]
        // |--------------------------------------| 100 % priority 1
        // |--|  |--|  |--|  |--|  |--|  |--|  |--| 1% priority 0

        [TestCase(100, 0, false)]
        // |--------------------------------------| 100 %
        // |--|  |--|  |--|  |--|  |--|  |--|  |--|
        //  50   150    50   150   50    150

        [Category("Benchmark")]
        public void FixedStartPointNoOverlapsWithOffsetWithSpan(double spanEfficiency, int spanPriority, bool assertIntervalsEquality)
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), numberOfIntervals)
                .Select(interval => new DateIntervalEfficiency(interval, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, spanEfficiency, spanPriority));

            RunAccountForEfficienciesBenchmarkTest(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min, assertIntervalsEquality);
        }

        [Test]
        [Category("Benchmark")]
        public void FixedStartPointWithOverlaps()
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

            RunAccountForEfficienciesBenchmarkTest(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min, assertIntervalsEquality: false);
        }

        [Test]
        [Category("Benchmark")]
        public void FixedStartPointWithOverlapsWithSpan()
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

            RunAccountForEfficienciesBenchmarkTest(initialInterval, dateIntervalEfficiencies, FixedEndPoint.Min, assertIntervalsEquality: true);
        }

        private void RunAccountForEfficienciesBenchmarkTest(
            DateInterval initialInterval,
            List<DateIntervalEfficiency> dateIntervalEfficiencies, 
            FixedEndPoint fixedEndPoint = FixedEndPoint.Min, 
            bool assertIntervalsEquality = false)
        {
            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, fixedEndPoint);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span whole collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
            if(assertIntervalsEquality)
            {
                Assert.AreEqual(initialInterval, effectualInterval);
            }

            initialInterval = (fixedEndPoint == FixedEndPoint.Min)
                ? new DateInterval(this.now, dateIntervalEfficiencies[this.numberOfIntervals / 2].Max.Value)
                : new DateInterval(dateIntervalEfficiencies[this.numberOfIntervals / 2].Max.Value, dateIntervalEfficiencies.Last().Max.Value);

            stopwatch.Reset();
            stopwatch.Start();
            effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, fixedEndPoint);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span half collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
            if (assertIntervalsEquality)
            {
                Assert.AreEqual(initialInterval, effectualInterval);
            }
        }
    }
}
