namespace Orc.DataStructures.IntervalSkipList.SkipList
{
    using System.Collections.Generic;

    public interface IScannableDictionary<K, V> : IDictionary<K, V>, IScannable<K, V> { }
}