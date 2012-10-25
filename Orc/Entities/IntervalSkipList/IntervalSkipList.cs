/*
 * Copyright (c) 2011, Benjamin Jacob Coverston
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 *
 *     Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 *     Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

/*
 * Ported to C# By Babak Gh
 */



namespace Orc.Entities.IntervalSkipList
{
    using System;
    using System.Collections.Generic;

    public class IntervalSkipList<T> where T : IComparable<T>
    {
        private readonly IntervalNode<T> _head;

        public IntervalSkipList(List<Interval<T>> intervals)
        {
            this._head = new IntervalNode<T>(intervals);
        }

        public LinkedList<Interval<T>> Search(Interval<T> searchInterval)
        {
            var retlist = new LinkedList<Interval<T>>();

            this.SearchInternal(this._head, searchInterval, retlist);
            return retlist;
        }

        protected void SearchInternal(IntervalNode<T> node, Interval<T> searchInterval, LinkedList<Interval<T>> retList)
        {
            if (null == node)
            {
                return;
            }

            if (node.SpanningInterval.Overlaps(searchInterval))
            {

                //if searchInterval.contains(node.v_pt)
                //then add every interval contained in this node to the result set then search left and right for further
                //overlapping intervals
                if (searchInterval.Contains(node.Point))
                {
                    foreach (var interval in node.LeftIntervals)
                    {
                        retList.AddLast(interval);
                    }

                    this.SearchInternal(node.LeftNode, searchInterval, retList);
                    this.SearchInternal(node.RightNode, searchInterval, retList);
                    return;
                }

                // point < searchInterval.left
                if (node.Point.CompareTo(searchInterval.Min.Value) < 0)
                {
                    // TODO: Could probably do a binary search to speed things up.

                    foreach (var interval in node.RightIntervals)
                    {
                        if (interval.Max.CompareTo(searchInterval.Min) >= 0)
                        {
                            retList.AddLast(interval);
                        }
                    }
                    return;
                }

                // point > searchInterval.right
                if (node.Point.CompareTo(searchInterval.Max.Value) > 0)
                {
                    foreach (var interval in node.LeftIntervals)
                    {
                        if (interval.Min.CompareTo(searchInterval.Max) <= 0)
                        {
                            retList.AddLast(interval);
                        }
                    }
                    return;
                }
            }

            //if v.pt < searchInterval.left
            //add intervals in v with v[i].right >= searchitnerval.left
            //L contains no overlaps
            //R May
            if (node.Point.CompareTo(searchInterval.Min.Value) < 0)
            {
                foreach (var interval in node.RightIntervals)
                {
                    if (interval.Max.CompareTo(searchInterval.Min) >= 0)
                    {
                        retList.AddLast(interval);
                    }
                    else break;
                }
                this.SearchInternal(node.RightNode, searchInterval, retList);
                return;
            }

            //if v.pt > searchInterval.right
            //add intervals in v with [i].left <= searchitnerval.right
            //R contains no overlaps
            //L May
            if (node.Point.CompareTo(searchInterval.Max.Value) > 0)
            {
                foreach (var interval in node.LeftIntervals)
                {
                    if (interval.Min.CompareTo(searchInterval.Max) <= 0)
                    {
                        retList.AddLast(interval);
                    }
                    else break;
                }

                this.SearchInternal(node.LeftNode, searchInterval, retList);
                return;
            }
        }
    }
}
