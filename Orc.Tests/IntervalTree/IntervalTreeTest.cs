namespace Orc.Tests.IntervalTree
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Entities.IntervalTree;

    [TestFixture, Ignore]
    public class IntervalTreeTest
    {

        [Test]
        public void Benchamrk()
        {
            var date = DateTime.Now;

            int numberOfIntervals = 10000;

            var intervals = GetDateRangesAllDescendingEndTimes(date, numberOfIntervals).ToList();

            var sb = new StringBuilder();
            Stopwatch sp = Stopwatch.StartNew();

            var it = new IntervalTree<Interval<DateTime>, DateTime, int>();

            var count = 0;
            foreach(var interval in intervals)
            {
                it.Insert(interval, count);
                count++;
            }

            sp.Stop();
            sb.AppendLine("Time taken to build tree: " + sp.ElapsedMilliseconds);

            sp.Reset();
            sp.Start();

            var result = it.GetOverlaps(new Interval<DateTime>(date, date.AddMinutes(numberOfIntervals)));

            sp.Stop();
            sb.AppendLine("Time taken for first: " + sp.ElapsedMilliseconds);

            sp.Reset();
            sp.Start();

            var result2 = it.GetOverlaps(new Interval<DateTime>(date.AddMinutes(-1), date.AddMinutes(1)));

            sp.Stop();
            sb.AppendLine("Time taken for second: " + sp.ElapsedMilliseconds);

            sp.Reset();
            sp.Start();

            var result3 = it.GetOverlaps(new Interval<DateTime>(date.AddMinutes(-numberOfIntervals), date.AddMinutes(numberOfIntervals)));

            sp.Stop();
            sb.AppendLine("Time taken for Third: " + sp.ElapsedMilliseconds);
            sp.Reset();
            sp.Start();

            var result4 = it.GetOverlaps(new Interval<DateTime>(date.AddMinutes(numberOfIntervals - 1), date.AddMinutes(numberOfIntervals)));

            sp.Stop();
            sb.AppendLine("Time taken for Fourth: " + sp.ElapsedMilliseconds);

            Assert.AreEqual(numberOfIntervals, result.Count());

            Assert.AreEqual(numberOfIntervals, result2.Count());

            Assert.AreEqual(numberOfIntervals, result3.Count());

            Assert.AreEqual(2, result4.Count());

            var test = sb.ToString();
        }

        public static IEnumerable<Interval<DateTime>> GetDateRangesAllDescendingEndTimes(DateTime date, int count)
        {
            var dateRanges = new Interval<DateTime>[count];

            for (int i = 1; i <= count; i++)
            {
                dateRanges[i - 1] = new Interval<DateTime>(date.AddMinutes(-i), date.AddMinutes(i));
            }

            return new List<Interval<DateTime>>(dateRanges).OrderBy(x => x.Min);
        }
    }
}