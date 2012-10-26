namespace Orc.Tests.IntervalTree
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Entities.IntervalTreeVvondra;

    [TestFixture]
    public class IntervalTreeTestVvondra
    {

        [Test]
        public void Benchamrk()
        {
            var date = DateTime.Now;

            int numberOfIntervals = 10000;

            var intervals = GetDateRangesAllDescendingEndTimes(date, numberOfIntervals).ToList();

            var sb = new StringBuilder();
            Stopwatch sp = Stopwatch.StartNew();

            var it = new IntervalTree<DateTime>();

            foreach (var interval in intervals)
            {
                it.Add(interval);
            }

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

        #region GetOverlap

        [Test]
        public void Overlap_DateRangeBeforeDateRange_ReturnEmpty()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new Interval<DateTime>(t3, t4);
            var beforeDateRange = new Interval<DateTime>(t1, t2);

            var tree = new IntervalTree<DateTime>();
            tree.Add(beforeDateRange);

            // Act
            var result = tree.Search(dateRange);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void Overlap_DateRangeAfterDateRange_ReturnEmpty()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new Interval<DateTime>(t1, t2);
            var afterDateRange = new Interval<DateTime>(t3, t4);

            var tree = new IntervalTree<DateTime>();
            tree.Add(afterDateRange);

            // Act
            var result = tree.Search(dateRange);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void Overlap_DateRangeBeforeDateRangeInteresct_ReturnEmpty()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            var dateRange = new Interval<DateTime>(t2, t3);
            var beforeDateRange = new Interval<DateTime>(t1, t2);

            var tree = new IntervalTree<DateTime>();
            tree.Add(beforeDateRange);

            // Act
            var result = tree.Search(dateRange);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void Overlap_DateRangeAfterDateRangeInterect_ReturnEmpty()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            var dateRange = new Interval<DateTime>(t1, t2);
            var afterDateRange = new Interval<DateTime>(t2, t3);

            var tree = new IntervalTree<DateTime>();
            tree.Add(afterDateRange);

            // Act
            var result = tree.Search(dateRange);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void Overlap_DateRangeStartBeforeEndIn_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new Interval<DateTime>(t2, t4);
            var startBeforeEndInDateRange = new Interval<DateTime>(t1, t3);

            var tree = new IntervalTree<DateTime>();
            tree.Add(startBeforeEndInDateRange);

            // Act
            var result = tree.Search(dateRange);
            var correctResult = new List<Interval2<DateTime>> { new Interval2<DateTime>(t2, t3) };

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        [Test]
        public void Overlap_DateRangeStartBeforeEndAfter_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new Interval<DateTime>(t2, t3);
            var startBeforeEndAfterDateRange = new Interval<DateTime>(t1, t4);

            var tree = new IntervalTree<DateTime>();
            tree.Add(startBeforeEndAfterDateRange);

            // Act
            var result = tree.Search(dateRange);
            var correctResult = new List<Interval2<DateTime>> { new Interval2<DateTime>(t2, t3) };

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        [Test]
        public void Overlap_DateRangeStartInEndIn_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new Interval<DateTime>(t1, t4);
            var startInEndInDateRange = new Interval<DateTime>(t2, t3);

            var tree = new IntervalTree<DateTime>();
            tree.Add(startInEndInDateRange);

            // Act
            var result = tree.Search(dateRange);
            var correctResult = new List<Interval<DateTime>> { new Interval<DateTime>(t2, t3) };

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        [Test]
        public void Overlap_DateRangeStartInteresctEndIntersect_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            var dateRange = new Interval<DateTime>(t1, t2);
            var startInteresctEndInteresctDateRange = new Interval<DateTime>(t1, t2);

            var tree = new IntervalTree<DateTime>();
            tree.Add(startInteresctEndInteresctDateRange);

            // Act
            var result = tree.Search(dateRange);
            var correctResult = new List<Interval<DateTime>> { new Interval<DateTime>(t1, t2) };

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        [Test]
        public void Overlap_DateRangeStartInEndAfter_ReturnCorrectDateRange()
        {
            // Arrange
            DateTime t1 = DateTime.Now;
            DateTime t2 = t1.AddHours(1);
            DateTime t3 = t1.AddHours(2);
            DateTime t4 = t1.AddHours(3);
            var dateRange = new Interval<DateTime>(t1, t3);
            var startInEndAfterDateRange = new Interval<DateTime>(t2, t4);

            var tree = new IntervalTree<DateTime>();
            tree.Add(startInEndAfterDateRange);

            // Act
            var result = tree.Search(dateRange);
            var correctResult = new List<Interval<DateTime>> { new Interval<DateTime>(t2, t3) };

            // Assert
            Assert.AreEqual(correctResult, result);
        }

        #endregion
    }
}