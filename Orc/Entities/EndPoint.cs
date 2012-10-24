// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndPoint.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The EndPoint
//   We want this class to be immutable:
//   http://www.bluebytesoftware.com/blog/2007/11/11/ImmutableTypesForC.aspx
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Entities
{
    using System;
    using System.Collections.Generic;

    using Orc.Interface;

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
        private bool isMinEndPoint;

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

            this.isMinEndPoint = endPointType == EndPointType.Min;

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

            this.isMinEndPoint = endPointType == EndPointType.Min;

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
        public bool IsMinEndPoint
        {
            get
            {
                return this.isMinEndPoint;
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
        /// Compare two EndPoints with each other.
        /// </summary>
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

            var result = Value.CompareTo(other.Value);

            if (result == 0)
            {
                if (this.IsInclusive != other.IsInclusive)
                {
                    if (this.IsMinEndPoint)
                    {
                        result = this.IsInclusive ? -1 : +1;
                    }
                    else
                    {
                        result = this.IsInclusive ? +1 : -1;
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
            return other != null && Equals(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.EndPointType.GetHashCode();
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(this.Value);
                hashCode = (hashCode * 397) ^ this.IsInclusive.GetHashCode();
                return hashCode;
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

            return this.IsMinEndPoint ? this.Interval.Max : this.Interval.Min;
        }
        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            if (this.IsMinEndPoint)
            {
                if (this.IsInclusive)
                {
                    return "[" + Value;
                }

                return "]" + this.Value;
            }

            if (this.IsInclusive)
            {
                return this.Value + "]";
            }

            return this.Value + "[";
        }
    }
}