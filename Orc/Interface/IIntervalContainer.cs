// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIntervalContainer.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The IntervalContainer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interface
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The IntervalContainer interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IIntervalContainer<T>
        where T : IComparable<T>
    {
        /// <summary>
        /// The add.
        /// </summary>
        void Add(IInterval<T> interval);

        /// <summary>
        /// The remove.
        /// </summary>
        void Remove(IInterval<T> interval);

        /// <summary>
        /// Query the interval container to see if the input interval overlaps
        /// any intervals in the container and returns the matches
        /// </summary>
        /// <param name="interval">
        /// The interval.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<IInterval<T>> Query(IInterval<T> interval);

        /// <summary>
        /// Query the interval container to see if the input value intersects 
        /// any intervals in the container and returns the matches
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<IInterval<T>> Query(T value);

        // Maybe bool Contains()
    }
}