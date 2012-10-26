namespace Orc.Entities.IntervalTreeVvondra
{
    using System;

    /// <summary>
    /// Representation of bounded interval
    /// </summary>
    /// <typeparam name="T">type of interval bounds</typeparam>
    public struct Interval<T> : IComparable<Interval<T>> where T : struct, IComparable<T>
    {
        public T Start
        {
            get;
            set;
        }

        public T End
        {
            get;
            set;
        }

        public Interval(T start, T end)
            : this()
        {
            this.Start = start;
            this.End = end;

            if (this.Start.CompareTo(this.End) > 0)
            {
                throw new ArgumentException("Start cannot be larger than End of interval");
            }
        }

        /// <summary>
        /// Tests if interval contains given interval
        /// </summary>
        public bool Contains(Interval<T> interval)
        {
            return this.Start.CompareTo(interval.Start) <= 0 && this.End.CompareTo(interval.End) >= 0;
        }

        /// <summary>
        /// Tests if interval contains given value
        /// </summary>
        public bool Contains(T val)
        {
            return this.Start.CompareTo(val) <= 0 && this.End.CompareTo(val) >= 0;
        }

        public bool Overlaps(Interval<T> interval)
        {
            return this.Start.CompareTo(interval.End) <= 0 && this.End.CompareTo(interval.Start) >= 0;
        }

        /// <summary>
        /// Porovná dva intervaly
        /// 
        /// Lineární uspořádání je def. 1) pořadím počátků 2) pořadím konců
        /// </summary>
        /// <param name="i">interval k porovnání</param>
        /// <returns></returns>
        public int CompareTo(Interval<T> i)
        {
            if (this.Start.CompareTo(i.Start) < 0)
            {
                return -1;
            }
            else if (this.Start.CompareTo(i.Start) > 0)
            {
                return 1;
            }
            else if (this.End.CompareTo(i.End) < 0)
            {
                return 1;
            }
            else if (this.End.CompareTo(i.End) > 0)
            {
                return -1;
            }

            // Identical interval
            return 0;
        }

        public override string ToString()
        {
            return String.Format("<{0}, {1}>", this.Start.ToString(), this.End.ToString());
        }
    }
}