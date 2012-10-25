namespace Orc.Tests.IntervalSkipList
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Entities;
    using Orc.Entities.IntervalSkipList;

    [TestFixture]
    public class IntervalSkipListTest
    {
        [Test]
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
    }
}
