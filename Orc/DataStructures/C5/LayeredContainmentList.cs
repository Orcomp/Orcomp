namespace Orc.DataStructures.C5
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Interval.Interface;

    using global::C5;

    public class LayeredContainmentList<T> : IIntervalContainer<T>, IEnumerable<IInterval<T>>
        where T : IComparable<T>
    {
        private int _count;
        private Node[][] _layers;
        private int[] _counts;
        private IInterval<T> _span;

        private System.Collections.Generic.IList<IInterval<T>> _intervals;
        private bool _isInSync;


        #region Node nested classes

        struct Node
        {
            internal IInterval<T> Interval { get; private set; }
            internal int Pointer { get; private set; }

            internal Node(IInterval<T> interval, int pointer)
                : this()
            {
                this.Interval = interval;
                this.Pointer = pointer;
            }

            internal Node(int pointer)
                : this()
            {
                this.Pointer = pointer;
            }

            public override string ToString()
            {
                return this.Interval != null ? this.Interval.ToString() : "*";
            }
        }

        #endregion

        #region Constructor

        public LayeredContainmentList(IEnumerable<IInterval<T>> intervals)
        {
            this._intervals = intervals.ToList();
            this._isInSync = false;

            this.BuildTree();
        }

        private void BuildTree()
        {
            // Make intervals to array to allow fast sorting and counting
            var intervalArray = this._intervals as IInterval<T>[] ?? this._intervals.ToArray();

            // Only do the work if we have something to work with
            if (!intervalArray.IsEmpty())
            {
                // Count intervals so we can use it later on
                this._count = intervalArray.Length;

                // Sort intervals
                //var comparer = ComparerFactory<IInterval<T>>.CreateComparer(IntervalExtensions.CompareTo);
                //Sorting.IntroSort(intervalArray, 0, this._count, comparer);
                intervalArray = intervalArray.OrderBy(x => x).ToArray();

                // Put intervals in the arrays
                var layers = this.createLayers(intervalArray);

                // Create the list that contains the containment layers
                this._layers = new Node[layers.Count][];
                this._counts = new int[layers.Count];
                // Create each containment layer
                for (var i = 0; i < this._counts.Length; i++)
                {
                    this._layers[i] = layers[i].ToArray();
                    this._counts[i] = layers[i].Count - 1; // Subtract one for the dummy node
                }

                // Save the span once
                this._span = new Orc.Interval.Interval<T>(this._layers[0][0].Interval.Min.Value, this._layers[0][this._counts[0] - 1].Interval.Max.Value);
            }
        }

        private ArrayList<ArrayList<Node>> createLayers(IInterval<T>[] intervals)
        {
            // Use a stack to keep track of current containment
            IStack<IInterval<T>> stack = new ArrayList<IInterval<T>>();
            var layers = new ArrayList<ArrayList<Node>> { new ArrayList<Node>() };

            foreach (var interval in intervals)
            {
                // Track containment
                while (!stack.IsEmpty)
                {
                    // If the interval is contained in the top of the stack, leave it...
                    if (stack.Last().Contains(interval))
                        break;

                    stack.Pop();
                }

                // Check if interval will be contained in the next layer
                while (!layers[stack.Count].IsEmpty && layers[stack.Count].Last.Interval.Contains(interval))
                    stack.Push(layers[stack.Count].Last.Interval);

                stack.Push(interval);

                while (layers.Count < stack.Count + 1)
                    layers.Add(new ArrayList<Node>());

                var layer = stack.Count - 1;

                layers[layer].Add(new Node(interval, layers[layer + 1].Count));
            }

            var lastCount = 0;
            for (var i = layers.Count - 1; i >= 0; i--)
            {
                layers[i].Add(new Node(lastCount));
                lastCount = layers[i].Count - 1;
            }

            return layers;
        }

        #endregion

        #region CollectionValue

        public bool IsEmpty
        {
            get { return this.Count == 0; }
        }

        public int Count { get { return this._count; } }

        public Speed CountSpeed
        {
            get { return Speed.Constant; }
        }

        public IInterval<T> Choose()
        {
            if (this.Count == 0)
                throw new NoSuchItemException();

            return this._layers.First().First().Interval;
        }

        #endregion

        public int CountOverlaps(IInterval<T> query)
        {
            // Break if we won't find any overlaps
            if (ReferenceEquals(query, null) || this.IsEmpty)
                return 0;

            return this.countOverlaps(0, 0, this._counts[0], query);
        }

        private int countOverlaps(int layer, int lower, int upper, IInterval<T> query)
        {
            // Theorem 2
            if (lower >= upper)
                return 0;

            var first = lower;

            // The first interval doesn't overlap we need to search for it
            if (!this._layers[layer][first].Interval.Overlaps(query))
            {
                // We know first doesn't overlap so we can increment it before searching
                first = this.searchHighInLows(layer, ++first, upper, query);

                // If index is out of bound, or found interval doesn't overlap, then the layer won't contain any overlaps
                if (first < lower || upper <= first || !this._layers[layer][first].Interval.Overlaps(query))
                    return 0;
            }

            // We can use first as lower to speed up the search
            var last = this.searchLowInHighs(layer, first, upper, query);

            return last - first + this.countOverlaps(layer + 1, this._layers[layer][first].Pointer, this._layers[layer][last].Pointer, query);
        }

        private int searchLowInHighs(int layer, int lower, int upper, IInterval<T> query)
        {
            int min = lower, max = upper - 1;

            while (min <= max)
            {
                var middle = min + ((max - min) >> 1); // Shift one is the same as dividing by 2

                var interval = this._layers[layer][middle].Interval;

                if (query.Overlaps(interval))
                {
                    // We know we have an overlap, but we need to check if it's the last one
                    // Only if the next interval to the overlapping interval does not overlap, we have found the right one
                    if (middle == upper - 1 || !query.Overlaps(this._layers[layer][middle + 1].Interval))
                        // The right interval is found, return the index for the next interval
                        return middle + 1;

                    // The previous interval overlap as well, move right
                    min = middle + 1;
                }
                else
                {
                    // The interval does not overlap, find out whether query is lower or higher
                    if (query.CompareTo(interval) < 0)
                        // The query is lower than the interval, move left
                        max = middle - 1;
                    else
                        // The query is higher than the interval, move right
                        min = middle + 1;
                }
            }

            return max;
        }

        public int searchFirst(int layer, int lower, int upper, IInterval<T> query)
        {
            int min = lower, max = upper - 1;

            while (min <= max)
            {
                // TODO: If max - min is small enough do a linear searchs

                var mid = min + ((max - min) >> 1);

                var interval = this._layers[layer][mid].Interval;

                // Check if 
                if (interval.Overlaps(query))
                {
                    // We don't need to check if for out-of-bound errors, as we've added 1 to lower before doing the search
                    var previousInterval = this._layers[layer][mid - 1].Interval;

                    if (!previousInterval.Overlaps(query))
                        return mid;

                    max = mid - 1;
                }
                else if (query.CompareTo(interval) < 0)
                    max = mid - 1;
                else
                    min = mid + 1;
            }

            return -1;
        }

        public int searchNextAfterLast(int layer, int lower, int upper, IInterval<T> query)
        {
            int min = lower, max = upper - 1;

            while (min <= max)
            {
                var mid = min + (max - min) / 2;

                var interval = this._layers[layer][mid].Interval;

                // Check if 
                if (interval.Overlaps(query))
                {
                    // TODO: Check that the next exists
                    var nextInterval = this._layers[layer][mid + 1].Interval;

                    if (!nextInterval.Overlaps(query))
                        return mid + 1;

                    min = mid + 1;
                }

                else if (query.CompareTo(interval) < 0)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Will return the index of the first interval that overlaps the query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private int searchHighInLows(int layer, int lower, int upper, IInterval<T> query)
        {
            // Upper is excluded so we subtract by one
            int min = lower, max = upper - 1;

            while (min <= max)
            {
                var middle = min + ((max - min) >> 1); // Shift one is the same as dividing by 2

                var interval = this._layers[layer][middle].Interval;

                if (query.Overlaps(interval))
                {
                    // Only if the previous interval to the overlapping interval does not overlap, we have found the right one
                    // TODO: Why middle == lower? For lower = 1, upper = 2, we stop instantly...
                    if (middle == lower || !query.Overlaps(this._layers[layer][middle - 1].Interval))
                        // The right interval is found
                        return middle;

                    // The previous interval overlap as well, move left
                    max = middle - 1;
                }
                else
                {
                    // The interval does not overlap, find out whether query is lower or higher
                    if (query.CompareTo(interval) < 0)
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

        public IEnumerator<IInterval<T>> GetEnumerator()
        {
            return this.getEnumerator(0, 0, this._counts[0]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerator<IInterval<T>> getEnumerator(int level, int start, int end)
        {
            while (start < end)
            {
                var node = this._layers[level][start];

                yield return node.Interval;

                // Check if we are at the last node
                if (node.Pointer < this._layers[level][start + 1].Pointer)
                {
                    var child = this.getEnumerator(level + 1, node.Pointer, this._layers[level][start + 1].Pointer);

                    while (child.MoveNext())
                        yield return (IInterval<T>) ((IEnumerator)child).Current;
                }

                start++;
            }
        }

        public IInterval<T> Span
        {
            get
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("An empty collection has no span");

                return this._span;
            }
        }

        public IEnumerable<IInterval<T>> FindOverlaps(T query)
        {
            // Break if we won't find any overlaps
            if (ReferenceEquals(query, null) || this.IsEmpty)
                return Enumerable.Empty<IInterval<T>>();

            return this.FindOverlaps(new Orc.Interval.Interval<T>(query, query));
        }

        public IEnumerable<IInterval<T>> FindOverlaps(IInterval<T> query)
        {
            // Break if we won't find any overlaps
            if (ReferenceEquals(query, null) || this.IsEmpty)
                yield break;

            int layer = 0, lower = 0, upper = this._counts[0];

            // Make sure first and last don't point at the same interval (theorem 2)
            while (lower < upper)
            {
                var currentLayer = this._layers[layer];

                var first = lower;

                // The first interval doesn't overlap we need to search for it
                if (!currentLayer[first].Interval.Overlaps(query))
                {
                    // We know first doesn't overlap so we can increment it before searching
                    // TODO: Optimize binary search
                    first = this.searchFirst(layer, ++first, upper, query);

                    // If index is out of bound, or found interval doesn't overlap, then the list won't contain any overlaps
                    if (first < lower || upper <= first || !currentLayer[first].Interval.Overlaps(query))
                        yield break;
                }

                // We can use first as lower to speed up the search
                var last = this.searchLowInHighs(layer, first, upper, query);

                // Save values for next iteration
                layer++;
                lower = currentLayer[first].Pointer; // 0
                upper = currentLayer[last].Pointer; // _counts[layer]

                while (first < last)
                    yield return currentLayer[first++].Interval;
            }
        }

        private IEnumerable<IInterval<T>> findOverlapsRecursive(int layer, int lower, int upper, IInterval<T> query)
        {
            // Make sure first and last don't point at the same interval (theorem 2)
            if (lower >= upper)
                yield break;

            var first = lower;
            var currentLayer = this._layers[layer];

            // The first interval doesn't overlap we need to search for it
            if (!currentLayer[first].Interval.Overlaps(query))
            {
                // We know first doesn't overlap so we can increment it before searching
                first = this.searchFirst(layer, ++first, upper, query);

                // If index is out of bound, or found interval doesn't overlap, then the list won't contain any overlaps
                if (first < lower || upper <= first || !currentLayer[first].Interval.Overlaps(query))
                    yield break;
            }

            // We can use first as lower to speed up the search
            var last = this.searchLowInHighs(layer, first, upper, query);

            // Find intervals in sublist
            foreach (var interval in this.findOverlapsRecursive(layer + 1, currentLayer[first].Pointer, currentLayer[last].Pointer, query))
                yield return interval;

            while (first < last)
                yield return currentLayer[first++].Interval;
        }

        public bool OverlapExists(IInterval<T> query)
        {
            // No overlap if query is null, collection is empty, or query doesn't overlap collection
            if (query == null || this.IsEmpty || !query.Overlaps(this.Span))
                return false;

            // Find first overlap
            var i = this.searchHighInLows(0, 0, this._counts[0], query);

            // Check if index is in bound and if the interval overlaps the query
            return 0 <= i && i < this._counts[0] && this._layers[0][i].Interval.Overlaps(query);
        }

        public string Graphviz()
        {
            return String.Format("digraph LayeredContainmentList {{\n\tnode [shape=record];\n\n{0}\n}}", this.graphviz());
        }

        private string graphviz()
        {
            var s = String.Empty;

            for (var layer = 0; layer < this._counts.Length; layer++)
            {
                var l = new ArrayList<string>();
                var p = String.Empty;
                for (var i = 0; i <= this._counts[layer]; i++)
                {
                    l.Add(String.Format("<n{0}> {0}: {1}", i, this._layers[layer][i]));

                    p += String.Format("layer{0}:n{1} -> layer{2}:n{3};\n\t", layer, i, layer + 1, this._layers[layer][i].Pointer);
                }

                s += String.Format("\tlayer{0} [label=\"{1}\"];\n\t{2}\n", layer, String.Join("|", l.ToArray()), p);
            }

            s += String.Format("\tlayer{0} [label=\"<n0> 0: *\"];", this._counts.Length);

            return s;
        }

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
