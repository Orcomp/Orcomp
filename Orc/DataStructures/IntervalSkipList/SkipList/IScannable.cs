namespace Orc.DataStructures.IntervalSkipList.SkipList
{
    using System;
    using System.Collections.Generic;

    public interface IScannable<K, V>
    {
        KeyValuePair<K, V> FindNext(IComparable<K> keytest, bool equal_ok);
        KeyValuePair<K, V> FindPrev(IComparable<K> keytest, bool equal_ok);
        IEnumerable<KeyValuePair<K, V>> scanForward(IScanner<K> scanner);
        IEnumerable<KeyValuePair<K, V>> scanBackward(IScanner<K> scanner);
    }
}