namespace Orc.Tests.IntervalContainer.NPerf
{
    using System;
    
    //concrete implementations of generic classes under test are required here because NPerf doesn't support generics 
    public class DateRBIntervalTree : Entities.IntervalTreeRB.IntervalTree<DateTime> { }
    public class DateAVLIntervalTree : Entities.IntervalTreeAVL.IntervalTree<DateTime> { }
    public class DateRangeIntervalTree : Entities.RangeTree.RangeTree<DateTime> { }
}
