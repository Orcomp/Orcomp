﻿// This file is part of the Loyc project. Licence: LGPL
namespace Orc.DataStructures.AList
{
    using System;
    using System.Collections.Generic;

    /// <summary>A read-only list indexed by an integer.</summary>
	/// <remarks>
	/// The .NET collection classes have a very simple and coarse interface.
	/// <para/>
	/// Member list:
	/// <code>
	/// public T this[int index] { get; }
	/// public T TryGet(int index, ref bool fail);
	/// public Iterator&lt;T> GetIterator();
	/// public int Count { get; }
	/// public IEnumerator&lt;T> GetEnumerator();
	/// System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator();
	/// </code>
	/// The term "source" means a read-only collection, as opposed to a "sink" which
	/// is a write-only collection. The purpose of IListSource is to make it easier
	/// to implement a read-only list, by lifting IList's requirement to write 
	/// implementations for Add(), Remove(), etc. A secondary purpose is, of course,
	/// to guarantee users don't mistakenly call those methods on a read-only
	/// collection.
	/// <para/>
	/// I have often wanted to access the "next" or "previous" item in a list, e.g.
	/// during parsing, but it inconvenient if you have to worry about whether the 
	/// the current item is the first or last. In that case you must check whether
	/// the array index is valid, which is both inconvenient and wasteful, because
	/// the list class will check the array index again, and then the .NET runtime
	/// will check the index a third time when reading the internal array. To make
	/// this more efficient, IListSource has a TryGet() method that does not throw
	/// on failure, but returns default(T).
	/// <para/>
	/// As IListSource is supposed to be a simpler alternative to IList, I didn't
	/// want to require implementers to implement more than two indexers. There are
	/// two additional TryGet extension methods, though:
	/// <code>
	///     bool TryGet(int index, ref T value);
	///     T TryGet(int, T defaultValue);
	/// </code>
	/// If T is defined as "out" (covariant) in C# 4, these methods are not allowed 
	/// in IListSource anyway and MUST be extension methods.
	/// <para/>
	/// Note that "value" is a "ref" rather than an "out" parameter, unlike
	/// Microsoft's own TryGetValue() implementations. Using ref parameter allows
	/// the caller to choose his own default value in case TryGet() returns false.
	/// <para/>
	/// Using <see cref="ListSourceBase{T}"/> as your base class can help you
	/// implement this interface faster.
	/// </remarks>
	#if DotNet4
	public interface IListSource<out T> : ISource<T>
	#else
	public interface IListSource<T> : ISource<T>
	#endif
	{
		/// <summary>Gets the item at the specified index.</summary>
		/// <exception cref="ArgumentOutOfRangeException">The index was not valid
		/// in this list.</exception>
		/// <param name="index">An index in the range 0 to Count-1.</param>
		/// <returns>The element at the specified index.</returns>
		T this[int index] { get; }

		/// <summary>Gets the item at the specified index, and does not throw an
		/// exception on failure.</summary>
		/// <param name="index">An index in the range 0 to Count-1.</param>
		/// <param name="fail">A flag that is set on failure. To improve
		/// performance slightly, this flag is not cleared on success.</param>
		/// <returns>The element at the specified index, or default(T) if the index
		/// is not valid.</returns>
		/// <remarks>In my original design, the caller could provide a value to 
		/// return on failure, but this would not allow T to be marked as "out" in 
		/// C# 4. For the same reason, we cannot have a ref/out T parameter.
		/// Instead, the following extension methods are provided:
		/// <code>
		///     bool TryGet(int index, ref T value);
		///     T TryGet(int, T defaultValue);
		/// </code>
		/// </remarks>
		T TryGet(int index, ref bool fail);
	}

	public static partial class LCInterfaces
	{
		/// <summary>Tries to get a value from the list at the specified index.</summary>
		/// <param name="index">The index to access. Valid indexes are between 0 and Count-1.</param>
		/// <param name="value">A variable that will be changed to the retrieved value. If the index is not valid, this variable is left unmodified.</param>
		/// <returns>True on success, or false if the index was not valid.</returns>
		public static bool TryGet<T>(this IListSource<T> list, int index, ref T value)
		{
			bool fail = false;
			T result = list.TryGet(index, ref fail);
			if (fail)
				return false;
			value = result;
			return true;
		}
		
		/// <summary>Tries to get a value from the list at the specified index.</summary>
		/// <param name="index">The index to access. Valid indexes are between 0 and Count-1.</param>
		/// <param name="defaultValue">A value to return if the index is not valid.</param>
		/// <returns>The retrieved value, or defaultValue if the index provided was not valid.</returns>
		public static T TryGet<T>(this IListSource<T> list, int index, T defaultValue)
		{
			bool fail = false;
			T result = list.TryGet(index, ref fail);
			if (fail)
				return defaultValue;
			else
				return result;
		}
		
		/// <summary>Determines the index of a specific value.</summary>
		/// <returns>The index of the value, if found, or -1 if it was not found.</returns>
		/// <remarks>
		/// At first, this method was a member of IListSource itself, just in 
		/// case the source might have some kind of fast lookup logic (e.g. binary 
		/// search) or custom comparer. However, since the item to find is an "in" 
		/// argument, it would prevent IListSource from being marked covariant when
		/// I upgrade to C# 4.
		/// </remarks>
		public static int IndexOf<T>(this IListSource<T> list, T item)
		{
			int count = list.Count;
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < count; i++)
				if (comparer.Equals(item, list[i]))
					return i;
			return -1;
		}

		public static int CopyTo<T>(this IListSource<T> c, T[] array, int arrayIndex)
		{
			int space = array.Length - arrayIndex;
			int count = c.Count;
			if (space < count) {
				if ((uint)arrayIndex >= (uint)count)
					throw new ArgumentOutOfRangeException("arrayIndex");
				else
					throw new ArgumentException(string.Format("CopyTo: array is too small ({0} < {1})", space, count));
			}
			
			for (int i = 0; i < count; i++)
				array[arrayIndex + i] = c[i];
			
			return arrayIndex + count;
		}

		/// <summary>Gets the lowest index at which a condition is true, or -1 if nowhere.</summary>
		public static int IndexWhere<T>(this IListSource<T> source, Func<T, bool> pred)
		{
			for (int i = 0; i < source.Count; i++)
				if (pred(source[i]))
					return i;
			return -1;
		}
		/// <summary>Gets the highest index at which a condition is true, or -1 if nowhere.</summary>
		public static int LastIndexWhere<T>(this IListSource<T> source, Func<T, bool> pred)
		{
			for (int i = source.Count-1; i > 0; i--)
				if (pred(source[i]))
					return i;
			return -1;
		}

		/// <summary>Copies the contents of an IListSource to an array.</summary>
		public static T[] ToArray<T>(this IListSource<T> c)
		{
			var array = new T[c.Count];
			for (int i = 0; i < array.Length; i++)
				array[i] = c[i];
			return array;
		}
	}
}
