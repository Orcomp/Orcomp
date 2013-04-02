namespace NPerf.Fixture.AList
{
    using System;
    using System.Collections;

    using NPerf.Framework;

    [PerfTester(typeof(IList), 10)]
    public class ListSortTester
    {
        private int count;
        private readonly Random random = new Random();

        [PerfSetUp]
        public void SetUp(int index, IList list)
        {
            this.count = index * 100000;

            for (int i = 0; i < this.count; ++i)
                list.Add(this.random.Next());
        }

        [PerfTearDown]
        public void TearDown(IList list)
        {
            list.Clear();
        }

        [PerfTest]
        public void Sort(IList list)
        {
            Helpers.Sort(list);
        }
    }
}
