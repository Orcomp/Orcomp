namespace Orc.Entities.SkipList
{
    using System.Collections.Generic;

    public interface IScannableDictionary<K, V> : IDictionary<K, V>, IScannable<K, V> { }
}