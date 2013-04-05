using System;
using System.Linq;
using System.Text;
using SCG = System.Collections.Generic;
using C5;

namespace Orc.DataStructures.IntervalNCList
{
    using System.Collections;

    using Orc.DataStructures.C5;
    using Orc.Interval;
    using Orc.Interval.Interface;

    public class NestedContainmentList<T> : IIntervalContainer<T>, SCG.IEnumerable<IInterval<T>>
        where T : struct, IComparable<T>
    {
        private IList<Node> _list;
        private IInterval<T> _span;

        private SCG.IList<IInterval<T>> _intervals;
        private bool _isInSync;

        #region Node nested classes

        struct Node
        {
            internal IInterval<T> Interval { get; private set; }
            internal IList<Node> Sublist { get; private set; }
            internal int NodesBefore { get; private set; }
            internal int NodesInSublist { get; private set; }

            internal Node(IInterval<T> interval, IList<Node> sublist, int nodesBefore, int nodesInSublist)
                : this()
            {
                Interval = interval;

                if (sublist != null && !sublist.IsEmpty)
                    Sublist = sublist;

                NodesBefore = nodesBefore;
                NodesInSublist = nodesInSublist;
            }
        }

        #endregion

        #region constructors

        /// <summary>
        /// A sorted list of IInterval&lt;T&gt; sorted with IntervalComparer&lt;T&gt;
        /// </summary>
        /// <param name="intervals">Sorted intervals</param>
        /// <returns>A list of nodes</returns>
        private static IList<Node> createList(SCG.IEnumerable<IInterval<T>> intervals)
        {
            // TODO: Make an in-place version
            // List to hold the nodes
            IList<Node> list = new ArrayList<Node>(); // TODO: Null and then init in if?
            // Remember the number of nodes before the current node to allow fast count operation
            var nodesBefore = 0;

            // Iterate through the intervals to build the list
            var enumerator = intervals.GetEnumerator();
            if (enumerator.MoveNext())
            {
                // Remember the previous node so we can check if the next nodes are contained in it
                var previous = enumerator.Current;
                IList<IInterval<T>> sublist = new ArrayList<IInterval<T>>();

                // Loop through intervals
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;

                    if (previous.Contains(current))
                        // Add contained intervals to sublist for previous node
                        sublist.Add(current);
                    else
                    {
                        // The current interval wasn't contained in the prevoius node, so we can finally add the previous to the list
                        list.Add(new Node(previous, createList(sublist), nodesBefore, sublist.Count));
                        // Add the sublist count and the current node to the nodes before
                        nodesBefore += sublist.Count + 1;

                        // Reset the looped values
                        sublist = new ArrayList<IInterval<T>>();
                        previous = current;
                    }
                }

                // TODO: Remove this if statement when we are sure
                if (previous == null)
                    throw new Exception("This shouldn't happen, right?");

                // Add the last node to the list when we are done looping through them
                list.Add(new Node(previous, createList(sublist), nodesBefore, sublist.Count));
            }

            return list;
        }

        public NestedContainmentList()
        {
            _intervals = new SCG.List<IInterval<T>>();
            _isInSync = false;
        }

        /// <summary>
        /// Create a Nested Containment List with a enumerable of intervals
        /// </summary>
        /// <param name="intervals">A collection of intervals in arbitrary order</param>
        public NestedContainmentList(SCG.IEnumerable<IInterval<T>> intervals)
        {
            _intervals = intervals.ToList();
            _isInSync = false;

            this.BuildTree();
        }

        private void BuildTree()
        {
            _list = null;
            _span = null;

            if (_intervals == null || !_intervals.Any())
                return;

            // Sort intervals using IntervalComparer
            var sortedIntervals = new TreeBag<IInterval<T>>();

            foreach (var interval in _intervals)
                sortedIntervals.Add(interval);

            // Build nested containment list recursively and save the upper-most list in the class
            _list = createList(sortedIntervals);

            // Use count from sorted intervals (constant speed)
            Count = sortedIntervals.Count;

            // Save span to allow for constant speeds on later requests
            IInterval<T> i = _list.First.Interval, j = _list.Last.Interval;
            Span = new Interval<T>(i.Min.Value, i.Max.Value, i.Min.IsInclusive, i.Max.IsInclusive);

            _isInSync = true;
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // TODO: Test the order is still the same as when sorted with IntervalComparer. This should be that case!
        /// <summary>
        /// Create an enumerator, enumerating the intervals in sorted order - sorted on low endpoint with shortest intervals first
        /// </summary>
        /// <returns>Enumerator</returns>
        // TODO: Test the order is still the same as when sorted with IntervalComparer. This should be that case!
        public SCG.IEnumerator<IInterval<T>> GetEnumerator()
        {
            return getEnumerator(_list);
        }

        private SCG.IEnumerator<IInterval<T>> getEnumerator(SCG.IEnumerable<Node> list)
        {
            // Just for good measures
            if (list == null)
                yield break;

            foreach (var node in list)
            {
                // Yield the interval itself before the sublist to maintain sorting order
                yield return node.Interval;

                if (node.Sublist != null)
                {
                    var child = getEnumerator(node.Sublist);

                    while (child.MoveNext())
                        yield return child.Current;
                }
            }
        }

        #endregion

        #region Formatting

        public string ToString(string format, IFormatProvider formatProvider)
        {
            // TODO: Correct implementation?
            return _list.ToString();
        }

        #region IShowable

        public bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region ICollectionValue

        #region Events

        // The structure is static and has therefore no meaningful events
        public SCG.IEnumerable<IInterval<T>> Filter(Fun<IInterval<T>, bool> filter)
        {
            throw new NotImplementedException();
        }

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

            foreach (var item in this)
                array[index++] = item;
        }

        public IInterval<T>[] ToArray()
        {
            var res = new IInterval<T>[Count];
            var i = 0;

            foreach (var item in this)
                res[i++] = item;

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
                return _list.First.Interval;

            throw new NoSuchItemException();
        }

        public SCG.IEnumerable<IInterval<T>> Filter(Func<IInterval<T>, bool> filter)
        {
            return this.Where(filter);
        }

        #endregion

        #region IIntervaled

        public IInterval<T> Span
        {
            get
            {
                // TODO: Use a better exception? Return null for empty collection?
                if (IsEmpty)
                    throw new InvalidOperationException("An empty collection has no span");

                return _span;
            }

            private set { _span = value; }
        }

        public Speed SpanSpeed { get { return Speed.Constant; } }

        public SCG.IEnumerable<IInterval<T>> Overlap(T query)
        {

            return overlap(_list, new Interval<T>(query, query));
        }

        // TODO: Test speed difference between version that takes overlap-loop and upper and low bound loop
        private static SCG.IEnumerable<IInterval<T>> overlap(IList<Node> list, IInterval<T> query)
        {
            if (list == null)
                yield break;

            var first = searchHighInLows(list, query);
            var last = searchLowInHighs(list, query);

            if (first < 0 || last < 0 || list.Count - 1 < first || list.Count - 1 < last)
                yield break;

            while (first <= last)
            {
                yield return list[first].Interval;

                // TODO: Don't do checks on sublist if interval is contained in query
                if (list[first].Sublist != null)
                    foreach (var interval in overlap(list[first].Sublist, query))
                        yield return interval;

                first++;
            }
        }

        /// <summary>
        /// Do a binary search for the high endpoint of the query interval in the collections low endpoints
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static int searchHighInLows(IList<Node> list, IInterval<T> query)
        {
            if (query == null || list.Count == 0)
                return -1;

            int min = 0, max = list.Count - 1;

            while (min <= max)
            {
                var middle = (max + min) / 2;

                if (query.Overlaps(list[middle].Interval))
                {
                    // Only if the previous interval to the overlapping interval does not overlap, we have found the right one
                    if (middle == 0 || !query.Overlaps(list[middle - 1].Interval))
                        // The right interval is found
                        return middle;

                    // The previous interval overlap as well, move left
                    max = middle - 1;
                }
                else
                {
                    // TODO: Check if this is the same error as in searchLowInHighs
                    // The interval does not overlap, found out whether query is lower or higher
                    if (query.Max.Value.CompareTo(list[middle].Interval.Min.Value) < 0)
                        // The query is lower than the interval, move left
                        max = middle - 1;
                    else
                        // The query is higher than the interval, move right
                        min = middle + 1;
                }
            }

            // We return min so we know if the query was lower or higher than the list
            return min;
        }

        private static int searchLowInHighs(IList<Node> list, IInterval<T> query)
        {
            if (query == null)
                return -1;

            int min = 0, max = list.Count - 1;

            while (min <= max)
            {
                var middle = (max + min) / 2;

                if (query.Overlaps(list[middle].Interval))
                {
                    // Only if the next interval to the overlapping interval does not overlap, we have found the right one
                    if (middle == (list.Count - 1) || !query.Overlaps(list[middle + 1].Interval))
                        // The right interval is found
                        return middle;

                    // The previous interval overlap as well, move right
                    min = middle + 1;
                }
                else
                {
                    // TODO: Should have been >= instead of the old =. Make a test to ensure this doesn't happen again.
                    // The interval does not overlap, found out whether query is lower or higher
                    if (query.Min.Value.CompareTo(list[middle].Interval.Max.Value) >= 0)
                        // The query is lower than the interval, move right
                        min = middle + 1;
                    else
                        // The query is higher than the interval, move left
                        max = middle - 1;
                }
            }

            return max;
        }

        public SCG.IEnumerable<IInterval<T>> Overlap(IInterval<T> query)
        {
            if (query == null)
                throw new NullReferenceException("Query can't be null");

            return overlap(_list, query);
        }

        public bool OverlapExists(IInterval<T> query)
        {
            if (query == null)
                throw new NullReferenceException("Query can't be null");

            // Check if query overlaps the collection at all
            if (_list == null || !query.Overlaps(Span))
                return false;

            // Find first overlap
            var i = searchHighInLows(_list, query);

            // Check if index is in bound and if the interval overlaps the query
            return 0 <= i && i < _list.Count && _list[i].Interval.Overlaps(query);
        }

        #region Static Intervaled

        public int OverlapCount(IInterval<T> query)
        {
            // TODO: Exception?
            if (query == null)
                return -1;

            // The number of overlaps is the difference between the number of nodes not after the last overlap
            // and the number of nodes before the first overlap
            return countNotAfter(_list, query) - countBefore(_list, query);
        }

        private static int countBefore(IList<Node> list, IInterval<T> query)
        {
            // Return 0 if list is empty
            if (list == null || list.IsEmpty)
                return 0;

            var i = searchHighInLows(list, query);

            // query is before the list's span
            if (i == 0 && !list.First.Interval.Overlaps(query))
                return 0;

            // query is after the list's span
            if (i >= list.Count)
                return nodesInList(list);

            // The interval overlaps so all intervals before don't
            // We still need to check the sublist though
            if (list[i].Interval.Overlaps(query))
                return list[i].NodesBefore + countBefore(list[i].Sublist, query);

            return list[i].NodesBefore;
        }

        private static int countNotAfter(IList<Node> list, IInterval<T> query)
        {
            // Return 0 if list is empty
            if (list == null || list.IsEmpty)
                return 0;

            var i = searchLowInHighs(list, query);

            // query is after the list's span
            if (i == list.Count - 1 && !list.First.Interval.Overlaps(query))
                return nodesInList(list);

            // query is before the list's span
            if (i < 0)
                return 0;

            // If the interval doesn't overlap
            if (!list[i].Interval.Overlaps(query))
                return nodesBeforeNext(list[i]);

            return list[i].NodesBefore + 1 + countNotAfter(list[i].Sublist, query);
        }

        private static int nodesInList(IList<Node> list)
        {
            return nodesBeforeNext(list.Last);
        }

        private static int nodesBeforeNext(Node node)
        {
            // Nodes before the node, one for the node itself and the number of nodes in the sublist
            return node.NodesBefore + 1 + node.NodesInSublist;
        }

        #endregion

        #endregion

        public void Add(IInterval<T> interval)
        {
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
