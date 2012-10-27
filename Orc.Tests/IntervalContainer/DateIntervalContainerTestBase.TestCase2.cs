namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Entities;

    public partial class DateIntervalContainerTestBase
    {
        //Not only count but values of queried intervals are verified here

        // ********************************************************
        // | X axis:                                              |
        // | 0    5    10   15   20   25   30   35   40   45   50 |
        // | |    |    |    |    |    |    |    |    |    |    |  |
        // | Container intervals:                                 |
        // |      [0--------------------------------------]       |
        // |           [1----------------------------]            |
        // |                [2------------------]                 |
        // |                     [3--------]                      |
        // |                        [4--]                         |
        // | Test intervals:                                      |
        // | [--------]                                           |
        // |      [-------------------]                           |
        // |           [---------]                                |
        // |                          [-------------------]       |
        // |                               [---------]            |
        // | [-------------------------------------------------]  |
        // | X axis:                                              |
        // | |    |    |    |    |    |    |    |    |    |    |  |
        // | 0    5    10   15   20   25   30   35   40   45   50 |
        // ********************************************************
        //Numbers at interval start points to their indexes in intervals list

        private List<Interval<DateTime>> CreateIntervalsForTestCase2()
        {
            var intervals = new List<Interval<DateTime>>();
            intervals.Add(ToDateTimeInterval(now, 5, 45));  //0
            intervals.Add(ToDateTimeInterval(now, 10, 40)); //1
            intervals.Add(ToDateTimeInterval(now, 15, 35)); //2
            intervals.Add(ToDateTimeInterval(now, 20, 30)); //3
            intervals.Add(ToDateTimeInterval(now, 23, 27)); //4

            return intervals;
        }

        private static object[] TestCase2Source
        {
            get
            {
                return new object[]
                {
                    new object[] {0, 9, true, new[] {0}},
                    new object[] {0, 9, false, new[] {0}},

                    new object[] {5, 25, true, new[] {0, 1, 2, 3, 4}},
                    new object[] {5, 25, false, new[] {0, 1, 2, 3, 4}},

                    new object[] {10, 20, true, new[] {0, 1, 2, 3}},
                    new object[] {10, 20, false, new[] {0, 1, 2}},

                    new object[] {25, 45, true, new[] {0, 1, 2, 3, 4}},
                    new object[] {25, 45, false, new[] {0, 1, 2, 3, 4}},

                    new object[] {30, 40, true, new[] {0, 1, 2, 3}},
                    new object[] {30, 40, false, new[] {0, 1, 2}},

                    new object[] {0, 50, true, new[] {0, 1, 2, 3, 4}},
                    new object[] {0, 50, false, new[] {0, 1, 2, 3, 4}}
                };
            }
        }

        [Test, TestCaseSource(typeof(DateIntervalContainerTestBase), "TestCase2Source")]
        public void Query_IntervalInTestCase2_ShouldReturnCorrectIntervals(int left, int right, bool include, int[] expectedIndexes)
        {
            var intervals = CreateIntervalsForTestCase2();
            var intervalToQuery = ToDateTimeInterval(now, left, right, include);
            TestQueryForIntervalWithExpectedIntervalIndexes(intervals, intervalToQuery, expectedIndexes);
        }
    }
}
