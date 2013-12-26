namespace Orc.Submissions.AccountForEfficiencies.SpecialIntervalTree
{
    public class IntervalNode
  {
      public IntervalNode parent;
      public int startIdx;
      public int endIdx;

      public IntervalNode(IntervalNode parent, int startIdx, int endIdx)
      {
        this.parent   = parent;
        this.startIdx = startIdx;
        this.endIdx   = endIdx;
      }
  }
}
