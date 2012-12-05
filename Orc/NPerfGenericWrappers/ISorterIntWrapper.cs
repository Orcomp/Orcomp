namespace Orc.NPerfGenericWrappers
{
    using Orc.Algorithms.Sort;
    using Orc.Algorithms.Sort.TimSort;

    //public class QuickSorter_Int : Orc.Algorithms.NSort.Generic.QuickSorter<int>{ }
    public class TimSort_Int : ListTimSort<int>{ }
    public class QuickSortBaseClass_Int :QuickSortBaseClass<int> { }
}
