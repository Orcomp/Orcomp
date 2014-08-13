﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orc.DataStructures.AList
{
	/// <summary>
	/// Provides view of an <see cref="IListSource{T}"/> in which element [i] is a
	/// <see cref="NegListSource{T}"/> N such that N[0] refers to element [i] in the 
	/// original list. See <see cref="LCExt.NegLists{T}(IListSource{T})"/> for more 
	/// information.
	/// </summary>
    /// <seealso cref="SelectNegLists{T}"/>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class SelectNegListSources<T> : IterableBase<NegListSource<T>>, IListSource<NegListSource<T>>
	{
		protected IListSource<T> _list;

		public SelectNegListSources(IListSource<T> list) { _list = list; }

		public IListSource<T> OriginalList { get { return _list; } }
		
		public NegListSource<T> this[int index]
		{
			get { return new NegListSource<T>(_list, index); }
		}
		public NegListSource<T> TryGet(int index, ref bool fail)
		{
			fail = (uint)index >= (uint)_list.Count;
			return new NegListSource<T>(_list, index);
		}
		public int Count
		{
			get { return _list.Count; }
		}
		public sealed override Iterator<NegListSource<T>> GetIterator()
		{
			int i = -1;
			return delegate(ref bool ended)
			{
				ended = ((uint)++i >= (uint)_list.Count);
				return new NegListSource<T>(_list, i);
			};
		}
	}
}
