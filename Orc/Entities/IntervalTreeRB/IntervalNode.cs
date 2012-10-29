namespace Orc.Entities.IntervalTreeRB
{
    using System;

    using Orc.Interface;

    /// <summary>
    /// Node of interval Tree
    /// </summary>
    /// <typeparam name="T">type of interval bounds</typeparam>
    internal class IntervalNode<T> : IComparable<IntervalNode<T>> where T : struct, IComparable<T>
    {

        public IntervalNode<T> Left { get; set; }
        public IntervalNode<T> Right { get; set; }
        public IntervalNode<T> Parent { get; set; }

        /// <summary>
        /// Maximum "end" value of interval in node subtree
        /// </summary>
        public IEndPoint<T> MaxEnd { get; set; }

        /// <summary>
        /// The interval this node holds
        /// </summary>
        public Interval<T> Interval;

        private NodeColor color;
        /// <summary>
        /// Color of the node used for R-B implementation
        /// </summary>
        public NodeColor Color
        {
            get { return this.color; }
            set
            {
                this.color = value;
            }

        }

        public IntervalNode()
        {
            this.Parent = this.Left = this.Right = IntervalTree<T>.Sentinel;
            this.Color = NodeColor.BLACK;
        }

        public IntervalNode(Interval<T> interval)
            : this()
        {
            this.MaxEnd = interval.Max;
            this.Interval = interval;
        }

        /// <summary>
        /// Indicates wheter the node has a parent
        /// </summary>
        public bool IsRoot
        {
            get { return this.Parent == IntervalTree<T>.Sentinel; }
        }

        /// <summary>
        /// Indicator whether the node has children
        /// </summary>
        public bool IsLeaf
        {
            get { return this.Right == IntervalTree<T>.Sentinel && this.Left == IntervalTree<T>.Sentinel; }
        }

        /// <summary>
        /// The direction of the parent, from the child point-of-view
        /// </summary>
        public NodeDirection ParentDirection
        {
            get
            {
                if (this.Parent == IntervalTree<T>.Sentinel)
                {
                    return NodeDirection.NONE;
                }

                return this.Parent.Left == this ? NodeDirection.RIGHT : NodeDirection.LEFT;
            }
        }

        public IntervalNode<T> GetSuccessor()
        {
            if (this.Right == IntervalTree<T>.Sentinel)
            {
                return IntervalTree<T>.Sentinel;
            }

            var node = this.Right;
            while (node.Left != IntervalTree<T>.Sentinel)
            {
                node = node.Left;
            }

            return node;
        }

        public int CompareTo(IntervalNode<T> other)
        {
            return this.Interval.CompareTo(other.Interval);
        }

        /// <summary>
        /// Refreshes the MaxEnd value after node manipulation
        /// 
        /// This is a local operation only
        /// </summary>
        public void RecalculateMaxEnd()
        {
            IEndPoint<T> max = this.Interval.Max;

            if (this.Right != IntervalTree<T>.Sentinel)
            {
                if (this.Right.MaxEnd.CompareTo(max) > 0)
                {
                    max = this.Right.MaxEnd;
                }
            }

            if (this.Left != IntervalTree<T>.Sentinel)
            {
                if (this.Left.MaxEnd.CompareTo(max) > 0)
                {
                    max = this.Left.MaxEnd;
                }
            }

            this.MaxEnd = max;
        }

        /// <summary>
        /// Return grandparent node
        /// </summary>
        /// <returns>grandparent node or IntervalTree<T>.Sentinel if none</returns>
        public IntervalNode<T> GrandParent
        {
            get
            {
                if (this.Parent != IntervalTree<T>.Sentinel)
                {
                    return this.Parent.Parent;
                }
                return IntervalTree<T>.Sentinel;
            }
        }

        /// <summary>
        /// Returns sibling of parent node
        /// </summary>
        /// <returns>sibling of parent node or IntervalTree<T>.Sentinel if none</returns>
        public IntervalNode<T> Uncle
        {
            get
            {
                var gparent = this.GrandParent;
                if (gparent == IntervalTree<T>.Sentinel)
                {
                    return IntervalTree<T>.Sentinel;
                }

                if (this.Parent == gparent.Left)
                {
                    return gparent.Right;
                }

                return gparent.Left;
            }
        }

        /// <summary>
        /// Returns sibling node
        /// </summary>
        /// <returns>sibling node or IntervalTree<T>.Sentinel if none</returns>
        public IntervalNode<T> Sibling
        {
            get
            {
                if (this.Parent != IntervalTree<T>.Sentinel)
                {
                    if (this.Parent.Right == this)
                    {
                        return this.Parent.Left;
                    }

                    return this.Parent.Right;
                }

                return IntervalTree<T>.Sentinel;
            }
        }
    }

    internal enum NodeColor
    {
        RED,
        BLACK
    }

    internal enum NodeDirection
    {
        LEFT,
        RIGHT,
        NONE
    }
}