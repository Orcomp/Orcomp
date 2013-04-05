using System;
using C5;
using SCG = System.Collections.Generic;

namespace Orc.DataStructures.C5
{
    using Orc.Interval.Interface;

    public interface IIntervaled<T> : ICollectionValue<IInterval<T>> where T : IComparable<T>
    {
        /// <summary>
        /// The smallest interval that spans all intervals in the collection.
        /// The lowest endpoint in the collection makes up the low endpoint of the span, and the highest the high endpoint. The span's endpoints are closed and open in accordance with the lowest and highest endpoint in the collection.
        /// <code>coll.Overlap(coll.Span())</code> will by definition return all intervals in the collection.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if called on an empty collection, as it can't have a span</exception>
        IInterval<T> Span { get; }

        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant).
        /// </summary>
        /// <value>A characterization of the speed of the 
        /// <code>Span</code> property in this collection.</value>
        Speed SpanSpeed { get; }

        // @design: made to spare the user of making a point interval ([q:q]) and allow for more effective implementations of the interface
        // TODO: param and return states nothing new...
        /// <summary>
        /// Create an enumerable, enumerating all intervals that overlap the query point.
        /// </summary>
        /// <param name="query">A query point</param>
        /// <returns>All intervals that overlap the query point</returns>
        /// <exception cref="NullReferenceException">Thrown if query is null</exception>
        SCG.IEnumerable<IInterval<T>> Overlap(T query);

        /// <summary>
        /// Create an enumerable, enumerating all intervals in the collection that overlap the query interval.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Thrown if query is null</exception>
        SCG.IEnumerable<IInterval<T>> Overlap(IInterval<T> query);

        // @design: only implemented with interval that it is most unlikely to query a single point. If that would be needed it would still be possible to query a point with a closed single-point interval ([q:q])
        /// <summary>
        /// Check if there exists an interval in the collection that overlaps the query point.
        /// </summary>
        /// <param name="query">The query point</param>
        /// <returns>True if at least one such interval exists</returns>
        bool OverlapExists(IInterval<T> query);

        /*
        // Removed for now. The methods are either covered by others or not useful. If support would be needed, this could be added again...
        /// <summary>
        ///  Check if there exists an interval in the collection that overlaps the query point and satisfies a
        /// specific predicate.
        
        /// </summary>
        /// <param name="query">The query point</param>
        /// <param name="predicate">A  delegate (<see><cref>T:Func`2</cref></see> with <code>R == bool</code>) defining the predicate</param>
        /// <returns>True if at least one such interval exists</returns>
        bool OverlapExists(IInterval<T> query, Func<IInterval<T>, bool> predicate);

        /// <summary>
        /// Create an enumerable, enumerating all intervals in the collection with both endpoints being overlaped by the query interval. 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        SCG.IEnumerable<IInterval<T>> Contained(IInterval<T> query);

        /// <summary>
        /// All intervals that start at the query point.
        /// Open/closed intervals are respected, meaning only left-closed intervals will be included if closed is set to true and vice versa.
        /// </summary>
        /// <param name="query">The low endpoint of the intervals</param>
        /// <param name="closed">If true only left-closed intervals will be included. If false only left-open intervals will be included.</param>
        /// <returns></returns>
        SCG.IEnumerable<IInterval<T>> Starts(T query, bool closed = true);

        /// <summary>
        /// All intervals that end at the query point.
        /// Open/closed intervals are respected, meaning only right-closed intervals will be included if closed is set to true and vice versa.
        /// </summary>
        /// <param name="query">The high endpoint of the intervals</param>
        /// <param name="closed">If true only right-closed intervals will be included. If false only right-open intervals will be included.</param>
        /// <returns></returns>
        SCG.IEnumerable<IInterval<T>> Ends(T query, bool closed = true);

        /// <summary>
        /// All overlapping pairs (that overlap a quwery interval) in the collection.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        //TODO: Use Tuple instead as both intervals are equal
        SCG.KeyValuePair<IInterval<T>, IInterval<T>> OverlappingPairs(IInterval<T> query = null);
        */
    }

    public interface IStaticIntervaled<T> : IIntervaled<T> where T : IComparable<T>
    {
        int OverlapCount(IInterval<T> query);
    }

    public interface IDynamicIntervaled<T> : IIntervaled<T>, ICollection<IInterval<T>> where T : IComparable<T>
    {

    }
}