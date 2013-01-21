namespace NPerf.Fixture.ISorter
{
    using System;
    using System.Collections;

    using NPerf.Framework;

    using Orc.Algorithms.Sort.Interfaces;

    [PerfTester(typeof(ISorter), 10, Description = "Sort Algorithm benchmark", FeatureDescription = "Collection size")]
    public class ISorterPerfs
    {
        private ArrayList list;

        public int CollectionCount(int testIndex)
        {
            int n = 0;

            if (testIndex < 0)
            {
                n = 10;
            }
            else
            {
                n = (testIndex + 1) * 100;
            }

            return n;
        }

        [PerfRunDescriptor]
        public double RunDescription(int testIndex)
        {
            return (double)this.CollectionCount(testIndex);
        }

        [PerfSetUp]
        public void SetUp(int testIndex, ISorter sorter)
        {
            Random rnd = new Random();
            this.list = new ArrayList();

            for (int i = 0; i < this.CollectionCount(testIndex); ++i)
            {
                this.list.Add(rnd.Next());
            }
        }

        [PerfTest]
        public void Sort(ISorter sorter)
        {
            sorter.Sort(this.list);
        }

        [PerfTearDown]
        public void TearDown(ISorter sorter)
        {
            // checking up
            for (int i = 0; i < this.list.Count - 1; ++i)
            {
                if ((int)this.list[i] > (int)this.list[i + 1])
                {
                    throw new Exception("list not sorted");
                }
            }
        }
    }
}