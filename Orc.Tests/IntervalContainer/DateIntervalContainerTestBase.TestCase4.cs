namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Interval;

    public partial class DateIntervalContainerTestBase
    {
        //Not only count but values of queried intervals are verified here

        // **************************************************
        // | X axis:                                        |
        // | 0    5    10   15   20   25   30   35   40   45|
        // | |    |    |    |    |    |    |    |    |    | |
        // | Container intervals:                           |
        // |      [0---]    [3---]    [6---]    [9---]      |
        // |      [1---]    [4---]    [7---]    [10--]      |
        // |      [2---]    [5---]    [8---]    [11--]      |
        // | Test intervals:                                |
        // |      [----------------------------------]      |
        // | [------]                                       |
        // |        [----]                                  |
        // |           [----]                               |
        // |              [------------------]              |
        // |                                 [-----]        |
        // |                                      [-------] |
        // |                        [---]                   |
        // | X axis:                                        |
        // | 0    5    10   15   20   25   30   35   40   45|
        // | |    |    |    |    |    |    |    |    |    | |
        // **************************************************
        //Numbers at interval start points to their indexes in intervals list

        private List<Interval<DateTime>> CreateIntervalsForTestCase4(bool includeLeftEdge, bool includeRightEdge)
        {
            var intervals = new List<Interval<DateTime>>();
            for (int i = 5; i <= 35; i += 10)
            {
                for (int j = 0; j < 3; j++)
                {
                    intervals.Add(ToDateTimeInterval(now, i, i + 5, includeLeftEdge, includeRightEdge));       
                }
            }            

            return intervals;
        }

        #region Interval container with inclusive nodes
        //test cases for where all interval container intervals include both edges

        private static object[] TestCase4InclusiveSource
        {
            get
            {
                return new object[]
                {
                    new object[] {5, 40, true, true, new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}},
                    new object[] {0, 7, true, true, new[] {0, 1, 2}},
                    new object[] {7, 12, true, true, new[] {0, 1, 2}},
                    new object[] {10, 15, true, true, new[] {0, 1, 2, 3, 4, 5}},
                    new object[] {13, 32, true, true, new[] {3, 4, 5, 6, 7, 8}},
                    new object[] {32, 38, true, true, new[] {9, 10, 11}},
                    new object[] {37, 45, true, true, new[] {9, 10, 11}},
                    new object[] {23, 27, true, true, new[] {6, 7, 8}}
                };
            }
        }

        [Test, TestCaseSource(typeof(DateIntervalContainerTestBase), "TestCase4InclusiveSource")]
        public void Query_InclusiveIntervals_TestCase4_ShouldReturnCorrectIntervals(int left, int right, bool includeLeft, bool includeRight, int[] expectedIndexes)
        {
            var intervals = CreateIntervalsForTestCase4(includeLeftEdge: true, includeRightEdge: true);
            var intervalToQuery = ToDateTimeInterval(now, left, right, includeLeft, includeRight);
            TestQueryForIntervalWithExpectedIntervalIndexes(intervals, intervalToQuery, expectedIndexes);
        }

        #endregion

        #region Interval container with exclusive nodes

        //test cases for where all interval container intervals do not include edges
        private static object[] TestCase4ExclusiveSource
        {
            get
            {
                //NOTE: all intervals remains the same as for inclusive vesion
                //but expected indexes array will be changed
                return new object[]
                {
                    new object[] {5, 40, false, false, new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}},
                    new object[] {0, 7, false, false, new[] {0, 1, 2}},
                    new object[] {7, 12, false, false, new[] {0, 1, 2}},
                    new object[] {10, 15, false, false, null},
                    new object[] {13, 32, false, false, new[] {3, 4, 5, 6, 7, 8}},
                    new object[] {32, 38, false, false, new[] {9, 10, 11}},
                    new object[] {37, 45, false, false, new[] {9, 10, 11}},
                    new object[] {23, 27, false, false, new[] {6, 7, 8}}
                };
            }
        }

        [Test, TestCaseSource(typeof(DateIntervalContainerTestBase), "TestCase4ExclusiveSource")]
        public void Query_ExclusiveIntervals_TestCase4_ShouldReturnCorrectIntervals(int left, int right, bool includeLeft, bool includeRight, int[] expectedIndexes)
        {
            var intervals = CreateIntervalsForTestCase4(includeLeftEdge: false, includeRightEdge: false);
            var intervalToQuery = ToDateTimeInterval(now, left, right, includeLeft, includeRight);
            TestQueryForIntervalWithExpectedIntervalIndexes(intervals, intervalToQuery, expectedIndexes);
        }

        #endregion

        #region Interval container with min inclusuve and max exclusive nodes

        //test cases for where all interval container intervals do not include edges
        private static object[] TestCase4MinInclusiveMaxExclusiveSource
        {
            get
            {
                //NOTE: all intervals remains the same as for inclusive vesion
                //but expected indexes array will be changed
                return new object[]
                {
                    new object[] {5, 40, true, false, new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}},
                    new object[] {0, 7, true, false, new[] {0, 1, 2}},
                    new object[] {7, 12, true, false, new[] {0, 1, 2}},
                    new object[] {10, 15, true, false, null},
                    new object[] {13, 32, true, false, new[] {3, 4, 5, 6, 7, 8}},
                    new object[] {32, 38, true, false, new[] {9, 10, 11}},
                    new object[] {37, 45, true, false, new[] {9, 10, 11}},
                    new object[] {23, 27, true, false, new[] {6, 7, 8}}
                };
            }
        }

        [Test, TestCaseSource(typeof(DateIntervalContainerTestBase), "TestCase4MinInclusiveMaxExclusiveSource")]
        public void Query_MinInclusiveMaxExclusiveInterval_TestCase4_ShouldReturnCorrectIntervals(int left, int right, bool includeLeft, bool includeRight, int[] expectedIndexes)
        {
            var intervals = CreateIntervalsForTestCase4(includeLeftEdge: true, includeRightEdge: false);
            var intervalToQuery = ToDateTimeInterval(now, left, right, includeLeft, includeRight);
            TestQueryForIntervalWithExpectedIntervalIndexes(intervals, intervalToQuery, expectedIndexes);
        }

        #endregion
    }
}
