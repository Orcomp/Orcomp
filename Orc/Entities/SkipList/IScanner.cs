
// IScanner - an efficient bi-directional scanning interface for sorted collections (like BDSkipList)
//
// authored by David W. Jeske (2008-2010)
//
// This code is provided without warranty to the public domain. You may use it for any
// purpose without restriction.

namespace Orc.Entities.SkipList
{
    using System;

    // ----- aggregate interface for scannable dictionaries 

    // TODO: consider how this will need to be reworked to efficiently handle 
    //   next/prev on prefix-compressed data

    public interface IScanner<K>
    {
        // currently scans are always performed (>=,<=) on the lowest/highest 
        // endpoints, because you can use MatchTo() to decide if you want to produce the record
        // TODO: consider if we need to simplify and define (>,<) operations on endpoints
        bool MatchTo(K candidate);
        IComparable<K> genLowestKeyTest();
        IComparable<K> genHighestKeyTest();
    }
}