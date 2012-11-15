namespace Orc.Tests.IntervalContainer.NPerf
{
    using System;
    
    //concrete implementations of generic classes under test are required here because NPerf doesn't support generics 
    public class DateRBIntervalTree : Orc.Entities.IntervalTreeRB.IntervalTree<DateTime> { }
    public class DateAVLIntervalTree : Orc.Entities.IntervalTreeAVL.IntervalTree<DateTime> { }
    public class DateRangeIntervalTree : Orc.Entities.RangeTree.RangeTree<DateTime> { }
}
