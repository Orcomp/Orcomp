namespace Orc.Tests.DateIntervalExtensionsAccountForEfficiencies
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Extensions;

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
        public void FixedStartPointNoOverlapsWithoutOffset()
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.Zero, numberOfIntervals).Select(x => new DateIntervalEfficiency(x, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span whole collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));

            initialInterval = new DateInterval(now, dateIntervalEfficiencies[numberOfIntervals/2].Max.Value);

            stopwatch.Reset();
            stopwatch.Start();
            effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span half collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
        }

        [Test]
        [Category("Benchmark")]
        public void FixedEndPointNoOverlapsWithoutOffset()
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.Zero, numberOfIntervals).Select(x => new DateIntervalEfficiency(x, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span whole collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));

            initialInterval = new DateInterval(dateIntervalEfficiencies[numberOfIntervals / 2].Max.Value, dateIntervalEfficiencies.Last().Max.Value);

            stopwatch.Reset();
            stopwatch.Start();
            effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span half collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
        }

        [Test]
        [Category("Benchmark")]
        public void FixedStartPointNoOverlapsWithOffset()
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), numberOfIntervals).Select(x => new DateIntervalEfficiency(x, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span whole collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));

            initialInterval = new DateInterval(now, dateIntervalEfficiencies[numberOfIntervals / 2].Max.Value);

            stopwatch.Reset();
            stopwatch.Start();
            effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span half collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
        }

        [Test]
        [Category("Benchmark")]
        public void FixedEndPointNoOverlapsWithOffset()
        {
            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), numberOfIntervals).Select(x => new DateIntervalEfficiency(x, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span whole collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));

            initialInterval = new DateInterval(dateIntervalEfficiencies[numberOfIntervals / 2].Max.Value, dateIntervalEfficiencies.Last().Max.Value);

            stopwatch.Reset();
            stopwatch.Start();
            effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Max);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span half collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
        }

        [Test]
        [Category("Benchmark")]
        public void FixedStartPointNoOverlapsWithOffsetWithSpan()
        {
            // |--------------------------------------| 100 % priority 1
            // |--|  |--|  |--|  |--|  |--|  |--|  |--| 1% priority 0


            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), numberOfIntervals).Select(x => new DateIntervalEfficiency(x, 1)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, 100, 1));

            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span whole collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
            Assert.AreEqual(initialInterval, effectualInterval);

            initialInterval = new DateInterval(now, dateIntervalEfficiencies[numberOfIntervals / 2].Max.Value);

            stopwatch.Reset();
            stopwatch.Start();
            effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span half collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
            Assert.AreEqual(initialInterval, effectualInterval);
        }

        [Test]
        [Category("Benchmark")]
        public void FixedStartPointNoOverlapsWithOffsetWithSpan2()
        {
            // |--------------------------------------| 100 %
            // |--|  |--|  |--|  |--|  |--|  |--|  |--|
            //  50   150    50   150   50    150

            // 1% efficiency
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.NoOverlaps(now, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), numberOfIntervals).Select((x, i) => new DateIntervalEfficiency(x, i % 2 == 0 ? 50 : 150 )).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, 100));

            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span whole collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));

            initialInterval = new DateInterval(now, dateIntervalEfficiencies[numberOfIntervals / 2].Max.Value);

            stopwatch.Reset();
            stopwatch.Start();
            effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span half collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
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
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.OverlapsWithDecreasingDuration(now, TimeSpan.FromMinutes(1), numberOfIntervals).Select((x, i) => new DateIntervalEfficiency(x, numberOfIntervals - i)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span whole collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));

            initialInterval = new DateInterval(now, dateIntervalEfficiencies[numberOfIntervals / 2].Max.Value);

            stopwatch.Reset();
            stopwatch.Start();
            effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span half collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
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
            var dateIntervalEfficiencies = DateIntervalCollectionGenerator.OverlapsWithDecreasingDuration(now, TimeSpan.FromMinutes(1), numberOfIntervals).Select((x, i) => new DateIntervalEfficiency(x, numberOfIntervals - i)).ToList();

            var initialInterval = new DateInterval(now, dateIntervalEfficiencies.Last().Max.Value);

            dateIntervalEfficiencies.Add(new DateIntervalEfficiency(initialInterval, 100, 1));

            var stopwatch = Stopwatch.StartNew();
            var effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span whole collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
            Assert.AreEqual(initialInterval, effectualInterval);

            initialInterval = new DateInterval(now, dateIntervalEfficiencies[numberOfIntervals / 2].Max.Value);

            stopwatch.Reset();
            stopwatch.Start();
            effectualInterval = initialInterval.AccountForEfficiencies(dateIntervalEfficiencies, FixedEndPoint.Min);
            stopwatch.Stop();

            Debug.WriteLine(string.Format("Span half collection, Elapsed time: {0} ms, result is: {1}", stopwatch.ElapsedMilliseconds, effectualInterval));
            Assert.AreEqual(initialInterval, effectualInterval);
        }
    }
}
