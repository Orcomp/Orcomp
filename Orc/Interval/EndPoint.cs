﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndPoint.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The EndPoint
//   We want this class to be immutable:
//   http://www.bluebytesoftware.com/blog/2007/11/11/ImmutableTypesForC.aspx
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Interval
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Orc.Interval.Interface;

    /// <summary>
    /// The EndPoint of an interval.
    /// </summary>
    /// <typeparam name="T">
    /// T must be a comparable type.
    /// </typeparam>
    public class EndPoint<T> : IEndPoint<T>, IEquatable<EndPoint<T>>
        where T : IComparable<T>
    {
        /// <summary>
        /// Backing field for EndPointType property
        /// </summary>
        private readonly EndPointType endPointType;

        /// <summary>
        /// Backing field for Interval property
        /// </summary>
        private readonly IInterval<T> interval;

        /// <summary>
        /// Backing field for IsInclusive property
        /// </summary>
        private readonly bool isInclusive;

        /// <summary>
        /// Backing field for IsMineEndPoint property
        /// </summary>
        private readonly bool isMin;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndPoint{T}"/> class. 
        /// </summary>
        /// <param name="interval">
        /// The interval.
        /// </param>
        /// <param name="endPointType">
        /// The edge type.
        /// </param>
        public EndPoint(IInterval<T> interval, EndPointType endPointType)
        {
            this.interval = interval;

            this.endPointType = endPointType;

            this.Value = endPointType == EndPointType.Min ? interval.Min.Value : interval.Max.Value;

            this.isMin = endPointType == EndPointType.Min;

            this.isInclusive = endPointType == EndPointType.Min ? interval.Min.IsInclusive : interval.Max.IsInclusive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndPoint{T}"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="endPointType">
        /// The end point type.
        /// </param>
        /// <param name="isInclusive">
        /// The is inclusive.
        /// </param>
        /// <param name="interval">
        /// The interval.
        /// </param>
        public EndPoint(T value, EndPointType endPointType, bool isInclusive, IInterval<T> interval)
        {
            this.interval = interval;
            this.endPointType = endPointType;

            this.Value = value;

            this.isMin = endPointType == EndPointType.Min;

            this.isInclusive = isInclusive;
        }

        /// <summary>
        /// Gets the edge type.
        /// </summary>
        /// <value>
        /// The edge type.
        /// </value>
        public EndPointType EndPointType
        {
            get
            {
                return this.endPointType;
            }
        }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        [IgnoreDataMember]
        public IInterval<T> Interval
        {
            get
            {
                return this.interval;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is inclusive.
        /// </summary>
        /// <value>
        /// The is inclusive.
        /// </value>
        public bool IsInclusive
        {
            get
            {
                return this.isInclusive;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the EndPoint is the Minimum.
        /// </summary>
        /// <value>
        /// The is start edge.
        /// </value>
        public bool IsMin
        {
            get
            {
                return this.isMin;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; private set; }

        public static bool operator !=(EndPoint<T> left, EndPoint<T> right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(EndPoint<T> left, EndPoint<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Compare two EndPoints with each other.
        ///     When sorting if two end points have the same value and are inclusive and you
        ///     want the min to come after max then use EndPointMinFirstComparer.
        /// </summary>
        /// <example>
        ///     [2 3][3 4]
        /// 
        ///     When comparing the max end point from the first interval with the min end point from the second interval.
        ///     Since both end points are inclusive and have the same value, the max end point will come first because it is 
        ///     in the *first* interval.
        /// 
        ///    NOTE: May need to revisit this later to see if we can change this behaviour, so start always comes before end.
        /// </example>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The result of the comparison between two EndPoints.
        /// </returns>
        public int CompareTo(IEndPoint<T> other)
        {
            if (other == null)
            {
                return 1;
            }

            var result = this.Value.CompareTo(other.Value);

            if (result == 0)
            {
                if (ReferenceEquals(this.interval, other.Interval))
                {
                    if (this.IsMin != other.IsMin)
                    {
                        result = this.IsMin ? -1 : 1;
                    }
                }
                else if (this.IsInclusive != other.IsInclusive)
                {
                    if (this.IsMin == !other.IsMin)
                    {
                        result = this.IsMin ? +1 : -1;
                    }
                    else if(this.isMin)
                    {
                        // Both endpoints are mins
                        result = this.IsInclusive ? -1 : +1;
                    }
                    else
                    {
                        // Both endpoints are max
                        result = this.IsInclusive ? +1 : -1;
                    }
                }
                else if(!this.isInclusive)
                {
                    // Both end points are exclusive
                    if (this.IsMin == !other.IsMin)
                    {
                        if (ReferenceEquals(this.interval, other.Interval))
                        {
                            result = this.isMin ? -1 : 1;
                        }
                        else
                        {
                            result = this.IsMin ? +1 : -1;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(EndPoint<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return this.EndPointType.Equals(other.EndPointType) && EqualityComparer<T>.Default.Equals(this.Value, other.Value) && this.IsInclusive.Equals(other.IsInclusive);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            var other = obj as EndPoint<T>;
            return other != null && this.Equals(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode

            unchecked
            {
                int hash = 17;

                //hash = hash * 23 + this.Interval.GetHashCode();
                hash = hash * 23 + this.EndPointType.GetHashCode();
                hash = hash * 23 + EqualityComparer<T>.Default.GetHashCode(this.Value);
                hash = hash * 23 + this.IsInclusive.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// The get other end point.
        /// </summary>
        /// <returns>
        /// The <see cref="IEndPoint{T}"/>.
        /// </returns>
        public IEndPoint<T> GetOtherEndPoint()
        {
            if (this.Interval == null)
            {
                return null;
            }

            return this.IsMin ? this.Interval.Max : this.Interval.Min;
        }
        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            if (this.IsMin)
            {
                if (this.IsInclusive)
                {
                    return "  [" + this.Value + "  ";
                }
                else
                {
                    return "  (" + this.Value + "  ";
                }
            }
            else
            {
                if (this.IsInclusive)
                {
                    return "  " + this.Value + "] ";
                }
                else
                {
                    return "  " + this.Value + ") ";
                }
            }
        }
    }
}
