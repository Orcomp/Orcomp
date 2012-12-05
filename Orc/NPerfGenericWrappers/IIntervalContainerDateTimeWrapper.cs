namespace Orc.NPerfGenericWrappers
{
    using System;

    using Orc.DataStructures.IntervalTreeRB;
    using Orc.DataStructures.RangeTree;

    //concrete implementations of generic classes under test are required here because NPerf doesn't support generics 
    public class IntervalTreeRB_DateTime : IntervalTree<DateTime> { }
    public class IntervalTreeAVL_DateTime : DataStructures.IntervalTreeAVL.IntervalTree<DateTime> { }
    public class RangeTree_DateTime : RangeTree<DateTime> { }
}
