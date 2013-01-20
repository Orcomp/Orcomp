namespace Orc.Entities.SpecialIntervalTree
{
  using System;
  using System.Collections.Generic;

  using Orc.Interval;

    public class IntervalTree
  {
    private readonly HashSet<long> UniqueTimePoints = new HashSet<long>();
    private readonly Dictionary<long, int> TimeToSupportIndex = new Dictionary<long, int>();
    private IntervalNode[] SupportPoints;
    private long[] EffectiveDeltaTime;
    private long[] RealTimePoints;

    public bool IsFullyCovered;


    public bool AddSupportPoint(long time)
    {
      return UniqueTimePoints.Add(time);
    }

    public int GetSupportIndexAtTime(long time)
    {
      return TimeToSupportIndex[time];
    }

    public IntervalNode GetTopNodeAtTime(long time)
    {
      return this.GetTopNodeAtIndex(TimeToSupportIndex[time]);
    }

    public IntervalNode GetTopNodeAtIndex(int idx)
    {
      IntervalNode topNode = SupportPoints[idx];
      if (topNode != null && topNode.parent != null) {
        // Get the "top" node
        IntervalNode top = topNode.parent;
        while (top.parent != null)
          top = top.parent;

        // Make the top node the parent of all nodes along the way from the "leaf"
        while (topNode.parent != top) {
          IntervalNode next = topNode.parent;
          topNode.parent = top;
          topNode = next;
        }

        topNode = top;
      }
      
      return topNode;
    }


    public void InitSupportAndMapping()
    {
      EffectiveDeltaTime = new long[UniqueTimePoints.Count];
      RealTimePoints = new long[UniqueTimePoints.Count];

      // Sort time points
      UniqueTimePoints.CopyTo(RealTimePoints);
      Array.Sort(RealTimePoints);

      // Create 'time -> support point index' mapping and integrate delta periods with 100% efficiency
      TimeToSupportIndex.Add(RealTimePoints[0], 0);
      EffectiveDeltaTime[0] = 0;
      for (int i = 1; i < RealTimePoints.Length; ++i) {
        TimeToSupportIndex.Add(RealTimePoints[i], i);                  // Create map entry
        EffectiveDeltaTime[i] = RealTimePoints[i] - RealTimePoints[i - 1]; // Integrate period from (i-1)'th to i'th time point
      }

      // Init support points array holding references to the node, "covering" the current time point
      SupportPoints = new IntervalNode[RealTimePoints.Length];
    }

    public void MergeAndIntegrate(DateIntervalEfficiency eff)
    {
      int leftIdx  = GetSupportIndexAtTime(eff.Min.Value.Ticks);
      int rightIdx = GetSupportIndexAtTime(eff.Max.Value.Ticks);

      IntervalNode leftNode  = this.GetTopNodeAtIndex(leftIdx);
      IntervalNode rightNode = this.GetTopNodeAtIndex(rightIdx);
      IntervalNode newTopNode = null;

      if (leftNode != null) {
        if (leftNode == rightNode)
          // If node is the same on both sides the current interval is fully covered
          return;

        newTopNode = new IntervalNode(null, leftNode.startIdx, rightIdx);

        // Adjust left marker
        leftIdx = leftNode.endIdx;
      }

      if (rightNode != null) {
        if (newTopNode == null) {
          newTopNode = new IntervalNode(null, leftIdx, rightNode.endIdx);
        }
        else
          newTopNode.endIdx = rightNode.endIdx;

        // Adjust right marker
        rightIdx = rightNode.startIdx;
      }

      // Create new leaf node and include the free points from 'leftIdx' to 'rightIdx'
      IntervalNode newLeafNode = new IntervalNode(null, leftIdx, rightIdx);

      // Scanning from left to right

      // Marking the start point, but skipping the integration, since it contains the ticks from the previous delta interval
      SupportPoints[leftIdx] = newLeafNode;

      // Mark empty support points inside interval range, while jumping over the already "marked" intervals
      while (++leftIdx <= rightIdx) {
        // Integrate the delta interval
        EffectiveDeltaTime[leftIdx] = (long)(EffectiveDeltaTime[leftIdx] * 0.01 * eff.Efficiency);

        IntervalNode curTopNode = this.GetTopNodeAtIndex(leftIdx);
        if (curTopNode != null)
          // Jump over existing interval
          leftIdx = curTopNode.endIdx;
        else
          // "Brand" point with the current node
          SupportPoints[leftIdx] = newLeafNode;
      }

      // Create links to new top node
      if (newTopNode != null) {
        if (leftNode != null) leftNode.parent = newTopNode;
        if (rightNode != null) rightNode.parent = newTopNode;
        newLeafNode.parent = newTopNode;

        // Check if we have the whole support range covered
        if (newTopNode.startIdx == 0 && newTopNode.endIdx == SupportPoints.Length - 1)
          IsFullyCovered = true;
      }
    }

    public long AccumulateEffectiveTime(long realTicks)
    {
      int idx = 0;
      while (++idx < EffectiveDeltaTime.Length && realTicks > 0)
        realTicks -= EffectiveDeltaTime[idx];
      --idx;

      // Check if the end is before the last processed support point time
      if (realTicks < 0) {
        double frac = -realTicks / (double)EffectiveDeltaTime[idx];
        long overTime = (long)(frac * (RealTimePoints[idx] - RealTimePoints[idx - 1]));

        return RealTimePoints[idx] - overTime - RealTimePoints[0];
      }

      return RealTimePoints[idx] + realTicks - RealTimePoints[0];
    }
  }
}
