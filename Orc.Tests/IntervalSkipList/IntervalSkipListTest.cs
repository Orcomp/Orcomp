namespace Orc.Tests.IntervalSkipList
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Collections.Generic;

    using Orc.Entities;
    using Orc.Entities.IntervalSkipList;

    [TestClass]
    public class IntervalSkipListTest
    {
        [TestMethod]
        public void TestSearch()
        {
            List<Interval<int>> intervals = new List<Interval<int>>();

            intervals.Add(new Interval<int>(-300, -200));
            intervals.Add(new Interval<int>(-3, -2));
            intervals.Add(new Interval<int>(1, 2));
            intervals.Add(new Interval<int>(3, 6));
            intervals.Add(new Interval<int>(2, 4));
            intervals.Add(new Interval<int>(5, 7));
            intervals.Add(new Interval<int>(1, 3));
            intervals.Add(new Interval<int>(4, 6));
            intervals.Add(new Interval<int>(8, 9));
            intervals.Add(new Interval<int>(15, 20));
            intervals.Add(new Interval<int>(40, 50));
            intervals.Add(new Interval<int>(49, 60));


            IntervalSkipList<int> it = new IntervalSkipList<int>(intervals);

            Assert.AreEqual(3, it.Search(new Interval<int>(4, 4)).Count);

            Assert.AreEqual(4, it.Search(new Interval<int>(4, 5)).Count);

            Assert.AreEqual(7, it.Search(new Interval<int>(-1, 10)).Count);

            Assert.AreEqual(0, it.Search(new Interval<int>(-1, -1)).Count);

            Assert.AreEqual(5, it.Search(new Interval<int>(1, 4)).Count);

            Assert.AreEqual(2, it.Search(new Interval<int>(0, 1)).Count);

            Assert.AreEqual(0, it.Search(new Interval<int>(10, 12)).Count);

            List<Interval<int>> intervals2 = new List<Interval<int>>();

            //stravinsky 1880-1971
            intervals2.Add(new Interval<int>(1880, 1971));
            //Schoenberg
            intervals2.Add(new Interval<int>(1874, 1951));
            //Grieg
            intervals2.Add(new Interval<int>(1843, 1907));
            //Schubert
            intervals2.Add(new Interval<int>(1779, 1828));
            //Mozart
            intervals2.Add(new Interval<int>(1756, 1828));
            //Schuetz
            intervals2.Add(new Interval<int>(1585, 1672));

            IntervalSkipList<int> it2 = new IntervalSkipList<int>(intervals2);

            Assert.AreEqual(0, it2.Search(new Interval<int>(1829, 1842)).Count);

            LinkedList<Interval<int>> intersection1 = it2.Search(new Interval<int>(1907, 1907));
            Assert.AreEqual(3, intersection1.Count);

            intersection1 = it2.Search(new Interval<int>(1780, 1790));
            Assert.AreEqual(2, intersection1.Count);
        }

        [TestMethod]
        public void Benchamrk()
        {
            var date = DateTime.Now;

            int numberOfIntervals = 1000000;

            var intervals = GetDateRangesAllDescendingEndTimes(date, numberOfIntervals).ToList();

            var sb = new StringBuilder();
            Stopwatch sp = Stopwatch.StartNew();

            var it = new IntervalSkipList<DateTime>(intervals);

            sp.Stop();
            sb.AppendLine("Time taken to build tree: " + sp.ElapsedMilliseconds);

            sp.Reset();
            sp.Start();


            var result = it.Search(new Interval<DateTime>(date, date.AddMinutes(numberOfIntervals)));

            sp.Stop();
            sb.AppendLine("Time taken for first: " + sp.ElapsedMilliseconds);

            sp.Reset();
            sp.Start();

            var result2 = it.Search(new Interval<DateTime>(date.AddMinutes(-1), date.AddMinutes(1)));

            sp.Stop();
            sb.AppendLine("Time taken for second: " + sp.ElapsedMilliseconds);

            sp.Reset();
            sp.Start();

            var result3 = it.Search(new Interval<DateTime>(date.AddMinutes(-numberOfIntervals), date.AddMinutes(numberOfIntervals)));

            sp.Stop();
            sb.AppendLine("Time taken for Third: " + sp.ElapsedMilliseconds);
            sp.Reset();
            sp.Start();

            var result4 = it.Search(new Interval<DateTime>(date.AddMinutes(numberOfIntervals - 1), date.AddMinutes(numberOfIntervals)));

            sp.Stop();
            sb.AppendLine("Time taken for Fourth: " + sp.ElapsedMilliseconds);

            Assert.AreEqual(numberOfIntervals, result.Count);

            Assert.AreEqual(numberOfIntervals, result2.Count);

            Assert.AreEqual(numberOfIntervals, result3.Count);

            Assert.AreEqual(2, result4.Count);

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
