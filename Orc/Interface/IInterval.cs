// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInterval.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The Interval interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interface
{
    using System;

    /// <summary>
    /// The Interval interface.
    /// </summary>
    /// <typeparam name="T">
    /// T must be comparable.
    /// </typeparam>
    public interface IInterval<T> : IEquatable<IInterval<T>>, IComparable<IInterval<T>>
        where T : IComparable<T>
    {
        /// <summary>
        /// Gets the interval start endPoint value.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        IEndPoint<T> Min { get; }

        /// <summary>
        /// Gets the interval end endPoint value.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        IEndPoint<T> Max { get; }

        /// <summary>
        /// Contains() returns a boolean on whether the specified value is contained in the interval.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Contains(T value);

        /// <summary>
        /// Contains() returns a boolean on whether the specified interval is contained in the parent interval
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool Contains(IInterval<T> other);

        /// <summary>
        /// Overlaps() returns a boolean on whether another interval overlaps the interval.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Overlaps(IInterval<T> other);

        /// <summary>
        /// Return the overlap with other interval.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IInterval<T> GetOverlap(IInterval<T> other);
    }
}