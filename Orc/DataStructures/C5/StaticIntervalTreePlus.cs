namespace Orc.DataStructures.C5
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using SCG = System.Collections.Generic;

    using Orc.Interval;
    using Orc.Interval.Interface;

    using global::C5;

    //TODO: Find another class for these methods
    public static class Utils
    {
        public static void Shuffle<T>(SCG.IList<T> list)
        {
            var random = new Random();
            var n = list.Count;
            while (--n > 0)
                list.Swap(random.Next(n + 1), n);
        }

        public static void Swap<T>(this SCG.IList<T> list, int i, int j)
        {
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        public static bool IsEmpty<T>(this SCG.IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static bool IsEmpty<T>(this SCG.IList<T> collection)
        {
            return collection == null || !collection.Any();
        }
    }


    // TODO: Duplicates?
    public class StaticIntervalTreePlus<T> : IIntervalContainer<T>, IEnumerable<IInterval<T>> where T : IComparable<T>
    {
        private  Node _root;
        private IInterval<T> _span;

        private System.Collections.Generic.IList<IInterval<T>> _intervals;
        private bool _isInSync;

        #region Node nested classes

        class Node
        {
            internal T Key { get; private set; }
            // Left and right subtree
            internal Node Left { get; private set; }
            internal Node Right { get; private set; }
            // Left and right list of intersecting intervals for Key
            internal ListNode LeftList { get; private set; }
            internal ListNode RightList { get; private set; }

            internal Node(IInterval<T>[] intervals, ref IInterval<T> span)
            {
                Key = getKey(intervals);

                SCG.IList<IInterval<T>>
                    keyIntersections = new ArrayList<IInterval<T>>(),
                    lefts = new ArrayList<IInterval<T>>(),
                    rights = new ArrayList<IInterval<T>>();


                // Compute I_mid and construct two sorted lists, LeftList and RightList
                // Divide intervals according to intersection with key
                foreach (var I in intervals)
                {
                    if (I.Max.Value.CompareTo(Key) < 0)
                        lefts.Add(I);
                    else if (Key.CompareTo(I.Min.Value) < 0)
                        rights.Add(I);
                    else
                        keyIntersections.Add(I);
                }

                // Sort intersecting intervals by Low and High
                var leftList = keyIntersections.OrderBy(I => I.Min.Value).ThenBy(I => I.Min.IsInclusive ? -1 : 1);
                var rightList = keyIntersections.OrderByDescending(I => I.Max.Value).ThenByDescending(I => I.Max.IsInclusive ? 1 : -1);

                // Create left list
                ListNode previous = null;
                foreach (var interval in leftList)
                {
                    var node = new ListNode(interval);

                    if (previous != null)
                        previous.Next = node;
                    else
                        LeftList = node;

                    previous = node;
                }

                // Create right list
                previous = null;
                foreach (var interval in rightList)
                {
                    var node = new ListNode(interval);

                    if (previous != null)
                        previous.Next = node;
                    else
                        RightList = node;

                    previous = node;
                }


                // Set span
                var lowestInterval = LeftList.Interval;
                var lowCompare = lowestInterval.Min.Value.CompareTo(span.Min.Value);
                // If the left most interval in the node has a lower Low than the current _span, update _span
                if (lowCompare < 0 || (lowCompare == 0 && !span.Min.IsInclusive && lowestInterval.Min.IsInclusive))
                    span = new Interval<T>(lowestInterval.Min.Value, span.Max.Value);

                // Set span
                var highestInterval = RightList.Interval;
                var highCompare = span.Max.Value.CompareTo(highestInterval.Max.Value);
                // If the right most interval in the node has a higher High than the current Span, update Span
                if (highCompare < 0 || (highCompare == 0 && !span.Max.IsInclusive && highestInterval.Max.IsInclusive))
                    span = new Interval<T>(span.Min.Value, highestInterval.Max.Value);


                // Construct interval tree recursively for Left and Right subtrees
                if (!lefts.IsEmpty())
                    Left = new Node(lefts.ToArray(), ref span);

                if (!rights.IsEmpty())
                    Right = new Node(rights.ToArray(), ref span);
            }
        }

        class ListNode
        {
            public ListNode Next { get; internal set; }
            public IInterval<T> Interval { get; private set; }

            public ListNode(IInterval<T> interval)
            {
                Interval = interval;
            }
        }

        #endregion

        #region Constructors

        public StaticIntervalTreePlus(SCG.IEnumerable<IInterval<T>> intervals)
        {
            this._intervals = intervals.ToList();
            this._isInSync = false;

           this.BuildTree();
        }

        private void BuildTree()
        {
            var intervalArray = _intervals as IInterval<T>[] ?? _intervals.ToArray();

            if (!intervalArray.IsEmpty())
            {
                Count = intervalArray.Count();

                IInterval<T> span = new Interval<T>(intervalArray.First().Min.Value, intervalArray.First().Max.Value);

                // TODO: Figure out how Orcomp does it
                // MaximumOverlap = IntervaledHelper<T>.MaximumOverlap(intervalArray);

                _root = new Node(intervalArray, ref span);

                Span = span;
            }
        }


    #region Median

        private static T getKey(IInterval<T>[] list)
        {
            SCG.IList<T> endpoints = new ArrayList<T>();

            foreach (var interval in list)
            {
                // Add both endpoints
                endpoints.Add(interval.Min.Value);
                endpoints.Add(interval.Max.Value);
            }

            return getK(endpoints, list.Count() - 1);
        }

        private static T getK(SCG.IList<T> list, int k)
        {
            Utils.Shuffle(list);

            int low = 0, high = list.Count - 1;

            while (high > low)
            {
                var j = partition(list, low, high);

                if (j > k)
                    high = j - 1;
                else if (j < k)
                    low = j + 1;
                else
                    return list[k];
            }

            return list[k];
        }


        private static int partition(SCG.IList<T> list, int low, int high)
        {
            int i = low, j = high + 1;
            var v = list[low];

            while (true)
            {
                while (list[++i].CompareTo(v) < 0)
                    if (i == high)
                        break;
                while (v.CompareTo(list[--j]) < 0)
                    if (j == low)
                        break;
                if (i >= j)
                    break;

                list.Swap(i, j);
            }

            list.Swap(low, j);

            return j;
        }

        #endregion

        #endregion

        #region IEnumerable

        public int CountOverlaps(IInterval<T> query)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public SCG.IEnumerator<IInterval<T>> GetEnumerator()
        {
            return getEnumerator(_root);
        }

        private SCG.IEnumerator<IInterval<T>> getEnumerator(Node node)
        {
            // Just return if tree is empty
            if (node == null) yield break;

            // Recursively retrieve intervals in left subtree
            if (node.Left != null)
            {
                SCG.IEnumerator<IInterval<T>> child = getEnumerator(node.Left);

                while (child.MoveNext())
                {
                    yield return child.Current;
                }
            }

            // Go through all intervals in the node
            var n = node.LeftList;
            while (n != null)
            {
                yield return n.Interval;
                n = n.Next;
            }

            // Recursively retrieve intervals in right subtree
            if (node.Right != null)
            {
                SCG.IEnumerator<IInterval<T>> child = getEnumerator(node.Right);

                while (child.MoveNext())
                {
                    yield return child.Current;
                }
            }

        }

        #endregion

        #region Formatting

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Print the tree structure in Graphviz format
        /// </summary>
        /// <returns></returns>
        public string Graphviz()
        {
            return "digraph StaticIntervalTree {\n"
                + "\troot [shape=plaintext,label=\"Root\"];\n"
                + graphviz(_root, "root")
                + "}\n";
        }

        private int nodeCounter;
        private int nullCounter;
        private string graphviz(Node root, string parent)
        {
            // Leaf
            int id;
            if (root == null || root.Left == null)
            {
                id = nullCounter++;
                return "\tleaf" + id + "[shape=point];\n"
                    + "\t" + parent + " -> leaf" + id + ";\n";
            }

            id = nodeCounter++;
            return "\tnode" + id + " [label=\"" + root.Key + "\"];\n"
                + "\t" + parent + " -> node" + id + ";\n"
                + graphviz(root.Left, "node" + id)
                + "\tnode" + id + "left [shape=plaintext, label=\"" + graphvizList(root.LeftList, false) + "\"];\n"
                + "\tnode" + id + " -> node" + id + "left;\n"
                + "\tnode" + id + "right [shape=plaintext, label=\"" + graphvizList(root.RightList, true) + "\"];\n"
                + "\tnode" + id + " -> node" + id + "right;\n"
                + graphviz(root.Right, "node" + id);
        }

        private string graphvizList(ListNode root, bool revert)
        {
            var node = root;
            var s = new ArrayList<string>();

            while (node != null)
            {
                s.Add(node.Interval.ToString());
                node = node.Next;
            }

            if (revert)
                s.Reverse();

            return String.Join(", ", s.ToArray());
        }

        #region IShowable

        public bool Show(StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region ICollectionValue

        #region Events

        // The structure is static and has therefore no meaningful events
        public EventTypeEnum ListenableEvents { get { return EventTypeEnum.None; } }
        public EventTypeEnum ActiveEvents { get { return EventTypeEnum.None; } }

        public event CollectionChangedHandler<IInterval<T>> CollectionChanged;
        public event CollectionClearedHandler<IInterval<T>> CollectionCleared;
        public event ItemsAddedHandler<IInterval<T>> ItemsAdded;
        public event ItemInsertedHandler<IInterval<T>> ItemInserted;
        public event ItemsRemovedHandler<IInterval<T>> ItemsRemoved;
        public event ItemRemovedAtHandler<IInterval<T>> ItemRemovedAt;

        #endregion

        public bool IsEmpty { get { return _root == null; } }
        public int Count { get; private set; }
        public Speed CountSpeed { get { return Speed.Constant; } }

        public void CopyTo(IInterval<T>[] array, int index)
        {
            if (index < 0 || index + Count > array.Length)
                throw new ArgumentOutOfRangeException();

            foreach (var item in this) array[index++] = item;
        }

        public IInterval<T>[] ToArray()
        {
            var res = new IInterval<T>[Count];
            int i = 0;

            foreach (var item in this) res[i++] = item;

            return res;
        }

        public void Apply(Action<IInterval<T>> action)
        {
            foreach (var item in this)
                action(item);
        }

        public bool Exists(Func<IInterval<T>, bool> predicate)
        {
            return this.Any(predicate);
        }

        public bool Find(Func<IInterval<T>, bool> predicate, out IInterval<T> item)
        {
            foreach (var jtem in this.Where(predicate))
            {
                item = jtem;
                return true;
            }
            item = default(IInterval<T>);
            return false;
        }

        public bool All(Func<IInterval<T>, bool> predicate)
        {
            return Enumerable.All(this, predicate);
        }

        public IInterval<T> Choose()
        {
            if (_root == null)
                throw new NoSuchItemException();

            return _root.LeftList.Interval;
        }

        public SCG.IEnumerable<IInterval<T>> Filter(Func<IInterval<T>, bool> filter)
        {
            return this.Where(filter);
        }

        #endregion

        #region IIntervaled

        public SCG.IEnumerable<IInterval<T>> FindOverlaps(T query)
        {
            if (ReferenceEquals(query, null))
                return Enumerable.Empty<IInterval<T>>();

            return findOverlap(_root, query);
        }

        private SCG.IEnumerable<IInterval<T>> findOverlap(Node root, T query)
        {
            // Don't search empty leaves
            if (root == null) yield break;

            // If query matches root key, we just yield all intervals in root and stop our search
            if (query.CompareTo(root.Key) == 0)
            {
                // yield all elements in lists
                var currentListNode = root.LeftList;
                while (currentListNode != null
                    && !(currentListNode.Interval.Min.Value.CompareTo(query) == 0 && !currentListNode.Interval.Min.IsInclusive)
                    && !(currentListNode.Interval.Max.Value.CompareTo(query) == 0 && !currentListNode.Interval.Max.IsInclusive))
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }
            }
            // If query comes before root key, we go through LeftList to find all intervals with a Low smaller than our query
            else if (query.CompareTo(root.Key) < 0)
            {
                var currentListNode = root.LeftList;
                while (currentListNode != null
                    // Low is before query
                    && (currentListNode.Interval.Min.Value.CompareTo(query) < 0
                    // Low is equal to query and Low is included
                    || (currentListNode.Interval.Min.Value.CompareTo(query) == 0 && currentListNode.Interval.Min.IsInclusive)))
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // Recurse Left
                foreach (var interval in findOverlap(root.Left, query))
                {
                    yield return interval;
                }
            }
            else if (root.Key.CompareTo(query) < 0)
            {
                var currentListNode = root.RightList;
                while (currentListNode != null
                    // High is after query
                    && (query.CompareTo(currentListNode.Interval.Max.Value) < 0
                    // High is equal to query and High is included
                    || (currentListNode.Interval.Max.Value.CompareTo(query) == 0 && currentListNode.Interval.Max.IsInclusive)))
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // Recurse Right
                foreach (var interval in findOverlap(root.Right, query))
                {
                    yield return interval;
                }
            }
        }

        public SCG.IEnumerable<IInterval<T>> FindOverlaps(IInterval<T> query)
        {
            if (ReferenceEquals(query, null))
                yield break;

            // Break if collection is empty or the query is outside the collections span
            if (IsEmpty || !Span.Overlaps(query))
                yield break;

            var splitNode = _root;
            // Use a lambda instead of out, as out or ref isn't allowed for itorators
            foreach (var interval in findSplitNode(_root, query, n => { splitNode = n; }))
                yield return interval;

            // Find all intersecting intervals in left subtree
            foreach (var interval in findLeft(splitNode.Left, query))
                yield return interval;

            // Find all intersecting intervals in right subtree
            foreach (var interval in findRight(splitNode.Right, query))
                yield return interval;
        }

        public bool OverlapExists(IInterval<T> query)
        {
            if (query == null)
                return false;

            return FindOverlaps(query).GetEnumerator().MoveNext();
        }

        public int MaximumOverlap { get; private set; }

        /// <summary>
        /// Create an enumerable, enumerating all intersecting intervals on the path to the split node. Returns the split node in splitNode.
        /// </summary>
        private SCG.IEnumerable<IInterval<T>> findSplitNode(Node root, IInterval<T> query, Action<Node> splitNode)
        {
            if (root == null) yield break;

            splitNode(root);

            // Interval is lower than root, go left
            if (query.Max.Value.CompareTo(root.Key) < 0)
            {
                var currentListNode = root.LeftList;
                while (currentListNode != null && query.Overlaps(currentListNode.Interval))
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // Recursively travese left subtree
                foreach (var interval in findSplitNode(root.Left, query, splitNode))
                    yield return interval;
            }
            // Interval is higher than root, go right
            else if (root.Key.CompareTo(query.Min.Value) < 0)
            {
                var currentListNode = root.RightList;
                while (currentListNode != null && query.Overlaps(currentListNode.Interval))
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // Recursively travese right subtree
                foreach (var interval in findSplitNode(root.Right, query, splitNode))
                    yield return interval;
            }
            // Otherwise add overlapping nodes in split node
            else
            {
                var node = root.LeftList;

                while (node != null)
                {
                    // TODO: A better way to go through them? What if query is [a:b] and splitnode is b, but all intervals are (b:c]?
                    if (query.Overlaps(node.Interval))
                        yield return node.Interval;

                    node = node.Next;
                }
            }
        }

        private SCG.IEnumerable<IInterval<T>> findLeft(Node root, IInterval<T> query)
        {
            // If root is null we have reached the end
            if (root == null) yield break;

            //
            if (root.Key.CompareTo(query.Min.Value) < 0)
            {
                // Add all intersecting intervals from right list
                var currentListNode = root.RightList;
                while (currentListNode != null && query.Overlaps(currentListNode.Interval))
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // Recursively travese right subtree
                foreach (var interval in findLeft(root.Right, query))
                {
                    yield return interval;
                }
            }
            //
            else if (query.Min.Value.CompareTo(root.Key) < 0)
            {
                // As our query interval contains the interval [root.Key:splitNode]
                // all intervals in root can be returned without any checks
                var currentListNode = root.RightList;
                while (currentListNode != null)
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // Recursively add all intervals in right subtree as they must be
                // contained by [root.Key:splitNode]
                SCG.IEnumerator<IInterval<T>> child = getEnumerator(root.Right);
                while (child.MoveNext())
                {
                    yield return child.Current;
                }

                // Recursively travese left subtree
                foreach (var interval in findLeft(root.Left, query))
                {
                    yield return interval;
                }
            }
            else
            {
                // Add all intersecting intervals from right list
                var currentListNode = root.RightList;
                while (currentListNode != null && query.Overlaps(currentListNode.Interval))
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // If we find the matching node, we can add everything in the left subtree
                SCG.IEnumerator<IInterval<T>> child = getEnumerator(root.Right);
                while (child.MoveNext())
                {
                    yield return child.Current;
                }
            }
        }

        private SCG.IEnumerable<IInterval<T>> findRight(Node root, IInterval<T> query)
        {
            // If root is null we have reached the end
            if (root == null) yield break;

            //
            if (query.Max.Value.CompareTo(root.Key) < 0)
            {
                // Add all intersecting intervals from left list
                var currentListNode = root.LeftList;
                while (currentListNode != null && query.Overlaps(currentListNode.Interval))
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // Otherwise Recursively travese left subtree
                foreach (var interval in findRight(root.Left, query))
                {
                    yield return interval;
                }
            }
            //
            else if (root.Key.CompareTo(query.Max.Value) < 0)
            {
                // As our query interval contains the interval [root.Key:splitNode]
                // all intervals in root can be returned without any checks
                var currentListNode = root.RightList;
                while (currentListNode != null)
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // Recursively add all intervals in right subtree as they must be
                // contained by [root.Key:splitNode]
                SCG.IEnumerator<IInterval<T>> child = getEnumerator(root.Left);
                while (child.MoveNext())
                {
                    yield return child.Current;
                }

                // Recursively travese left subtree
                foreach (var interval in findRight(root.Right, query))
                {
                    yield return interval;
                }
            }
            else
            {
                // Add all intersecting intervals from left list
                var currentListNode = root.LeftList;
                while (currentListNode != null && query.Overlaps(currentListNode.Interval))
                {
                    yield return currentListNode.Interval;
                    currentListNode = currentListNode.Next;
                }

                // If we find the matching node, we can add everything in the left subtree
                SCG.IEnumerator<IInterval<T>> child = getEnumerator(root.Left);
                while (child.MoveNext())
                {
                    yield return child.Current;
                }
            }
        }

        public IInterval<T> Span
        {
            get
            {
                if (_span == null)
                    throw new InvalidOperationException("An empty collection has no span");

                return _span;
            }

            private set { _span = value; }
        }

        public Speed SpanSpeed { get { return Speed.Constant; } }

        #endregion

        public void Add(IInterval<T> interval)
        {
            if (interval == null)
            {
                throw new ArgumentNullException();
            }

            this._isInSync = false;

            this._intervals.Add(interval);
        }

        public void Remove(IInterval<T> interval)
        {
            this._isInSync = false;

            this._intervals.Remove(interval);
        }

        public System.Collections.Generic.IEnumerable<IInterval<T>> Query(IInterval<T> interval)
        {
            if (interval == null)
            {
                return Enumerable.Empty<IInterval<T>>();
            }

            if (!this._isInSync)
                this.BuildTree();

            return this.FindOverlaps(interval);
        }

        public System.Collections.Generic.IEnumerable<IInterval<T>> Query(T value)
        {
            if (!this._isInSync)
                this.BuildTree();

            return this.FindOverlaps(value);
        }
    }
}
