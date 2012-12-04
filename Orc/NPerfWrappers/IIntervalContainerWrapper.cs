namespace Orc.NPerfWrappers
{
    using System;

    //concrete implementations of generic classes under test are required here because NPerf doesn't support generics 
    public class IntervalTreeRB_DateTime : Entities.IntervalTreeRB.IntervalTree<DateTime> { }
    public class IntervalTreeAVL_DateTime : Entities.IntervalTreeAVL.IntervalTree<DateTime> { }
    public class RangeTree_DateTime : Entities.RangeTree.RangeTree<DateTime> { }
}
