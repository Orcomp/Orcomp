namespace Orc.Tests.IntervalContainer.NPerf
{
    using System;

    using Orc.Entities;

    public abstract class DateIntervalContainerBenchmarkBase
    {
        protected readonly DateTime now = DateTime.Now;        

        protected int numberOfIntervals;

        protected Interval<DateTime> ToDateTimeInterval(DateTime startTime, int leftEdgeMinutes, int rightEdgeMinutes, bool includeEdges = true)
        {
            return ToDateTimeInterval(startTime, leftEdgeMinutes, rightEdgeMinutes, includeEdges, includeEdges);
        }

        protected Interval<DateTime> ToDateTimeInterval(DateTime startTime, int leftEdgeMinutes, int rightEdgeMinutes, bool includeLefEdge, bool includeRigthEdge)
        {
            return new Interval<DateTime>(startTime.AddMinutes(leftEdgeMinutes), startTime.AddMinutes(rightEdgeMinutes), includeLefEdge, includeRigthEdge);
        }

        protected int CollectionCount(int testIndex)
        {
            return (int) Math.Pow(20, testIndex + 1);
        }
    }
}
