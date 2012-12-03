namespace Orc.Algorithms.Interfaces
{
    using System.Collections;

    /// <summary>
	/// Object swapper interface
	/// </summary>
	public interface ISwap
	{
		void Swap(IList array, int left, int right);
		void Set(IList array, int left, int right);
		void Set(IList array, int left, object obj);
	}
}
