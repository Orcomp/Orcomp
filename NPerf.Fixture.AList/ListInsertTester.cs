namespace NPerf.Fixture.AList
{
    using System;
    using System.Collections;

    using NPerf.Framework;

    [PerfTester(typeof(IList), 10)]
    public class ListInsertTester
    {
        private int count;
        private readonly Random random = new Random();

        [PerfSetUp]
        public void SetUp(int index, IList list)
        {
            this.count = index * 10000;
        }

        [PerfTearDown]
        public void TearDown(IList list)
        {
            list.Clear();
        }

        #region Insert Tests

        [PerfTest]
        public void InsertFirst(IList list)
        {
            for (int i = 0; i < this.count; ++i)
                list.Insert(0, this.random.Next());
        }

        [PerfTest]
        public void InsertMiddle(IList list)
        {
            for (int i = 0; i < this.count; ++i)
                list.Insert(list.Count / 2, this.random.Next());
        }

        [PerfTest]
        public void InsertLast(IList list)
        {
            for (int i = 0; i < this.count; ++i)
                list.Add(this.random.Next());
        }

        [PerfTest]
        public void InsertRandom(IList list)
        {
            for (int i = 0; i < this.count; ++i)
                list.Insert(this.random.Next(list.Count), this.random.Next());
        }

        #endregion
    }
}
