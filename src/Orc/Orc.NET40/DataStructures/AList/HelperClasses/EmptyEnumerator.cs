// This file is part of the Loyc project. Licence: LGPL
namespace Orc.DataStructures.AList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

#if !SILVERLIGHT
    [Serializable]
#endif
    public class EmptyEnumerator<T> : IEnumerator<T>, IEnumerator
	{
		public static readonly EmptyEnumerator<T> Value = new EmptyEnumerator<T>();

		public T Current { get { return default(T); } }
		object IEnumerator.Current { get { return this.Current; } }
		public void Dispose() { }
		public bool MoveNext() { return false; }
		public void Reset() { }
	}
}
