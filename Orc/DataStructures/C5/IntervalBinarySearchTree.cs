namespace Orc.DataStructures.C5
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Interval;
    using Orc.Interval.Interface;

    using global::C5;

    // TODO: Document reference equality duplicates
    public class IntervalBinarySearchTree<T> : IIntervalContainer<T>, IEnumerable<IInterval<T>>
        where T : IComparable<T>
    {
        private const bool RED = true;
        private const bool BLACK = false;

        private Node _root;
        private int _count;

        #region Red-black tree helper methods

        private static bool isRed(Node node)
        {
            // Null nodes are by convention black
            return node != null && node.Color == RED;
        }

        private static Node rotate(Node root)
        {
            if (isRed(root.Right) && !isRed(root.Left))
                root = rotateLeft(root);
            if (isRed(root.Left) && isRed(root.Left.Left))
                root = rotateRight(root);
            if (isRed(root.Left) && isRed(root.Right))
            {
                root.Color = RED;
                root.Left.Color = root.Right.Color = BLACK;
            }

            return root;
        }

        private static Node rotateRight(Node root)
        {
            // Rotate
            var node = root.Left;
            root.Left = node.Right;
            node.Right = root;
            node.Color = root.Color;
            root.Color = RED;


            // 1
            node.Less.AddAll(root.Less);
            node.Equal.AddAll(root.Less);

            // 2
            var between = node.Greater - root.Greater;
            root.Less.AddAll(between);
            node.Greater.RemoveAll(between);

            // 3
            root.Equal.RemoveAll(node.Greater);
            root.Greater.RemoveAll(node.Greater);


            // Update PMO
            root.UpdateMaximumOverlap();
            node.UpdateMaximumOverlap();

            return node;
        }

        private static Node rotateLeft(Node root)
        {
            // Rotate
            var node = root.Right;
            root.Right = node.Left;
            node.Left = root;
            node.Color = root.Color;
            root.Color = RED;


            // 1
            node.Greater.AddAll(root.Greater);
            node.Equal.AddAll(root.Greater);

            // 2
            var between = node.Less - root.Less;
            root.Greater.AddAll(between);
            node.Less.RemoveAll(between);

            // 3
            root.Equal.RemoveAll(node.Less);
            root.Less.RemoveAll(node.Less);


            // Update PMO
            root.UpdateMaximumOverlap();
            node.UpdateMaximumOverlap();

            return node;
        }

        #endregion

        #region Node nested classes

        class Node
        {
            public T Key { get; private set; }

            private IntervalSet _less;
            private IntervalSet _equal;
            private IntervalSet _greater;

            public IntervalSet Less
            {
                get { return this._less ?? (this._less = new IntervalSet()); }
            }
            public IntervalSet Equal
            {
                get { return this._equal ?? (this._equal = new IntervalSet()); }
            }
            public IntervalSet Greater
            {
                get { return this._greater ?? (this._greater = new IntervalSet()); }
            }

            public Node Left { get; internal set; }
            public Node Right { get; internal set; }

            public int Delta { get; internal set; }
            public int DeltaAfter { get; internal set; }
            private int Sum { get; set; }
            public int Max { get; private set; }

            public bool Color { get; set; }

            public Node(T key)
            {
                this.Key = key;
                this.Color = RED;
            }

            public void UpdateMaximumOverlap()
            {
                this.Sum = (this.Left != null ? this.Left.Sum : 0) + this.Delta + this.DeltaAfter + (this.Right != null ? this.Right.Sum : 0);

                this.Max = (new[]
                    {
                        (this.Left != null ? this.Left.Max : 0),
                        (this.Left != null ? this.Left.Sum : 0) + this.Delta,
                        (this.Left != null ? this.Left.Sum : 0) + this.Delta + this.DeltaAfter,
                        (this.Left != null ? this.Left.Sum : 0) + this.Delta + this.DeltaAfter + (this.Right != null ? this.Right.Max : 0)
                    }).Max();
            }

            public override string ToString()
            {
                return String.Format("{0}-{1}", this.Color ? "R" : "B", this.Key);
            }
        }

        public sealed class IntervalSet : global::C5.HashSet<IInterval<T>>
        {
            public IntervalSet(System.Collections.Generic.IEnumerable<IInterval<T>> intervals)
            {
                this.AddAll(intervals);
            }

            public IntervalSet()
            {
            }

            public static IntervalSet operator -(IntervalSet s1, IntervalSet s2)
            {
                if (s1 == null || s2 == null)
                    throw new ArgumentNullException("Set-Set");

                var res = new IntervalSet(s1);
                res.RemoveAll(s2);
                return res;
            }

            public static IntervalSet operator +(IntervalSet s1, IntervalSet s2)
            {
                if (s1 == null || s2 == null)
                    throw new ArgumentNullException("Set+Set");

                var res = new IntervalSet(s1);
                res.AddAll(s2);
                return res;
            }
        }

        #endregion

        #region Constructors

        public IntervalBinarySearchTree(System.Collections.Generic.IEnumerable<IInterval<T>> intervals)
        {
            foreach (var interval in intervals)
                this.Add(interval);
        }

        public IntervalBinarySearchTree()
        {
        }

        #endregion

        #region ICollection, IExtensible

        #region insertion

        private Node addLow(Node root, Node right, IInterval<T> interval)
        {
            if (root == null)
                root = new Node(interval.Min.Value);

            var compareTo = root.Key.CompareTo(interval.Min.Value);

            if (compareTo < 0)
            {
                root.Right = this.addLow(root.Right, right, interval);
            }
            else if (compareTo == 0)
            {
                // If everything in the right subtree of root will lie within the interval
                if (right != null && right.Key.CompareTo(interval.Max.Value) <= 0)
                    root.Greater.Add(interval);

                if (interval.Min.IsInclusive)
                    root.Equal.Add(interval);

                // Update delta
                if (interval.Min.IsInclusive)
                    root.Delta++;
                else
                    root.DeltaAfter++;
            }
            else if (compareTo > 0)
            {
                // Everything in the right subtree of root will lie within the interval
                if (right != null && right.Key.CompareTo(interval.Max.Value) <= 0)
                    root.Greater.Add(interval);

                // root key is between interval.Min.Value and interval.Max.Value
                if (root.Key.CompareTo(interval.Max.Value) < 0)
                    root.Equal.Add(interval);

                // TODO: Figure this one out: if (interval.Min.Value != -inf.)
                root.Left = this.addLow(root.Left, root, interval);
            }

            // Red Black tree rotations
            root = rotate(root);

            // Update PMO
            root.UpdateMaximumOverlap();

            return root;
        }

        private Node addHigh(Node root, Node left, IInterval<T> interval)
        {
            if (root == null)
                root = new Node(interval.Max.Value);

            var compareTo = root.Key.CompareTo(interval.Max.Value);

            if (compareTo > 0)
            {
                root.Left = this.addHigh(root.Left, left, interval);
            }
            else if (compareTo == 0)
            {
                // If everything in the right subtree of root will lie within the interval
                if (left != null && left.Key.CompareTo(interval.Min.Value) >= 0)
                    root.Less.Add(interval);

                if (interval.Max.IsInclusive)
                    root.Equal.Add(interval);

                if (!interval.Max.IsInclusive)
                    root.Delta--;
                else
                    root.DeltaAfter--;
            }
            else if (compareTo < 0)
            {
                // Everything in the right subtree of root will lie within the interval
                if (left != null && left.Key.CompareTo(interval.Min.Value) >= 0)
                    root.Less.Add(interval);

                // root key is between interval.Min.Value and interval.Max.Value
                if (root.Key.CompareTo(interval.Min.Value) > 0)
                    root.Equal.Add(interval);

                // TODO: Figure this one out: if (interval.Min.Value != -inf.)
                root.Right = this.addHigh(root.Right, root, interval);
            }

            // Red Black tree rotations
            root = rotate(root);

            // TODO: Figure out if this is still the correct place to put update
            // Update PMO
            root.UpdateMaximumOverlap();

            return root;
        }

        public void Add(IInterval<T> interval)
        {
            // TODO: Add event!

            this._root = this.addLow(this._root, null, interval);
            this._root = this.addHigh(this._root, null, interval);

            this._root.Color = BLACK;
        }

        #endregion

        #region deletion

        private Node removeByLow(Node root, Node right, IInterval<T> interval)
        {
            if (root == null)
                return null;

            var compareTo = root.Key.CompareTo(interval.Min.Value);

            if (compareTo < 0)
            {
                root.Right = this.removeByLow(root.Right, right, interval);
            }
            else if (compareTo == 0)
            {
                // Remove interval from Greater set
                if (right != null && right.Key.CompareTo(interval.Max.Value) <= 0)
                    root.Greater.Remove(interval);

                // Remove interval from Equal set
                if (interval.Min.IsInclusive)
                    root.Equal.Remove(interval);

                // Update delta
                if (interval.Min.IsInclusive)
                    root.Delta--;
                else
                    root.DeltaAfter--;
            }
            else if (compareTo > 0)
            {
                // Everything in the right subtree of root will lie within the interval
                if (right != null && right.Key.CompareTo(interval.Max.Value) <= 0)
                    root.Greater.Remove(interval);

                // root key is between interval.Min.Value and interval.Max.Value
                if (root.Key.CompareTo(interval.Max.Value) < 0)
                    root.Equal.Remove(interval);

                // TODO: Figure this one out: if (interval.Min.Value != -inf.)
                root.Left = this.removeByLow(root.Left, root, interval);
            }

            // Update PMO
            root.UpdateMaximumOverlap();

            return root;
        }

        private Node removeByHigh(Node root, Node left, IInterval<T> interval)
        {
            if (root == null)
                return null;

            var compareTo = root.Key.CompareTo(interval.Max.Value);

            if (compareTo > 0)
            {
                root.Left = this.removeByHigh(root.Left, left, interval);
            }
            else if (compareTo == 0)
            {
                // If everything in the right subtree of root will lie within the interval
                if (left != null && left.Key.CompareTo(interval.Min.Value) >= 0)
                    root.Less.Remove(interval);

                if (interval.Max.IsInclusive)
                    root.Equal.Remove(interval);

                if (!interval.Max.IsInclusive)
                    root.Delta++;
                else
                    root.DeltaAfter++;
            }
            else if (compareTo < 0)
            {
                // Everything in the right subtree of root will lie within the interval
                if (left != null && left.Key.CompareTo(interval.Min.Value) >= 0)
                    root.Less.Remove(interval);

                // root key is between interval.Min.Value and interval.Max.Value
                if (root.Key.CompareTo(interval.Min.Value) > 0)
                    root.Equal.Remove(interval);

                // TODO: Figure this one out: if (interval.Min.Value != -inf.)
                root.Right = this.removeByHigh(root.Right, root, interval);
            }
            // Update PMO
            root.UpdateMaximumOverlap();

            return root;
        }

        private bool notAlone(Node root, T endpoint)
        {
            Node nodeForEndpoint = this.findNode(root, endpoint);

            if (nodeForEndpoint != null)
            {
                foreach (IInterval<T> interval in nodeForEndpoint.Greater)
                {
                    if (interval.Min.Value.Equals(endpoint) || interval.Max.Value.Equals(endpoint))
                    {
                        return true;
                    }
                }

                foreach (IInterval<T> interval in nodeForEndpoint.Less)
                {
                    if (interval.Min.Value.Equals(endpoint) || interval.Max.Value.Equals(endpoint))
                    {
                        return true;
                    }
                }

                foreach (IInterval<T> interval in nodeForEndpoint.Equal)
                {
                    if (interval.Min.Value.Equals(endpoint) || interval.Max.Value.Equals(endpoint))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Node findNode(Node root, T endpoint)
        {
            if (root.Key.CompareTo(endpoint) == 0)
            {
                return root;
            }

            if (root.Key.CompareTo(endpoint) < 0)
            {
                return this.findNode(root.Right, endpoint);
            }

            if (root.Key.CompareTo(endpoint) > 0)
            {
                return this.findNode(root.Left, endpoint);
            }

            return null;
        }

        // Flip the colors of a node and its two children
        private void flipColors(Node h)
        {
            // h must have opposite color of its two children
            if ((h != null) && (h.Left != null) && (h.Right != null)
                && ((!isRed(h) && isRed(h.Left) && isRed(h.Right))
                || (isRed(h) && !isRed(h.Left) && !isRed(h.Right))))
            {
                h.Color = !h.Color;
                h.Left.Color = !h.Left.Color;
                h.Right.Color = !h.Right.Color;
            }
        }

        // Assuming that h is red and both h.left and h.left.left
        // are black, make h.left or one of its children red.
        private Node moveRedLeft(Node h)
        {
            this.flipColors(h);
            if (isRed(h.Right.Left))
            {
                h.Right = rotateRight(h.Right);
                h = rotateLeft(h);
                // flipColors(h);
            }
            return h;
        }

        // Assuming that h is red and both h.right and h.right.left
        // are black, make h.right or one of its children red.
        private Node moveRedRight(Node h)
        {
            this.flipColors(h);
            if (isRed(h.Left.Left))
            {
                h = rotateRight(h);
                // flipColors(h);
            }
            return h;
        }

        // 
        private Node deleteNode(Node h, Node toRemove)
        {
            if (h.Left == toRemove)
            {
                h.Left = null;
                return null;
            }

            if (!isRed(h.Left) && !isRed(h.Left.Left))
                h = this.moveRedLeft(h);

            h.Left = this.deleteNode(h.Left, toRemove);

            return rotate(h);
        }

        // the smallest key; null if no such key
        public T min()
        {
            return this.min(this._root).Key;
        }

        // the smallest key in subtree rooted at x; null if no such key
        private Node min(Node x)
        {
            if (x.Left == null)
                return x;
            return this.min(x.Left);
        }

        // delete the key-value pair with the given key
        public void remove(T endpoint)
        {

            // if both children of root are black, set root to red
            if (!isRed(this._root.Left) && !isRed(this._root.Right))
                this._root.Color = RED;

            this._root = this.remove(this._root, null, false, endpoint);
            // TODO: if (!isEmpty()) root.color = BLACK;
        }

        // delete the key-value pair with the given key rooted at h
        private Node remove(Node root, Node parent, bool left, T endpoint)
        {

            if (endpoint.CompareTo(root.Key) < 0)
            {
                if (!isRed(root.Left) && !isRed(root.Left.Left))
                {
                    if (isRed(root))
                        root = this.moveRedLeft(root);
                }
                root.Left = this.remove(root.Left, root, true, endpoint);
            }
            else
            {
                if (isRed(root.Left))
                    root = rotateRight(root);
                if (endpoint.CompareTo(root.Key) == 0 && (root.Right == null))
                    return null;
                if (!isRed(root.Right) && !isRed(root.Right.Left))
                    if (isRed(root))
                        root = this.moveRedRight(root);
                if (endpoint.CompareTo(root.Key) == 0)
                {
                    // Save the Greater and Less set of root
                    IntervalSet rootGreater = root.Greater;
                    IntervalSet rootLess = root.Less;

                    // Save key and sets of right child's minimum
                    Node minChild = this.min(root.Right);
                    IntervalSet minGreater = minChild.Greater;
                    IntervalSet minLess = minChild.Less;
                    IntervalSet minEqual = minChild.Equal;

                    // Make new node with the Key of the right child's minimum
                    var node = new Node(minChild.Key) { Left = root.Left, Right = root.Right };
                    node.Greater.AddAll(minGreater);
                    node.Less.AddAll(minLess);
                    node.Equal.AddAll(minEqual);

                    node.Greater.AddAll(root.Greater);
                    node.Less.AddAll(root.Less);
                    node.Equal.AddAll(root.Equal);

                    // Update deltas
                    node.Delta = root.Delta + minChild.Delta;
                    node.DeltaAfter = root.DeltaAfter + minChild.DeltaAfter;

                    if (parent == null)
                        this._root = node;
                    else
                    {
                        if (left)
                            parent.Left = node;
                        else parent.Right = node;
                    }

                    this.deleteNode(root.Right, minChild);

                    return rotate(node);
                }
                root.Right = this.remove(root.Right, root, false, endpoint);
            }
            return rotate(root);
        }

        public void Remove(IInterval<T> interval)
        {
            // Delete the interval from the sets
            this._root = this.removeByLow(this._root, null, interval);
            this._root = this.removeByHigh(this._root, null, interval);

            // If no other interval has the same endpoint, delete the endpoint
            if (!this.notAlone(this._root, interval.Min.Value))
            {
                // Delete endpoint
                this.remove(interval.Min.Value);
            }

            if (!this.notAlone(this._root, interval.Max.Value))
            {
                // Delete endpoint
                this.remove(interval.Max.Value);
            }

            // return true;
        }

        #endregion

        public void Clear()
        {
            this._root = null;
            this._count = 0;
            // TODO: Add what ever is missing
        }

        public bool Contains(IInterval<T> item)
        {
            throw new NotImplementedException();
        }


        public bool IsEmpty { get { return this._root == null; } }

        // TODO: Implement
        public int Count
        {
            get { return this._count; }
        }

        public bool IsReadOnly { get { return false; } }

        public Speed CountSpeed { get { return Speed.Constant; } }
        public bool AllowsDuplicates { get { return true; } }

        #endregion


        #region IEnumerable

        public System.Collections.Generic.IEnumerator<IInterval<T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GraphViz

        /// <summary>
        /// Print the tree structure in Graphviz format
        /// </summary>
        /// <returns></returns>
        public string Graphviz()
        {
            return "digraph IntervalBinarySearchTree {\n"
                + "\tnode [shape=record];\n"
                + this.graphviz(this._root, "root", null)
                + "}\n";
        }

        private int nodeCounter;
        private int nullCounter;

        private string graphviz(IntervalBinarySearchTree<T>.Node root, string parent, string direction)
        {
            int id;
            if (root == null)
            {
                id = this.nullCounter++;
                return "\tleaf" + id + "[shape=point];\n"
                    + "\t" + parent + ":" + direction + " -> leaf" + id + ";\n";
            }

            id = this.nodeCounter++;
            string rootString;
            if (direction == null)
                rootString = "";
            else rootString = "\t" + parent + " -> struct" + id + ";\n";

            return
                // Creates the structid: structid [label="<key> keyValue|{lessSet|equalSet|greaterSet}|{<idleft> leftChild|<idright> rightChild}"];
                "\tstruct" + id + " [label=\"{<key> " + root.Key + "|{"
                + this.graphSet(root.Less) + "|" + this.graphSet(root.Equal) + "|" + this.graphSet(root.Greater)
                + "}}\"];\n"

                // Links the parents leftChild to nodeid: parent:left -> structid:key;
                + rootString

                // Calls graphviz() recursively on leftChild
                + this.graphviz(root.Left, "struct" + id, "left")

                // Calls graphviz() recursively on rightChild
                + this.graphviz(root.Right, "struct" + id, "right");
        }

        private string graphSet(IntervalSet set)
        {
            var s = new ArrayList<string>();

            foreach (var interval in set)
            {
                s.Add(interval.ToString());
            }

            if (s.IsEmpty())
                return "empty";
            return String.Join("\n", s.ToArray());
        }

        #endregion

        #region ICollectionValue

        public IInterval<T> Choose()
        {
            if (this._root == null)
                throw new NoSuchItemException();

            return (this._root.Less + this._root.Equal + this._root.Greater).Choose();
        }

        #endregion

        #region IIntervaled

        public IInterval<T> Span
        {
            get
            {
                if (this._root == null)
                    throw new InvalidOperationException("An empty collection has no span");

                return new Interval<T>(getLowest(this._root).Max.Value, getHighest(this._root).Min.Value);
            }
        }

        private static IInterval<T> getLowest(Node root)
        {
            // TODO: Assert root != null

            if (!root.Less.IsEmpty)
                return root.Less.Choose();

            if (root.Left != null)
                return getLowest(root.Left);

            return !root.Equal.IsEmpty ? root.Equal.Choose() : root.Greater.Choose();
        }

        private static IInterval<T> getHighest(Node root)
        {
            // TODO: Assert root != null

            if (!root.Greater.IsEmpty)
                return root.Greater.Choose();

            if (root.Right != null)
                return getHighest(root.Right);

            return !root.Equal.IsEmpty ? root.Equal.Choose() : root.Less.Choose();
        }

        public System.Collections.Generic.IEnumerable<IInterval<T>> FindOverlaps(T query)
        {
            if (ReferenceEquals(query, null))
                return Enumerable.Empty<IInterval<T>>();

            var set = new IntervalSet();

            foreach (var interval in this.findOverlap(this._root, query))
                set.Add(interval);

            return set;
        }

        private System.Collections.Generic.IEnumerable<IInterval<T>> findOverlap(Node root, T query)
        {
            if (root == null)
                yield break;

            var compareTo = query.CompareTo(root.Key);
            if (compareTo < 0)
            {
                foreach (var interval in root.Less)
                    yield return interval;

                foreach (var interval in this.findOverlap(root.Left, query))
                    yield return interval;
            }
            else if (compareTo > 0)
            {
                foreach (var interval in root.Greater)
                    yield return interval;

                foreach (var interval in this.findOverlap(root.Right, query))
                    yield return interval;
            }
            else
            {
                foreach (var interval in root.Equal)
                    yield return interval;
            }
        }

        public System.Collections.Generic.IEnumerable<IInterval<T>> FindOverlaps(IInterval<T> query)
        {
            if (ReferenceEquals(query, null))
                yield break;

            // Break if collection is empty or the query is outside the collections span
            if (this.IsEmpty || !this.Span.Overlaps(query))
                yield break;

            var set = new IntervalSet();

            var splitNode = this._root;
            // Use a lambda instead of out, as out or ref isn't allowed for itorators
            set.AddAll(this.findSplitNode(this._root, query, n => { splitNode = n; }));

            // Find all intersecting intervals in left subtree
            if (query.Min.Value.CompareTo(splitNode.Key) < 0)
                set.AddAll(this.findLeft(splitNode.Left, query));

            // Find all intersecting intervals in right subtree
            if (splitNode.Key.CompareTo(query.Max.Value) < 0)
                set.AddAll(this.findRight(splitNode.Right, query));

            foreach (var interval in set)
                yield return interval;
        }

        /// <summary>
        /// Create an enumerable, enumerating all intersecting intervals on the path to the split node. Returns the split node in splitNode.
        /// </summary>
        private System.Collections.Generic.IEnumerable<IInterval<T>> findSplitNode(Node root, IInterval<T> query, Action<Node> splitNode)
        {
            if (root == null) yield break;

            splitNode(root);

            // Interval is lower than root, go left
            if (query.Max.Value.CompareTo(root.Key) < 0)
            {
                foreach (var interval in root.Less)
                    yield return interval;

                // Recursively travese left subtree
                foreach (var interval in this.findSplitNode(root.Left, query, splitNode))
                    yield return interval;
            }
            // Interval is higher than root, go right
            else if (root.Key.CompareTo(query.Min.Value) < 0)
            {
                foreach (var interval in root.Greater)
                    yield return interval;

                // Recursively travese right subtree
                foreach (var interval in this.findSplitNode(root.Right, query, splitNode))
                    yield return interval;
            }
            // Otherwise add overlapping nodes in split node
            else
            {
                foreach (var interval in root.Less + root.Equal + root.Greater)
                    if (query.Overlaps(interval))
                        yield return interval;
            }
        }

        private System.Collections.Generic.IEnumerable<IInterval<T>> findLeft(Node root, IInterval<T> query)
        {
            // If root is null we have reached the end
            if (root == null) yield break;

            var compareTo = query.Min.Value.CompareTo(root.Key);

            //
            if (compareTo > 0)
            {
                foreach (var interval in root.Greater)
                    yield return interval;

                // Recursively travese right subtree
                foreach (var interval in this.findLeft(root.Right, query))
                    yield return interval;
            }
            //
            else if (compareTo < 0)
            {
                foreach (var interval in root.Less + root.Equal + root.Greater)
                    yield return interval;

                // Recursively add all intervals in right subtree as they must be
                // contained by [root.Key:splitNode]
                var child = this.getEnumerator(root.Right);
                while (child.MoveNext())
                    yield return child.Current;

                // Recursively travese left subtree
                foreach (var interval in this.findLeft(root.Left, query))
                    yield return interval;
            }
            else
            {
                // Add all intersecting intervals from right list
                foreach (var interval in root.Greater)
                    yield return interval;

                if (query.Min.IsInclusive)
                    foreach (var interval in root.Equal)
                        yield return interval;

                // If we find the matching node, we can add everything in the left subtree
                System.Collections.Generic.IEnumerator<IInterval<T>> child = this.getEnumerator(root.Right);
                while (child.MoveNext())
                    yield return child.Current;
            }
        }

        private System.Collections.Generic.IEnumerable<IInterval<T>> findRight(Node root, IInterval<T> query)
        {
            // If root is null we have reached the end
            if (root == null) yield break;

            var compareTo = query.Max.Value.CompareTo(root.Key);

            //
            if (compareTo < 0)
            {
                // Add all intersecting intervals from left list
                foreach (var interval in root.Less)
                    yield return interval;

                // Otherwise Recursively travese left subtree
                foreach (var interval in this.findRight(root.Left, query))
                    yield return interval;
            }
            //
            else if (compareTo > 0)
            {
                // As our query interval contains the interval [root.Key:splitNode]
                // all intervals in root can be returned without any checks
                foreach (var interval in root.Less + root.Equal + root.Greater)
                    yield return interval;

                // Recursively add all intervals in right subtree as they must be
                // contained by [root.Key:splitNode]
                var child = this.getEnumerator(root.Left);
                while (child.MoveNext())
                    yield return child.Current;

                // Recursively travese left subtree
                foreach (var interval in this.findRight(root.Right, query))
                    yield return interval;
            }
            else
            {
                // Add all intersecting intervals from left list
                foreach (var interval in root.Less)
                    yield return interval;

                if (query.Max.IsInclusive)
                    foreach (var interval in root.Equal)
                        yield return interval;

                // If we find the matching node, we can add everything in the left subtree
                var child = this.getEnumerator(root.Left);
                while (child.MoveNext())
                    yield return child.Current;
            }
        }


        private System.Collections.Generic.IEnumerator<IInterval<T>> getEnumerator(Node node)
        {
            // Just return if tree is empty
            if (node == null) yield break;

            // Recursively retrieve intervals in left subtree
            if (node.Left != null)
            {
                var child = this.getEnumerator(node.Left);

                while (child.MoveNext())
                    yield return child.Current;
            }

            // Go through all intervals in the node
            foreach (var interval in node.Less + node.Equal + node.Greater)
                yield return interval;

            // Recursively retrieve intervals in right subtree
            if (node.Right != null)
            {
                var child = this.getEnumerator(node.Right);

                while (child.MoveNext())
                    yield return child.Current;
            }

        }

        public bool OverlapExists(IInterval<T> query)
        {
            if (query == null)
                return false;

            return this.FindOverlaps(query).GetEnumerator().MoveNext();
        }

        public int CountOverlaps(IInterval<T> query)
        {
            return this.FindOverlaps(query).Count();
        }

        public int MaximumOverlap
        {
            get { return this._root != null ? this._root.Max : 0; }
        }

        #endregion

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public System.Collections.Generic.IEnumerable<IInterval<T>> Query(IInterval<T> interval)
        {
            if (interval == null)
            {
                return Enumerable.Empty<IInterval<T>>();
            }

            return this.FindOverlaps(interval);
        }

        public System.Collections.Generic.IEnumerable<IInterval<T>> Query(T value)
        {
            return this.FindOverlaps(value);
        }
    }
}
