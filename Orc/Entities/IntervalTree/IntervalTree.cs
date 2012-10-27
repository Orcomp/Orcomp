namespace Orc.Entities.IntervalTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Interface;

    /// <summary>
    /// Tree capable of adding arbitrary intervals and performing search queries on them
    /// </summary>
    public partial class IntervalTree<T> : IIntervalContainer<T>, IEnumerable<Interval<T>> where T : struct, IComparable<T>
    {
        //TODO: Replace Interval<T> with IInterval<T> throughout.

        internal static IntervalNode<T> Sentinel = new IntervalNode<T>(new Interval<T>(default(T), default(T)));

        IntervalNode<T> Root
        {
            get;
            set;
        }

        public IntervalTree()
        {
            this.Root = Sentinel;
            this.Root.Left = Sentinel;
            this.Root.Right = Sentinel;
            this.Root.Parent = Sentinel;
        }

        #region Tree searching
        /// <summary>
        /// Search interval tree for a given point
        /// </summary>
        /// <param name="val">value to be searched for</param>
        /// <returns>list of intervals which contain the value</returns>
        public List<Interval<T>> Search(T val)
        {
            var result = new List<Interval<T>>();
            this.SearchSubtree(this.Root, val, result);
            return result;
        }

        public IEnumerable<IInterval<T>> Query(T value)
        {
            var result = new List<Interval<T>>();
            this.SearchSubtree(this.Root, value, result);
            return result.Cast<IInterval<T>>();
        }

        /// <summary>
        /// Search interval tree for intervals overlapping with given
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public List<Interval<T>> Search(Interval<T> i)
        {
            var result = new List<Interval<T>>();
            this.SearchSubtree(this.Root, i, result);
            return result;
        }

        public IEnumerable<IInterval<T>> Query(IInterval<T> interval)
        {
            var result = new List<Interval<T>>();
            this.SearchSubtree(this.Root, interval as Interval<T>, result);
            return result.Cast<IInterval<T>>();
        }

        /// <summary>
        /// Searches for the first overlapping interval
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Interval<T> SearchFirstOverlapping(Interval<T> i)
        {
            var node = this.Root;

            while (node != Sentinel && !node.Interval.Overlaps(i))
            {
                if (node.Left != Sentinel && node.Left.MaxEnd.CompareTo(i.Min) >= 0)
                {
                    node = node.Left;
                }
                else
                {
                    node = node.Right;
                }
            }

            if (node == Sentinel)
            {
                throw new KeyNotFoundException("No overlapping interval found.");
            }

            return node.Interval;
        }

        private void SearchSubtree(IntervalNode<T> node, Interval<T> i, List<Interval<T>> result)
        {
            if (node == Sentinel)
            {
                return;
            }

            if (node.Left != Sentinel)
            {
                this.SearchSubtree(node.Left, i, result);
            }

            if (i.Overlaps(node.Interval))
            {
                result.Add(node.Interval);
            }

            // Interval start is greater than largest endpoint in this subtree
            if (node.Right != Sentinel && i.Min.CompareTo(node.MaxEnd) <= 0)
            {
                this.SearchSubtree(node.Right, i, result);
            }
        }

        private IntervalNode<T> FindInterval(IntervalNode<T> tree, Interval<T> i)
        {

            while (tree != Sentinel)
            {

                if (tree.Interval.CompareTo(i) > 0)
                {
                    tree = tree.Left;
                    continue;
                }

                if (tree.Interval.CompareTo(i) < 0)
                {
                    tree = tree.Right;
                    continue;
                }

                if (tree.Interval.CompareTo(i) == 0)
                {
                    return tree;
                }
            }


            return Sentinel;

        }

        /// <summary>
        /// Recursively descends down the tree and adds valids results to the resultset
        /// </summary>
        /// <param name="node">subtree to be searched</param>
        /// <param name="val">value to be searched for</param>
        /// <param name="result">current resultset</param>
        private void SearchSubtree(IntervalNode<T> node, T val, List<Interval<T>> result)
        {
            if (node == Sentinel)
            {
                return;
            }

            // Value is higher than any interval in this subtree
            // TODO: Inclusiveness is ignored. Should use endpoint
            if (val.CompareTo(node.MaxEnd.Value) > 0)
            {
                return;
            }

            if (node.Left != Sentinel)
            {
                this.SearchSubtree(node.Left, val, result);
            }

            if (node.Interval.Contains(val))
            {
                result.Add(node.Interval);
            }

            // TODO: Inclusiveness is ignored. Should use endpoint
            if (val.CompareTo(node.Interval.Min.Value) < 0)
            {
                return;
            }

            if (node.Right != Sentinel)
            {
                this.SearchSubtree(node.Right, val, result);
            }
        }

        #endregion search

        /// <summary>
        /// Insert new interval to interval tree
        /// </summary>
        /// <param name="interval">interval to add</param>
        public void Add(Interval<T> interval)
        {
            var node = new IntervalNode<T>(interval);
            if (this.Root == Sentinel)
            {
                node.Color = NodeColor.BLACK;
                this.Root = node;
            }
            else
            {
                this.InsertInterval(interval, this.Root);
            }
        }

        public void Add(IInterval<T> interval )
        {
            this.Add(interval as Interval<T>);
        }

        #region Tree insertion internals

        /// <summary>
        /// Recursively descends to the correct spot for interval insertion in the tree
        /// When a free spot is found for the node, it is attached and tree state is validated
        /// </summary>
        /// <param name="interval">interval to be added</param>
        /// <param name="currentNode">subtree accessed in recursion</param>
        private void InsertInterval(Interval<T> interval, IntervalNode<T> currentNode)
        {
            IntervalNode<T> addedNode = Sentinel;
            if (interval.CompareTo(currentNode.Interval) < 0)
            {
                if (currentNode.Left == Sentinel)
                {
                    addedNode = new IntervalNode<T>(interval);
                    addedNode.Color = NodeColor.RED;
                    currentNode.Left = addedNode;
                    addedNode.Parent = currentNode;
                }
                else
                {
                    this.InsertInterval(interval, currentNode.Left);
                    return;
                }
            }
            else if (interval.CompareTo(currentNode.Interval) > 0)
            {
                if (currentNode.Right == Sentinel)
                {
                    addedNode = new IntervalNode<T>(interval);
                    addedNode.Color = NodeColor.RED;
                    currentNode.Right = addedNode;
                    addedNode.Parent = currentNode;
                }
                else
                {
                    this.InsertInterval(interval, currentNode.Right);
                    return;
                }
            }
            else
            {
                return;
            }

            addedNode.Parent.RecalculateMaxEnd();

            this.RenewConstraintsAfterInsert(addedNode);

            this.Root.Color = NodeColor.BLACK;
        }

        /// <summary>
        /// Validates and applies RB-tree constaints to node 
        /// </summary>
        /// <param name="node">node to be validated and fixed</param>
        private void RenewConstraintsAfterInsert(IntervalNode<T> node)
        {
            if (node.Parent == Sentinel)
            {
                return;
            }

            if (node.Parent.Color == NodeColor.BLACK)
            {
                return;
            }

            var uncle = node.Uncle;

            if (uncle != Sentinel && uncle.Color == NodeColor.RED)
            {
                node.Parent.Color = uncle.Color = NodeColor.BLACK;

                var gparent = node.GrandParent;
                if (gparent != Sentinel && !gparent.IsRoot)
                {
                    gparent.Color = NodeColor.RED;
                    this.RenewConstraintsAfterInsert(gparent);
                }
            }
            else
            {
                if (node.ParentDirection == NodeDirection.LEFT && node.Parent.ParentDirection == NodeDirection.RIGHT)
                {
                    this.RotateLeft(node.Parent);
                    node = node.Left;
                }
                else if (node.ParentDirection == NodeDirection.RIGHT && node.Parent.ParentDirection == NodeDirection.LEFT)
                {
                    this.RotateRight(node.Parent);
                    node = node.Right;
                }

                node.Parent.Color = NodeColor.BLACK;

                if (node.GrandParent == Sentinel)
                {
                    return;
                }
                node.GrandParent.Color = NodeColor.RED;

                if (node.ParentDirection == NodeDirection.RIGHT)
                {
                    this.RotateRight(node.GrandParent);
                }
                else
                {
                    this.RotateLeft(node.GrandParent);
                }
            }

        }

        #endregion Tree insertion internals

        /// <summary>
        /// Removes interval from tree (if present in tree)
        /// </summary>
        /// <param name="?"></param>
        public void Remove(Interval<T> interval)
        {
            this.RemoveNode(this.FindInterval(this.Root, interval));
        }

        public void Remove(IInterval<T> interval)
        {
            this.Remove(interval as Interval<T>);
        }

        private void RemoveNode(IntervalNode<T> node)
        {
            if (node == Sentinel)
            {
                return;
            }

            IntervalNode<T> temp = node;
            if (node.Right != Sentinel && node.Left != Sentinel)
            {
                // Trick when deleting node with both children, switch it with closest in order node
                // swap values and delete the bottom node converting it to other cases

                temp = node.GetSuccessor();
                node.Interval = temp.Interval;

                node.RecalculateMaxEnd();
                while (node.Parent != Sentinel)
                {
                    node = node.Parent;
                    node.RecalculateMaxEnd();
                }
            }
            node = temp;
            temp = node.Left != Sentinel ? node.Left : node.Right;

            // we will replace node with temp and delete node
            temp.Parent = node.Parent;
            if (node.IsRoot)
            {
                this.Root = temp; // Set new root
            }
            else
            {

                // Reattach node to parent
                if (node.ParentDirection == NodeDirection.RIGHT)
                {
                    node.Parent.Left = temp;
                }
                else
                {
                    node.Parent.Right = temp;
                }

                IntervalNode<T> maxAux = node.Parent;
                maxAux.RecalculateMaxEnd();
                while (maxAux.Parent != Sentinel)
                {
                    maxAux = maxAux.Parent;
                    maxAux.RecalculateMaxEnd();
                }
            }

            if (node.Color == NodeColor.BLACK)
            {
                this.RenewConstraintsAfterDelete(temp);
            }

        }

        /// <summary>
        /// Ensures constraints still apply after node deletion
        /// 
        ///  - made with the help of algorithm from Cormen et Al. Introduction to Algorithms 2nd ed.
        /// </summary>
        /// <param name="node"></param>
        private void RenewConstraintsAfterDelete(IntervalNode<T> node)
        {
            // Need to bubble up and fix
            while (node != this.Root && node.Color == NodeColor.BLACK)
            {
                if (node.ParentDirection == NodeDirection.RIGHT)
                {
                    IntervalNode<T> aux = node.Parent.Right;
                    if (aux.Color == NodeColor.RED)
                    {
                        aux.Color = NodeColor.BLACK;
                        node.Parent.Color = NodeColor.RED;
                        this.RotateLeft(node.Parent);
                        aux = node.Parent.Right;
                    }

                    if (aux.Left.Color == NodeColor.BLACK && aux.Right.Color == NodeColor.BLACK)
                    {
                        aux.Color = NodeColor.RED;
                        node = node.Parent;
                    }
                    else
                    {
                        if (aux.Right.Color == NodeColor.BLACK)
                        {
                            aux.Left.Color = NodeColor.BLACK;
                            aux.Color = NodeColor.RED;
                            this.RotateRight(aux);
                            aux = node.Parent.Right;
                        }

                        aux.Color = node.Parent.Color;
                        node.Parent.Color = NodeColor.BLACK;
                        aux.Right.Color = NodeColor.BLACK;
                        this.RotateLeft(node.Parent);
                        node = this.Root;
                    }
                }
                else
                {
                    IntervalNode<T> aux = node.Parent.Left;
                    if (aux.Color == NodeColor.RED)
                    {
                        aux.Color = NodeColor.BLACK;
                        node.Parent.Color = NodeColor.RED;
                        this.RotateRight(node.Parent);
                        aux = node.Parent.Left;
                    }

                    if (aux.Left.Color == NodeColor.BLACK && aux.Right.Color == NodeColor.BLACK)
                    {
                        aux.Color = NodeColor.RED;
                        node = node.Parent;
                    }
                    else
                    {
                        if (aux.Left.Color == NodeColor.BLACK)
                        {
                            aux.Right.Color = NodeColor.BLACK;
                            aux.Color = NodeColor.RED;
                            this.RotateLeft(aux);
                            aux = node.Parent.Left;
                        }

                        aux.Color = node.Parent.Color;
                        node.Parent.Color = NodeColor.BLACK;
                        aux.Left.Color = NodeColor.BLACK;
                        this.RotateRight(node.Parent);
                        node = this.Root;
                    }
                }
            }

            node.Color = NodeColor.BLACK;
        }

        /// <summary>
        /// General right rotation
        /// </summary>
        /// <param name="node">Top of rotated subtree</param>
        private void RotateRight(IntervalNode<T> node)
        {
            var pivot = node.Left;
            NodeDirection dir = node.ParentDirection;
            var parent = node.Parent;
            var tempTree = pivot.Right;
            pivot.Right = node;
            node.Parent = pivot;
            node.Left = tempTree;
            if (tempTree != Sentinel)
            {
                tempTree.Parent = node;
            }

            if (dir == NodeDirection.LEFT)
            {
                parent.Right = pivot;
            }
            else if (dir == NodeDirection.RIGHT)
            {
                parent.Left = pivot;
            }
            else
            {
                this.Root = pivot;
            }

            pivot.Parent = parent;

            pivot.RecalculateMaxEnd();
            node.RecalculateMaxEnd();

        }

        /// <summary>
        /// General left rotation
        /// </summary>
        /// <param name="node">top of rotated subtree</param>
        private void RotateLeft(IntervalNode<T> node)
        {
            var pivot = node.Right;
            NodeDirection dir = node.ParentDirection;
            var parent = node.Parent;
            var tempTree = pivot.Left;
            pivot.Left = node;
            node.Parent = pivot;
            node.Right = tempTree;
            if (tempTree != Sentinel)
            {
                tempTree.Parent = node;
            }

            if (dir == NodeDirection.LEFT)
            {
                parent.Right = pivot;
            }
            else if (dir == NodeDirection.RIGHT)
            {
                parent.Left = pivot;
            }
            else
            {
                this.Root = pivot;
            }

            pivot.Parent = parent;

            pivot.RecalculateMaxEnd();
            node.RecalculateMaxEnd();
        }

        #region Enumerators
        private IEnumerable<Interval<T>> InOrderWalk(IntervalNode<T> node)
        {
            if (node.Left != Sentinel)
            {
                foreach (Interval<T> val in this.InOrderWalk(node.Left))
                {
                    yield return val;
                }
            }

            if (node != Sentinel)
            {
                yield return node.Interval;
            }

            if (node.Right != Sentinel)
            {
                foreach (Interval<T> val in this.InOrderWalk(node.Right))
                {
                    yield return val;
                }
            }
        }

        public IEnumerator<Interval<T>> GetEnumerator()
        {
            foreach (Interval<T> val in this.InOrderWalk(this.Root))
            {
                yield return val;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}