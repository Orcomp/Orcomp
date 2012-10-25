namespace Orc.Tests.IntervalSkipList
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Collections.Generic;

    using Orc.Entities;
    using Orc.Entities.IntervalSkipList;

    [TestClass]
    public class IntervalNodeTest
    {
        [TestMethod]
        public void TestFindMedianEndpoint()
        {
            List<Interval<int>> intervalList = new List<Interval<int>>();

            intervalList.Add(new Interval<int>(1, 2));
            intervalList.Add(new Interval<int>(2, 3));

            IntervalNode<int> node = new IntervalNode<int>(intervalList);
            IComparable<int> c = node.FindMedianEndpoint(intervalList);

            Assert.IsTrue(c.CompareTo(2) == 0);
            Assert.IsFalse(c.CompareTo(1) == 0);
            Assert.IsFalse(c.CompareTo(0) == 0);
            Assert.IsFalse(c.CompareTo(3) == 0);
        }

        [TestMethod]
        public void TestGetLeftIntervals()
        {
            List<Interval<int>> intervalList = new List<Interval<int>>();
            intervalList.Add(new Interval<int>(1, 2));
            intervalList.Add(new Interval<int>(2, 3));
            intervalList.Add(new Interval<int>(3, 4));
            intervalList.Add(new Interval<int>(4, 5));
            intervalList.Add(new Interval<int>(6, 7));


            IntervalNode<int> node = new IntervalNode<int>(intervalList);

            List<Interval<int>> leftInterval = node.GetLeftIntervals(intervalList);
            List<Interval<int>> rightInterval = node.GetRightIntervals(intervalList);
            List<Interval<int>> centerInterval = node.GetIntersectingIntervals(intervalList);

            Assert.AreEqual(leftInterval.Count, 2);
            Assert.AreEqual(rightInterval.Count, 1);
            Assert.AreEqual(centerInterval.Count, 2);
        }
    }
}
