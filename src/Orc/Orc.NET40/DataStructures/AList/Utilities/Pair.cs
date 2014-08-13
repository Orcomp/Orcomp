namespace Orc.DataStructures.AList.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public static class Pair
	{
		public static Pair<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
			{ return new Pair<T1, T2>(item1, item2); }
		public static Triplet<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) 
			{ return new Triplet<T1, T2, T3>(item1, item2, item3); }
	}
	/// <summary>A tuple of two values, in a struct.</summary>
	public struct Pair<T1,T2> : IComparable, IComparable<Pair<T1, T2>>
	{
		public Pair(T1 a, T2 b) { this.A = a; this.B = b; }
		public T1 A;
		public T2 B;
		public T1 Item1 { [DebuggerStepThrough] get { return this.A; } set { this.A = value; } }
		public T2 Item2 { [DebuggerStepThrough] get { return this.B; } set { this.B = value; } }
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] // reduce clutter in debugger
		public T1 Key   { [DebuggerStepThrough] get { return this.A; } set { this.A = value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] // reduce clutter in debugger
		public T2 Value { [DebuggerStepThrough] get { return this.B; } set { this.B = value; } }

		public override bool Equals(object obj)
		{
			if (obj is Pair<T1, T2>)
			{
				Pair<T1, T2> rhs = (Pair<T1, T2>)obj;
				return this.A.Equals(rhs.A) && this.B.Equals(rhs.B);
			}
			return false;
		}
		public override int GetHashCode()
		{
			return this.A.GetHashCode() ^ this.B.GetHashCode();
		}
		public override string ToString()
		{
			return string.Format("({0}, {1})", this.A, this.B);
		}
		public int CompareTo(Pair<T1, T2> other)
		{
			int c = Comparer<T1>.Default.Compare(this.A, other.A);
			if (c == 0) {
				c = Comparer<T2>.Default.Compare(this.B, other.B);
			}
			return c;
		}
		public int CompareTo(object obj)
		{
			return this.CompareTo((Pair<T1, T2>)obj);
		}
	}

	/// <summary>A tuple of three values, in a struct.</summary>
	public struct Triplet<T1, T2, T3>
	{
		public Triplet(T1 a, T2 b, T3 c) { this.A = a; this.B = b; this.C = c; }
		public T1 A;
		public T2 B;
		public T3 C;
		public T1 Item1 { [DebuggerStepThrough] get { return this.A; } set { this.A = value; } }
		public T2 Item2 { [DebuggerStepThrough] get { return this.B; } set { this.B = value; } }
		public T3 Item3 { [DebuggerStepThrough] get { return this.C; } set { this.C = value; } }

		public override bool Equals(object obj)
		{
			if (obj is Triplet<T1, T2, T3>)
			{
				Triplet<T1, T2, T3> rhs = (Triplet<T1, T2, T3>)obj;
				return this.A.Equals(rhs.A) && this.B.Equals(rhs.B) && this.C.Equals(rhs.C);
			}
			return false;
		}
		public override int GetHashCode()
		{
			return this.A.GetHashCode() ^ this.B.GetHashCode() ^ this.C.GetHashCode();
		}
		public override string ToString()
		{
			return string.Format("({0},{1},{2})", this.A, this.B, this.C);
		}
		public int CompareTo(Triplet<T1, T2, T3> other)
		{
			int c = Comparer<T1>.Default.Compare(this.A, other.A);
			if (c == 0) {
				c = Comparer<T2>.Default.Compare(this.B, other.B);
				if (c == 0) {
					c = Comparer<T3>.Default.Compare(this.C, other.C);
				}
			}
			return c;
		}
		public int CompareTo(object obj)
		{
			return this.CompareTo((Triplet<T1, T2, T3>)obj);
		}
	}

	/// <summary>A trivial class that holds a single value of type T in the 
	/// <see cref="Value"/> property.
	/// </summary><remarks>
	/// This class is useful mainly as an alternative to standard boxing. When you 
	/// box a structure in C#, you lose access to the members of that structure.
	/// This class, in contrast, provides access to the "boxed" value.
	/// </remarks>
	public class Holder<T> : WrapperBase<T>
	{
		public Holder(T value) : base(value) { }
		public Holder() : base(default(T)) { }

		/// <summary>Any value of type T.</summary>
		public T Value { get { return _obj; } set { _obj = value; } }
	}
}
