namespace Orc.Entities.SkipList
{
    using System;

    public class ScanRange<K> : IScanner<K>
    {
        IComparable<K> lowkey, highkey;
        IComparable<K> matchtest;
        bool scan_all = false;
        public ScanRange(IComparable<K> lowkey, IComparable<K> highkey, IComparable<K> matchtest)
        { // scan between low and high key
            this.lowkey = lowkey;
            this.highkey = highkey;
            this.matchtest = matchtest;
        }

        public ScanRange(IComparable<K> matchtest)
        { // scan all
            this.lowkey = new minKey();
            this.highkey = new maxKey();
            this.scan_all = true;
        }
        public bool MatchTo(K value)
        {
            return true;
        }
        public class maxKey : IComparable<K>
        {
            public int CompareTo(K val)
            {
                return 1;
            }
        }
        public class minKey : IComparable<K>
        {
            public int CompareTo(K val)
            {
                return -1;
            }
        }
        public static ScanRange<K> All()
        {
            return new ScanRange<K>(null);
        }
        public IComparable<K> genLowestKeyTest()
        {
            if (this.scan_all)
            {
                return new minKey();
            }
            else
            {
                return this.lowkey;
            }
        }
        public IComparable<K> genHighestKeyTest()
        {
            if (this.scan_all)
            {
                return new maxKey();
            }
            else
            {
                return this.highkey;
            }
        }
    }
}