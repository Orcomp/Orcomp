using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.Algodat
{
    /// <summary>
    /// A node of the range tree. Given a list of items, it builds
    /// its subtree. Also contains methods to query the subtree.
    /// Basically, all interval tree logic is here.
    /// </summary>
    public class RangeTreeNode<TKey, T>
        where TKey : IComparable<TKey>
        where T : IRangeProvider<TKey>
    {
        private TKey _center;
        private RangeTreeNode<TKey, T> _leftNode;
        private RangeTreeNode<TKey, T> _rightNode;
        private List<T> _items;

        /// <summary>
        /// Initializes an empty node.
        /// </summary>
        /// <param name="rangeComparer">The comparer used to compare two items.</param>
        public RangeTreeNode(IComparer<T> rangeComparer = null) : this(null, rangeComparer) { }

        /// <summary>
        /// Initializes a node with a list of items, builds the sub tree.
        /// </summary>
        /// <param name="items">Data to include in this node.</param>
        /// <param name="rangeComparer">The comparer used to compare two items.</param>
        public RangeTreeNode(IEnumerable<T> items, IComparer<T> rangeComparer = null)
        {
            rangeComparer = rangeComparer ?? new DefaultRangeComparer<TKey, T>();

            // No items? nothing to do
            if (items == null || items.Count() == 0) return;

            // first, find the median
            var endPoints = new List<TKey>();
            foreach (var o in items)
            {
                var range = o.Range;
                endPoints.Add(range.From);
                endPoints.Add(range.To);
            }
            endPoints.Sort();

            // the median is used as center value
            _center = endPoints[endPoints.Count / 2];
            _items = new List<T>();

            var left = new List<T>();
            var right = new List<T>();

            // iterate over all items
            // if the range of an item is completely left of the center, add it to the left items
            // if it is on the right of the center, add it to the right items
            // otherwise (range overlaps the center), add the item to this node's items
            foreach (var o in items)
            {
                var range = o.Range;

                if (range.To.CompareTo(_center) < 0)
                    left.Add(o);
                else if (range.From.CompareTo(_center) > 0)
                    right.Add(o);
                else
                    _items.Add(o);
            }

            // sort the items, this way the query is faster later on
            if (_items.Count > 0)
                _items.Sort(rangeComparer);
            else
                _items = null;

            // create left and right nodes, if there are any items
            if (left.Count > 0)
                _leftNode = new RangeTreeNode<TKey, T>(left);
            if (right.Count > 0)
                _rightNode = new RangeTreeNode<TKey, T>(right);
        }

        /// <summary>
        /// Performans a "stab" query with a single value.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<T> Query(TKey value)
        {
            // If the node has items, check their ranges.
            if (_items != null)
            {
                var localSearch = _items
                    .TakeWhile(o => o.Range.From.CompareTo(value) <= 0)
                    .Where(o => o.Range.Contains(value));

                foreach (var o in localSearch)
                {
                    yield return o;
                }
            }

            // go to the left or go to the right of the tree, depending
            // where the query value lies compared to the center
            if (value.CompareTo(_center) < 0 && _leftNode != null)
            {
                foreach (var leftValue in _leftNode.Query(value))
                {
                    yield return leftValue;
                }
            }
            else if (value.CompareTo(_center) > 0 && _rightNode != null)
            {
                foreach (var rightValue in _rightNode.Query(value))
                {
                    yield return rightValue;
                }
            }
        }

        /// <summary>
        /// Performans a range query.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<T> Query(Range<TKey> range)
        {
            // If the node has items, check their ranges.
            if (_items != null)
            {
                var localSearch = _items
                    .TakeWhile(o => o.Range.From.CompareTo(range.To) <= 0)
                    .Where(o => o.Range.Intersects(range));

                foreach (var o in localSearch)
                {
                    yield return o;
                }
            }

            // go to the left or go to the right of the tree, depending
            // where the query value lies compared to the center
            if (range.From.CompareTo(_center) < 0 && _leftNode != null)
            {
                foreach (var leftValue in _leftNode.Query(range))
                {
                    yield return leftValue;
                }
            }
            if (range.To.CompareTo(_center) > 0 && _rightNode != null)
            {
                foreach (var rightValue in _rightNode.Query(range))
                {
                    yield return rightValue;
                }
            }
        }
    }
}