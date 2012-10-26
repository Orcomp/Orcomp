namespace Orc.Entities.RangeTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The standard range tree implementation. Keeps a root node and
    /// forwards all queries to it.
    /// Whenenver new items are added or items are removed, the tree 
    /// goes "out of sync" and is rebuild when it's queried next.
    /// </summary>
    /// <typeparam name="TKey">The type of the range.</typeparam>
    /// <typeparam name="T">The type of the data items.</typeparam>
    public class RangeTree<TKey, T> : IRangeTree<TKey, T>
        where TKey : IComparable<TKey>
        where T : IRangeProvider<TKey>
    {
        private RangeTreeNode<TKey, T> _root;
        private List<T> _items;
        private bool _isInSync;
        private bool _autoRebuild;
        private IComparer<T> _rangeComparer;

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
        public IEnumerable<T> Items
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
        public RangeTree(IComparer<T> rangeComparer = null) : this(null, rangeComparer) { }

        /// <summary>
        /// Initializes a tree with a list of items to be added.
        /// </summary>
        public RangeTree(IEnumerable<T> items, IComparer<T> rangeComparer = null)
        {
            this._rangeComparer = rangeComparer ?? new DefaultRangeComparer<TKey, T>();
            this._items = items != null ? items.ToList() : new List<T>();
            this._root = new RangeTreeNode<TKey, T>(this._items, rangeComparer);
            this._isInSync = true;
            this._autoRebuild = true;
        }

        /// <summary>
        /// Performans a "stab" query with a single value.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<T> Query(TKey value)
        {
            if (!this._isInSync && this._autoRebuild)
                this.Rebuild();

            return this._root.Query(value);
        }

        /// <summary>
        /// Performans a range query.
        /// All items with overlapping ranges are returned.
        /// </summary>
        public IEnumerable<T> Query(Range<TKey> range)
        {
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

            this._root = new RangeTreeNode<TKey, T>(this._items, this._rangeComparer);
            this._isInSync = true;
        }

        /// <summary>
        /// Adds the specified item. Tree will go out of sync.
        /// </summary>
        public void Add(T item)
        {
            this._isInSync = false;
            this._items.Add(item);
        }

        /// <summary>
        /// Adds the specified items. Tree will go out of sync.
        /// </summary>
        public void Add(IEnumerable<T> items)
        {
            this._isInSync = false;
            this._items.AddRange(items);
        }

        /// <summary>
        /// Removes the specified item. Tree will go out of sync.
        /// </summary>
        public void Remove(T item)
        {
            this._isInSync = false;
            this._items.Remove(item);
        }

        /// <summary>
        /// Removes the specified items. Tree will go out of sync.
        /// </summary>
        public void Remove(IEnumerable<T> items)
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
            this._root = new RangeTreeNode<TKey, T>(this._rangeComparer);
            this._items = new List<T>();
            this._isInSync = true;
        }
    }


}