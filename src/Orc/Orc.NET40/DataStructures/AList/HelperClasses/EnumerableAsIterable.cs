﻿/*
 * Created by SharpDevelop.
 * User: Pook
 * Date: 4/10/2011
 * Time: 8:56 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Orc.DataStructures.AList
{
    using System;
    using System.Collections.Generic;

    using Orc.DataStructures.AList.Utilities;

    /// <summary>
	/// Helper type returned from <see cref="LCExt.AsIterable{T}"/>.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class EnumerableAsIterable<T> : WrapperBase<IEnumerable<T>>, IIterable<T>, IEnumerable<T>
	{
		public EnumerableAsIterable(IEnumerable<T> list) : base(list) { }
		
		public Iterator<T> GetIterator()
		{
			return _obj.GetEnumerator().AsIterator();
		}
		public IEnumerator<T> GetEnumerator()
		{
			return _obj.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _obj.GetEnumerator();
		}
	}
}
