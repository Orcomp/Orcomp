﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Orc.DataStructures.AList
{
	/// <summary>Holds the Count property found in nearly all collection interfaces.</summary>
	/// <remarks>Any interface derived from ICount automatically gets the Any()
	/// extension method.</remarks>
	public interface ICount
	{
		/// <summary>Gets the number of items in the collection.</summary>
		int Count { get; }
	}

	/// <summary>
	/// This class contains extension methods that are provided as part of various 
	/// Loyc.Collections interfaces. For example, it provides methods such as IndexOf(),
	/// Contains() and CopyTo(), that the traditional <see cref="ICollection{T}"/> 
	/// and <see cref="IList{T}"/> interfaces require the author to write himself.
	/// </summary>
	/// <remarks>
	/// For covariant collections such as <see cref="ISource{T}"/> and <see 
	/// cref="IListSource{T}"/>, the CLR actually prohibits methods such as 
	/// Contains(T) and IndexOf(T), because T is not allowed in "input" positions. 
	/// Therefore, these extension methods must be used to fill the gap. Even
	/// methods such as <i>bool TryGet(int, out T)</i> are prohibited, so TryGet()
	/// has the signature <i>T TryGet(ref bool failed)</i> instead, and extension
	/// methods provide the original version of the method in addition.
	/// </remarks>
	public static partial class LCInterfaces
	{
		/// <summary>Returns true if the collection contains any elements.</summary>
		public static bool Any(this ICount c) { return c.Count > 0; }
	}
}
