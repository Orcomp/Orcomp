/// http://www.informit.com/guides/content.aspx?g=dotnet&seqNum=878


// TODO: Need tests for this. Make it implement IDictionary<T>

namespace Orc.Entities
{
    using System;
    using System.Collections.Generic;

    public class SkipList<T> : IEnumerable<T>
    {
        private class SortComparer : IComparer<T>
        {
            private Comparison<T> Comparer;

            public SortComparer(Comparison<T> comp)
            {
                this.Comparer = comp;
            }

            public int Compare(T x, T y)
            {
                return this.Comparer(x, y);
            }
        }

        private SkiplistNodes Nodes;

        public int Level { get; private set; }
        public int Count { get; private set; }
        private IComparer<T> Comparer;

        public const int DefaultCapacity = 1024;

        private bool _allowDupes = true;
        public bool AllowDuplicates
        {
            get { return this._allowDupes; }
            set
            {
                // Early out if setting to the existing value
                if (value == this._allowDupes)
                    return;

                // If there are multiple items in the collection and duplicates are allowed,
                // then disallow setting _allowDupes to false, because
                // there might already be duplicates.
                if (this.Count > 1 && this._allowDupes)
                {
                    throw new ApplicationException("Cannot restrict duplicates because duplicates might already exist.");
                }
                this._allowDupes = value;
            }
        }

        public SkipList(IComparer<T> comp, int initialCapacity)
        {
            this.Count = 0;
            this.Comparer = comp;
            this.Level = 0;

            // Initialize the Nodes array.
            this.Nodes = new SkiplistNodes(initialCapacity);
        }

        public SkipList(Comparison<T> comp)
            : this(new SortComparer(comp), DefaultCapacity)
        {
        }

        public SkipList()
            : this(Comparer<T>.Default, DefaultCapacity)
        {
        }

        public SkipList(int initialCap)
            : this(Comparer<T>.Default, initialCap)
        {
        }

        // Random number generator for level assignment.
        private Random SLRandom = new Random();
        const double Probability = 0.50;
        const int MaxLevel = 31;

        private int GetNextLevel()
        {
            int lvl = 0;

            // Determines the next node level.
            while (this.SLRandom.NextDouble() < Probability &&

                lvl <= this.Level && lvl < MaxLevel)
            {
                lvl++;
            }

            return lvl;
        }

        public bool Add(T val)
        {
            // Determine what level we want this node at.
            int newLevel = this.GetNextLevel();

            if (newLevel > this.Level)
            {
                this.Level = newLevel;
            }

            int[] forwards = new int[newLevel + 1];

            // Find out where the node goes,
            // keeping track of nodes that will point to it.
            var searchNode = this.Nodes.Root();
            for (int lvl = this.Level; lvl >= 0; --lvl)
            {
                for (; ; )
                {
                    var nextNode = this.Nodes.GetForwardLink(searchNode, lvl);
                    if (nextNode == SkiplistNodes.NotFound)
                    {
                        break;
                    }
                    int compRslt = this.Comparer.Compare(this.Nodes.GetValue(nextNode), val);
                    if (compRslt == 0 && !this.AllowDuplicates)
                    {
                        // It's a duplicate and duplicates aren't allowed.
                        return false;
                    }
                    if (compRslt >= 0)
                    {
                        break;
                    }
                    searchNode = nextNode;
                }
                // Save the searchNode in the forwards array so that we can update later.
                if (lvl <= newLevel)
                {
                    forwards[lvl] = searchNode;
                }
            }

            this.Nodes.Add(val, newLevel, forwards);

            ++this.Count;
            return true;
        }

        public bool Remove(T val)
        {
            T deleted;
            return this.Remove(val, out deleted);
        }

        public bool Remove(T val, out T deleted)
        {
            // find the node in each of the levels
            int[] linksToUpdate = new int[MaxLevel + 1];

            var searchNode = this.Nodes.Root();
            int foundNode = SkiplistNodes.NotFound;
            for (int lvl = this.Level; lvl >= 0; --lvl)
            {
                for (; ; )
                {
                    var nextNode = this.Nodes.GetForwardLink(searchNode, lvl);
                    if (nextNode == SkiplistNodes.NotFound)
                    {
                        // End of list at this level
                        break;
                    }
                    int compRslt = this.Comparer.Compare(this.Nodes.GetValue(nextNode), val);
                    if (compRslt == 0)
                    {
                        linksToUpdate[lvl] = searchNode;
                        if (foundNode == SkiplistNodes.NotFound)
                        {
                            foundNode = nextNode;
                        }
                        break;
                    }
                    if (compRslt > 0)
                    {
                        // No node at this level points to the node.
                        break;
                    }
                    searchNode = nextNode;
                }
            }
            if (foundNode == SkiplistNodes.NotFound)
            {
                deleted = default(T);
                return false;
            }
            return this.RemoveNodeByIndex(foundNode, linksToUpdate, out deleted);
        }

        private bool RemoveNodeByIndex(int foundNode, int[] linksToUpdate, out T deleted)
        {
            // Update the node pointers.
            deleted = this.Nodes.Remove(foundNode, linksToUpdate);
            --this.Count;
            return true;
        }

        public bool Find(T val, out T rslt)
        {
            var searchNode = this.Nodes.Root();
            for (int lvl = this.Level; lvl >= 0; --lvl)
            {
                for (; ; )
                {
                    var nextNode = this.Nodes.GetForwardLink(searchNode, lvl);
                    if (nextNode == SkiplistNodes.NotFound)
                    {
                        break;
                    }
                    T valToCheck = this.Nodes.GetValue(nextNode);
                    int compRslt = this.Comparer.Compare(valToCheck, val);
                    if (compRslt == 0)
                    {
                        rslt = valToCheck;
                        return true;
                    }
                    if (compRslt > 0)
                    {
                        break;
                    }
                    searchNode = nextNode;
                }
            }
            rslt = default(T);
            return false;
        }

        public T DeleteMin()
        {
            T rslt;
            if (!this.TryDeleteMin(out rslt))
            {
                throw new ApplicationException("The Skiplist is empty.");
            }
            return rslt;
        }

        public bool TryDeleteMin(out T rslt)
        {
            if (this.Count == 0)
            {
                rslt = default(T);
                return false;
            }

            // Get the first node in the list.
            var firstNode = this.Nodes.First();

            int nodeLevel = this.Nodes.GetLevel(firstNode);

            int[] linksToUpdate = new int[MaxLevel + 1];

            // Now unlink it from every list that it's in.
            // Since this is the first node in the list, we just have to unlink at the head.
            for (int lvl = 0; lvl <= nodeLevel; ++lvl)
            {
                linksToUpdate[lvl] = this.Nodes.Root();
            }
            return this.RemoveNodeByIndex(firstNode, linksToUpdate, out rslt);
        }

        public T Peek()
        {
            // return the first node in the list
            T rslt;
            if (!this.TryPeek(out rslt))
            {
                throw new ApplicationException("The Skiplist is empty.");
            }
            return rslt;
        }

        public bool TryPeek(out T rslt)
        {
            if (this.Count == 0)
            {
                rslt = default(T);
                return false;
            }
            var firstNode = this.Nodes.First();
            rslt = this.Nodes.GetValue(firstNode);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var iNode = this.Nodes.First();
            while (iNode != SkiplistNodes.NotFound)
            {
                yield return this.Nodes.GetValue(iNode);
                iNode = this.Nodes.GetForwardLink(iNode, 0);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class SkiplistNodes
        {
            // The Nodes array is treated like an array of variable-length structures.
            // Each node has this format:
            // struct SkiplistNode
            // {
            //    int ValueIndex;
            //    fixed int Forwards[]
            // }
            // The ValueIndex is a packed value:
            //    Bits 0-26  - Index into the Values array
            //    Bits 27-31 - The node's level (0 to 31)
            // This will allow the skiplist to hold 2^27 items.
            // The Forwards array is variable length, from 1 to 32, as defined by the Level value.
            //
            // The first 66 positions in the Nodes array are reserved for bookkeeping.
            // Nodes[0] through Nodes[32] represent a SkiplistNode of level 32, containing pointers to the individual levels' lists.
            // Nodes[33] through Nodes[64] are pointers to the free lists for level N-33.
            // Nodes[65] points to "free space" at the end of the buffer.
            //
            // If a node is in the free list, the ValueIndex is an index to the next node in the free list.
            // If the node is at the end of the free list, ValueIndex will be -1.
            //
            // The free space node is marked with a level of 0, and the ValueIndex part is the number of free slots open.
            // ValueIndex in the free space should be (array.Length - X - 1), where X is the position of the free space node.

            // SkiplistValues class abstracts value storage.
            private class SkiplistValues
            {
                private List<T> Values;
                private Stack<int> FreeList;

                public SkiplistValues(int capacity)
                {
                    // Initialize an empty values array and free list
                    this.Values = new List<T>(capacity);

                    this.FreeList = new Stack<int>(capacity);
                }

                public T this[int ix]
                {
                    get { return this.Values[ix]; }
                }

                public int Add(T val)
                {
                    int iValue;
                    // First, try to re-use a position from the free list.
                    if (this.FreeList.Count > 0)
                    {
                        iValue = this.FreeList.Pop();
                        this.Values[iValue] = val;
                    }
                    else
                    {
                        // Nothing in the free list, so take up a new position from the Values list.
                        iValue = this.Values.Count;
                        this.Values.Add(val);
                    }
                    return iValue;
                }

                public T Remove(int vindex)
                {
                    T deleted = this.Values[vindex];
                    this.Values[vindex] = default(T);
                    this.FreeList.Push(vindex);
                    return deleted;
                }
            }

            private int[] Nodes;
            private SkiplistValues Values;

            public const int NotFound = -1;

            private const int RootNode = 0;
            private const int Freelist0 = 33;
            private const int FreespaceHead = 65;

            public SkiplistNodes(int initialCapacity)
            {
                this.Nodes = new int[3 * initialCapacity];

                // Initialize root node
                this.Nodes[RootNode] = SetLevelAndIndex(MaxLevel, -1);
                for (int i = RootNode + 1; i <= RootNode + MaxLevel + 1; ++i)
                {
                    this.Nodes[i] = -1;
                }

                // Initialize free list nodes
                for (int i = Freelist0; i <= Freelist0 + MaxLevel; ++i)
                {
                    this.Nodes[i] = -1;
                }

                // Initialize free space
                int iFree = FreespaceHead + 1;
                this.Nodes[FreespaceHead] = iFree;
                this.Nodes[iFree] = this.Nodes.Length - iFree - 1;

                this.Values = new SkiplistValues(initialCapacity);
            }

            public int Root()
            {
                return RootNode;
            }

            public int First()
            {
                return this.Nodes[RootNode + 1];
            }

            public T GetValue(int inode)
            {
                return this.Values[NodeValueIndex(this.Nodes[inode])];
            }

            public int GetLevel(int inode)
            {
                return NodeLevel(this.Nodes[inode]);
            }

            public int GetForwardLink(int node, int level)
            {
                return this.Nodes[node + level + 1];
            }

            private void SetForwardLink(int node, int level, int value)
            {
                this.Nodes[node + level + 1] = value;
            }

            private static int NodeValueIndex(int v)
            {
                return v & 0x07FFFFFF;
            }

            private static int NodeLevel(int v)
            {
                return (v >> 27) & 0x1F;
            }

            private static int SetLevelAndIndex(int level, int vindex)
            {
                return (level << 27) | NodeValueIndex(vindex);
            }

            public T Remove(int iNode, int[] linksToUpdate)
            {
                // Unlink the node
                int nodeLevel = this.GetLevel(iNode);
                for (int lvl = 0; lvl <= nodeLevel; ++lvl)
                {
                    int fixNode = linksToUpdate[lvl];
                    this.SetForwardLink(fixNode, lvl, this.GetForwardLink(iNode, lvl));
                }

                // Remove the value from the values list
                int vindex = NodeValueIndex(this.Nodes[iNode]);
                T deleted = this.Values.Remove(vindex);

                // And add the node to the head of the free list for this level
                this.Nodes[iNode] = this.Nodes[Freelist0 + nodeLevel];
                this.Nodes[Freelist0 + nodeLevel] = iNode;

                return deleted;
            }

            public void Add(T val, int newLevel, int[] forwards)
            {
                // Allocate a new node for this value
                var newNode = this.Alloc(newLevel, val);

                // And link it into the lists
                for (int lvl = 0; lvl <= newLevel; ++lvl)
                {
                    this.SetForwardLink(newNode, lvl, this.GetForwardLink(forwards[lvl], lvl));
                    this.SetForwardLink(forwards[lvl], lvl, newNode);
                }
            }

            private int Alloc(int level, T val)
            {
                int iNode = -1;
                // First check the free list for this level.
                if (this.Nodes[Freelist0 + level] != -1)
                {
                    // Get the next item from the free list
                    iNode = NodeValueIndex(this.Nodes[Freelist0 + level]);
                    // And the new head of the free list is
                    // that node's Next pointer.
                    this.Nodes[Freelist0 + level] = this.Nodes[iNode];
                }
                else
                {
                    // Try to allocate from free space
                    if ((iNode = this.AllocFromFreeSpace(level)) == -1)
                    {
                        // Allocation failed.
                        // See if we can resize the array.
                        this.Resize((this.Nodes.Length * 3) / 2);
                        // And then do the allocation again.
                        iNode = this.AllocFromFreeSpace(level);
                    }
                }

                // Add to the values list and get an index
                int vindex = this.Values.Add(val);

                // Initialize the node and return its index.
                this.Nodes[iNode] = SetLevelAndIndex(level, vindex);

                return iNode;
            }

            private int AllocFromFreeSpace(int level)
            {
                // Need to allocate (level+1) items from the free space.
                int iFreeNode = this.Nodes[FreespaceHead];
                if (iFreeNode == -1 || this.Nodes[iFreeNode] <= level)
                {
                    // Not enough free space in buffer.
                    return -1;
                }

                // There's enough space.
                // Allocate it and move the free space pointer up.
                int iNode = iFreeNode;

                iFreeNode = iNode + level + 2;
                if (iFreeNode < this.Nodes.Length)
                {
                    // Store the remaining free space
                    this.Nodes[iFreeNode] = this.Nodes.Length - iFreeNode - 1;
                }
                else
                {
                    iFreeNode = -1;
                }
                this.Nodes[FreespaceHead] = iFreeNode;

                // iNode is the newly allocated node.
                return iNode;
            }

            private void Resize(int newLength)
            {
                // @NOTE: This is designed to increase the size of the array.
                // It won't check to see if the array is being made to small.

                // Need this in case the free space node is -1.
                int oldLength = this.Nodes.Length;

                Array.Resize(ref this.Nodes, newLength);

                // Adjust the freespace pointer
                int iFreeNode = this.Nodes[FreespaceHead];
                if (iFreeNode == -1)
                {
                    iFreeNode = oldLength;
                    this.Nodes[FreespaceHead] = iFreeNode;
                }
                this.Nodes[iFreeNode] = this.Nodes.Length - iFreeNode - 1;
            }
        }
    }
}