namespace Orc.Entities.IntervalTree
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Orc.Interface;
    using Orc.Extensions;

    // based on http://www.superstarcoders.com/blogs/posts/efficient-avl-tree-in-c-sharp.aspx

    public class IntervalTree<TKey, TEdge, TValue> : IEnumerable<TValue>
        where TKey : class, IInterval<TEdge>
        where TEdge : IComparable<TEdge>
    {
        private IComparer<TKey> _cmp_keys = Comparer<TKey>.Default;
        private IComparer<TEdge> _cmp_edges = Comparer<TEdge>.Default;
        private IntervalNode _root;

        public IntervalTree(IComparer<TKey> cmp_keys = null, IComparer<TEdge> cmp_edges = null)
        {
            this._cmp_keys = cmp_keys ?? this._cmp_keys;
            this._cmp_edges = cmp_edges ?? this._cmp_edges;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return new IntervalNodeEnumerator(this._root);
        }

        public void Clear()
        {
            this._root = null;
        }

        public bool Search(TKey key, out TValue value)
        {
            IntervalNode node = this._root;

            while (node != null)
            {
                if (this._cmp_keys.Compare(key, node.Key) < 0)
                {
                    node = node.Left;
                }
                else if (this._cmp_keys.Compare(key, node.Key) > 0)
                {
                    node = node.Right;
                }
                else
                {
                    value = node.Value;

                    return true;
                }
            }

            value = default(TValue);

            return false;
        }

        public IEnumerable<TKey> GetOverlaps(TKey key)
        {
            var ovelaps = new List<TKey>();
            this.GetOverlaps(this._root, key, ovelaps);

            return ovelaps;
        }

        private void GetOverlaps(IntervalNode node, TKey key, List<TKey> overlaps)
        {
            if (node == null)
                return;

            if (this._cmp_edges.Compare(key.Min.Value, node.Max) > 0)
                return;

            if (key.Overlaps(node.Key))
            {
                var overlap = key.GetOverlap(node.Key) as TKey;
                if (overlap != null)
                    overlaps.Add(overlap);
            }

            this.GetOverlaps(node.Left, key, overlaps);

            if (this._cmp_edges.Compare(key.Max.Value, node.Key.Min.Value) > 0)
            {
                this.GetOverlaps(node.Right, key, overlaps);
            }
        }

        public IEnumerable<TKey> GetOverlappingKeys(TKey key)
        {
            return this.GetOverlaps(key);
        }

        public IEnumerable<TValue> GetOverlappingValues(TKey key)
        {
            return this.GetOverlaps(key).Select(k =>
            {
                TValue v;
                return (this.Search(k, out v) ? v : default(TValue));
            });
        }

        public void Insert(TKey key, TValue value)
        {
            if (this._root == null)
            {
                this._root = new IntervalNode { Key = key, Value = value };
                this.UpdateMax(this._root, false);
            }
            else
            {
                IntervalNode node = this._root;

                while (node != null)
                {
                    int compare = this._cmp_keys.Compare(key, node.Key);

                    if (compare < 0)
                    {
                        IntervalNode left = node.Left;

                        if (left == null)
                        {
                            node.Left = new IntervalNode { Key = key, Value = value, Parent = node };

                            this.InsertBalance(node, 1);

                            return;
                        }
                        else
                        {
                            node = left;
                        }
                    }
                    else if (compare > 0)
                    {
                        IntervalNode right = node.Right;

                        if (right == null)
                        {
                            node.Right = new IntervalNode { Key = key, Value = value, Parent = node };

                            this.InsertBalance(node, -1);

                            return;
                        }
                        else
                        {
                            node = right;
                        }
                    }
                    else
                    {
                        node.Value = value;

                        return;
                    }
                }
            }
        }

        private void InsertBalance(IntervalNode node, int balance)
        {
            while (node != null)
            {
                balance = (node.Balance += balance);

                if (balance == 0)
                {
                    return;
                }
                else if (balance == 2)
                {
                    if (node.Left.Balance == 1)
                    {
                        this.RotateLeft(node, true);
                    }
                    else
                    {
                        this.RotateLeftRight(node, true);
                    }

                    return;
                }
                else if (balance == -2)
                {
                    if (node.Right.Balance == -1)
                    {
                        this.RotateRight(node, true);
                    }
                    else
                    {
                        this.RotateRightLeft(node, true);
                    }

                    return;
                }

                IntervalNode parent = node.Parent;

                if (parent != null)
                {
                    balance = parent.Left == node ? 1 : -1;
                }

                node = parent;
            }
        }

        public bool Delete(TKey key)
        {
            IntervalNode node = this._root;

            while (node != null)
            {
                if (this._cmp_keys.Compare(key, node.Key) < 0)
                {
                    node = node.Left;
                }
                else if (this._cmp_keys.Compare(key, node.Key) > 0)
                {
                    node = node.Right;
                }
                else
                {
                    IntervalNode left = node.Left;
                    IntervalNode right = node.Right;

                    if (left == null)
                    {
                        if (right == null)
                        {
                            if (node == this._root)
                            {
                                this._root = null;
                            }
                            else
                            {
                                IntervalNode parent = node.Parent;

                                if (parent.Left == node)
                                {
                                    parent.Left = null;
                                    this.DeleteBalance(parent, -1);
                                }
                                else
                                {
                                    parent.Right = null;
                                    this.DeleteBalance(parent, 1);
                                }
                            }
                        }
                        else
                        {
                            Replace(node, right);
                            this.DeleteBalance(node, 0);
                        }
                    }
                    else if (right == null)
                    {
                        Replace(node, left);
                        this.DeleteBalance(node, 0);
                    }
                    else
                    {
                        IntervalNode successor = right;

                        if (successor.Left == null)
                        {
                            IntervalNode parent = node.Parent;

                            successor.Parent = parent;
                            successor.Left = left;
                            successor.Balance = node.Balance;

                            left.Parent = successor;

                            if (node == this._root)
                            {
                                this._root = successor;
                            }
                            else
                            {
                                if (parent.Left == node)
                                {
                                    parent.Left = successor;
                                }
                                else
                                {
                                    parent.Right = successor;
                                }
                            }

                            this.DeleteBalance(successor, 1);
                        }
                        else
                        {
                            while (successor.Left != null)
                            {
                                successor = successor.Left;
                            }

                            IntervalNode parent = node.Parent;
                            IntervalNode successorParent = successor.Parent;
                            IntervalNode successorRight = successor.Right;

                            if (successorParent.Left == successor)
                            {
                                successorParent.Left = successorRight;
                            }
                            else
                            {
                                successorParent.Right = successorRight;
                            }

                            if (successorRight != null)
                            {
                                successorRight.Parent = successorParent;
                            }

                            successor.Parent = parent;
                            successor.Left = left;
                            successor.Right = right;
                            successor.Balance = node.Balance;

                            right.Parent = successor;
                            left.Parent = successor;

                            if (node == this._root)
                            {
                                this._root = successor;
                            }
                            else
                            {
                                if (parent.Left == node)
                                {
                                    parent.Left = successor;
                                }
                                else
                                {
                                    parent.Right = successor;
                                }
                            }

                            this.DeleteBalance(successorParent, -1);
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private void DeleteBalance(IntervalNode node, int balance)
        {
            while (node != null)
            {
                balance = (node.Balance += balance);

                if (balance == 2)
                {
                    if (node.Left.Balance >= 0)
                    {
                        node = this.RotateLeft(node, false);

                        if (node.Balance == -1)
                        {
                            this.UpdateMax(node, true);
                            return;
                        }
                    }
                    else
                    {
                        node = this.RotateLeftRight(node, false);
                    }
                }
                else if (balance == -2)
                {
                    if (node.Right.Balance <= 0)
                    {
                        node = this.RotateRight(node, false);

                        if (node.Balance == 1)
                        {
                            this.UpdateMax(node, true);
                            return;
                        }
                    }
                    else
                    {
                        node = this.RotateRightLeft(node, false);
                    }
                }
                else if (balance != 0)
                {
                    this.UpdateMax(node, true);
                    return;
                }
                else
                {
                    this.UpdateMax(node, false);
                }

                IntervalNode parent = node.Parent;

                if (parent != null)
                {
                    balance = parent.Left == node ? -1 : 1;
                }

                node = parent;
            }
        }

        private IntervalNode RotateRight(IntervalNode node, bool walk)
        {
            IntervalNode right = node.Right;
            IntervalNode rightLeft = right.Left;
            IntervalNode parent = node.Parent;

            right.Parent = parent;
            right.Left = node;
            node.Right = rightLeft;
            node.Parent = right;

            if (rightLeft != null)
            {
                rightLeft.Parent = node;
            }

            if (node == this._root)
            {
                this._root = right;
            }
            else if (parent.Right == node)
            {
                parent.Right = right;
            }
            else
            {
                parent.Left = right;
            }

            right.Balance++;
            node.Balance = -right.Balance;

            if (walk)
            {
                this.UpdateMax(node, true);
            }
            else
            {
                this.UpdateMax(node, false);
                this.UpdateMax(right, false);
            }

            return right;
        }

        private IntervalNode RotateLeft(IntervalNode node, bool walk)
        {
            IntervalNode left = node.Left;
            IntervalNode leftRight = left.Right;
            IntervalNode parent = node.Parent;

            left.Parent = parent;
            left.Right = node;
            node.Left = leftRight;
            node.Parent = left;

            if (leftRight != null)
            {
                leftRight.Parent = node;
            }

            if (node == this._root)
            {
                this._root = left;
            }
            else if (parent.Left == node)
            {
                parent.Left = left;
            }
            else
            {
                parent.Right = left;
            }

            left.Balance--;
            node.Balance = -left.Balance;

            if (walk)
            {
                this.UpdateMax(node, true);
            }
            else
            {
                this.UpdateMax(node, false);
                this.UpdateMax(left, false);
            }

            return left;
        }

        private IntervalNode RotateLeftRight(IntervalNode node, bool walk)
        {
            IntervalNode left = node.Left;
            IntervalNode leftRight = left.Right;
            IntervalNode parent = node.Parent;
            IntervalNode leftRightRight = leftRight.Right;
            IntervalNode leftRightLeft = leftRight.Left;

            leftRight.Parent = parent;
            node.Left = leftRightRight;
            left.Right = leftRightLeft;
            leftRight.Left = left;
            leftRight.Right = node;
            left.Parent = leftRight;
            node.Parent = leftRight;

            if (leftRightRight != null)
            {
                leftRightRight.Parent = node;
            }

            if (leftRightLeft != null)
            {
                leftRightLeft.Parent = left;
            }

            if (node == this._root)
            {
                this._root = leftRight;
            }
            else if (parent.Left == node)
            {
                parent.Left = leftRight;
            }
            else
            {
                parent.Right = leftRight;
            }

            if (leftRight.Balance == -1)
            {
                node.Balance = 0;
                left.Balance = 1;
            }
            else if (leftRight.Balance == 0)
            {
                node.Balance = 0;
                left.Balance = 0;
            }
            else
            {
                node.Balance = -1;
                left.Balance = 0;
            }

            leftRight.Balance = 0;

            if (walk)
            {
                this.UpdateMax(left, false);
                this.UpdateMax(node, true);
            }
            else
            {
                this.UpdateMax(left, false);
                this.UpdateMax(node, false);
                this.UpdateMax(leftRight, false);
            }


            return leftRight;
        }

        private IntervalNode RotateRightLeft(IntervalNode node, bool walk)
        {
            IntervalNode right = node.Right;
            IntervalNode rightLeft = right.Left;
            IntervalNode parent = node.Parent;
            IntervalNode rightLeftLeft = rightLeft.Left;
            IntervalNode rightLeftRight = rightLeft.Right;

            rightLeft.Parent = parent;
            node.Right = rightLeftLeft;
            right.Left = rightLeftRight;
            rightLeft.Right = right;
            rightLeft.Left = node;
            right.Parent = rightLeft;
            node.Parent = rightLeft;

            if (rightLeftLeft != null)
            {
                rightLeftLeft.Parent = node;
            }

            if (rightLeftRight != null)
            {
                rightLeftRight.Parent = right;
            }

            if (node == this._root)
            {
                this._root = rightLeft;
            }
            else if (parent.Right == node)
            {
                parent.Right = rightLeft;
            }
            else
            {
                parent.Left = rightLeft;
            }

            if (rightLeft.Balance == 1)
            {
                node.Balance = 0;
                right.Balance = -1;
            }
            else if (rightLeft.Balance == 0)
            {
                node.Balance = 0;
                right.Balance = 0;
            }
            else
            {
                node.Balance = 1;
                right.Balance = 0;
            }

            rightLeft.Balance = 0;

            if (walk)
            {
                this.UpdateMax(right, false);
                this.UpdateMax(node, true);
            }
            else
            {
                this.UpdateMax(right, false);
                this.UpdateMax(node, false);
                this.UpdateMax(rightLeft, false);
            }

            return rightLeft;
        }

        private void UpdateMax(IntervalNode node, bool walk)
        {
            while (node != null)
            {
                node = this.UpdateMax(node) & walk ? node.Parent : null;
            }
        }

        private bool UpdateMax(IntervalNode node)
        {
            if (node == null)
                return false;

            var old_max = node.Max;

            if ((node.Left == null) && (node.Right == null))
                node.Max = node.Key.Max.Value;
            else if (node.Left == null)
                node.Max = node.Right.Max;
            else if (node.Right == null)
                node.Max = node.Left.Max;
            else
                node.Max = this._cmp_edges.Compare(node.Left.Key.Max.Value, node.Right.Key.Max.Value) > 0 ? node.Left.Key.Max.Value : node.Right.Key.Max.Value;

            return (this._cmp_edges.Compare(node.Max, old_max) != 0);
        }

        private static void Replace(IntervalNode target, IntervalNode source)
        {
            IntervalNode left = source.Left;
            IntervalNode right = source.Right;

            target.Balance = source.Balance;
            target.Key = source.Key;
            target.Value = source.Value;
            target.Left = left;
            target.Right = right;

            if (left != null)
            {
                left.Parent = target;
            }

            if (right != null)
            {
                right.Parent = target;
            }
        }

        sealed class IntervalNode
        {
            public IntervalNode Parent;
            public IntervalNode Left;
            public IntervalNode Right;
            public TKey Key;
            public TEdge Max;
            public TValue Value;
            public int Balance;
        }

        sealed class IntervalNodeEnumerator : IEnumerator<TValue>
        {
            private IntervalNode _root;
            private Action _action;
            private IntervalNode _current;
            private IntervalNode _right;

            public IntervalNodeEnumerator(IntervalNode root)
            {
                this._right = this._root = root;

                this._action = root == null ? Action.End : Action.Right;
            }

            public bool MoveNext()
            {
                switch (this._action)
                {
                    case Action.Right:
                        this._current = this._right;

                        while (this._current.Left != null)
                        {
                            this._current = this._current.Left;
                        }

                        this._right = this._current.Right;

                        this._action = this._right != null ? Action.Right : Action.Parent;

                        return true;
                    case Action.Parent:
                        while (this._current.Parent != null)
                        {
                            IntervalNode previous = this._current;

                            this._current = this._current.Parent;

                            if (this._current.Left == previous)
                            {
                                this._right = this._current.Right;

                                this._action = this._right != null ? Action.Right : Action.Parent;

                                return true;
                            }
                        }

                        this._action = Action.End;

                        return false;
                    default:
                        return false;
                }
            }

            public void Reset()
            {
                this._right = this._root;

                this._action = this._root == null ? Action.End : Action.Right;
            }

            public TValue Current
            {
                get
                {
                    return this._current.Value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Dispose()
            {

            }

            enum Action
            {
                Parent,
                Right,
                End
            }
        }
    }
}