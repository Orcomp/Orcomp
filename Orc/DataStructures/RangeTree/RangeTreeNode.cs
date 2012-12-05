namespace Orc.DataStructures.RangeTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Interval.Interface;

    /// <summary>
    /// A node of the range tree. Given a list of items, it builds
    /// its subtree. Also contains methods to query the subtree.
    /// Basically, all interval tree logic is here.
    /// </summary>
    public class RangeTreeNode<T> where T : IComparable<T>
    {
        private T _center;
        private RangeTreeNode<T> _leftNode;
        private RangeTreeNode<T> _rightNode;
        private List<IInterval<T>> _items;

        /// <summary>
        /// Initializes an empty node.
        /// </summary>
        /// <param name="rangeComparer">The comparer used to compare two items.</param>
        public RangeTreeNode(IComparer<IInterval<T>> rangeComparer = null) : this(null, rangeComparer) { }

        /// <summary>
        /// Initializes a node with a list of items, builds the sub tree.
        /// </summary>
        /// <param name="items">Data to include in this node.</param>
        /// <param name="rangeComparer">The comparer used to compare two items.</param>
        public RangeTreeNode(IEnumerable<IInterval<T>> items, IComparer<IInterval<T>> rangeComparer = null)
        {
            rangeComparer = rangeComparer ?? Comparer<IInterval<T>>.Default;

            // No items? nothing to do
            if (items == null || !items.Any()) return;

            // first, find the median
            var endPoints = new List<T>();
            foreach (var o in items)
            {
                var range = o;
                endPoints.Add(range.Min.Value);
                endPoints.Add(range.Max.Value);
            }
            endPoints.Sort();

            // the median is used as center value
            this._center = endPoints[endPoints.Count / 2];
            this._items = new List<IInterval<T>>();

            var left = new List<IInterval<T>>();
            var right = new List<IInterval<T>>();

            // iterate over all items
            // if the range of an item is completely left of the center, add it to the left items
            // if it is on the right of the center, add it to the right items
            // otherwise (range overlaps the center), add the item to this node's items
            foreach (var o in items)
            {
                var range = o;

                if (range.Max.Value.CompareTo(this._center) < 0)
                    left.Add(o);
                else if (range.Min.Value.CompareTo(this._center) > 0)
                    right.Add(o);
                else
                    this._items.Add(o);
            }

            // sort the items, this way the query is faster later on
            if (this._items.Count > 0)
                this._items.Sort(rangeComparer);
            else
                this._items = null;

            // create left and right nodes, if there are any items
            if (left.Count > 0)
                this._leftNode = new RangeTreeNode<T>(left);
            if (right.Count > 0)
                this._rightNode = new RangeTreeNode<T>(right);
        }

        /// <summary>
        /// Performans a "stab" query with a single value.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<IInterval<T>> Query(T value)
        {
            // If the node has items, check their ranges.
            if (this._items != null)
            {
                var localSearch = this._items
                    .TakeWhile(o => o.Min.Value.CompareTo(value) <= 0)
                    .Where(o => o.Contains(value));

                foreach (var o in localSearch)
                {
                    yield return o;
                }
            }

            // go to the left or go to the right of the tree, depending
            // where the query value lies compared to the center
            if (value.CompareTo(this._center) < 0 && this._leftNode != null)
            {
                foreach (var leftValue in this._leftNode.Query(value))
                {
                    yield return leftValue;
                }
            }
            else if (value.CompareTo(this._center) > 0 && this._rightNode != null)
            {
                foreach (var rightValue in this._rightNode.Query(value))
                {
                    yield return rightValue;
                }
            }
        }

        /// <summary>
        /// Performans a range query.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<IInterval<T>> Query(IInterval<T> range)
        {
            // If the node has items, check their ranges.
            if (this._items != null)
            {
                var localSearch = this._items
                    .TakeWhile(o => o.Min.Value.CompareTo(range.Max.Value) <= 0)
                    .Where(o => o.Overlaps(range));

                foreach (var o in localSearch)
                {
                    yield return o;
                }
            }

            // go to the left or go to the right of the tree, depending
            // where the query value lies compared to the center
            if (range.Min.Value.CompareTo(this._center) < 0 && this._leftNode != null)
            {
                foreach (var leftValue in this._leftNode.Query(range))
                {
                    yield return leftValue;
                }
            }
            if (range.Max.Value.CompareTo(this._center) > 0 && this._rightNode != null)
            {
                foreach (var rightValue in this._rightNode.Query(range))
                {
                    yield return rightValue;
                }
            }
        }
    }
}