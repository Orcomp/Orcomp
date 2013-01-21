namespace Orc.Submissions.AccountForEfficiencies.SpecialIntervalTree
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
      return this.UniqueTimePoints.Add(time);
    }

    public int GetSupportIndexAtTime(long time)
    {
      return this.TimeToSupportIndex[time];
    }

    public IntervalNode GetTopNodeAtTime(long time)
    {
      return this.GetTopNodeAtIndex(this.TimeToSupportIndex[time]);
    }

    public IntervalNode GetTopNodeAtIndex(int idx)
    {
      IntervalNode topNode = this.SupportPoints[idx];
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
      this.EffectiveDeltaTime = new long[this.UniqueTimePoints.Count];
      this.RealTimePoints = new long[this.UniqueTimePoints.Count];

      // Sort time points
      this.UniqueTimePoints.CopyTo(this.RealTimePoints);
      Array.Sort(this.RealTimePoints);

      // Create 'time -> support point index' mapping and integrate delta periods with 100% efficiency
      this.TimeToSupportIndex.Add(this.RealTimePoints[0], 0);
      this.EffectiveDeltaTime[0] = 0;
      for (int i = 1; i < this.RealTimePoints.Length; ++i) {
        this.TimeToSupportIndex.Add(this.RealTimePoints[i], i);                  // Create map entry
        this.EffectiveDeltaTime[i] = this.RealTimePoints[i] - this.RealTimePoints[i - 1]; // Integrate period from (i-1)'th to i'th time point
      }

      // Init support points array holding references to the node, "covering" the current time point
      this.SupportPoints = new IntervalNode[this.RealTimePoints.Length];
    }

    public void MergeAndIntegrate(DateIntervalEfficiency eff)
    {
      int leftIdx  = this.GetSupportIndexAtTime(eff.Min.Value.Ticks);
      int rightIdx = this.GetSupportIndexAtTime(eff.Max.Value.Ticks);

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
      this.SupportPoints[leftIdx] = newLeafNode;

      // Mark empty support points inside interval range, while jumping over the already "marked" intervals
      while (++leftIdx <= rightIdx) {
        // Integrate the delta interval
        this.EffectiveDeltaTime[leftIdx] = (long)(this.EffectiveDeltaTime[leftIdx] * 0.01 * eff.Efficiency);

        IntervalNode curTopNode = this.GetTopNodeAtIndex(leftIdx);
        if (curTopNode != null)
          // Jump over existing interval
          leftIdx = curTopNode.endIdx;
        else
          // "Brand" point with the current node
          this.SupportPoints[leftIdx] = newLeafNode;
      }

      // Create links to new top node
      if (newTopNode != null) {
        if (leftNode != null) leftNode.parent = newTopNode;
        if (rightNode != null) rightNode.parent = newTopNode;
        newLeafNode.parent = newTopNode;

        // Check if we have the whole support range covered
        if (newTopNode.startIdx == 0 && newTopNode.endIdx == this.SupportPoints.Length - 1)
          this.IsFullyCovered = true;
      }
    }

    public long AccumulateEffectiveTime(long realTicks)
    {
      int idx = 0;
      while (++idx < this.EffectiveDeltaTime.Length && realTicks > 0)
        realTicks -= this.EffectiveDeltaTime[idx];
      --idx;

      // Check if the end is before the last processed support point time
      if (realTicks < 0) {
        double frac = -realTicks / (double)this.EffectiveDeltaTime[idx];
        long overTime = (long)(frac * (this.RealTimePoints[idx] - this.RealTimePoints[idx - 1]));

        return this.RealTimePoints[idx] - overTime - this.RealTimePoints[0];
      }

      return this.RealTimePoints[idx] + realTicks - this.RealTimePoints[0];
    }
  }
}
