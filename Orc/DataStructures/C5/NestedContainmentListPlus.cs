namespace Orc.DataStructures.C5
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Interval;
    using Orc.Interval.Interface;

    using global::C5;

    public class NestedContainmentListPlus<T> : IIntervalContainer<T>, IEnumerable<IInterval<T>>
        where T : IComparable<T>
    {
        private Node[] _list;
        private IInterval<T> _span;
        private Section _section;

        private System.Collections.Generic.IList<IInterval<T>> _intervals;
        private bool _isInSync;

        struct Section
        {
            public Section(int offset, int length)
                : this()
            {
                this.Offset = offset;
                this.Length = length;
            }

            public int Offset { get; private set; }
            public int Length { get; private set; }
        }

        #region Node nested classes

        struct Node
        {
            internal IInterval<T> Interval { get; private set; }
            internal Section Sublist { get; private set; }

            internal int NodesBefore { get; private set; }
            internal int NodesInSublist { get; private set; }

            internal Node(IInterval<T> interval, Section section, int nodesBefore, int nodesInSublist)
                : this()
            {
                this.Interval = interval;
                this.Sublist = section;

                this.NodesBefore = nodesBefore;
                this.NodesInSublist = nodesInSublist;
            }

            public override string ToString()
            {
                return String.Format("{0} - {1}/{2}", this.Interval, this.Sublist.Length, this.Sublist.Offset);
            }
        }

        #endregion

        #region constructors

        /// <summary>
        /// A sorted list of IInterval&lt;T&gt; sorted with IntervalComparer&lt;T&gt;
        /// </summary>
        /// <param name="intervals">Sorted intervals</param>
        /// <returns>A list of nodes</returns>
        private int createList(IInterval<T>[] intervals, Section source, Section target)
        {
            var end = target.Offset + target.Length;
            var t = target.Offset;

            // Remember the number of nodes before the current node to allow fast count operation
            var nodesBefore = 0;

            for (int s = source.Offset; s < source.Offset + source.Length; s++)
            {
                var interval = intervals[s];
                var contained = 0;
                var length = 0;

                // Continue as long as we have more intervals
                while (s + 1 < source.Offset + source.Length)
                {
                    var nextInterval = intervals[s + 1];

                    if (!interval.Contains(nextInterval))
                        break;

                    contained++;
                    s++;
                }

                if (contained > 0)
                {
                    end -= contained;
                    length = this.createList(intervals, new Section(s - contained + 1, contained), new Section(end, contained));
                }

                this._list[t++] = new Node(interval, new Section(end, length), nodesBefore, contained);

                nodesBefore += contained + 1;
            }

            return t - target.Offset;
        }

        /// <summary>
        /// Create a Nested Containment List with a enumerable of intervals
        /// </summary>
        /// <param name="intervals">A collection of intervals in arbitrary order</param>
        public NestedContainmentListPlus(System.Collections.Generic.IEnumerable<IInterval<T>> intervals)
        {
            this._intervals = intervals.ToList();
            this._isInSync = false;

            this.BuildTree();
        }

        private void BuildTree()
        {
            var intervalsArray = this._intervals as IInterval<T>[] ?? this._intervals.ToArray();

            if (!intervalsArray.IsEmpty())
            {
                this.Count = intervalsArray.Count();

                // NOTE: Not sure about this
                // Sorting.IntroSort(intervalsArray, 0, Count, ComparerFactory<IInterval<T>>.CreateComparer(IntervalExtensions.CompareTo));
                intervalsArray = intervalsArray.OrderBy(x => x).ToArray();

                // TODO: Figure out how Orcomp does it
                //MaximumOverlap = IntervaledHelper<T>.MaximumOverlap(intervalsArray);

                var totalSection = new Section(0, intervalsArray.Count());
                this._list = new Node[totalSection.Length];

                // Build nested containment list recursively and save the upper-most list in the class
                this._section = new Section(0, this.createList(intervalsArray, totalSection, totalSection));

                // Save span to allow for constant speeds on later requests
                IInterval<T> i = this._list[this._section.Offset].Interval, j = this._list[this._section.Length + this._section.Offset - 1].Interval;
                this.Span = new Interval<T>(i.Min.Value, j.Max.Value, i.Min.IsInclusive, j.Max.IsInclusive);
            }
            else
            {
                this._list = new Node[0];
                this._section = new Section(0, 0);
            }
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // TODO: Test the order is still the same as when sorted with IntervalComparer. This should be that case!
        /// <summary>
        /// Create an enumerator, enumerating the intervals in sorted order - sorted on low endpoint with shortest intervals first
        /// </summary>
        /// <returns>Enumerator</returns>
        // TODO: Test the order is still the same as when sorted with IntervalComparer. This should be that case!
        public System.Collections.Generic.IEnumerator<IInterval<T>> GetEnumerator()
        {
            return this.getEnumerator(this._section);
        }

        private System.Collections.Generic.IEnumerator<IInterval<T>> getEnumerator(Section section)
        {
            // Just for good measures
            if (this._list == null || section.Length == 0)
                yield break;

            for (int i = section.Offset; i < section.Offset + section.Length; i++)
            {
                var node = this._list[i];
                // Yield the interval itself before the sublist to maintain sorting order
                yield return node.Interval;

                if (node.Sublist.Length > 0)
                {
                    var child = this.getEnumerator(node.Sublist);

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
            return this._list.ToString();
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
        public EventTypeEnum ListenableEvents { get { return EventTypeEnum.None; } }
        public EventTypeEnum ActiveEvents { get { return EventTypeEnum.None; } }

        public event CollectionChangedHandler<IInterval<T>> CollectionChanged;
        public event CollectionClearedHandler<IInterval<T>> CollectionCleared;
        public event ItemsAddedHandler<IInterval<T>> ItemsAdded;
        public event ItemInsertedHandler<IInterval<T>> ItemInserted;
        public event ItemsRemovedHandler<IInterval<T>> ItemsRemoved;
        public event ItemRemovedAtHandler<IInterval<T>> ItemRemovedAt;

        #endregion

        public bool IsEmpty { get { return this.Count == 0; } }
        public int Count { get; private set; }
        public Speed CountSpeed { get { return Speed.Constant; } }

        public void CopyTo(IInterval<T>[] array, int index)
        {
            if (index < 0 || index + this.Count > array.Length)
                throw new ArgumentOutOfRangeException();

            foreach (var item in this)
                array[index++] = item;
        }

        public IInterval<T>[] ToArray()
        {
            var res = new IInterval<T>[this.Count];
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
            if (this.Count > 0)
                return this._list[this._section.Offset].Interval;

            throw new NoSuchItemException();
        }

        public System.Collections.Generic.IEnumerable<IInterval<T>> Filter(Func<IInterval<T>, bool> filter)
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
                if (this.IsEmpty)
                    throw new InvalidOperationException("An empty collection has no span");

                return this._span;
            }

            private set { this._span = value; }
        }

        public Speed SpanSpeed { get { return Speed.Constant; } }

        public System.Collections.Generic.IEnumerable<IInterval<T>> FindOverlaps(T query)
        {
            if (ReferenceEquals(query, null))
                return Enumerable.Empty<IInterval<T>>();

            return this.findOverlap(this._section, new Interval<T>(query, query));
        }

        // TODO: Test speed difference between version that takes overlap-loop and upper and low bound loop
        private System.Collections.Generic.IEnumerable<IInterval<T>> findOverlap(Section section, IInterval<T> query)
        {
            if (this._list == null || section.Length == 0)
                yield break;

            // Find first overlapping interval
            var first = this.searchHighInLows(section, query);

            // If index is out of bound, or interval doesn't overlap, we can just stop our search
            if (first < section.Offset || section.Offset + section.Length - 1 < first || !this._list[first].Interval.Overlaps(query))
                yield break;

            var last = this.searchLowInHighs(section, query);

            while (first <= last)
            {
                var node = this._list[first++];

                yield return node.Interval;

                if (node.Sublist.Length > 0)
                    // If the interval is contained in the query, all intervals in the sublist must overlap the query
                    foreach (var interval in this.findOverlap(node.Sublist, query))
                        yield return interval;
            }
        }

        /// <summary>
        /// Get the index of the first node with an interval the overlaps the query
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private int searchHighInLows(Section section, IInterval<T> query)
        {
            if (query == null || section.Length == 0)
                return -1;

            int min = section.Offset, max = section.Offset + section.Length - 1;

            while (min <= max)
            {
                var middle = (max + min) / 2;

                if (query.Overlaps(this._list[middle].Interval))
                {
                    // Only if the previous interval to the overlapping interval does not overlap, we have found the right one
                    if (middle == section.Offset || !query.Overlaps(this._list[middle - 1].Interval))
                        // The right interval is found
                        return middle;

                    // The previous interval overlap as well, move left
                    max = middle - 1;
                }
                else
                {
                    // The interval does not overlap, found out whether query is lower or higher
                    if (query.CompareTo(this._list[middle].Interval) < 0)
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

        /// <summary>
        /// Get the index of the last node with an interval the overlaps the query
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private int searchLowInHighs(Section section, IInterval<T> query)
        {
            if (query == null || section.Length == 0)
                return -1;

            int min = section.Offset, max = section.Offset + section.Length - 1;

            while (min <= max)
            {
                var middle = (max + min) / 2;

                if (query.Overlaps(this._list[middle].Interval))
                {
                    // Only if the next interval to the overlapping interval does not overlap, we have found the right one
                    if (middle == (section.Offset + section.Length - 1) || !query.Overlaps(this._list[middle + 1].Interval))
                        // The right interval is found
                        return middle;

                    // The previous interval overlap as well, move right
                    min = middle + 1;
                }
                else
                {
                    // The interval does not overlap, found out whether query is lower or higher
                    if (query.CompareTo(this._list[middle].Interval) < 0)
                        // The query is lower than the interval, move left
                        max = middle - 1;
                    else
                        // The query is higher than the interval, move right
                        min = middle + 1;
                }
            }

            return max;
        }

        public System.Collections.Generic.IEnumerable<IInterval<T>> FindOverlaps(IInterval<T> query)
        {
            if (query == null)
                return Enumerable.Empty<IInterval<T>>();

            return this.findOverlap(this._section, query);
        }

        public bool OverlapExists(IInterval<T> query)
        {
            if (query == null)
                return false;

            // Check if query overlaps the collection at all
            if (this._list.IsEmpty() || !query.Overlaps(this.Span))
                return false;

            // Find first overlap
            var i = this.searchHighInLows(this._section, query);

            // Check if index is in bound and if the interval overlaps the query
            return this._section.Offset <= i && i < this._section.Offset + this._section.Length && this._list[i].Interval.Overlaps(query);
        }

        public int MaximumOverlap { get; private set; }

        #region Static Intervaled

        public int CountOverlaps(IInterval<T> query)
        {
            if (query == null)
                return 0;

            // The number of overlaps is the difference between the number of nodes not after the last overlap
            // and the number of nodes before the first overlap
            return this.countNotAfter(this._section, query) - this.countBefore(this._section, query);
        }

        private int countBefore(Section section, IInterval<T> query)
        {
            // Return 0 if list is empty
            if (this._list == null || section.Length == 0)
                return 0;

            var i = this.searchHighInLows(section, query);

            // query is before the list's span
            if (i == section.Offset && !query.Overlaps(this._list[section.Offset].Interval))
                return 0;

            // query is after the list's span
            if (i >= section.Offset + section.Length)
                return this.nodesInList(section);

            // The interval overlaps so all intervals before don't
            // We still need to check the sublist though
            if (query.Overlaps(this._list[i].Interval))
                return this._list[i].NodesBefore + this.countBefore(this._list[i].Sublist, query);

            return this._list[i].NodesBefore;
        }

        private int countNotAfter(Section section, IInterval<T> query)
        {
            // Return 0 if list is empty
            if (this._list == null || section.Length == 0)
                return 0;

            var i = this.searchLowInHighs(section, query);

            // query is after the list's span
            if (i == section.Offset + section.Length - 1 && !query.Overlaps(this._list[section.Offset].Interval))
                return this.nodesInList(section);

            // query is before the list's span
            if (i < section.Offset)
                return 0;

            // If the interval doesn't overlap
            if (!query.Overlaps(this._list[i].Interval))
                return nodesBeforeNext(this._list[i]);

            return this._list[i].NodesBefore + 1 + this.countNotAfter(this._list[i].Sublist, query);
        }

        private int nodesInList(Section section)
        {
            return nodesBeforeNext(this._list[section.Offset + section.Length - 1]);
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
