namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Interval;

    public partial class DateIntervalContainerTestBase
    {
        //Not only count but values of queried intervals are verified here

        // ********************************************************
        // | X axis:                                              |
        // | 0    5    10   15   20   25   30   35   40   45   50 |
        // | |    |    |    |    |    |    |    |    |    |    |  |
        // | Container intervals:                                 |
        // | [0-------------]    [1---]         [2--------]       |
        // |   [3------][4-----------------]                      |
        // |      [5-------------------------------------------]  |
        // | Test intervals:                                      |
        // |        [---------------]                             |
        // |                               [---------]            |
        // | X axis:                                              |
        // | |    |    |    |    |    |    |    |    |    |    |  |
        // | 0    5    10   15   20   25   30   35   40   45   50 |
        // ********************************************************
        //Numbers at interval start points to their indexes in intervals list

        private List<Interval<DateTime>> CreateIntervalsForTestCase1()
        {
            var intervals = new List<Interval<DateTime>>();
            intervals.Add(ToDateTimeInterval(now, 0, 15));  //0
            intervals.Add(ToDateTimeInterval(now, 20, 25)); //1
            intervals.Add(ToDateTimeInterval(now, 35, 45)); //2
            intervals.Add(ToDateTimeInterval(now, 3, 10));  //3
            intervals.Add(ToDateTimeInterval(now, 11, 30)); //4
            intervals.Add(ToDateTimeInterval(now, 5, 50));  //5

            return intervals;
        }

        private static object[] TestCase1Source
        {
            get
            {
                return new object[]
                {
                    new object[] {7, 23, true, new[] {0, 1, 3, 4, 5}},
                    new object[] {7, 23, false, new[] {0, 1, 3, 4, 5}},

                    new object[] {30, 40, true, new[] {2, 4, 5}},
                    new object[] {30, 40, false, new[] {2, 5}}
                };
            }
        }

        [Test, TestCaseSource(typeof(DateIntervalContainerTestBase), "TestCase1Source")]
        public void Query_IntervalInTestCase1_ShouldReturnCorrectIntervals(int left, int right, bool include, int[] expectedIndexes)
        {
            var intervals = CreateIntervalsForTestCase1();
            var intervalToQuery = ToDateTimeInterval(now, left, right, include);
            TestQueryForIntervalWithExpectedIntervalIndexes(intervals, intervalToQuery, expectedIndexes);
        }
    }
}
