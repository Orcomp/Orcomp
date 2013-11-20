namespace Orc.DataStructures.AList.Utilities
{
    using System;

    public class InvalidStateException : InvalidOperationException
	{
		public InvalidStateException() : base(string.Format("This object is in an invalid state.")) { }
		public InvalidStateException(string msg) : base(msg) { }
		public InvalidStateException(string msg, Exception innerException) : base(msg, innerException) { }
	}

	public class ConcurrentModificationException : InvalidOperationException
	{
        public ConcurrentModificationException() : base(string.Format("A concurrect access was detected during modification.")) { }
		public ConcurrentModificationException(string msg) : base(msg) { }
		public ConcurrentModificationException(string msg, Exception innerException) : base(msg, innerException) { }
	}

	public class ReadOnlyException : InvalidOperationException
	{
        public ReadOnlyException() : base(string.Format("An attempt was made to modify a read-only object.")) { }
		public ReadOnlyException(string msg) : base(msg) { }
		public ReadOnlyException(string msg, Exception innerException) : base(msg, innerException) { }
	}

	public static class CheckParam
	{
		public static void IsNotNull(string paramName, object arg)
		{
			if (arg == null)
				ThrowArgumentNull(paramName);
		}
		public static void IsNotNegative(string argName, int value)
		{
			if (value < 0)
                throw new ArgumentOutOfRangeException(argName, string.Format(@"Argument ""{0}"" value '{1}' should not be negative.", argName, value));
		}
		public static void Range(string paramName, int value, int min, int max)
		{
			if (value < min || value > max)
				ThrowOutOfRange(paramName, value, min, max);
		}
		public static void ThrowOutOfRange(string argName)
		{
			throw new ArgumentOutOfRangeException(argName);
		}
		public static void ThrowOutOfRange(string argName, int value, int min, int max)
		{
            throw new ArgumentOutOfRangeException(argName, string.Format(@"Argument ""{0}"" value '{1}' is not within the expected range ({2}..{3})", argName, value, min, max)); 
		}
		public static void ThrowArgumentNull(string argName)
		{
			throw new ArgumentNullException(argName);
		}
	}

    public class EnumerationException : InvalidOperationException
	{
        public EnumerationException() : base(string.Format("The collection was modified after enumeration started.")) { }
		public EnumerationException(string msg) : base(msg) { }
		public EnumerationException(string msg, Exception innerException) : base(msg, innerException) { }
	}

	public class KeyAlreadyExistsException : InvalidOperationException
	{
        public KeyAlreadyExistsException() : base(string.Format("The item or key being added already exists in the collection.")) { }
		public KeyAlreadyExistsException(string msg) : base(msg) { }
		public KeyAlreadyExistsException(string msg, Exception innerException) : base(msg, innerException) { }
	}
}
