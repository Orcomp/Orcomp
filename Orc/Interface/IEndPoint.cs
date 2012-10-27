// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEndPoint.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The EndPoint interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Interface
{
    using System;

    using Orc.Entities;

    /// <summary>
    /// The EndPoint interface.
    /// </summary>
    /// <typeparam name="T">
    /// T must be comparable.
    /// </typeparam>
    public interface IEndPoint<T> : IComparable<IEndPoint<T>>
        where T : IComparable<T>
    {
        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        IInterval<T> Interval { get; }

        /// <summary>
        /// Gets the edge type.
        /// </summary>
        /// <value>
        /// The edge type.
        /// </value>
        EndPointType EndPointType { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        T Value { get; }

        /// <summary>
        /// Gets a value indicating whether is inclusive.
        /// </summary>
        /// <value>
        /// The is inclusive.
        /// </value>
        bool IsInclusive { get; }

        /// <summary>
        /// Returns whether the EndPoint is min or max
        /// </summary>
        Boolean IsMinEndPoint { get; }
    }
}