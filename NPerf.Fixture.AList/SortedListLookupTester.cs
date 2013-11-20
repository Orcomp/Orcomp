namespace NPerf.Fixture.AList
{
    using System;
    using System.Collections;

    using NPerf.Framework;

    using Orc.DataStructures.AList;

    [PerfTester(typeof(IList), 10)]
    public class SortedListLookupTester
    {
        private int count;
        private readonly Random random = new Random();

        [PerfSetUp]
        public void SetUp(int index, IList list)
        {
            this.count = index * 100000;

            for (int i = 0; i < this.count; ++i)
                list.Add(this.random.Next());

            Helpers.Sort(list);
        }

        [PerfTearDown]
        public void TearDown(IList list)
        {
            list.Clear();
        }

        [PerfTest]
        public void Lookup(IList list)
        {
            if (list.Count <= 0)
            {
                return;
            }

            if (list is AListInt)
            {
                list = new IndexedAList<int>((AListInt)list);
            }

            for (int ix = 0; ix < 10000; ix++)
            {
                var it = (int)list[this.random.Next(list.Count)];

                if (list is IndexedAList<int>)
                {
                    ((IndexedAList<int>)list).IndexOf(it);
                }
                else if (list is SystemListInt)
                {
                    ((SystemListInt)list).BinarySearch(it);
                }
            }
        }

    }
}
