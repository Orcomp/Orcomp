using System;
using System.Linq;
using System.Collections;
using SCG = System.Collections.Generic;
using System.Text;
using C5;

namespace Orc.DataStructures.C5.IntervalStaticTree
{
    using Orc.Interval;
    using Orc.Interval.Interface;

    public static class Utils
    {
        public static bool IsEmpty<T>(this SCG.IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }
    }

    // TODO: Duplicates?
    public class StaticIntervalTree<T> : IIntervalContainer<T>, SCG.IEnumerable<IInterval<T>> where T : IComparable<T>
    {
        private Node _root;
        // TODO: Does it make sence that _span is static? What if we have more than one collection?!
        private IInterval<T> _span;

        #region Node nested classes

        class Node
        {
            public T Key { get; private set; }
            // Left and right subtree
            public Node Left { get; private set; }
            public Node Right { get; private set; }
            // Left and right list of intersecting intervals for Key
            public ListNode LeftList { get; private set; }
            public ListNode RightList { get; private set; }

            public Node(SCG.IEnumerable<IInterval<T>> intervals, StaticIntervalTree<T> parent)
            {
                // If interval set is empty return empty leaf
                if (intervals.IsEmpty())
                    return;

                Key = GetKey(intervals);

                IList<IInterval<T>>
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
                if (LeftList != null)
                {
                    var interval = LeftList.Interval;
                    // If the left most interval in the node has a lower Low than the current _span, update _span
                    if (interval.Min.Value.CompareTo(parent.Span.Min.Value) < 0
                        || (interval.Min.Value.CompareTo(parent.Span.Min.Value) == 0 && !parent.Span.Min.IsInclusive && interval.Min.IsInclusive))
                    {
                        parent.Span = new Orc.Interval.Interval<T>(interval.Min.Value, parent.Span.Max.Value, interval.Min.IsInclusive, parent.Span.Max.IsInclusive);
                    }
                }

                // Set span
                if (RightList != null)
                {
                    var interval = RightList.Interval;
                    // If the right most interval in the node has a higher High than the current Span, update Span
                    if (parent.Span.Max.Value.CompareTo(interval.Max.Value) < 0
                        || (parent.Span.Max.Value.CompareTo(interval.Max.Value) == 0 && !parent.Span.Max.IsInclusive && interval.Max.IsInclusive))
                    {
                        parent.Span = new Orc.Interval.Interval<T>(parent.Span.Min.Value, interval.Max.Value, parent.Span.Min.IsInclusive, interval.Max.IsInclusive);
                    }
                }

                // Construct interval tree recursively for Left and Right subtrees
                Left = new Node(lefts, parent);
                Right = new Node(rights, parent);
            }
        }

        class ListNode
        {
            public ListNode Next { get; set; }
            public IInterval<T> Interval { get; private set; }

            public ListNode(IInterval<T> interval)
            {
                Interval = interval;
            }
        }

        #endregion

        #region Constructors


        private SCG.IList<IInterval<T>> _intervals;
        private bool _isInSync;

        public StaticIntervalTree()
        {
            _intervals = new SCG.List<IInterval<T>>();
            _isInSync = false;
        }

        /// <summary>
        /// Create a Nested Containment List with a enumerable of intervals
        /// </summary>
        /// <param name="intervals">A collection of intervals in arbitrary order</param>
        public StaticIntervalTree(SCG.IEnumerable<IInterval<T>> intervals)
        {
            _intervals = intervals == null ? new SCG.List<IInterval<T>>() : intervals.ToList();
            _isInSync = false;

            this.BuildTree();
        }

        private void BuildTree()
        {
            _root = null;
            _span = null;

            if (!_intervals.IsEmpty())
            {
                // TODO: Counted twice as it is also counted inside Node() constructor
                Count = _intervals.Count();

                var i = _intervals.First();
                Span = new Interval<T>(i.Min.Value, i.Max.Value, i.Min.IsInclusive, i.Max.IsInclusive);
            }

            // TODO: Fix span - sample100
            _root = new Node(_intervals, this);

            _isInSync = true;
        }

        #region Median

        private static T GetKey(SCG.IEnumerable<IInterval<T>> list)
        {
            IList<T> endpoints = new ArrayList<T>();

            var n = 0;
            foreach (var interval in list)
            {
                // Count intervals to avoid going through collection too many times
                n++;

                // Add both endpoints
                endpoints.Add(interval.Min.Value);
                endpoints.Add(interval.Max.Value);
            }

            return GetK(endpoints, n - 1);
        }

        private static T GetK(IList<T> list, int k)
        {
            if (k < 0 || list.Count < k)
                throw new ArgumentOutOfRangeException();

            list = scramble(list);
            int low = 0, high = list.Count - 1;

            while (high > low)
            {
                int j = partition(list, low, high);

                if (j > k)
                    high = j - 1;
                else if (j < k)
                    low = j + 1;
                else
                    return list[k];
            }

            return list[k];
        }

        private static IList<T> scramble(IList<T> endpoints)
        {
            var random = new Random();
            var n = endpoints.Count;
            for (var i = 0; i < n; i++)
            {
                var randomNumber = random.Next(0, n);
                swap(endpoints, randomNumber, i);
            }

            return endpoints;
        }

        private static int partition(IList<T> list, int low, int high)
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

                swap(list, i, j);
            }

            swap(list, low, j);

            return j;
        }

        private static void swap(IList<T> list, int i, int j)
        {
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        #endregion

        #endregion

        #region IEnumerable

        public int OverlapCount(IInterval<T> query)
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

        public bool IsEmpty { get { return Count == 0; } }
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
            if (Count > 0)
                return _root.LeftList.Interval;

            throw new NoSuchItemException();
        }

        public SCG.IEnumerable<IInterval<T>> Filter(Func<IInterval<T>, bool> filter)
        {
            return this.Where(filter);
        }

        #endregion

        #region IIntervaled

        public SCG.IEnumerable<IInterval<T>> Overlap(T query)
        {
            if (query == null)
                throw new NullReferenceException("Query can't be null");

            return overlap(_root, query);
        }

        private SCG.IEnumerable<IInterval<T>> overlap(Node root, T query)
        {
            // Don't search empty leaves
            if (root == null || root.LeftList == null) yield break;

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
                foreach (var interval in overlap(root.Left, query))
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
                foreach (var interval in overlap(root.Right, query))
                {
                    yield return interval;
                }
            }
        }

        public SCG.IEnumerable<IInterval<T>> Overlap(IInterval<T> query)
        {
            if (query == null)
                throw new NullReferenceException("Query can't be null");

            // Break if collection is empty
            if (IsEmpty) yield break;

            // Break if query is outside the collections span
            if (!Span.Overlaps(query))
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
                throw new NullReferenceException("Query can't be null");

            throw new NotImplementedException();
        }

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
                {
                    yield return interval;
                }
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
                {
                    yield return interval;
                }
            }
            // Otherwise add overlapping nodes in split node
            else
            {
                var node = root.LeftList;

                while (node != null)
                {
                    // TODO: A better way to go through them? What if query is [a:b] and splitnode is b, but all intervals are (b:c]?
                    if (query.Overlaps(node.Interval))
                    {
                        yield return node.Interval;
                    }

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

            _isInSync = false;

            _intervals.Add(interval);
        }

        public void Remove(IInterval<T> interval)
        {
            _isInSync = false;

            _intervals.Remove(interval);
        }

        public SCG.IEnumerable<IInterval<T>> Query(IInterval<T> interval)
        {
            if (interval == null)
            {
                return Enumerable.Empty<IInterval<T>>();
            }

            if (!this._isInSync)
                this.BuildTree();

            return this.Overlap(interval);
        }

        public SCG.IEnumerable<IInterval<T>> Query(T value)
        {
            if (!this._isInSync)
                this.BuildTree();

            return this.Overlap(value);
        }
    }
}
