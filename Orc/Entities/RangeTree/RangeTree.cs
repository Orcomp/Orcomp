namespace Orc.Entities.RangeTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Interface;

    /// <summary>
    /// The standard range tree implementation. Keeps a root node and
    /// forwards all queries to it.
    /// Whenenver new items are added or items are removed, the tree 
    /// goes "out of sync" and is rebuild when it's queried next.
    /// </summary>
    /// <typeparam name="T">The type of the range.</typeparam>
    public class RangeTree<T> : IIntervalContainer<T> where T : struct, IComparable<T>
    {
        private RangeTreeNode<T> _root;
        private List<IInterval<T>> _items;
        private bool _isInSync;
        private bool _autoRebuild;
        private IComparer<IInterval<T>> _rangeComparer;

        /// <summary>
        /// Whether the tree is currently in sync or not. If it is "out of sync"
        /// you can either rebuild it manually (call Rebuild) or let it rebuild
        /// automatically when you query it next.
        /// </summary>
        public bool IsInSync
        {
            get { return this._isInSync; }
        }

        /// <summary>
        /// All items of the tree.
        /// </summary>
        public IEnumerable<IInterval<T>> Items
        {
            get { return this._items; }
        }

        /// <summary>
        /// Count of all items.
        /// </summary>
        public int Count
        {
            get { return this._items.Count; }
        }

        /// <summary>
        /// Whether the tree should be rebuild automatically. Defaults to true.
        /// </summary>
        public bool AutoRebuild
        {
            get { return this._autoRebuild; }
            set { this._autoRebuild = value; }
        }

        /// <summary>
        /// Initializes an empty tree.
        /// </summary>
        public RangeTree(IComparer<IInterval<T>> rangeComparer = null) : this(null, rangeComparer) { }

        /// <summary>
        /// Initializes a tree with a list of items to be added.
        /// </summary>
        public RangeTree(IEnumerable<IInterval<T>> items, IComparer<IInterval<T>> rangeComparer = null)
        {
            this._rangeComparer = rangeComparer ?? Comparer<IInterval<T>>.Default;
            this._items = items != null ? items.ToList() : new List<IInterval<T>>();
            this._root = new RangeTreeNode<T>(this._items, rangeComparer);
            this._isInSync = true;
            this._autoRebuild = true;
        }

        /// <summary>
        /// Performans a "stab" query with a single value.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<IInterval<T>> Query(T value)
        {
            if (!this._isInSync && this._autoRebuild)
                this.Rebuild();

            return this._root.Query(value);
        }

        /// <summary>
        /// Performans a range query.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<IInterval<T>> Query(IInterval<T> range)
        {
            if(range == null)
            {
                return Enumerable.Empty<IInterval<T>>();
            }

            if (!this._isInSync && this._autoRebuild)
                this.Rebuild();

            return this._root.Query(range);
        }

        /// <summary>
        /// Rebuilds the tree if it is out of sync.
        /// </summary>
        public void Rebuild()
        {
            if (this._isInSync)
                return;

            this._root = new RangeTreeNode<T>(this._items, this._rangeComparer);
            this._isInSync = true;
        }

        /// <summary>
        /// Adds the specified item. Tree will go out of sync.
        /// </summary>
        public void Add(IInterval<T> item)
        {
            this._isInSync = false;
            this._items.Add(item);
        }

        /// <summary>
        /// Adds the specified items. Tree will go out of sync.
        /// </summary>
        public void Add(IEnumerable<IInterval<T>> items)
        {
            this._isInSync = false;
            this._items.AddRange(items);
        }

        /// <summary>
        /// Removes the specified item. Tree will go out of sync.
        /// </summary>
        public void Remove(IInterval<T> item)
        {
            this._isInSync = false;
            this._items.Remove(item);
        }

        /// <summary>
        /// Removes the specified items. Tree will go out of sync.
        /// </summary>
        public void Remove(IEnumerable<IInterval<T>> items)
        {
            this._isInSync = false;

            foreach (var item in items)
                this._items.Remove(item);
        }

        /// <summary>
        /// Clears the tree (removes all items).
        /// </summary>
        public void Clear()
        {
            this._root = new RangeTreeNode<T>(this._rangeComparer);
            this._items = new List<IInterval<T>>();
            this._isInSync = true;
        }
    }


}