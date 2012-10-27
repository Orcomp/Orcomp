# Interval

## Purpose

- Well documented library for dealing with Intevals and their EndPoints.
- Nomenclature taken from accepted papers (mainly http://web.cacs.louisiana.edu/~logan/521_f08/Doc/p832-allen.pdf)
- EndPoints can be inclusive or exclusive.

## Introduction

An interval always has two EndPoints. One at each extremum.

The left EndPoint is called the Min.
The right EndPoint is called the Max.

An EndPoint is aware of the interval it belongs to.
(i.e. An EndPoint can only exist if there is an interval.)

## Methods

**Overlaps**: Returns true if two invervals overlap each other

**Meets**: Return true if the EndPoint of two intervals meet each other.

## DataStructures

- IntervalTree

## IIntervalContainer

IIntervalContainer has a Query() method instead of Search() or Find() because these last two method names sugges we are searching for something definite, whereas Query is vague enough to
allow for a list of matching intervals to be returned.

## Notes

The main purpose of using intervals is to query their relationships (most noteably whether they overlap or not)

**Starts Before Start, Ends before End
>      +-----------+
>         +------------+

**Starts Before Start, Ends after End (Contains)
>      +---------------+
>        +------+


**Starts After Start, Ends before End (During, or IsContained)
>        +------------+
>     +-----------------+

**Starts After End
>              +----------+
>     +-----+

**Ends Before Start
>     +------+
>              +-------+

**Starts At Start, Ends at End
>      +---------+
>      +---------+

# EndPoint

EndPoints can be inclusive or exclusive of their value.
EndPoints can also represent +/- infinity by specidying double.PositiveInfinity or double.NegativeInfinity


# Links

- Maintaining Knowledge about Temporal Intervals (http://web.cacs.louisiana.edu/~logan/521_f08/Doc/p832-allen.pdf)

# Interval Structures

General information about interval trees: http://en.wikipedia.org/wiki/Interval_tree

## Interval Tree Implementations in C#

http://code.google.com/p/intervaltree/source/browse/IntervalTree.cs
https://github.com/vvondra/Interval-Tree (Already in Orc)
https://github.com/mbuchetics/RangeTree/blob/master/RangeTreeExamples/Program.cs (Already in Orc)
http://intervaltree.codeplex.com/


## Algorithmns to be implemented in C#
http://bioinformatics.oxfordjournals.org/content/23/11/1386.full

## Other useful references
- C++ Segment Trees http://code.google.com/p/multidimalgorithm/
