﻿namespace Orc.DataStructures.AList.Impl
{
	using System;
	using System.Diagnostics;

#if !SILVERLIGHT
    [Serializable]
#endif
	public abstract class AListLeaf<K, T> : AListNode<K, T>
	{
		public const int DefaultMaxNodeSize = 48;

		protected InternalDList<T> _list = InternalDList<T>.Empty;

		public AListLeaf(ushort maxNodeSize)
		{
			Debug.Assert(maxNodeSize >= 3);
			_maxNodeSize = maxNodeSize;
		}
		public AListLeaf(ushort maxNodeSize, InternalDList<T> list) : this(maxNodeSize)
		{
			_list = list;
		}

		public AListLeaf(AListLeaf<K, T> frozen)
		{
			// TODO: Rethink: original node doesn't have to be frozen because we are deep-cloning. Right??
			//       Probably DetachedClone() doesn't have to freeze a leaf node.
			Debug.Assert(frozen.IsFrozen);
			_list = frozen._list.Clone();
			_maxNodeSize = frozen._maxNodeSize;
			_isFrozen = false;
			//_userByte = frozen._userByte;
		}

		public sealed override int LocalCount
		{
			get { return _list.Count; }
		}

		public override T this[uint index]
		{
			get { return _list[(int)index]; }
		}
		public override void SetAt(uint index, T item, IAListTreeObserver<K, T> tob)
		{
			Debug.Assert(!_isFrozen);
			if (tob != null) {
				tob.ItemRemoved(_list[(int)index], this);
				tob.ItemAdded(item, this);
			}
			_list[(int)index] = item;
		}

		internal sealed override uint TakeFromRight(AListNode<K, T> child, IAListTreeObserver<K, T> tob)
		{
			var right = (AListLeaf<K, T>)child;
			if (IsFullLeaf || _isFrozen || right._isFrozen)
				return 0;
			T item = right._list.First;
			_list.PushLast(item);
			right._list.PopFirst(1);
			if (tob != null) tob.ItemMoved(item, right, this);
			return 1;
		}

		internal sealed override uint TakeFromLeft(AListNode<K, T> child, IAListTreeObserver<K, T> tob)
		{
			var left = (AListLeaf<K, T>)child;
			if (IsFullLeaf || _isFrozen || left._isFrozen)
				return 0;
			T item = left._list.Last;
			_list.PushFirst(item);
			left._list.PopLast(1);
			if (tob != null) tob.ItemMoved(item, left, this);
			return 1;
		}

		public override T GetLastItem()
		{
			return _list.Last;
		}

		public sealed override uint TotalCount
		{
			get { return (uint)_list.Count; }
		}

		public sealed override bool IsFullLeaf
		{
			get { return _list.Count >= _maxNodeSize; }
		}
		public override bool IsUndersized
		{
			get { return _list.Count * 3 <= _maxNodeSize; }
		}

		public override bool RemoveAt(uint index, uint count, IAListTreeObserver<K, T> tob)
		{
			Debug.Assert(!_isFrozen);

			if (tob != null) tob.RemovingItems(_list, (int)index, (int)count, this, false);
			_list.RemoveRange((int)index, (int)count);
			return (_list.Count << 1) <= _maxNodeSize && IsUndersized;
		}

		public override void Freeze()
		{
			_isFrozen = true;
		}

		public override int CapacityLeft { get { return _maxNodeSize - LocalCount; } }

		public Iterator<T> GetIterator(int start, int subcount)
		{
			return _list.GetIterator(start, subcount);
		}

		public void Sort(int start, int subcount, Comparison<T> comp)
		{
			Debug.Assert(!_isFrozen);
			_list.Sort(start, subcount, comp);
		}

		public int IndexOf(T item, int startIndex)
		{
			return _list.IndexOf(item, startIndex);
		}
		
		public override int ImmutableCount()
		{
			return IsFrozen ? (int)TotalCount : 0;
		}
	}

	/// <summary>
	/// Leaf node of <see cref="AList{T}"/>.
	/// </summary>
    /// <typeparam name="T"></typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class AListLeaf<T> : AListLeaf<T, T>
	{
		public AListLeaf(ushort maxNodeSize) : base(maxNodeSize) { }
		public AListLeaf(ushort maxNodeSize, InternalDList<T> list) : base(maxNodeSize, list) { }
		public AListLeaf(AListLeaf<T> frozen) : base(frozen) { }

		public override AListNode<T, T> DetachedClone()
		{
			_isFrozen = true;
			return new AListLeaf<T>(this);
		}

		public override AListNode<T, T> CopySection(uint index, uint count, AListBase<T,T> list)
		{
			Debug.Assert((int)(count | index) > 0);
			if (index == 0 && count == _list.Count)
				return DetachedClone();

			return new AListLeaf<T>(_maxNodeSize, _list.CopySection((int)index, (int)count));
		}

		public override AListNode<T, T> Insert(uint index, T item, out AListNode<T, T> splitRight, IAListTreeObserver<T, T> tob)
		{
			Debug.Assert(!_isFrozen);

			if (_list.Count < _maxNodeSize)
			{
				_list.AutoRaiseCapacity(1, _maxNodeSize);
				_list.Insert((int)index, item);
				splitRight = null;
				if (tob != null) tob.ItemAdded(item, this);
				return null;
			}
			else
			{
				// Split node in half in the middle
				int divAt = _list.Count >> 1;
				var left = new AListLeaf<T>(_maxNodeSize, _list.CopySection(0, divAt));
				var right = new AListLeaf<T>(_maxNodeSize, _list.CopySection(divAt, _list.Count - divAt));
				
				// Note: don't pass tob to left.Insert() or right.Insert() because 
				// parent node will send required notifications to tob instead.
				if (index <= divAt)
					left.Insert(index, item, out splitRight, null);
				else
					right.Insert(index - (uint)divAt, item, out splitRight, null);
				Debug.Assert(splitRight == null);
				splitRight = right;
				return left;
			}
		}
		public override AListNode<T, T> InsertRange(uint index, IListSource<T> source, ref int sourceIndex, out AListNode<T, T> splitRight, IAListTreeObserver<T, T> tob)
		{
			Debug.Assert(!_isFrozen);

			// Note: (int)index may sometimes be negative, but the adjustedIndex is not.
			int adjustedIndex = (int)index + sourceIndex;
			int leftHere = (int)_maxNodeSize - _list.Count;
			int leftIns = source.Count - sourceIndex;
			Debug.Assert(leftIns > 0);
			if (leftHere > 2)
			{
				int amtToIns = Math.Min(leftHere, leftIns);
				_list.AutoRaiseCapacity(amtToIns, _maxNodeSize);
				var slice = source.Slice(sourceIndex, amtToIns);
				_list.InsertRange(adjustedIndex, slice);
				sourceIndex += amtToIns;
				splitRight = null;
				if (tob != null) tob.AddingItems(slice, this, false);
				return null;
			}
			else
				return Insert((uint)adjustedIndex, source[sourceIndex++], out splitRight, tob);
		}
	}
}
