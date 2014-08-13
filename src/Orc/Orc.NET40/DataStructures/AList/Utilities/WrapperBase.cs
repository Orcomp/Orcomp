﻿namespace Orc.DataStructures.AList.Utilities
{
    using System;

    /// <summary>Abstract class that helps you implement wrappers by automatically
    /// forwarding calls to Equals(), GetHashCode() and ToString().</summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class WrapperBase<T>
	{
		protected T _obj;
		protected WrapperBase(T wrappedObject)
		{
			if (wrappedObject == null)
				throw new ArgumentNullException("wrappedObject");
			this._obj = wrappedObject;
		}

		/// <summary>Returns true iff the parameter 'obj' is a wrapper around the same object that this object wraps.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <remarks>If obj actually refers to the wrapped object, this method returns false to preserve commutativity of the "Equals" relation.</remarks>
		public override bool Equals(object obj)
		{
			var w = obj as WrapperBase<T>;
			return w != null && object.Equals(w._obj, this._obj);
		}
		/// <summary>Returns the hashcode of the wrapped object.</summary>
		public override int GetHashCode()
		{
			return this._obj.GetHashCode();
		}
		/// <summary>Returns ToString() of the wrapped object.</summary>
		public override string ToString()
		{
			return this._obj.ToString();
		}
	}
}
