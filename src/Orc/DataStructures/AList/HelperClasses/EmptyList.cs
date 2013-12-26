﻿// This file is part of the Loyc project. Licence: LGPL
using System;
using System.Collections.Generic;
using System.Text;

namespace Orc.DataStructures.AList
{
	[Serializable]
	public class EmptyList<T> : IList<T>, IListSource<T>
	{
		public static readonly EmptyList<T> Value = new EmptyList<T>();

		public int IndexOf(T item)
		{
			return -1;
		}
		public void Insert(int index, T item)
		{
			ReadOnly();
		}
		private void ReadOnly()
		{
			throw new InvalidOperationException("Collection is read-only");
		}
		public void RemoveAt(int index)
		{
			ReadOnly();
		}
		public T this[int index]
		{
			get {
				throw new IndexOutOfRangeException();
			}
			set {
				throw new IndexOutOfRangeException();
			}
		}
		public T TryGet(int index, ref bool fail)
		{
			fail = true;
			return default(T);
		}
		public void Add(T item)
		{
			ReadOnly();
		}
		public void Clear()
		{
		}
		public bool Contains(T item)
		{
			return false;
		}
		public void CopyTo(T[] array, int arrayIndex)
		{
		}
		public int Count
		{
			get { return 0; }
		}
		public bool IsReadOnly
		{
			get { return true; }
		}
		public bool Remove(T item)
		{
			return false;
		}
		public IEnumerator<T> GetEnumerator()
		{
			return EmptyEnumerator<T>.Value;
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return EmptyEnumerator<T>.Value;
		}
		public Iterator<T> GetIterator()
		{
			return EmptyIterator<T>.Value;
		}
	}
}
