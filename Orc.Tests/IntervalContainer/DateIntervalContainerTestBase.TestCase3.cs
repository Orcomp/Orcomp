namespace Orc.Tests.IntervalContainer
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Orc.Entities;

    public partial class DateIntervalContainerTestBase
    {
        //Not only count but values of queried intervals are verified here

        // ****************************************
        // | X axis:                              |
        // | 0    5    10   15   20   25   30   35|
        // | |    |    |    |    |    |    |    | |
        // | Container intervals:                 |
        // |                [0---]                |
        // |            [1--]    [2--]            |
        // |      [3--]                [4--]      |
        // | Test intervals:                      |
        // | [---]                                |
        // | [----]                               |
        // | [--------]                           |
        // |      [---]                           |
        // |       [-]                            |
        // |          []                          |
        // |          [-]                         |
        // |      [---------]                     |
        // |                                [---] |
        // |                               [----] |
        // |                           [--------] |
        // |                           [---]      |
        // |                            [-]       |
        // |                          []          |
        // |                         [-]          |
        // |                     [---------]      |
        // |          [----------------]          |
        // |      [------------------------]      |
        // | [----------------------------------] |
        // | X axis:                              |
        // | |    |    |    |    |    |    |    | |
        // | 0    5    10   15   20   25   30   35|
        // ****************************************
        //Numbers at interval start points to their indexes in intervals list

        private List<Interval<DateTime>> CreateIntervalsForTestCase3(bool includeEdges)
        {
            var intervals = new List<Interval<DateTime>>();
            intervals.Add(ToDateTimeInterval(now, 15, 20, includeEdges)); //0
            intervals.Add(ToDateTimeInterval(now, 11, 15, includeEdges)); //1
            intervals.Add(ToDateTimeInterval(now, 20, 24, includeEdges)); //2
            intervals.Add(ToDateTimeInterval(now, 5, 9, includeEdges));   //3
            intervals.Add(ToDateTimeInterval(now, 26, 30, includeEdges)); //4

            return intervals;
        }

        #region Interval container with inclusive nodes
        //test cases for where all interval container intervals include both edges

        private static object[] TestCase3InclusiveSource
        {
            get
            {
                return new object[]
                {
                    //left side
                    new object[] {0, 4, true, null},
                    new object[] {0, 4, false, null},

                    new object[] {0, 5, true, new[] {3}},
                    new object[] {0, 5, false, null},

                    new object[] {0, 9, true, new[] {3}},
                    new object[] {0, 9, false, new[] {3}},

                    new object[] {5, 9, true, new[] {3}},
                    new object[] {5, 9, false, new[] {3}},

                    new object[] {6, 8, true, new[] {3}},
                    new object[] {6, 8, false, new[] {3}},

                    new object[] {9, 10, true, new[] {3}},
                    new object[] {9, 10, false, null},

                    new object[] {9, 11, true, new[] {3, 1}},
                    new object[] {9, 11, false, null},

                    new object[] {5, 15, true, new[] {3, 1, 0}},
                    new object[] {5, 15, false, new[] {3, 1}},

                    //rigth side
                    new object[] {31, 35, true, null},
                    new object[] {31, 35, false, null},

                    new object[] {30, 35, true, new[] {4}},
                    new object[] {30, 35, false, null},

                    new object[] {26, 35, true, new[] {4}},
                    new object[] {26, 35, false, new[] {4}},

                    new object[] {26, 30, true, new[] {4}},
                    new object[] {26, 30, false, new[] {4}},

                    new object[] {27, 29, true, new[] {4}},
                    new object[] {27, 29, false, new[] {4}},

                    new object[] {25, 26, true, new[] {4}},
                    new object[] {25, 26, false, null},

                    new object[] {24, 26, true, new[] {4, 2}},
                    new object[] {24, 26, false, null},

                    new object[] {20, 30, true, new[] {4, 2, 0}},
                    new object[] {20, 30, false, new[] {4, 2}},

                    //center
                    new object[] {9, 26, true, new[] {0, 1, 2, 3, 4}},
                    new object[] {9, 26, false, new[] {1, 0, 2}},

                    new object[] {5, 30, true, new[] {0, 1, 2, 3, 4}},
                    new object[] {5, 30, false, new[] {0, 1, 2, 3, 4}},

                    new object[] {0, 35, true, new[] {0, 1, 2, 3, 4}},
                    new object[] {0, 35, false, new[] {0, 1, 2, 3, 4}}
                };
            }
        }

        [Test, TestCaseSource(typeof(DateIntervalContainerTestBase), "TestCase3InclusiveSource")]
        public void Query_IntervalForInclusiveList_TestCase3_ShouldReturnCorrectIntervals(int left, int right, bool include, int[] expectedIndexes)
        {
            var intervals = CreateIntervalsForTestCase3(includeEdges: true);
            var intervalToQuery = ToDateTimeInterval(now, left, right, include);
            TestQueryForIntervalWithExpectedIntervalIndexes(intervals, intervalToQuery, expectedIndexes);
        }

        #endregion

        #region Interval container with exclusive nodes

        //test cases for where all interval container intervals do not include edges
        private static object[] TestCase3ExclusiveSource
        {
            get
            {
                //NOTE: all intervals remains the same as for inclusive vesion
                //but expected indexes array will be changed
                return new object[]
                {
                    //left side
                    new object[] {0, 4, true, null},
                    new object[] {0, 4, false, null},

                    new object[] {0, 5, true, null},
                    new object[] {0, 5, false, null},

                    new object[] {0, 9, true, new[] {3}},
                    new object[] {0, 9, false, new[] {3}},

                    new object[] {5, 9, true, new[] {3}},
                    new object[] {5, 9, false, new[] {3}},

                    new object[] {6, 8, true, new[] {3}},
                    new object[] {6, 8, false, new[] {3}},

                    new object[] {9, 10, true, null},
                    new object[] {9, 10, false, null},

                    new object[] {9, 11, true, null},
                    new object[] {9, 11, false, null},

                    new object[] {5, 15, true, new[] {3, 1}},
                    new object[] {5, 15, false, new[] {3, 1}},

                    //rigth side
                    new object[] {31, 35, true, null},
                    new object[] {31, 35, false, null},

                    new object[] {30, 35, true, null},
                    new object[] {30, 35, false, null},

                    new object[] {26, 35, true, new[] {4}},
                    new object[] {26, 35, false, new[] {4}},

                    new object[] {26, 30, true, new[] {4}},
                    new object[] {26, 30, false, new[] {4}},

                    new object[] {27, 29, true, new[] {4}},
                    new object[] {27, 29, false, new[] {4}},

                    new object[] {25, 26, true, null},
                    new object[] {25, 26, false, null},

                    new object[] {24, 26, true, null},
                    new object[] {24, 26, false, null},

                    new object[] {20, 30, true, new[] {4, 2}},
                    new object[] {20, 30, false, new[] {4, 2}},

                    //center
                    new object[] {9, 26, true, new[] {1, 0, 2}},
                    new object[] {9, 26, false, new[] {1, 0, 2}},

                    new object[] {5, 30, true, new[] {0, 1, 2, 3, 4}},
                    new object[] {5, 30, false, new[] {0, 1, 2, 3, 4}},

                    new object[] {0, 35, true, new[] {0, 1, 2, 3, 4}},
                    new object[] {0, 35, false, new[] {0, 1, 2, 3, 4}}
                };
            }
        }

        [Test, TestCaseSource(typeof(DateIntervalContainerTestBase), "TestCase3ExclusiveSource")]
        public void Query_IntervalForExclusiveList_TestCase3_ShouldReturnCorrectIntervals(int left, int right, bool include, int[] expectedIndexes)
        {
            var intervals = CreateIntervalsForTestCase3(includeEdges: false);
            var intervalToQuery = ToDateTimeInterval(now, left, right, include);
            TestQueryForIntervalWithExpectedIntervalIndexes(intervals, intervalToQuery, expectedIndexes);
        }

        #endregion
    }
}
