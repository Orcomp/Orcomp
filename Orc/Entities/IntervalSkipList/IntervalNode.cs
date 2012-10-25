//using System.Threading.Tasks;

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
    using System.Linq;

    using Orc.Entities.SkipList;

    public class IntervalNode<T> where T : IComparable<T>
    {
        public T Point { get; set; }

        public T MaxPoint { get; set; }

        public T MinPoint { get; set; }

        public Interval<T> SpanningInterval { get; set; }

        public IntervalNode<T> LeftNode = null;
        public IntervalNode<T> RightNode = null;

        public List<Interval<T>> LeftIntervals { get; set; }

        public List<Interval<T>> RightIntervals { get; set; }

        public IntervalNode(List<Interval<T>> intervals)
        {
            if (intervals.Count == 0)
            {
                return;
            }

            this.Point = this.FindMedianEndpoint(intervals);

            this.LeftIntervals = this.GetIntersectingIntervals(intervals).OrderBy(x => x.Min).ToList();
            this.RightIntervals = this.GetIntersectingIntervals(intervals).OrderBy(x => x.Max).ToList();

            this.MinPoint = this.LeftIntervals.First().Min.Value;
            this.MaxPoint = this.RightIntervals.Last().Max.Value;

            this.SpanningInterval = new Interval<T>(this.MinPoint, this.MaxPoint);

            //if i.min < v_pt then it goes to the left subtree
            var leftSegment = this.GetLeftIntervals(intervals);
            var rightSegment = this.GetRightIntervals(intervals);

            if (leftSegment.Count > 0)
            {
                this.LeftNode = new IntervalNode<T>(leftSegment);
            }

            if (rightSegment.Count > 0)
            {
                this.RightNode = new IntervalNode<T>(rightSegment);
            }
        }

        public List<Interval<T>> GetLeftIntervals(List<Interval<T>> candidates)
        {
            return candidates.Where(candidate => candidate.Max.Value.CompareTo(this.Point) < 0).ToList();
        }

        public List<Interval<T>> GetRightIntervals(List<Interval<T>> candidates)
        {
            return candidates.Where(candidate => candidate.Min.Value.CompareTo(this.Point) > 0).ToList();
        }

        public List<Interval<T>> GetIntersectingIntervals(List<Interval<T>> candidates)
        {
            return candidates.Where(candidate => candidate.Min.Value.CompareTo(this.Point) <= 0 && candidate.Max.Value.CompareTo(this.Point) >= 0).ToList();
        }

        public T FindMedianEndpoint(List<Interval<T>> intervals)
        {
            var sortedSet = new BDSkipList<T, Interval<T>>();

            foreach (var interval in intervals)
            {
                try
                {
                    sortedSet.Add(interval.Min.Value, interval);
                }
                catch
                {
                    //Bend.BDSkilpList rasise exception on duplicatation
                }

                try
                {
                    sortedSet.Add(interval.Max.Value, interval);
                }
                catch
                {
                    //Bend.BDSkilpList rasise exception on duplicatation
                }
            }

            int medianIndex = sortedSet.Count / 2;

            if (sortedSet.Count > 0)
            {
                return sortedSet.ToList()[medianIndex].Key;
            }

            return default(T);
        }
    }
}
